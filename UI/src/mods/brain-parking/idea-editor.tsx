import React, { useEffect, useRef, useState } from "react";
import { Idea, setIdeaDescription, setIdeaNote } from "./bindings";
import { DescriptionLine, formatDescription, parseDescription } from "./description";
import { CategoryGlyph } from "theme/glyphs";
import { IDEA_COLOURS_ORDER, IdeaColour, bulletColour, ideaColour } from "theme/tokens";
import { useSurface } from "theme/surface";
import { TextField } from "theme/text-field";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * Changing a parked idea after the fact: its line, and the list of notes underneath it.
 *
 * Asked for by a player who wanted to fix a typo, which is a smaller request than it sounds and a
 * fair one - a thought written in four seconds while the tool was still in hand is exactly the
 * kind of thing that comes out slightly wrong. Until now the only way to correct one was to forget
 * it and park a new one somewhere approximately right.
 *
 * The description is a list rather than a block of prose, and every line has a coloured dot. That
 * is the whole of the colour feature: the words stay in the panel's ordinary ink, the idea keeps
 * the colour of its category, and the dots are there to group the lines by whatever the player
 * decides they mean. An earlier version put the colour on the text itself and it was worse - a
 * list in eight colours of prose is harder to read than one that is all white.
 *
 * Nothing here is required and nothing is a step. An idea with no line and no notes is a complete
 * idea - it is a ring on a map, which was always the whole point.
 *
 * It saves on the way out rather than behind a button. There is no version of this where somebody
 * types a paragraph, closes the card and finds it gone.
 */
export const IdeaEditor = ({ idea, onClose }: { idea: Idea; onClose: () => void }) => {
  const text = useText();
  const surface = useSurface();

  const [note, setNote] = useState(idea.note ?? "");
  // An idea with nothing written under it opens with nothing under it - no line waiting, just the
  // way to add one. A blank field sitting there is a small request to fill it in, and most parked
  // thoughts never need a word beyond the one on the ring.
  const [lines, setLines] = useState<DescriptionLine[]>(() => parseDescription(idea.description));

  /**
   * The line whose colours are currently on offer, if any.
   *
   * The nine swatches used to sit permanently under the list, which put nine coloured dots on
   * screen at all times to serve a choice made once in a while - and the whole reason the colour
   * moved off the text in the first place was that too much of it costs more than it gives.
   *
   * Now they are only there when asked for. The control is the bullet on the line itself: click it
   * and the colours appear under that line, pick one and they go away again. A line with no bullet
   * still holds that place in the row and still takes the click, which is what makes giving a line
   * a colour and taking one away the same gesture in the same spot.
   *
   * The same shape the category picker in the ideas list already uses, for the same reason.
   */
  const [picking, setPicking] = useState(-1);

  // Held in a ref so the unmount handler below sees what was actually typed. Reading the state in
  // there would close over whatever it held when the effect was created, which is the empty form.
  const latest = useRef({ note, lines });
  latest.current = { note, lines };

  // Saved once, on the way out, however the card was closed - the X, Escape, the game's Close, or the
  // panel being shut from under it. The alternative is a Save button, and a Save button is a thing
  // to forget to press.
  useEffect(
    () => () => {
      const { note: finalNote, lines: finalLines } = latest.current;

      if (finalNote.trim() !== (idea.note ?? "")) {
        setIdeaNote(idea.id, finalNote.trim());
      }

      const description = formatDescription(finalLines);

      if (description !== (idea.description ?? "")) {
        setIdeaDescription(idea.id, description);
      }
    },
    [idea.id]
  );

  const kind = ideaColour(idea.category);

  const change = (index: number, patch: Partial<DescriptionLine>) =>
    setLines((current) => current.map((line, i) => (i === index ? { ...line, ...patch } : line)));

  const addLine = (after: number) =>
    setLines((current) => {
      const next = [...current];
      next.splice(after + 1, 0, { colour: IdeaColour.Plain, text: "" });
      return next;
    });

  const removeLine = (index: number) =>
    setLines((current) => current.filter((_, i) => i !== index));

  return (
    <div className={styles.ideaEditor} style={{ background: surface }}>
      {/*
        The idea's own title, in the place a title goes, and editable where it is read. It used to
        be a fixed "This idea" with an unexplained empty box underneath holding the actual name -
        two things saying one thing, and the one that mattered was the one with no label on it.

        An idea with no title of its own shows the name of its kind, exactly as its row in the list
        does. That is the honest answer to "what is this": the same words the player just clicked
        on, rather than an invitation to name something that never needed a name.
      */}
      <div className={styles.ideaEditorHeader}>
        <CategoryGlyph category={idea.category} size={20} color={kind} />

        <TextField
          className={styles.ideaTitleInput}
          value={note}
          placeholder={text(K.category[idea.category])}
          onChange={setNote}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === "Escape") {
              onClose();
            }
          }}
        />

        <button className={styles.panelClose} onClick={onClose}>
          X
        </button>
      </div>

      <div className={styles.ideaNotes}>
        {lines.map((line, index) => (
          <React.Fragment key={index}>
            <div className={styles.ideaNote}>
              {/* The bullet is the control. On a line with no colour it is an empty space that
                  still holds its place in the row and still takes the click. */}
              <button
                className={styles.ideaBullet}
                onClick={() => setPicking(picking === index ? -1 : index)}
              >
                <span
                  className={styles.ideaBulletDot}
                  style={{ background: bulletColour(line.colour) }}
                />
              </button>

              <TextField
                className={styles.ideaNoteInput}
                value={line.text}
                placeholder={text(K.ideaDescriptionPlaceholder)}
                onChange={(value) => change(index, { text: value })}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    addLine(index);
                  }

                  // Backspace on a line that is already empty removes it, the way it would in any
                  // list anywhere. Never the last one: an empty list with no line in it has
                  // nowhere to start typing again.
                  if (e.key === "Backspace" && line.text.length === 0 && lines.length > 1) {
                    removeLine(index);
                    setPicking(-1);
                  }

                  if (e.key === "Escape") {
                    onClose();
                  }
                }}
              />

              <button className={styles.ideaNoteRemove} onClick={() => removeLine(index)}>
                X
              </button>
            </div>

            {picking === index && (
              <div className={styles.ideaBulletPicker}>
                {IDEA_COLOURS_ORDER.map((colour) => (
                  <button
                    key={colour}
                    className={
                      colour === IdeaColour.Plain
                        ? `${styles.ideaSwatch} ${styles.ideaSwatchNone}`
                        : styles.ideaSwatch
                    }
                    style={{ background: bulletColour(colour) }}
                    onClick={() => {
                      change(index, { colour });
                      setPicking(-1);
                    }}
                  >
                    {/* A ring marks the one this line already wears. Not a tick, which would have
                        to be dark on the pale colours and pale on the dark ones. */}
                    {line.colour === colour && <span className={styles.ideaSwatchRing} />}
                  </button>
                ))}
              </div>
            )}
          </React.Fragment>
        ))}

        <button className={styles.ideaNoteAdd} onClick={() => addLine(lines.length - 1)}>
          {text(K.ideaAddNote)}
        </button>
      </div>

    </div>
  );
};
