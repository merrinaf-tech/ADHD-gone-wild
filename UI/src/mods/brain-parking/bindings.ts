import { bindValue, trigger } from "cs2/api";
import { IdeaCategory } from "theme/tokens";

/**
 * Everything the panel is allowed to know, and everything it is allowed to ask for.
 *
 * One group, "adhd", shared with the C# side. The UI holds no state of its own about ideas: the
 * repository on the other side is the only copy, so a marker cannot exist on screen and not on
 * disk.
 */

/** Mirrors the record written by BrainParkingUISystem.WriteIdeas. Keep the two in step. */
export interface Idea {
  id: string;
  category: IdeaCategory;
  /** The one line on the row. Optional: typing is never required to park a thought. */
  note: string;
  /**
   * The notes under the idea, read and written in the editor rather than on the row. Usually
   * empty. One per line, each carrying its bullet's colour - see description.ts for the format.
   */
  description: string;
  createdUnixUtc: number;
  x: number;
  z: number;
}

const GROUP = "adhd";

export const ideas$ = bindValue<Idea[]>(GROUP, "ideas", []);
export const parking$ = bindValue<boolean>(GROUP, "parking", false);
export const brainParkingEnabled$ = bindValue<boolean>(GROUP, "brainParkingEnabled", true);

/**
 * Whether a row says how long ago its idea was parked. Off unless the player asked for it - see
 * AdhdSettings.ShowIdeaAge for why an age is the one thing that can turn a note into a debt.
 */
export const showIdeaAge$ = bindValue<boolean>(GROUP, "showIdeaAge", false);
/** Id of the idea just parked, or empty. Non-empty means the refine card is offered. */
export const justParked$ = bindValue<string>(GROUP, "justParked", "");

export const startParking = () => trigger(GROUP, "startParking");
export const cancelParking = () => trigger(GROUP, "cancelParking");
/** Screen coordinates - DOM client X/Y from the click event, not world space. */
export const parkAt = (screenX: number, screenY: number) => trigger(GROUP, "parkAt", screenX, screenY);
export const forgetIdea = (id: string) => trigger(GROUP, "forgetIdea", id);
export const jumpToIdea = (id: string) => trigger(GROUP, "jumpToIdea", id);
export const setIdeaCategory = (id: string, category: IdeaCategory) =>
  trigger(GROUP, "setIdeaCategory", id, category);
export const setIdeaNote = (id: string, note: string) => trigger(GROUP, "setIdeaNote", id, note);
export const setIdeaDescription = (id: string, description: string) =>
  trigger(GROUP, "setIdeaDescription", id, description);
export const dismissJustParked = () => trigger(GROUP, "dismissJustParked");

/**
 * Every click on the map, offered to the C# side so it can say whether one of the rings was
 * under it. All the guards live there - see BrainParkingUISystem.ClickWorld - because the game
 * is the only thing that knows whether the cursor was over an interface or a tool was running.
 */
export const clickWorld = (screenX: number, screenY: number) =>
  trigger(GROUP, "clickWorld", screenX, screenY);

/** Id of an idea whose ring was clicked, or empty. The panel opens on it and then clears it. */
export const focusedIdea$ = bindValue<string>(GROUP, "focusedIdea", "");

export const clearIdeaFocus = () => trigger(GROUP, "clearIdeaFocus");
