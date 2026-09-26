import React, { useEffect, useRef, useState } from "react";
import { bindValue, useValue } from "cs2/api";
import { InputActionConsumer } from "cs2/input";
import { dismissTop, hasDismissEntries, subscribeDismissEntries } from "./use-dismiss";

const gameCancelRevision$ = bindValue<number>("adhd", "gameCancelRevision", 0);

/**
 * Follow the player's actual vanilla Tool / Cancel binding.
 *
 * The C# bridge reads that action directly from InputManager and increments a revision for every
 * press. This component turns a new revision into one dismissal. Keeping the previous value in a
 * ref also means a press made while nothing is open cannot close something opened afterwards.
 *
 * Back remains here for Escape and the gamepad back button. The registry chooses one surface, so
 * a card over the panel never closes both at once.
 */
export const GameDismissInput = () => {
  const [active, setActive] = useState(hasDismissEntries);
  const cancelRevision = useValue(gameCancelRevision$);
  const previousRevision = useRef(cancelRevision);

  useEffect(() => subscribeDismissEntries(() => setActive(hasDismissEntries())), []);

  useEffect(() => {
    if (cancelRevision !== previousRevision.current) {
      previousRevision.current = cancelRevision;
      dismissTop();
    }
  }, [cancelRevision]);

  if (!active) {
    return null;
  }

  return <InputActionConsumer actions={{ Back: dismissTop }} ignoreFocusState />;
};
