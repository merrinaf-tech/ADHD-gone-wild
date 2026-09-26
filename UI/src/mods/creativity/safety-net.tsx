import React, { useEffect, useState } from "react";
import { useValue } from "cs2/api";
import {
  checkpoint$,
  createCheckpoint,
  forgetCheckpoint,
  restoreCheckpoint,
  safetyNetEnabled$,
} from "./bindings";
import { ReturnGlyph } from "theme/glyphs";
import { SECTION_STRIPE_NEUTRAL, Status, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { timeAgo, useText } from "theme/use-text";
import styles from "../panel.module.scss";
import { EmptyLine, Section } from "../section";

/**
 * The way back from an experiment.
 *
 * With no checkpoint it offers to make one. With a checkpoint it says, plainly, that there is a
 * way back - and that sentence is the feature. A save the player has forgotten gives them no
 * courage to try anything; keeping the reassurance visible is the whole reason this exists rather
 * than just telling them to save the game.
 *
 * What it must never turn into is an errand. There is no count, nothing expires loudly, and
 * "keep what I did" is an option rather than a step - a player who ignores this forever loses
 * nothing, and is never reminded that they ignored it.
 *
 * Going back is the one irreversible thing in the panel, so it is not the loudest thing in it. It
 * was a full-width button, the most prominent control on screen, and one click reloaded the city
 * and discarded everything since. For an impulsive click that is the worst possible default. It is
 * now a quiet action that asks once, in place - the single confirmation the product principles
 * allow for, because this is the single action that cannot be taken back.
 */
export const SafetyNet = () => {
  const enabled = useValue(safetyNetEnabled$);
  const checkpoint = useValue(checkpoint$);
  const text = useText();
  const [confirming, setConfirming] = useState(false);

  // A confirmation belongs to the checkpoint it was asked about. If that one goes away - kept,
  // replaced, or restored - the question goes with it.
  useEffect(() => {
    if (!checkpoint.exists || checkpoint.busy) {
      setConfirming(false);
    }
  }, [checkpoint.exists, checkpoint.busy, checkpoint.takenUnixUtc]);

  if (!enabled) {
    return null;
  }

  const now = Date.now();

  return (
    <Section
      icon={<ReturnGlyph size={18} />}
      title={text(K.safetyNetTitle)}
      // Green only while a way back actually exists; nothing is wrong when it does not.
      stripe={
        checkpoint.exists && !checkpoint.busy
          ? statusToken(Status.Resolved).color
          : SECTION_STRIPE_NEUTRAL
      }
    >
      {checkpoint.busy ? (
        <EmptyLine>{text(K.safetyNetWorking)}</EmptyLine>
      ) : checkpoint.exists && confirming ? (
        <div className={styles.safetyNetConfirm}>
          <span className={styles.safetyNetLine}>{text(K.safetyNetConfirm)}</span>
          <div className={styles.safetyNetActions}>
            <button className={styles.textButton} onClick={restoreCheckpoint}>
              {text(K.safetyNetConfirmYes)}
            </button>
            <button className={styles.parkButton} onClick={() => setConfirming(false)}>
              {text(K.safetyNetConfirmNo)}
            </button>
          </div>
        </div>
      ) : checkpoint.exists ? (
        // One line: that there is a way back, and when it is from. The two actions sit at the
        // right, both quiet - neither is the thing this panel wants the player to do.
        <div className={styles.safetyNetHave}>
          <span className={styles.rowText}>
            <span className={styles.safetyNetLine}>{text(K.safetyNetHave)}</span>
            <span className={styles.rowWhen}>{timeAgo(checkpoint.takenUnixUtc, now)}</span>
          </span>
          <button className={styles.textButton} onClick={() => setConfirming(true)}>
            {text(K.safetyNetRestore)}
          </button>
          <button className={styles.textButton} onClick={forgetCheckpoint}>
            {text(K.safetyNetKeep)}
          </button>
        </div>
      ) : (
        /*
          The sentence first, the button under it, both across the full width. This branch used to
          borrow the panel's footer class, which is a row: the button ended up as a narrow column
          with its label broken over two lines and the explanation crushed against its side.
        */
        <div className={styles.safetyNetEmpty}>
          <div className={styles.safetyNetHint}>{text(K.safetyNetHint)}</div>
          <button className={styles.parkButton} onClick={createCheckpoint}>
            {text(K.safetyNetCreate)}
          </button>
        </div>
      )}
    </Section>
  );
};
