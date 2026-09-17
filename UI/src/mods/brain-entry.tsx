import React, { useEffect, useState } from "react";
import { useValue } from "cs2/api";
import { Button, Tooltip } from "cs2/ui";
import { AlertCounts, AlertsPanel } from "./alerts/alerts-panel";
import { alerts$, setAlertsOpen, smartAlertsEnabled$ } from "./alerts/bindings";
import { IdeasSection } from "./brain-parking/ideas-panel";
import { IdeaEditor } from "./brain-parking/idea-editor";
import { SafetyNet } from "./creativity/safety-net";
import { safetyNetEnabled$ } from "./creativity/bindings";
import { TrailSection } from "./trail/trail-section";
import { trailEnabled$ } from "./trail/bindings";
import { ToolbarFold } from "./calm/toolbar-fold";
import { toolbarButtonVisible$ } from "./calm/bindings";
import {
  brainParkingEnabled$,
  clearIdeaFocus,
  focusedIdea$,
  ideas$,
  startParking,
} from "./brain-parking/bindings";
import owlIcon from "images/owl.svg";
import { Status, statusToken } from "theme/tokens";
import { useSurface } from "theme/surface";
import { DismissPriority, useDismissOnGameClose } from "theme/use-dismiss";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "./panel.module.scss";

/**
 * The whole mod, as one small button.
 *
 * It is the game's own button so it sits in the row exactly like its neighbours, and it says as
 * little as it can get away with: one number for what is wrong right now. That is a count of
 * what is true about the city - a count of things the player has not got round to would not be
 * allowed here.
 *
 * Everything else is behind one click.
 */
export const BrainEntry = () => {
  const parkingEnabled = useValue(brainParkingEnabled$);
  const foldVisible = useValue(toolbarButtonVisible$);
  const alertsEnabled = useValue(smartAlertsEnabled$);
  const safetyNetEnabled = useValue(safetyNetEnabled$);
  const trailEnabled = useValue(trailEnabled$);
  const alerts = useValue(alerts$);
  const text = useText();
  const surface = useSurface();

  const focused = useValue(focusedIdea$);

  const [open, setOpen] = useState(false);

  /**
   * The idea whose editor is open, if any.
   *
   * Held here rather than inside the list because two things depend on knowing: the card is drawn
   * beside the panel, and while it is up the game's Close action shuts the card only.
   */
  const [editingId, setEditingId] = useState<string | null>(null);
  const allIdeas = useValue(ideas$);
  const editing = editingId ? allIdeas.find((idea) => idea.id === editingId) : undefined;

  // The C# side collects less often when nobody is looking. Telling it either way is the whole
  // reason that optimisation is safe.
  useEffect(() => {
    setAlertsOpen(open);
    return () => setAlertsOpen(false);
  }, [open]);

  // Clicking an idea's ring on the map opens the panel on it, and the card with what was written
  // there. The C# side decides whether a click landed on a ring; all this does is answer.
  //
  // A ring is a thought left in a place, and clicking one is asking what the thought was. Opening
  // the list scrolled to its row answered "which one" and left "what was it" for a second click.
  useEffect(() => {
    if (focused) {
      setOpen(true);
      setEditingId(focused);
    }
  }, [focused]);

  // Closing the panel puts the pointing finger down. Left set, the next time the panel opened for
  // any other reason it would open highlighting something the player had long since moved on from.
  useEffect(() => {
    if (!open && focused) {
      clearIdeaFocus();
    }
  }, [open, focused]);

  const closePanel = () => {
    setEditingId(null);
    setOpen(false);
  };

  // Follow the game's Close/Back binding and dismiss only the uppermost surface.
  useDismissOnGameClose(open, closePanel, DismissPriority.Panel);
  useDismissOnGameClose(Boolean(open && editing), () => setEditingId(null), DismissPriority.Editor);

  // Every feature inside the panel needs the owl, even when parking and alerts are off.
  // In particular, hiding it while the toolbar is folded removes the way to unfold it.
  if (!parkingEnabled && !alertsEnabled && !foldVisible && !safetyNetEnabled && !trailEnabled) {
    return null;
  }

  // Only the two levels that mean "something is happening". Monitor is worth a look when the
  // panel is open, not a number on the toolbar.
  const needsAttention = alerts.immediate + alerts.important;
  const peak = statusToken(alerts.immediate > 0 ? Status.Immediate : Status.Important);

  return (
    <div className={styles.entry}>
      {/*
        The game's own button, not one of ours. It is the only way to actually match the controls
        beside it: size, background, hover, focus and the pressed look all come from the game's
        theme, so they stay right even when that theme changes.
      */}
      {/*
        The icon goes in as `src`, not as children. Passing children left the game's own button
        laying it out on the text baseline, which sat it lower than every other icon in the row -
        with `src` the button places it exactly as it places all the vanilla ones.
      */}
      <Tooltip tooltip={text(K.toolbarTooltip)}>
        <Button
          variant="floating"
          src={owlIcon}
          selected={open}
          onSelect={() => (open ? closePanel() : setOpen(true))}
        />
      </Tooltip>

      {/*
        The count rides on top of the button rather than inside it. Inline, it made this button a
        different width from every other control in the row, which is exactly what looked wrong.
      */}
      {alertsEnabled && needsAttention > 0 && (
        <span className={styles.brainBadge} style={{ background: peak.color }}>
          {needsAttention}
        </span>
      )}

      {open && (
        <div className={styles.panel} style={{ background: surface }}>
          {/*
            The way out, where every window in the player's life keeps it. The game's Close action
            works too, but a gesture nobody was told about is not a way out -
            it is a shortcut for the people who already found it.
          */}
          <div className={styles.panelHeader}>
            <span className={styles.panelName}>{text(K.toolbarTooltip)}</span>

            <button className={styles.panelClose} onClick={closePanel}>
              X
            </button>
          </div>

          {/*
            Everything that can grow lives in here and scrolls. The panel itself must not: it has
            to keep the action below in view. With a busy city - five alerts, a checkpoint and a
            few parked ideas - the panel simply ran past the bottom of its own box and took the
            button with it.
          */}
          <div className={styles.panelBody}>
            {alertsEnabled && <AlertsPanel />}

            <SafetyNet />

            {parkingEnabled && (
              <IdeasSection
                focusedId={focused}
                onEdit={setEditingId}
                onPark={() => {
                  // Out of the way immediately: the next click belongs to the map, not to this
                  // panel.
                  setOpen(false);
                  startParking();
                }}
              />
            )}

            {/*
              Last, and that is a judgement rather than an accident. What is true about the city
              comes first, then the way back, then what the player chose to keep - and only then
              where they happen to have been. The trail is the only section here nobody asked for
              in the moment, so it is the one that has to earn a scroll.
            */}
            <TrailSection />
          </div>

          {/*
            Pinned, not scrolled with the rest, and now the fold alone.

            "Park an idea" used to be the loud row above it, kept here so a busy city could never
            push it out of reach. That guarantee cost it its heading: last in the panel, it sat
            under the trail and read as something you did to "Where was I?". It has moved into the
            section it belongs to, under the heading that names it.

            What is left is the quiet one, and the footer still suits it: folding the toolbar has
            nothing to do with any section above, and it is reached perhaps twice a session, so a
            fixed place at the bottom is exactly where it should wait.
          */}
          {foldVisible && (
            <div className={styles.panelFooter}>
              <ToolbarFold />
            </div>
          )}
        </div>
      )}

      {/*
        Beside the panel rather than over it: editing an idea is usually a small correction, and
        losing sight of the list you picked it from to make one is a worse trade than a wider
        footprint. It is anchored to this mod's own panel, not to anything the game owns, so it
        cannot land on a vanilla control the way the fold button once did.
      */}
      {open && editing && (
        // Keyed by the idea, so opening the card on a different one builds a fresh card rather
        // than reusing this one with the previous idea's words still in its fields - and so the
        // outgoing card's save-on-unmount runs before the new one appears.
        <IdeaEditor key={editing.id} idea={editing} onClose={() => setEditingId(null)} />
      )}
    </div>
  );
};
