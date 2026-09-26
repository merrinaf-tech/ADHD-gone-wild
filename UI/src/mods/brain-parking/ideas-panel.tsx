import React, { useEffect, useRef, useState } from "react";
import { useValue } from "cs2/api";
import { Tooltip } from "cs2/ui";
import {
  Idea,
  forgetIdea,
  ideas$,
  jumpToIdea,
  setIdeaCategory,
  showIdeaAge$,
} from "./bindings";
import { CategoryGlyph } from "theme/glyphs";
import { IDEA_CATEGORIES, IdeaCategory, Status, ideaColour, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { timeAgo, useText } from "theme/use-text";
import styles from "../panel.module.scss";
import { EmptyLine, Section } from "../section";

/**
 * The thoughts the player left lying around this city.
 *
 * A set of memories, not a queue of work. Nothing here has a state, a percentage or a deadline;
 * the only two things that ever happen to an entry are being looked at and being forgotten, and
 * the player decides which. Newest first, because the thing you just put down is the thing you
 * are most likely to want back.
 */
export const IdeasSection = ({
  focusedId,
  onEdit,
  onPark,
}: {
  /** An idea whose ring was just clicked on the map, or empty. */
  focusedId?: string;
  /**
   * Asked to open the editor on one. The card itself is the panel's to own rather than this
   * section's: while it is open the panel must not close on the game's Close action, and only something
   * above both of them can know that.
   */
  onEdit: (id: string) => void;
  /**
   * Asked to park a new one. The panel has to get out of the way first - the next click belongs to
   * the map - and only the panel can do that, so the action is handed down rather than run here.
   */
  onPark: () => void;
}) => {
  const ideas = useValue(ideas$);
  const showAge = useValue(showIdeaAge$);
  const text = useText();

  // Recomputed on render rather than ticking: nothing here needs to be accurate to the second,
  // and a timer running behind a panel nobody has open is pure cost.
  const now = Date.now();

  const [picking, setPicking] = useState<string | null>(null);

  // Ideas are the player's own, so they always carry the personal status - never a severity.
  const token = statusToken(Status.Personal);

  return (
    <Section
      icon={<CategoryGlyph category={IdeaCategory.Idea} size={18} />}
      title={text(K.panelTitle)}
      stripe={token.color}
      trailing={ideas.length > 0 && <span className={styles.count}>{ideas.length}</span>}
    >

      {/*
        Directly under the heading, above the list, and both halves of that are deliberate.

        It lived in the panel's pinned footer, where it was the last thing in the panel - under the
        trail. Pinned meant it could never be scrolled out of reach, which was the point, but it
        also meant it sat below a section it has nothing to do with: it read as the button that
        parks "Where was I?", which is not a thing that can be parked.

        Above the list rather than below it, because a list of ideas can be long. Below, the one
        control this section is for would slide further away with every idea parked - the same
        fault as the footer, just wearing the right heading.
      */}
      <div className={styles.sectionAction}>
        {/* How it works, one hover away, on the control it explains. */}
        <Tooltip tooltip={text(K.emptyHint)}>
          <button className={styles.parkButton} onClick={onPark}>
            <CategoryGlyph category={IdeaCategory.Idea} size={18} color={token.color} />
            {text(K.parkIdea)}
          </button>
        </Tooltip>
      </div>

      {ideas.length === 0 ? (
        <EmptyLine>{text(K.empty)}</EmptyLine>
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
              onEdit={() => onEdit(idea.id)}
              showAge={showAge}
              text={text}
            />
          ))}
        </div>
      )}
    </Section>
  );
};

interface RowProps {
  idea: Idea;
  now: number;
  colour: string;
  focused: boolean;
  picking: boolean;
  onTogglePicking: () => void;
  onEdit: () => void;
  showAge: boolean;
  text: (key: string) => string;
}

const IdeaRow = ({
  idea,
  now,
  colour,
  focused,
  picking,
  onTogglePicking,
  onEdit,
  showAge,
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
      {/*
        Clicking the row opens the idea and moves nothing. "View" beside it is the one that takes
        you there.

        These were the same gesture for a while and that was wrong: reading what you wrote and
        going to where you wrote it are different wants, and fusing them meant you could not have
        the first without the second. Wanting to check a note without being thrown across the map
        is the ordinary case, so it gets the ordinary click - and the word "View" is left meaning
        what it says on every other row in this panel.
      */}
      <div
        ref={row}
        className={focused ? `${styles.row} ${styles.rowFocused}` : styles.row}
        onClick={() => onEdit()}
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
          {/*
            The second line, when there is anything true enough to put on it.

            An age is only here if the player asked for one: see AdhdSettings.ShowIdeaAge. Built as
            one string rather than as several nodes - Gameface lays sibling text nodes out on
            separate lines, which is how "5 muted" once arrived as two stacked words.
          */}
          {subtitle(idea, now, showAge, text) && (
            <span className={styles.rowWhen}>{subtitle(idea, now, showAge, text)}</span>
          )}
        </span>

        <span className={styles.rowActions}>
          {/* The one that travels. The row itself opens the card and stays put. */}
          <button
            className={styles.textButton}
            onClick={(e) => {
              e.stopPropagation();
              jumpToIdea(idea.id);
              onEdit();
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

/**
 * What goes under an idea's name: that it has notes, how old it is, both, or nothing at all.
 *
 * Nothing at all is a perfectly good answer and the commonest one. A row with a single line on it
 * is a thought, and that is all a parked thought has to be.
 */
const subtitle = (
  idea: Idea,
  now: number,
  showAge: boolean,
  text: (key: string) => string
): string => {
  const parts: string[] = [];

  if (showAge) {
    parts.push(timeAgo(idea.createdUnixUtc, now));
  }

  if (idea.description && idea.description.length > 0) {
    parts.push(text(K.hasDescription));
  }

  return parts.join(" - ");
};
