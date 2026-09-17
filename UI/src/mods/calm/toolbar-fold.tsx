import React from "react";
import { useValue } from "cs2/api";
import { setToolbarCollapsed, toolbarButtonVisible$, toolbarCollapsed$ } from "./bindings";
import { ChevronGlyph } from "theme/glyphs";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The control that folds the game's build toolbar away, and brings it back.
 *
 * It lives in this mod's own panel, at the bottom, and that placement is the feature. It floated
 * over the map for three releases and was in the wrong place every time: first inside a column the
 * game sets `pointer-events: none` on, so it could not be clicked at all; then inside the very
 * region folding slides away, so it hid itself and left one player restarting the game to get
 * their toolbar back; then at a fixed offset that lands on top of a vanilla button at 1280x800,
 * where a player gave up and switched the feature off.
 *
 * Three placements, three faults, one cause: any fixed coordinate over the game's own UI is wrong
 * for somebody, and the mod cannot know whose. Making it draggable would only move that decision
 * onto the player - who has to discover they can drag it first, and who wanted *less* on screen,
 * not one more floating thing.
 *
 * Inside the panel there is nothing to collide with, ever. It costs a click to reach, which is the
 * honest price of a control that cannot be in the wrong place.
 *
 * It is a labelled row rather than an icon, and it is not the game's `Button`. Beside "Park an
 * idea" it was a round floating chevron next to a wide labelled button: two controls that have
 * nothing to do with each other, sharing a line, in two different visual languages. Now it is a
 * quiet row under the loud one - same width, same alignment, plainly a different kind of thing.
 *
 * The way back is still guaranteed: the panel opens from the owl in the top-left row, which sits
 * outside the part of the screen that folds.
 */
export const ToolbarFold = () => {
  const visible = useValue(toolbarButtonVisible$);
  const collapsed = useValue(toolbarCollapsed$);
  const text = useText();

  if (!visible) {
    return null;
  }

  return (
    <button className={styles.foldButton} onClick={() => setToolbarCollapsed(!collapsed)}>
      <ChevronGlyph size={16} up={collapsed} />
      <span className={styles.foldLabel}>
        {text(collapsed ? K.toolbarUnfold : K.toolbarFold)}
      </span>
    </button>
  );
};
