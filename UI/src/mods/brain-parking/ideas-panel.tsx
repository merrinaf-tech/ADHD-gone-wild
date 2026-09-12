import React, { useEffect, useRef, useState } from "react";
import { useValue } from "cs2/api";
import { Idea, forgetIdea, ideas$, jumpToIdea, setIdeaCategory } from "./bindings";
import { CategoryGlyph } from "theme/glyphs";
import { IDEA_CATEGORIES, IdeaCategory, Status, ideaColour, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { timeAgo, useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The thoughts the player left lying around this city.
 *
 * A set of memories, not a queue of work. Nothing here has a state, a percentage or a deadline;
 * the only two things that ever happen to an entry are being looked at and being forgotten, and
 * the player decides which. Newest first, because the thing you just put down is the thing you
 * are most likely to want back.
 */
export const IdeasSection = ({
  onPark,
  focusedId,
}: {
  onPark: () => void;
  /** An idea whose ring was just clicked on the map, or empty. */
  focusedId?: string;
}) => {
  const ideas = useValue(ideas$);
  const text = useText();

  // Recomputed on render rather than ticking: nothing here needs to be accurate to the second,
  // and a timer running behind a panel nobody has open is pure cost.
  const now = Date.now();

  const [picking, setPicking] = useState<string | null>(null);

  // Ideas are the player's own, so they always carry the personal status - never a severity.
  const token = statusToken(Status.Personal);

  return (
    <>
      <div className={styles.sectionHeader}>
        <span className={styles.sectionTitle}>{text(K.panelTitle)}</span>
        {ideas.length > 0 && <span className={styles.count}>{ideas.length}</span>}
      </div>

      {ideas.length === 0 ? (
        <div className={styles.empty}>
          <span>{text(K.empty)}</span>
          <span className={styles.emptyHint}>{text(K.emptyHint)}</span>
        </div>
      ) : (
        <div className={styles.list}>
          {ideas.map((idea) => (
            <IdeaRow
              key={idea.id}
              idea={idea}
              now={now}
              colour={ideaColour(idea.category)}
              focused={focusedId === idea.id}
              picking={picking === idea.id}
              onTogglePicking={() => setPicking(picking === idea.id ? null : idea.id)}
              text={text}
            />
          ))}
        </div>
      )}

      <div className={styles.panelFooter}>
        <button className={styles.parkButton} onClick={onPark}>
          <CategoryGlyph category={IdeaCategory.Idea} size={18} color={token.color} />
          {text(K.parkIdea)}
        </button>
      </div>
    </>
  );
};

interface RowProps {
  idea: Idea;
  now: number;
  colour: string;
  focused: boolean;
  picking: boolean;
  onTogglePicking: () => void;
  text: (key: string) => string;
}

const IdeaRow = ({
  idea,
  now,
  colour,
  focused,
  picking,
  onTogglePicking,
  text,
}: RowProps) => {
  const label = idea.note && idea.note.length > 0 ? idea.note : text(K.category[idea.category]);
  const row = useRef<HTMLDivElement>(null);

  // Bring the clicked one into view when the list is longer than the panel. Guarded because
  // Coherent Gameface is not a browser and does not implement all of the DOM: if scrollIntoView
  // is missing, the row is still highlighted and the player can still scroll to it themselves,
  // which is a smaller loss than a thrown error taking the whole panel down.
  useEffect(() => {
    if (!focused || !row.current) {
      return;
    }

    try {
      row.current.scrollIntoView({ block: "nearest" });
    } catch {
      // Nothing to do, and nothing worth saying.
    }
  }, [focused]);

  return (
    <>
      <div
        ref={row}
        className={focused ? `${styles.row} ${styles.rowFocused}` : styles.row}
        onClick={() => jumpToIdea(idea.id)}
      >
        <span className={styles.severityBar} style={{ background: colour }} />

        <button
          className={styles.categoryButton}
          style={{ color: colour }}
          onClick={(e) => {
            e.stopPropagation();
            onTogglePicking();
          }}
        >
          <CategoryGlyph category={idea.category} size={22} color={colour} />
        </button>

        <span className={styles.rowText}>
          <span className={styles.rowLabel}>{label}</span>
          <span className={styles.rowWhen}>{timeAgo(idea.createdUnixUtc, now)}</span>
        </span>

        <span className={styles.rowActions}>
          <button
            className={styles.textButton}
            onClick={(e) => {
              e.stopPropagation();
              jumpToIdea(idea.id);
            }}
          >
            {text(K.view)}
          </button>
          <button
            className={styles.textButton}
            onClick={(e) => {
              e.stopPropagation();
              forgetIdea(idea.id);
            }}
          >
            {text(K.forget)}
          </button>
        </span>
      </div>

      {picking && (
        <div className={styles.categoryPicker}>
          {IDEA_CATEGORIES.map((category) => (
            <button
              key={category}
              className={
                category === idea.category
                  ? `${styles.categoryOption} ${styles.categoryOptionActive}`
                  : styles.categoryOption
              }

              onClick={(e) => {
                e.stopPropagation();
                setIdeaCategory(idea.id, category);
                onTogglePicking();
              }}
            >
              <CategoryGlyph
                category={category}
                size={22}
                color={category === idea.category ? colour : undefined}
              />
            </button>
          ))}
        </div>
      )}
    </>
  );
};
