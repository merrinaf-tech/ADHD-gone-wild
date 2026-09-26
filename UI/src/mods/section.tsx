import React from "react";
import styles from "./panel.module.scss";

/**
 * One section of the panel: a card with a heading.
 *
 * The panel used to be one continuous column where a small grey heading and a hairline were all
 * that separated "what is wrong" from "what I parked". Every section weighed the same, so the eye
 * had nowhere to land. Now each is its own card, the heading carries a subject icon and reads at
 * full ink, and whatever counts belong to the section sit at the right of the heading - the state
 * of the section is visible before a word of it is read.
 *
 * The icon is a subject, never a status: colour stays where theme/tokens.ts puts it.
 */
export const Section = ({
  icon,
  title,
  trailing,
  children,
}: {
  icon: React.ReactNode;
  title: string;
  /** Counts or a compact action, right-aligned in the heading. */
  trailing?: React.ReactNode;
  children?: React.ReactNode;
}) => (
  <div className={styles.section}>
    <div className={styles.sectionHeader}>
      <span className={styles.sectionIcon}>{icon}</span>
      <span className={styles.sectionTitle}>{title}</span>
      {trailing}
    </div>
    {children}
  </div>
);

/**
 * The one line a section shows when it has nothing to show.
 *
 * Empty states used to be a sentence and an explanation each, which made "nothing here" the
 * largest thing in the panel. The explanation is still there, one hover away.
 */
export const EmptyLine = ({ children }: { children: React.ReactNode }) => (
  <div className={styles.emptyLine}>{children}</div>
);
