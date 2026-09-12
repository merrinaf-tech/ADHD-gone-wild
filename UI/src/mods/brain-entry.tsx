import React, { useEffect, useState } from "react";
import { useValue } from "cs2/api";
import { Button, Tooltip } from "cs2/ui";
import { AlertCounts, AlertsPanel } from "./alerts/alerts-panel";
import { alerts$, setAlertsOpen, smartAlertsEnabled$ } from "./alerts/bindings";
import { IdeasSection } from "./brain-parking/ideas-panel";
import { SafetyNet } from "./creativity/safety-net";
import {
  brainParkingEnabled$,
  clearIdeaFocus,
  focusedIdea$,
  startParking,
} from "./brain-parking/bindings";
import owlIcon from "images/owl.svg";
import { Status, statusToken } from "theme/tokens";
import { useSurface } from "theme/surface";
import { K } from "theme/l10n";
import { useText } from "theme/use-text";
import styles from "./panel.module.scss";

/**
 * The whole mod, as one small button.
 *
 * It is the game's own button so it sits in the row exactly like its neighbours, and it says as
 * little as it can get away with: one number for what is wrong right now. That is a count of
 * what is true about the city - a count of things the player has not got round to would not be
 * allowed here.
 *
 * Everything else is behind one click.
 */
export const BrainEntry = () => {
  const parkingEnabled = useValue(brainParkingEnabled$);
  const alertsEnabled = useValue(smartAlertsEnabled$);
  const alerts = useValue(alerts$);
  const text = useText();
  const surface = useSurface();

  const focused = useValue(focusedIdea$);

  const [open, setOpen] = useState(false);

  // The C# side collects less often when nobody is looking. Telling it either way is the whole
  // reason that optimisation is safe.
  useEffect(() => {
    setAlertsOpen(open);
    return () => setAlertsOpen(false);
  }, [open]);

  // Clicking an idea's ring on the map opens the panel on it. The C# side decides whether a
  // click landed on one; all this does is answer.
  useEffect(() => {
    if (focused) {
      setOpen(true);
    }
  }, [focused]);

  // Closing the panel puts the pointing finger down. Left set, the next time the panel opened for
  // any other reason it would open highlighting something the player had long since moved on from.
  useEffect(() => {
    if (!open && focused) {
      clearIdeaFocus();
    }
  }, [open, focused]);

  if (!parkingEnabled && !alertsEnabled) {
    return null;
  }

  // Only the two levels that mean "something is happening". Monitor is worth a look when the
  // panel is open, not a number on the toolbar.
  const needsAttention = alerts.immediate + alerts.important;
  const peak = statusToken(alerts.immediate > 0 ? Status.Immediate : Status.Important);

  return (
    <div className={styles.entry}>
      {/*
        The game's own button, not one of ours. It is the only way to actually match the controls
        beside it: size, background, hover, focus and the pressed look all come from the game's
        theme, so they stay right even when that theme changes.
      */}
      {/*
        The icon goes in as `src`, not as children. Passing children left the game's own button
        laying it out on the text baseline, which sat it lower than every other icon in the row -
        with `src` the button places it exactly as it places all the vanilla ones.
      */}
      <Tooltip tooltip={text(K.toolbarTooltip)}>
        <Button
          variant="floating"
          src={owlIcon}
          selected={open}
          onSelect={() => setOpen(!open)}
        />
      </Tooltip>

      {/*
        The count rides on top of the button rather than inside it. Inline, it made this button a
        different width from every other control in the row, which is exactly what looked wrong.
      */}
      {alertsEnabled && needsAttention > 0 && (
        <span className={styles.brainBadge} style={{ background: peak.color }}>
          {needsAttention}
        </span>
      )}

      {open && (
        <div className={styles.panel} style={{ background: surface }}>
          {alertsEnabled && <AlertsPanel />}

          <SafetyNet />

          {parkingEnabled && (
            <IdeasSection
              focusedId={focused}
              onPark={() => {
                // Out of the way immediately: the next click belongs to the map, not to this panel.
                setOpen(false);
                startParking();
              }}
            />
          )}
        </div>
      )}
    </div>
  );
};
