import { useEffect } from "react";
import { useValue } from "cs2/api";
import { brainParkingEnabled$, clickWorld, parking$ } from "./bindings";

/**
 * Listens for clicks on the map so that clicking a parked idea's ring opens it.
 *
 * It draws nothing. It exists only to hold a listener, which is why it returns null rather than
 * being folded into a component that has something to show - anything that renders would come
 * and go with its own state, and this has to be listening the whole time.
 *
 * Every click in the game reaches this, which sounds alarming and is the reason none of the
 * judgement happens here. The DOM cannot tell a click on the ground from a click on some other
 * mod's panel, and guessing at it - walking up parent nodes, sniffing class names - would be
 * wrong in ways that only show up in other people's setups. So the coordinates go straight to
 * C#, which asks the game: is the cursor over an interface, is a tool running, are the markers
 * even drawn. See BrainParkingUISystem.ClickWorld.
 *
 * Nothing is consumed. No preventDefault, no stopPropagation, bubble phase only: the click still
 * belongs to the game, and this is only overhearing it.
 *
 * While parking is on it stands down entirely - that click belongs to ParkingHint, and a single
 * click must never both place an idea and open one.
 */
export const RingClicks = () => {
  const enabled = useValue(brainParkingEnabled$);
  const parking = useValue(parking$);

  const listening = enabled && !parking;

  useEffect(() => {
    if (!listening) {
      return;
    }

    const onClick = (e: MouseEvent) => clickWorld(e.clientX, e.clientY);

    document.addEventListener("click", onClick);
    return () => document.removeEventListener("click", onClick);
  }, [listening]);

  return null;
};
