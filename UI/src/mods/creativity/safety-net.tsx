import React from "react";
import { useValue } from "cs2/api";
import {
  checkpoint$,
  createCheckpoint,
  forgetCheckpoint,
  restoreCheckpoint,
  safetyNetEnabled$,
} from "./bindings";
import { K } from "theme/l10n";
import { timeAgo, useText } from "theme/use-text";
import styles from "../panel.module.scss";

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
 */
export const SafetyNet = () => {
  const enabled = useValue(safetyNetEnabled$);
  const checkpoint = useValue(checkpoint$);
  const text = useText();

  if (!enabled) {
    return null;
  }

  const now = Date.now();

  return (
    <>
      <div className={styles.sectionHeader}>
        <span className={styles.sectionTitle}>{text(K.safetyNetTitle)}</span>
      </div>

      {checkpoint.busy ? (
        <div className={styles.empty}>
          <span className={styles.emptyHint}>{text(K.safetyNetWorking)}</span>
        </div>
      ) : checkpoint.exists ? (
        <div className={styles.safetyNetHave}>
          <span className={styles.safetyNetLine}>{text(K.safetyNetHave)}</span>
          <span className={styles.rowWhen}>{timeAgo(checkpoint.takenUnixUtc, now)}</span>

          <div className={styles.safetyNetActions}>
            <button className={styles.parkButton} onClick={restoreCheckpoint}>
              {text(K.safetyNetRestore)}
            </button>
            <button className={styles.textButton} onClick={forgetCheckpoint}>
              {text(K.safetyNetKeep)}
            </button>
          </div>
        </div>
      ) : (
        <div className={styles.panelFooter}>
          <button className={styles.parkButton} onClick={createCheckpoint}>
            {text(K.safetyNetCreate)}
          </button>
          <div className={styles.safetyNetHint}>{text(K.safetyNetHint)}</div>
        </div>
      )}
    </>
  );
};
