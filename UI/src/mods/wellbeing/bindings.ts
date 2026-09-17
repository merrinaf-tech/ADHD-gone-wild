import { bindValue, trigger } from "cs2/api";

const GROUP = "adhd";

/**
 * Where the card is. Mirrors HyperfocusCorner on the C# side - appended to, never renumbered.
 *
 * The C# side decides which one, and rotates so that no two cards in a row land in the same
 * place. Top-left is absent on purpose: it is where the mod's own panel opens.
 */
export enum HyperfocusCorner {
  BottomLeft = 0,
  TopRight = 1,
  BottomRight = 2,
}

/** Mirrors HyperfocusSystem.Write. Keep the two in step. */
export interface Hyperfocus {
  visible: boolean;
  /** The wall clock at the moment the card appeared, already formatted. */
  clock: string;
  /** Minutes spent in a city this run, at that same moment. */
  minutes: number;
  /** Localisation key for the word about the body. Empty when the player asked for just the time. */
  noteKey: string;
  corner: HyperfocusCorner;
}

export const EMPTY_HYPERFOCUS: Hyperfocus = {
  visible: false,
  clock: "",
  minutes: 0,
  noteKey: "",
  corner: HyperfocusCorner.BottomLeft,
};

export const hyperfocus$ = bindValue<Hyperfocus>(GROUP, "hyperfocus", EMPTY_HYPERFOCUS);
export const hyperfocusEnabled$ = bindValue<boolean>(GROUP, "hyperfocusEnabled", true);

/** "Thanks." Say it again after the usual gap. */
export const dismissHyperfocus = () => trigger(GROUP, "dismissHyperfocus");

/** "Later." Say it again shortly. */
export const snoozeHyperfocus = () => trigger(GROUP, "snoozeHyperfocus");
