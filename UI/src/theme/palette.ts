import { bindValue, useValue } from "cs2/api";
import { PaletteId } from "./tokens";

/**
 * Which set of status colours the whole UI is drawing in.
 *
 * Shared rather than per-feature: two panels disagreeing about what "important" looks like would
 * undo the point of having a semantic palette at all.
 */
export const palette$ = bindValue<number>("adhd", "palette", 0);

export function usePalette(): PaletteId {
  return useValue(palette$) as PaletteId;
}
