import React from "react";
import { ModRegistrar, ModuleRegistry } from "cs2/modding";
import { useValue } from "cs2/api";
import { BrainEntry } from "mods/brain-entry";
import { ParkingHint } from "mods/brain-parking/parking-hint";
import { JustParked } from "mods/brain-parking/just-parked";
import { RingClicks } from "mods/brain-parking/ring-clicks";
import { toolbarCollapsed$ } from "mods/calm/bindings";
import { WelcomeBack } from "mods/welcome-back/welcome-back";
import { HyperfocusCard } from "mods/wellbeing/hyperfocus-card";
import { GameDismissInput } from "theme/game-dismiss-input";
import styles from "mods/panel.module.scss";

/**
 * Where ADHD gone wild attaches itself to the game.
 *
 * A few small things appended to the game's own anchors, and the build toolbar extended so it can
 * be folded away.
 *
 * The button goes in the top-left row, where it behaves like the controls around it. The parking
 * hint, the "anything to add?" card, the welcome-back card and the fold control all sit on the
 * full-screen anchor: the first three because they appear on their own and leave the moment they
 * are done, and the fold control because every anchor next to the toolbar is inside the part that
 * folds.
 *
 * All state comes from the C# side over the "adhd" binding group.
 */

/**
 * Folding is mostly not something this mod draws. The vanilla toolbar takes an
 * `onlyStatusVisible` prop and animates the change itself, sliding its first child - the left
 * menu, the notification menu, the bulldozer bar - out of the way. So we do not replace it, hide
 * it or cover it; we pass it the prop it already understands.
 *
 * The incoming prop is preserved with `||` rather than overwritten: the game sets it itself in
 * some modes, and stamping over that would fight the game instead of asking it.
 */
const useCollapsed = (props: any) => {
  const collapsed = useValue(toolbarCollapsed$);
  return collapsed || props.onlyStatusVisible;
};

/**
 * The legacy toolbar keeps its row of build tools inside the same block as the side menus, so
 * asking for `onlyStatusVisible` folds everything away and leaves the status strip. Nothing more
 * to do.
 */
const foldLegacyToolbar = (Original: any) => (props: any) => (
  <Original {...props} onlyStatusVisible={useCollapsed(props)} />
);

/**
 * The newer toolbar moved the build tools in beside the status strip, so `onlyStatusVisible`
 * leaves them on screen - which is most of what the player wanted gone. There is no prop for
 * that half, so it is folded with a class of ours; see `.foldedToolbar` for how, and why only
 * this toolbar gets it.
 */
const foldToolbar = (Original: any) => (props: any) => {
  const collapsed = useCollapsed(props);
  return (
    <Original
      {...props}
      onlyStatusVisible={collapsed}
      className={collapsed ? `${props.className || ""} ${styles.foldedToolbar}` : props.className}
    />
  );
};

/**
 * The game ships two toolbars and picks between them at render time:
 * `w ? <ToolbarLegacy .../> : <Toolbar .../>`. Which one a given player gets is not ours to know,
 * so both are extended - extending only one is why the fold control did nothing at first.
 *
 * Each is wrapped on its own, because `extend` throws outright if a module is missing: one
 * unavailable path must not take the other down with it, nor the appends above.
 */
function extendSafely(registry: ModuleRegistry, path: string, exportName: string, wrap: any) {
  try {
    registry.extend(path, exportName, wrap);
  } catch (e) {
    console.warn(`[ADHDGoneWild] Could not extend ${exportName} in ${path}: ${e}`);
  }
}

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.append("Game", GameDismissInput);
  moduleRegistry.append("GameTopLeft", BrainEntry);
  moduleRegistry.append("Game", ParkingHint);
  moduleRegistry.append("Game", RingClicks);
  moduleRegistry.append("Game", JustParked);
  moduleRegistry.append("Game", WelcomeBack);
  moduleRegistry.append("Game", HyperfocusCard);

  extendSafely(moduleRegistry, "game-ui/game/components/toolbar/toolbar.tsx", "Toolbar", foldToolbar);
  extendSafely(
    moduleRegistry,
    "game-ui/game/components/toolbar/toolbar-legacy.tsx",
    "ToolbarLegacy",
    foldLegacyToolbar
  );
};

export default register;
