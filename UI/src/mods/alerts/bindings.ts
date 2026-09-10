import { bindValue, trigger } from "cs2/api";
import { AlertSubject, Status } from "theme/tokens";

/**
 * Mirrors the record written by AlertsUISystem.WriteAlerts. Keep the two in step.
 */
export interface Alert {
  id: string;
  subject: AlertSubject;
  status: Status;
  /** Localisation key. May have no translation - see `alertTitle` in alerts-panel. */
  titleKey: string;
  /** The game's own name for this, used when the mod has nothing better to say. */
  rawName: string;
  /** Path to the game's own icon for this, relative to the UI root. See AlertIcons.cs. */
  icon: string;
  /** Buildings in this state. Zero means the alert is about the city as a whole. */
  affected: number;
  /** Whether there is somewhere to fly to. City-wide alerts have nowhere. */
  canView: boolean;
}

export interface AlertsSnapshot {
  /** The worst status currently true, for the compact badge. */
  peak: Status;
  immediate: number;
  important: number;
  monitor: number;
  /** Silenced by the player, and still counted. Always equals `mutedItems.length`. */
  muted: number;
  items: Alert[];
  /** The silenced ones themselves, so a mute can be undone rather than only counted. */
  mutedItems: Alert[];
}

const GROUP = "adhd";

export const EMPTY_ALERTS: AlertsSnapshot = {
  // Status.Resolved: nothing to say.
  peak: 5 as Status,
  immediate: 0,
  important: 0,
  monitor: 0,
  muted: 0,
  items: [],
  mutedItems: [],
};

export const alerts$ = bindValue<AlertsSnapshot>(GROUP, "alerts", EMPTY_ALERTS);
export const smartAlertsEnabled$ = bindValue<boolean>(GROUP, "smartAlertsEnabled", true);

/**
 * Tells the C# side whether anyone is looking. It collects less often when nobody is - see
 * AlertsUISystem.
 */
export const setAlertsOpen = (open: boolean) => trigger(GROUP, "setAlertsOpen", open);

export const viewAlert = (id: string) => trigger(GROUP, "viewAlert", id);
export const muteAlert = (id: string, muted: boolean) => trigger(GROUP, "muteAlert", id, muted);
