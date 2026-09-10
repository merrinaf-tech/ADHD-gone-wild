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

export const enum PaletteId {
  Standard = 0,
  ColourBlindFriendly = 1,
}

type StatusTokens = { [K in Status]: StatusToken };

/**
 * The default set. Warm hues climb towards red as a situation gets more immediate; purple is
 * reserved entirely for things the player made, and is never used for a problem.
 */
const STANDARD: StatusTokens = {
  [Status.Immediate]: { color: "#ff4d4d", ink: "#ffffff", wash: "rgba(255,77,77,0.15)", shape: "disc" },
  [Status.Important]: { color: "#ff9d2e", ink: "#1b1b1b", wash: "rgba(255,157,46,0.10)", shape: "triangle" },
  [Status.Monitor]: { color: "#ffd93d", ink: "#1b1b1b", wash: "rgba(255,217,61,0.06)", shape: "square" },
  [Status.Muted]: { color: "#9aa4ae", ink: "#ffffff", wash: "rgba(154,164,174,0.04)", shape: "bar" },
  [Status.Personal]: { color: "#b98cff", ink: "#ffffff", wash: "rgba(185,140,255,0.08)", shape: "diamond" },
  [Status.Resolved]: { color: "#4fd18b", ink: "#1b1b1b", wash: "rgba(79,209,139,0.14)", shape: "check" },
};

/**
 * Okabe-Ito derived. Red and green are the pair that most often collapses, so the severities are
 * carried by blue-to-vermillion instead, and the shapes do more of the work.
 */
const COLOUR_BLIND_FRIENDLY: StatusTokens = {
  [Status.Immediate]: { color: "#d55e00", ink: "#ffffff", wash: "rgba(213,94,0,0.16)", shape: "disc" },
  [Status.Important]: { color: "#e69f00", ink: "#1b1b1b", wash: "rgba(230,159,0,0.14)", shape: "triangle" },
  [Status.Monitor]: { color: "#f0e442", ink: "#1b1b1b", wash: "rgba(240,228,66,0.12)", shape: "square" },
  [Status.Muted]: { color: "#999999", ink: "#ffffff", wash: "rgba(153,153,153,0.10)", shape: "bar" },
  [Status.Personal]: { color: "#cc79a7", ink: "#1b1b1b", wash: "rgba(204,121,167,0.16)", shape: "diamond" },
  [Status.Resolved]: { color: "#0072b2", ink: "#ffffff", wash: "rgba(0,114,178,0.12)", shape: "check" },
};

const PALETTES: { [K in PaletteId]: StatusTokens } = {
  [PaletteId.Standard]: STANDARD,
  [PaletteId.ColourBlindFriendly]: COLOUR_BLIND_FRIENDLY,
};

export function statusToken(palette: PaletteId, status: Status): StatusToken {
  const set = PALETTES[palette] ?? STANDARD;
  return set[status] ?? set[Status.Muted];
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

export const IDEA_CATEGORIES: IdeaCategory[] = [
  IdeaCategory.Idea,
  IdeaCategory.Build,
  IdeaCategory.Decoration,
  IdeaCategory.Transport,
  IdeaCategory.Fix,
  IdeaCategory.Other,
];
