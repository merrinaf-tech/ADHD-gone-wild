import { useEffect, useRef } from "react";

/**
 * One dismissal per press of the game's Tool / Cancel mouse binding or UI Back action. The mouse
 * path reads the game's resolved binding in C#, so a player who rebinds Cancel does not need to
 * configure this mod too.
 *
 * Surfaces register only while visible. The active interaction wins; among cards, the most
 * recently shown wins. There is no separate DOM mouse binding to keep in sync.
 */
export enum DismissPriority {
  Card = 0,
  Panel = 10,
  Editor = 20,
  Parking = 30,
}

interface DismissEntry {
  priority: DismissPriority;
  dismiss: () => void;
}

const entries = new Map<number, DismissEntry>();
const listeners = new Set<() => void>();
let nextId = 0;

export const hasDismissEntries = () => entries.size > 0;

export const subscribeDismissEntries = (listener: () => void) => {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
};

const notify = () => listeners.forEach((listener) => listener());

/** Called by the shared game-input bridge, not once by each visible surface. */
export const dismissTop = (): boolean => {
  let chosenId = -1;
  let chosen: DismissEntry | undefined;

  entries.forEach((entry, id) => {
    if (!chosen || entry.priority > chosen.priority ||
        (entry.priority === chosen.priority && id > chosenId)) {
      chosen = entry;
      chosenId = id;
    }
  });

  if (!chosen) {
    return false;
  }

  chosen.dismiss();
  return true;
};

export function useDismissOnGameClose(
  active: boolean,
  dismiss: () => void,
  priority: DismissPriority = DismissPriority.Card
) {
  // Most callers pass a new callback on every render. Keep the latest callback without changing
  // when the surface entered the stack.
  const latest = useRef(dismiss);
  latest.current = dismiss;

  useEffect(() => {
    if (!active) {
      return;
    }

    const id = ++nextId;
    entries.set(id, { priority, dismiss: () => latest.current() });
    notify();

    return () => {
      entries.delete(id);
      notify();
    };
  }, [active, priority]);
}
