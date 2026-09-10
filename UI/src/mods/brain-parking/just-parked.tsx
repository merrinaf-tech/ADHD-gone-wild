import React, { useEffect, useState } from "react";
import { useValue } from "cs2/api";
import {
  dismissJustParked,
  ideas$,
  justParked$,
  setIdeaCategory,
  setIdeaNote,
} from "./bindings";
import { CategoryGlyph } from "theme/glyphs";
import { usePalette } from "theme/palette";
import { IDEA_CATEGORIES, Status, ideaColour, statusToken } from "theme/tokens";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The offer to say a little more about the thought that was just parked.
 *
 * It appears *after* the idea is already saved, never before. That order is the whole point: the
 * thought is safe the instant the click lands, and everything here - the kind, a word of title -
 * is optional. Ignoring this card entirely loses nothing, which is what keeps the promise that
 * parking a thought never costs more than a shortcut and a click.
 *
 * So: no confirm button, no required field, and closing it is not cancelling anything.
 */
export const JustParked = () => {
  const id = useValue(justParked$);
  const ideas = useValue(ideas$);
  const palette = usePalette();
  const text = useText();

  const [title, setTitle] = useState("");

  const idea = ideas.find((x) => x.id === id);

  // A fresh idea starts with an empty title; reset when a different one is parked so the previous
  // one's half-typed words never leak into it.
  useEffect(() => setTitle(idea ? idea.note : ""), [id]);

  if (!id || !idea) {
    return null;
  }

  const token = statusToken(palette, Status.Personal);

  const commit = (value: string) => {
    setIdeaNote(id, value.trim());
    dismissJustParked();
  };

  return (
    <div className={styles.justParkedCard}>
      <div className={styles.justParkedHeader} style={{ color: token.color }}>
        {text(K.justParkedSaved)}
      </div>

      <div className={styles.justParkedKinds}>
        {IDEA_CATEGORIES.map((category) => {
          const active = category === idea.category;
          return (
            <button
              key={category}
              className={
                active
                  ? `${styles.justParkedKind} ${styles.justParkedKindActive}`
                  : styles.justParkedKind
              }
              style={active ? { color: ideaColour(category) } : undefined}
              onClick={() => setIdeaCategory(id, category)}
            >
              <CategoryGlyph category={category} size={22} />
              <span className={styles.justParkedKindLabel}>{text(K.category[category])}</span>
            </button>
          );
        })}
      </div>

      {/*
        stopPropagation on every key is what keeps typing out of the game: without it the letters
        reach the world as shortcuts and the city starts building things. Enter accepts, Escape
        walks away - and walking away still leaves the idea exactly where it was parked.
      */}
      <input
        className={styles.justParkedInput}
        value={title}
        placeholder={text(K.notePlaceholder)}
        onChange={(e) => setTitle(e.currentTarget.value)}
        onKeyDown={(e) => {
          e.stopPropagation();
          if (e.key === "Enter") {
            commit(e.currentTarget.value);
          }
          if (e.key === "Escape") {
            dismissJustParked();
          }
        }}
      />

      <div className={styles.justParkedActions}>
        <button className={styles.textButton} onClick={() => commit(title)}>
          {text(K.close)}
        </button>
      </div>
    </div>
  );
};
