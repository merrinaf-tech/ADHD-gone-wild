import { bindValue, trigger } from "cs2/api";

const GROUP = "adhd";

/** Whether the game's build toolbar is currently folded away. */
export const toolbarCollapsed$ = bindValue<boolean>(GROUP, "toolbarCollapsed", false);

/** Whether the fold control is on screen at all. */
export const toolbarButtonVisible$ = bindValue<boolean>(GROUP, "toolbarButtonVisible", true);

export const setToolbarCollapsed = (collapsed: boolean) =>
  trigger(GROUP, "setToolbarCollapsed", collapsed);
