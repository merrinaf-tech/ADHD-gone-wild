import React from "react";
import { useValue } from "cs2/api";
import { Button, Tooltip } from "cs2/ui";
import { setToolbarCollapsed, toolbarButtonVisible$, toolbarCollapsed$ } from "./bindings";
import chevronDown from "images/chevron-down.svg";
import chevronUp from "images/chevron-up.svg";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The control that folds the game's build toolbar away, and brings it back.
 *
 * It never hides itself. That is the whole reason folding the toolbar is allowed at all: the
 * buttons are one click from returning at any moment, so nothing has been taken away - it has
 * only been put down. A fold with no visible way back would be exactly the destructive filtering
 * the product principles rule out.
 *
 * It sits on the full-screen anchor and is positioned by hand, just above the status strip. That
 * is not a style choice. Both of the anchors the game offers beside the toolbar - GameBottomLeft
 * and GameBottomRight - are rendered *inside* the block that folding slides away, so a control
 * placed in either one disappeared with everything else, leaving a folded toolbar and nothing on
 * screen to unfold it. Across a restart, since the fold is remembered.
 */
export const ToolbarFold = () => {
  const visible = useValue(toolbarButtonVisible$);
  const collapsed = useValue(toolbarCollapsed$);
  const text = useText();

  if (!visible) {
    return null;
  }

  return (
    <div className={styles.toolbarFold}>
      <Tooltip tooltip={text(collapsed ? K.toolbarUnfold : K.toolbarFold)}>
        <Button
          variant="floating"
          src={collapsed ? chevronUp : chevronDown}
          selected={collapsed}
          onSelect={() => setToolbarCollapsed(!collapsed)}
        />
      </Tooltip>
    </div>
  );
};
