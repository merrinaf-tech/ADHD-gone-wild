import { bindValue, trigger } from "cs2/api";

/**
 * "Where was I?" - the places the player spent time in, this session.
 *
 * The C# side owns the whole list and every decision behind it. This is a view of something
 * already decided: nothing here merges, promotes or forgets on its own.
 */

const GROUP = "adhd";

/** Mirrors the record written by TrailUISystem.Write. Keep the two in step. */
export interface TrailPlace {
  id: string;
  /**
   * Every kind of work seen at this place, as bits of TrailActivity. A place is usually one
   * thing and occasionally three, which is why it is a mask and not a single value.
   */
  activities: number;
  lastSeenUnixUtc: number;
  x: number;
  z: number;
}

/** Most recent first, omitting the place the camera is viewing right now. */
export const trail$ = bindValue<TrailPlace[]>(GROUP, "trail", []);

/** Off by default - the one feature in this mod the player has to ask for. */
export const trailEnabled$ = bindValue<boolean>(GROUP, "trailEnabled", false);

/** Puts the camera back where it was, framing and all - not just the spot. */
export const jumpToTrailPlace = (id: string) => trigger(GROUP, "jumpToTrailPlace", id);

export const forgetTrailPlace = (id: string) => trigger(GROUP, "forgetTrailPlace", id);
