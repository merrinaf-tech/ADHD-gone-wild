import React from "react";
import styles from "../mods/panel.module.scss";

/**
 * A text field that actually shows its placeholder.
 *
 * Coherent Gameface does not render the `placeholder` attribute. The game does not rely on it
 * either - it draws its own and marks it with a `.placeholder` class, which is why there is a
 * global `.placeholder { color: rgba(255,255,255,.35) }` in its stylesheet and no `::placeholder`
 * rule anywhere. An empty field with a placeholder attribute is simply empty here, which is how
 * an editor opened on an untitled idea came to show a blank space where its name should be.
 *
 * So the hint is a real element, sitting over the field and out of the way of the pointer. It
 * disappears the moment there is anything to read instead.
 */
export const TextField = ({
  className,
  value,
  placeholder,
  onChange,
  onFocus,
  onKeyDown,
}: {
  className: string;
  value: string;
  /** What the field means when it is empty. Never an instruction - "Roundabout", not "Type here". */
  placeholder: string;
  onChange: (value: string) => void;
  onFocus?: () => void;
  onKeyDown?: (e: React.KeyboardEvent<HTMLInputElement>) => void;
}) => (
  <span className={styles.field}>
    <input
      className={className}
      value={value}
      onChange={(e) => onChange(e.currentTarget.value)}
      onFocus={onFocus}
      /*
        stopPropagation on every key is what keeps typing out of the game: without it the letters
        reach the world as shortcuts and the city starts building things. It is done here, once,
        rather than trusted to every caller remembering.
      */
      onKeyDown={(e) => {
        e.stopPropagation();
        onKeyDown?.(e);
      }}
    />

    {value.length === 0 && <span className={styles.fieldHint}>{placeholder}</span>}
  </span>
);
