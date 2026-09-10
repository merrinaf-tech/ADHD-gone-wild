import React from "react";
import { useValue } from "cs2/api";
import { dismissHyperfocus, hyperfocus$, snoozeHyperfocus } from "./bindings";
import { useLocalization } from "cs2/l10n";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The Hyperfocus Guardian, as the player meets it: a clock and a sentence.
 *
 * Everything this does not say is deliberate. It does not suggest stopping, does not ask how the
 * player is feeling, does not mention breaks, water or posture, and has no opinion about the
 * length of a session. Advice would make it a productivity tool with a caring voice, which is the
 * thing this mod exists in opposition to.
 *
 * What hyperfocus takes is the sense of time passing. So this hands it back, in the largest type
 * on the card, and then gets out of the way. Reading "23:40" and carrying on building is a fine
 * outcome - the player now knows something they did not know a second ago, which was the entire
 * job.
 *
 * It never covers the toolbar, never pauses the game, and closing it costs one click.
 */
export const HyperfocusCard = () => {
  const state = useValue(hyperfocus$);
  const text = useText();
  const { translate } = useLocalization();

  if (!state.visible) {
    return null;
  }

  // Sent as a key rather than a sentence, and rotated on the C# side. An unknown key shows
  // nothing at all rather than a raw identifier: a missing translation should quietly cost the
  // second line, never turn the card into gibberish.
  const note = state.noteKey ? translate(state.noteKey, null) : null;

  return (
    <div className={styles.hyperfocusCard}>
      {/* The one fact worth being large. */}
      <div className={styles.hyperfocusClock}>{state.clock}</div>

      <div className={styles.hyperfocusElapsed}>
        {`${text(K.hyperfocusHereFor)} ${elapsed(state.minutes, text)}`}
      </div>

      {/*
        The word about the body. Second, smaller, and written to sound like it comes from
        something on your side - see LocaleEN for why the first, more indirect version had to go.
        It is the part most likely to wear out its welcome, so it is also the part with its own
        switch in the options.
      */}
      {note && <div className={styles.hyperfocusNote}>{note}</div>}

      <div className={styles.hyperfocusActions}>
        <button className={styles.textButton} onClick={snoozeHyperfocus}>
          {text(K.hyperfocusLater)}
        </button>
        <button className={styles.textButton} onClick={dismissHyperfocus}>
          {text(K.hyperfocusThanks)}
        </button>
      </div>
    </div>
  );
};

/**
 * "2 hours", "90 minutes". Rounded down, never to the minute: the point is the order of
 * magnitude the player has lost track of, and a precise figure invites reading it as a score.
 *
 * Built as one string rather than as several nodes - Gameface lays sibling text nodes out on
 * separate lines, which is how "5 muted" once arrived as two stacked words.
 */
const elapsed = (minutes: number, text: (key: string) => string) => {
  if (minutes < 60) {
    return `${Math.max(minutes, 1)} ${text(K.hyperfocusMinutesMany)}`;
  }

  const hours = Math.floor(minutes / 60);
  return `${hours} ${text(hours === 1 ? K.hyperfocusHourOne : K.hyperfocusHoursMany)}`;
};
