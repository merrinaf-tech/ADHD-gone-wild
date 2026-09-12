import React, { useEffect } from "react";
import { useValue } from "cs2/api";
import { cancelParking, parkAt, parking$ } from "./bindings";
import { CategoryGlyph } from "theme/glyphs";
import { IdeaCategory, Status, statusToken } from "theme/tokens";
import { useSurface } from "theme/surface";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * The parking interaction: one line saying what the next click will do, and a listener waiting
 * for it.
 *
 * The click is caught on the document rather than by a transparent panel over the world. A panel
 * that covers the screen is, as far as the game is concerned, UI under the cursor - which
 * switches off camera control, so the player cannot scroll or zoom to find the spot they actually
 * wanted. Listening instead leaves the world exactly as interactive as it was.
 *
 * The tint stays, but only as paint: `pointer-events: none` keeps it out of the game's hit
 * testing entirely.
 *
 * The listener is on the bubble phase on purpose, so a click on the "never mind" button reaches
 * that button first and stops there instead of also parking a thought.
 */
export const ParkingHint = () => {
  const parking = useValue(parking$);
  const text = useText();
  const surface = useSurface();

  useEffect(() => {
    if (!parking) {
      return;
    }

    // The click that asked for parking is still on its way up to the document when this listener
    // is added, so it arrives here as if it were the player's choice of spot - and the thought
    // gets dropped on the button that was just pressed. Waiting for a press that *begins* after
    // the hint exists rules that out without depending on how fast anything is dispatched.
    let armed = false;

    const onDown = () => {
      armed = true;
    };

    const onClick = (e: MouseEvent) => {
      if (!armed) {
        return;
      }
      armed = false;
      parkAt(e.clientX, e.clientY);
    };

    document.addEventListener("mousedown", onDown);
    document.addEventListener("click", onClick);
    return () => {
      document.removeEventListener("mousedown", onDown);
      document.removeEventListener("click", onClick);
    };
  }, [parking]);

  if (!parking) {
    return null;
  }

  const token = statusToken(Status.Personal);

  return (
    <>
      <div className={styles.parkingTint} />

      <div className={styles.hint} style={{ background: surface }}>
        <CategoryGlyph category={IdeaCategory.Idea} size={20} color={token.color} />
        <span>{text(K.parkingHint)}</span>
        <button
          className={styles.textButton}
          onClick={(e) => {
            e.stopPropagation();
            cancelParking();
          }}
        >
          {text(K.parkingCancel)}
        </button>
      </div>
    </>
  );
};
