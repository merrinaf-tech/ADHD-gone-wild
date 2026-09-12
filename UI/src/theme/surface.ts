import { bindValue, useValue } from "cs2/api";

/**
 * The background of the mod's own panels, as a finished CSS colour.
 *
 * Sent whole rather than as a hue: the conversion lives once, in C#, in InterfaceColour, where it
 * is tested outside the game. Nothing here has to know that only the hue is the player's or what
 * the fixed saturation and lightness are.
 *
 * The stylesheet still carries a `background` on each of these panels. That is not redundant - it
 * is what they are painted with until this binding arrives, and what they fall back to if it never
 * does. A panel with no background at all would be unreadable text floating over the map.
 */
export const surface$ = bindValue<string>("adhd", "surface", "rgba(34, 49, 65, 0.98)");

export function useSurface(): string {
  return useValue(surface$);
}
