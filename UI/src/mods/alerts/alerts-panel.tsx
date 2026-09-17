import React, { useState } from "react";
import { useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { Alert, alerts$, muteAlert, smartAlertsEnabled$, viewAlert } from "./bindings";
import { StatusShapeGlyph } from "theme/glyphs";
import { Status, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * What is true about the city, in as few rows as it can honestly be said.
 *
 * The game will happily put the same warning over forty buildings. Aggregation happens on the C#
 * side, so what arrives here is already one row per kind, ordered worst first, with the count
 * beside it. This component's only job is to make that readable without making it loud.
 *
 * Every row states something. None of them says what to do about it - and there is deliberately
 * no button here that acts on the city. Mute is the exception, and it acts on the mod.
 */
export const AlertsPanel = () => {
  const enabled = useValue(smartAlertsEnabled$);
  const snapshot = useValue(alerts$);
  const text = useText();
  const { translate } = useLocalization();

  // Closed by default. What the player asked not to hear about should not be the first thing
  // they see, but it must never become unreachable either - see the note on the summary line.
  const [showMuted, setShowMuted] = useState(false);

  if (!enabled) {
    return (
      <div className={styles.empty}>
        <span className={styles.emptyHint}>{text(K.alertsDisabled)}</span>
      </div>
    );
  }

  /**
   * The mod's own sentence when it has one, the game's name for the thing when it does not.
   *
   * Falling through rather than showing a raw key is what lets an unrecognised notification type
   * still say something true, instead of disappearing or shouting an identifier.
   */
  const titleOf = (alert: Alert) =>
    translate(alert.titleKey, null) ?? alert.rawName ?? text(K.alertsTitle);

  const affectedOf = (alert: Alert) => {
    if (alert.affected <= 0) {
      return null;
    }

    return alert.affected === 1
      ? text(K.affectedOne)
      : `${alert.affected} ${text(K.affectedMany)}`;
  };

  const row = (alert: Alert, muted: boolean) => {
    const token = statusToken(muted ? Status.Muted : alert.status);
    const affected = affectedOf(alert);

    return (
      <div
        key={alert.id}
        className={`${styles.row} ${styles.alertRow}`}
        style={{ background: token.wash }}
        onClick={() => alert.canView && viewAlert(alert.id)}
      >
        {/* The severity, at full strength, on the edge of the row. It is decoration: what is
            actually wrong is written in the words beside it. */}
        <span className={styles.severityBar} style={{ background: token.color }} />

        <AlertIcon src={alert.icon} />

        <span className={styles.rowText}>
          <span className={styles.rowLabel}>{titleOf(alert)}</span>
          {affected && <span className={styles.rowWhen}>{affected}</span>}
        </span>

        {/*
          On a muted row the way back is always visible, never on hover. Hover is fine for
          tidying a list you are already reading; it is not fine for the only route out of a
          decision the player made an hour ago and has since forgotten.
        */}
        <span className={muted ? styles.rowActionsShown : styles.rowActions}>
          {!muted && alert.canView && (
            <button
              className={styles.textButton}
              onClick={(e) => {
                e.stopPropagation();
                viewAlert(alert.id);
              }}
            >
              {text(K.view)}
            </button>
          )}
          <button
            className={styles.textButton}
            onClick={(e) => {
              e.stopPropagation();
              muteAlert(alert.id, !muted);
            }}
          >
            {text(muted ? K.alertsUnmute : K.alertsMute)}
          </button>
        </span>
      </div>
    );
  };

  return (
    <>
      <div className={styles.sectionHeader}>
        <span className={styles.sectionTitle}>{text(K.alertsTitle)}</span>
        <AlertCounts snapshot={snapshot} />
      </div>

      {snapshot.items.length === 0 ? (
        <div className={styles.empty}>
          <span>{text(K.alertsQuiet)}</span>
          <span className={styles.emptyHint}>{text(K.alertsQuietHint)}</span>
        </div>
      ) : (
        <div className={styles.list}>{snapshot.items.map((alert) => row(alert, false))}</div>
      )}

      {/*
        One line, one text node. Written as `{n} {word}` it arrived as separate text nodes and
        Gameface laid each one out on its own line - "5" above "muted" - which read as two facts
        instead of one.

        It is a button because muting has to be undoable. A silence the player cannot lift is not
        a preference they expressed, it is information the mod decided to keep from them.
      */}
      {snapshot.muted > 0 && (
        <button className={styles.mutedNote} onClick={() => setShowMuted(!showMuted)}>
          {`${snapshot.muted} ${text(K.alertsMutedCount)}`}
        </button>
      )}

      {showMuted && snapshot.mutedItems.length > 0 && (
        <div className={styles.list}>{snapshot.mutedItems.map((alert) => row(alert, true))}</div>
      )}
    </>
  );
};

/** The generic marker the game uses when nothing more specific fits. Matches AlertIcons.Fallback. */
const FALLBACK_ICON = "Media/Game/Icons/Notifications.svg";

/**
 * The game's own icon for this alert - the same one it draws over the affected buildings.
 *
 * Not every notification type ships a file under the name we derive, so a failed load falls back
 * to the generic marker rather than leaving a hole and a ragged column.
 */
const AlertIcon = ({ src }: { src: string }) => {
  const [failed, setFailed] = useState(false);

  return (
    <img
      className={styles.subjectIcon}
      src={!src || failed ? FALLBACK_ICON : src}
      onError={() => setFailed(true)}
    />
  );
};

/**
 * "1 immediate, 2 important" in the smallest space that stays readable.
 *
 * Only statuses that are actually present get a chip. A row of zeroes would be four things to
 * read to learn nothing, which is the opposite of the point.
 */
export const AlertCounts = ({
  snapshot,
  size = 12,
}: {
  snapshot: { immediate: number; important: number; monitor: number };
  size?: number;
}) => {

  const chips: { status: Status; count: number }[] = [
    { status: Status.Immediate, count: snapshot.immediate },
    { status: Status.Important, count: snapshot.important },
    { status: Status.Monitor, count: snapshot.monitor },
  ];

  const present = chips.filter((c) => c.count > 0);
  if (present.length === 0) {
    return null;
  }

  return (
    <span className={styles.counts}>
      {present.map(({ status, count }) => {
        const token = statusToken(status);
        return (
          <span key={status} className={styles.countChip} style={{ color: token.color }}>
            <StatusShapeGlyph shape={token.shape} size={size} color={token.color} />
            <span className={styles.countNumber}>{count}</span>
          </span>
        );
      })}
    </span>
  );
};
