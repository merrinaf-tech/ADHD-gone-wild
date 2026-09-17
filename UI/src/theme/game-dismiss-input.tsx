import React, { useEffect, useState } from "react";
import { InputActionConsumer } from "cs2/input";
import { dismissTop, hasDismissEntries, subscribeDismissEntries } from "./use-dismiss";

/**
 * Listen to the game's own Close and Back actions only while this mod has something to dismiss.
 * Close follows the player's mouse rebinding; Back also covers Escape and the gamepad back button.
 * The registry chooses one surface, so a card over the panel never closes both at once.
 */
export const GameDismissInput = () => {
  const [active, setActive] = useState(hasDismissEntries);

  useEffect(() => subscribeDismissEntries(() => setActive(hasDismissEntries())), []);

  if (!active) {
    return null;
  }

  return <InputActionConsumer actions={{ Close: dismissTop, Back: dismissTop }} ignoreFocusState />;
};
