/**
 * The mod's semantic design tokens.
 *
 * Colour means STATUS. The icon means SUBJECT. Electricity is not "yellow"; an electricity
 * problem worth watching is. Nothing outside this file is allowed to write a colour, so the day
 * a palette changes it changes in one place.
 *
 * Accessibility: colour is never the only carrier of meaning. Every status here also has a
 * shape and a label, and everything that shows a status also shows a subject icon. A player who
 * cannot separate the reds from the greens still reads the interface from shape, icon and text.
 *
 * The Status numbering matches ADHDGoneWild.Core.Status on the C# side. Keep them in step.
 */

export const enum Status {
  Immediate = 0,
  Important = 1,
  Monitor = 2,
  Muted = 3,
  Personal = 4,
  Resolved = 5,
}

/** The shape drawn behind a status. Redundant with colour, on purpose. */
export type StatusShape = "disc" | "triangle" | "square" | "bar" | "diamond" | "check";

export interface StatusToken {
  /** Fill for the status marker. */
  color: string;
  /** Ink drawn on top of that fill. */
  ink: string;
  /** A translucent wash for the row the status belongs to. */
  wash: string;
  shape: StatusShape;
}

type StatusTokens = { [K in Status]: StatusToken };

/**
 * Warm hues climb towards red as a situation gets more immediate; purple is reserved entirely for
 * things the player made, and is never used for a problem.
 *
 * There used to be a second, Okabe-Ito palette behind an accessibility option. It was removed: the
 * two sets really were different, but the only coloured surfaces in the panel are a 4rem bar and a
 * 16rem glyph, so switching between them changed almost nothing anyone could see. The meaning is
 * carried by the sentence on the row - "Part of the city is without electricity" against
 * "Electricity demand is close to production" - and always was. Colour is decoration here, which
 * is why losing the alternative set costs nothing.
 */
const STANDARD: StatusTokens = {
  [Status.Immediate]: { color: "#ff4d4d", ink: "#ffffff", wash: "rgba(255,77,77,0.15)", shape: "disc" },
  [Status.Important]: { color: "#ff9d2e", ink: "#1b1b1b", wash: "rgba(255,157,46,0.10)", shape: "triangle" },
  [Status.Monitor]: { color: "#ffd93d", ink: "#1b1b1b", wash: "rgba(255,217,61,0.06)", shape: "square" },
  [Status.Muted]: { color: "#9aa4ae", ink: "#ffffff", wash: "rgba(154,164,174,0.04)", shape: "bar" },
  [Status.Personal]: { color: "#b98cff", ink: "#ffffff", wash: "rgba(185,140,255,0.08)", shape: "diamond" },
  [Status.Resolved]: { color: "#4fd18b", ink: "#1b1b1b", wash: "rgba(79,209,139,0.14)", shape: "check" },
};

export function statusToken(status: Status): StatusToken {
  return STANDARD[status] ?? STANDARD[Status.Muted];
}

/**
 * The subject icons. Matches ADHDGoneWild.BrainParking.IdeaCategory; the numbers are written to
 * disk on the C# side, so append here rather than renumbering.
 */
export const enum IdeaCategory {
  Idea = 0,
  Build = 1,
  Decoration = 2,
  Transport = 3,
  Fix = 4,
  Other = 5,
}

/**
 * What an alert is about. Mirrors ADHDGoneWild.Alerts.AlertSubject; keep the two in step.
 *
 * Other is not a failure mode. The game has more notification types than this list names, and an
 * unrecognised one still appears with the game's own label - it just borrows a generic icon.
 */
export const enum AlertSubject {
  Other = 0,
  Electricity = 1,
  Water = 2,
  Sewage = 3,
  Garbage = 4,
  Health = 5,
  Deathcare = 6,
  Fire = 7,
  Police = 8,
  Education = 9,
  Transport = 10,
  Mail = 11,
  Traffic = 12,
  Housing = 13,
  Workplace = 14,
}

/**
 * A colour per kind of idea, matching the rings drawn on the map.
 *
 * A deliberate, scoped exception to "colour means status": every parked idea shares one status -
 * the player's own - so within that family hue is free to carry the kind instead. Nothing in the
 * alert list is ever coloured by its subject.
 *
 * Kept in step by hand with ADHDGoneWild/BrainParking/IdeaColours.cs, which the engine uses for
 * the rings.
 */
export const IDEA_COLOURS: { [K in IdeaCategory]: string } = {
  [IdeaCategory.Idea]: "#b98cff",
  [IdeaCategory.Build]: "#ffb547",
  [IdeaCategory.Decoration]: "#6fd47e",
  [IdeaCategory.Transport]: "#58b6ff",
  [IdeaCategory.Fix]: "#ff8a5c",
  [IdeaCategory.Other]: "#b9c2cc",
};

export function ideaColour(category: IdeaCategory): string {
  return IDEA_COLOURS[category] ?? IDEA_COLOURS[IdeaCategory.Idea];
}

/**
 * A colour for one bullet in an idea's description.
 *
 * Mirrors IdeaColour on the C# side - appended to, never renumbered, and the numbers are written
 * into the stored description. The colour is on the dot and never on the words: a list in eight
 * colours of prose is harder to read than one that is all white, which is the opposite of what
 * colour coding is for.
 */
export enum IdeaColour {
  Plain = 0,
  Purple = 1,
  Blue = 2,
  Green = 3,
  Yellow = 4,
  Orange = 5,
  Red = 6,
  Pink = 7,
  Grey = 8,
}

const BULLET_COLOURS: Partial<Record<IdeaColour, string>> = {
  [IdeaColour.Purple]: "#b98cff",
  [IdeaColour.Blue]: "#58b6ff",
  [IdeaColour.Green]: "#6fd47e",
  [IdeaColour.Yellow]: "#ffd93d",
  [IdeaColour.Orange]: "#ff9d2e",
  // Lighter and pinker than the #ff4d4d of an immediate alert, so a red bullet does not read as a
  // red warning at a glance.
  [IdeaColour.Red]: "#ff6b6b",
  [IdeaColour.Pink]: "#ff8ad4",
  [IdeaColour.Grey]: "#b9c2cc",
};

/** In the order the swatches appear. Plain comes first, as the way back. */
export const IDEA_COLOURS_ORDER: IdeaColour[] = [
  IdeaColour.Plain,
  IdeaColour.Purple,
  IdeaColour.Blue,
  IdeaColour.Green,
  IdeaColour.Yellow,
  IdeaColour.Orange,
  IdeaColour.Red,
  IdeaColour.Pink,
  IdeaColour.Grey,
];

/**
 * A bullet's colour, or nothing at all.
 *
 * `Plain` really is no bullet rather than a faint one. A line without a dot is a line the player
 * decided needs no marking, and putting a grey dot there anyway would make "unmarked" into its own
 * ninth colour - one more thing on screen saying nothing, in a mod about having less of those.
 */
export function bulletColour(colour: IdeaColour): string {
  return BULLET_COLOURS[colour] ?? "transparent";
}

export const IDEA_CATEGORIES: IdeaCategory[] = [
  IdeaCategory.Idea,
  IdeaCategory.Build,
  IdeaCategory.Decoration,
  IdeaCategory.Transport,
  IdeaCategory.Fix,
  IdeaCategory.Other,
];
