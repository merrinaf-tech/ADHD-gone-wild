import { IdeaColour } from "theme/tokens";

/**
 * An idea's description, as a list of lines that each carry the colour of their bullet.
 *
 * The colour is on the dot, never on the words. A list where every line is a different colour of
 * prose costs the eye its whole budget before a single word has been read, which is the opposite
 * of what colour coding is for - so the text stays in the panel's ordinary ink and the dot beside
 * it does the sorting.
 *
 * It is all stored in the one description string the C# side already had, because nothing over
 * there ever looks inside it: the whole format lives in this file and is never shown to anybody.
 */
export interface DescriptionLine {
  colour: IdeaColour;
  text: string;
}

/**
 * `3|some text`, one line each.
 *
 * Decoding splits on the FIRST separator only, so a player who types "3|whatever" into a line
 * gets exactly that back - the marker this writes is always in front of whatever they wrote, and
 * only the front is ever read.
 *
 * A line with no marker is text written before bullets existed, or by some other build. It comes
 * back as a plain bullet, which keeps every word and loses only a colour nobody had chosen.
 */
const MARKER = /^([0-8])\|/;

export function parseDescription(stored: string): DescriptionLine[] {
  if (!stored) {
    return [];
  }

  return stored.split("\n").map((line) => {
    const match = MARKER.exec(line);

    if (!match) {
      return { colour: IdeaColour.Plain, text: line };
    }

    return {
      colour: Number(match[1]) as IdeaColour,
      text: line.slice(match[0].length),
    };
  });
}

/**
 * Back to one string. Empty lines at the end are dropped: they are what a player leaves behind by
 * adding a line and then deciding against it, and keeping them would mean an idea slowly filling
 * with blank bullets nobody asked for.
 */
export function formatDescription(lines: DescriptionLine[]): string {
  const kept = [...lines];

  while (kept.length > 0 && kept[kept.length - 1].text.trim().length === 0) {
    kept.pop();
  }

  return kept.map((line) => `${line.colour}|${line.text}`).join("\n");
}
