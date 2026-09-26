import React, { useState } from "react";
import { useValue } from "cs2/api";
import { Tooltip } from "cs2/ui";
import { forgetTrailPlace, jumpToTrailPlace, trail$, trailEnabled$, TrailPlace } from "./bindings";
import { TrailGlyph } from "theme/glyphs";
import { Status, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { timeAgo, useText } from "theme/use-text";
import styles from "../panel.module.scss";
import { EmptyLine, Section } from "../section";

/**
 * "Where was I?" - a short trail of places this session went, most recent first. The place the
 * camera is currently viewing stays in memory but is absent from this list until the player
 * leaves it; "Take me back" should never lead to the view already on screen.
 *
 * Every line here is a statement about what happened, and none of them is a statement about what
 * should happen next. "Drawing, 8 min ago" is something the mod watched the player do. "Unfinished
 * roadworks" would be something it decided about them, and it is not entitled to that: it can see
 * a road tool was in their hand and it cannot see why.
 *
 * Which is also why there is no count beside the title, no total minutes on a row, and nothing
 * that grows. The list is capped on the C# side, the oldest place drops off the back, and the
 * whole thing disappears when the city closes. A memory that cannot outlive the session cannot
 * turn into a backlog waiting for you tomorrow.
 *
 * It carries the personal colour, like parked ideas: this is the player's own activity, never a
 * severity, and it must never look like something is wrong.
 */
export const TrailSection = () => {
  const enabled = useValue(trailEnabled$);
  const places = useValue(trail$);
  const text = useText();

  // Fixed while the panel is open. A binding update must move a revisited place to the top, but
  // it must not make every other "8 min ago" label drift to "9 min ago" while it is being read.
  const [now] = useState(() => Date.now());

  if (!enabled) {
    return null;
  }

  const token = statusToken(Status.Personal);

  return (
    <Section icon={<TrailGlyph size={18} />} title={text(K.trailTitle)} stripe={token.color}>
      {places.length === 0 ? (
        <EmptyLine>
          <Tooltip tooltip={text(K.trailEmptyHint)}>
            <span>{text(K.trailEmpty)}</span>
          </Tooltip>
        </EmptyLine>
      ) : (
        <div className={styles.list}>
          {places.map((place) => (
            <TrailRow
              key={place.id}
              place={place}
              now={now}
              colour={token.color}
              text={text}
            />
          ))}
        </div>
      )}
    </Section>
  );
};

interface RowProps {
  place: TrailPlace;
  now: number;
  colour: string;
  text: (key: string) => string;
}

const TrailRow = ({ place, now, colour, text }: RowProps) => (
  <div className={styles.row} onClick={() => jumpToTrailPlace(place.id)}>
    <span className={styles.severityBar} style={{ background: colour }} />

    <span className={styles.categoryButton} style={{ color: colour }}>
      <TrailGlyph size={22} color={colour} />
    </span>

    <span className={styles.rowText}>
      <span className={styles.rowLabel}>{activities(place.activities, text)}</span>
      <span className={styles.rowWhen}>{timeAgo(place.lastSeenUnixUtc, now)}</span>
    </span>

    <span className={styles.rowActions}>
      <button
        className={styles.textButton}
        onClick={(e) => {
          e.stopPropagation();
          jumpToTrailPlace(place.id);
        }}
      >
        {text(K.trailBack)}
      </button>
      <button
        className={styles.textButton}
        onClick={(e) => {
          e.stopPropagation();
          forgetTrailPlace(place.id);
        }}
      >
        {text(K.forget)}
      </button>
    </span>
  </div>
);

/**
 * The mask, as words. "Drawing, Demolishing" - what was done there, in the order the activities
 * are declared, never ranked by how much of each.
 *
 * Built as one string rather than as several nodes: Gameface lays sibling text nodes out on
 * separate lines, which is how "5 muted" once arrived as two stacked words.
 */
const activities = (mask: number, text: (key: string) => string): string => {
  const words: string[] = [];

  // From 1 rather than 0: "Looking around" is bit 0, it is true of nearly every place, and it
  // says the least of any of them. It is only worth printing when nothing else happened there.
  for (let i = 1; i < K.trailActivity.length; i++) {
    if ((mask & (1 << i)) !== 0) {
      words.push(text(K.trailActivity[i]));
    }
  }

  if (words.length === 0) {
    return text(K.trailActivity[0]);
  }

  // Three is where a label stops being a label. A place the player did six different things at
  // is one the words can no longer describe, and a row wrapping to three lines is worse than a
  // vague one.
  return words.slice(0, 3).join(", ");
};
