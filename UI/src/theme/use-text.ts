import { useCallback } from "react";
import { useLocalization } from "cs2/l10n";
import { FALLBACK } from "./l10n";

/**
 * One place to turn a key into a sentence.
 *
 * The game answers first. The bundled English in theme/l10n.ts is only what appears if the
 * localisation source did not load at all, so a missing translation is never a blank label.
 */
export function useText(): (key: string) => string {
  const { translate } = useLocalization();

  return useCallback(
    (key: string) => translate(key, FALLBACK[key] ?? key) ?? FALLBACK[key] ?? key,
    [translate]
  );
}

/**
 * How long ago, in the vaguest useful terms.
 *
 * Deliberately imprecise, and never counts upward towards anything. "3 days ago" is a fact about
 * a memory; "3 days overdue" would be a fact about a debt, and this mod does not issue those.
 */
export function timeAgo(createdUnixUtc: number, nowMs: number): string {
  const seconds = Math.max(0, Math.floor(nowMs / 1000) - createdUnixUtc);

  if (seconds < 90) {
    return "just now";
  }

  const minutes = Math.round(seconds / 60);
  if (minutes < 60) {
    return `${minutes} min ago`;
  }

  const hours = Math.round(minutes / 60);
  if (hours < 24) {
    return hours === 1 ? "an hour ago" : `${hours} hours ago`;
  }

  const days = Math.round(hours / 24);
  if (days < 30) {
    return days === 1 ? "yesterday" : `${days} days ago`;
  }

  const months = Math.round(days / 30);
  return months === 1 ? "a month ago" : `${months} months ago`;
}
