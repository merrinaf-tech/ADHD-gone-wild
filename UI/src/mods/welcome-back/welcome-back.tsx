import React from "react";
import { useValue } from "cs2/api";
import { dismissWelcomeBack, viewLastPlace, welcomeBackInfo$, welcomeBackVisible$ } from "./bindings";
import { alerts$ } from "../alerts/bindings";
import { AlertCounts } from "../alerts/alerts-panel";
import { PlaceGlyph } from "theme/glyphs";
import { Status } from "theme/tokens";
import { useSurface } from "theme/surface";
import { useDismissOnGameClose } from "theme/use-dismiss";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "../panel.module.scss";

/**
 * Enough context to skip reconstructing it yourself, and nothing that looks like a to-do list.
 *
 * Appears on its own, at most once per city load, and only after a real break - see
 * WelcomeBackRule on the C# side. It never says "here is what you need to finish"; it says what
 * is there, so your brain does not have to hold it while you decide what to do with it.
 *
 * Dismissible immediately, and never reopens itself once closed for this load.
 */
export const WelcomeBack = () => {
  const visible = useValue(welcomeBackVisible$);
  const info = useValue(welcomeBackInfo$);
  const alerts = useValue(alerts$);
  const text = useText();
  const surface = useSurface();

  useDismissOnGameClose(visible, dismissWelcomeBack);

  if (!visible) {
    return null;
  }

  // Muted (3) and Personal (4) are never a city-wide peak - only Immediate/Important/Monitor are.
  // Resolved (5) means nothing to report, so the state line is skipped rather than shown as green.
  const showCityState = alerts.peak <= Status.Monitor;

  return (
    <div className={styles.welcomeCard} style={{ background: surface }}>
      <div className={styles.welcomeHeader}>
        <span>
          {text(K.welcomeBackTitle)} <span className={styles.welcomeCity}>{info.cityName}</span>
        </span>
      </div>

      {info.hasLastPlace && (
        <button className={styles.welcomePlaceButton} onClick={viewLastPlace}>
          <PlaceGlyph size={16} />
          {text(K.welcomeBackViewPlace)}
        </button>
      )}

      {info.ideaCount > 0 && (
        <div className={styles.welcomeRow}>
          {info.ideaCount === 1
            ? text(K.welcomeBackIdeasOne)
            : `${info.ideaCount} ${text(K.welcomeBackIdeasMany)}`}
        </div>
      )}

      {showCityState && (
        <div className={styles.welcomeRow}>
          <AlertCounts snapshot={alerts} size={12} />
        </div>
      )}

      <div className={styles.welcomeActions}>
        <button className={styles.textButton} onClick={dismissWelcomeBack}>
          {text(K.close)}
        </button>
      </div>
    </div>
  );
};
