import { bindValue, trigger } from "cs2/api";

/** Mirrors the record written by WelcomeBackUISystem.WriteInfo. Keep the two in step. */
export interface WelcomeBackInfo {
  cityName: string;
  ideaCount: number;
  hasLastPlace: boolean;
}

const GROUP = "adhd";

const EMPTY_INFO: WelcomeBackInfo = {
  cityName: "",
  ideaCount: 0,
  hasLastPlace: false,
};

export const welcomeBackVisible$ = bindValue<boolean>(GROUP, "welcomeBackVisible", false);
export const welcomeBackInfo$ = bindValue<WelcomeBackInfo>(GROUP, "welcomeBackInfo", EMPTY_INFO);

export const dismissWelcomeBack = () => trigger(GROUP, "dismissWelcomeBack");
export const viewLastPlace = () => trigger(GROUP, "viewLastPlace");
