import { bindValue, trigger } from "cs2/api";

/** Mirrors the record written by CheckpointSystem.WriteCheckpoint. Keep the two in step. */
export interface Checkpoint {
  exists: boolean;
  takenUnixUtc: number;
  /** A save or a load is in flight. Both are slow, and neither should be asked for twice. */
  busy: boolean;
}

const GROUP = "adhd";

export const checkpoint$ = bindValue<Checkpoint>(GROUP, "checkpoint", {
  exists: false,
  takenUnixUtc: 0,
  busy: false,
});

export const safetyNetEnabled$ = bindValue<boolean>(GROUP, "safetyNetEnabled", true);

export const createCheckpoint = () => trigger(GROUP, "createCheckpoint");
export const restoreCheckpoint = () => trigger(GROUP, "restoreCheckpoint");
export const forgetCheckpoint = () => trigger(GROUP, "forgetCheckpoint");
