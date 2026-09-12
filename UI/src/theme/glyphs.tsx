import React from "react";
import { IdeaCategory, StatusShape } from "./tokens";

/**
 * Every icon the mod draws, as inline SVG.
 *
 * Drawn rather than referenced so that nothing depends on a game asset path staying where it is,
 * and so each one is legible at the sizes this UI actually uses (12-20px).
 *
 * The colour is passed in and written straight onto the `stroke` attribute. These used to say
 * `stroke="currentColor"` and inherit it from a parent style, which is the ordinary way to do this
 * on the web and left every glyph rendering black on a dark panel. Coherent Gameface is not a
 * browser: `currentColor` appears once in the whole of the game's own 460KB stylesheet, never in
 * its JavaScript, and never in any of its own SVG files - it colours icons with a mask and a
 * background instead. Rather than establish whether it is supported, this stops depending on it.
 *
 * These are subject icons. The colour around them is the status, and the two are set
 * independently on purpose - see theme/tokens.ts.
 */

interface GlyphProps {
  size?: number;
  className?: string;
  /** Written onto the stroke. Defaults to the panel's ordinary ink. */
  color?: string;
}

/** What a glyph is drawn in when nothing says otherwise. Matches $ink in panel.module.scss. */
const DEFAULT_INK = "rgba(255, 255, 255, 0.92)";

function svg(
  size: number,
  className: string | undefined,
  children: React.ReactNode,
  color: string = DEFAULT_INK
) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      className={className}
      fill="none"
      stroke={color}
      strokeWidth={2.4}
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      {children}
    </svg>
  );
}

const CATEGORY_GLYPHS: Record<IdeaCategory, (p: GlyphProps) => JSX.Element> = {
  /** A thought. The default, and by far the most common. */
  [IdeaCategory.Idea]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <path d="M9.2 16.5a5.2 5.2 0 1 1 5.6 0v1.3a1 1 0 0 1-1 1h-3.6a1 1 0 0 1-1-1z" />
        <path d="M10.2 21h3.6" />
      </>,
      color
    ),

  /** Something to put here. */
  [IdeaCategory.Build]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <path d="M4 20h16" />
        <path d="M6 20V9l6-4 6 4v11" />
        <path d="M10 20v-5h4v5" />
      </>,
      color
    ),

  /** Trees, benches, the pleasant work. */
  [IdeaCategory.Decoration]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <path d="M12 4 6.5 13h11z" />
        <path d="M12 9.5 8.5 16h7z" />
        <path d="M12 16v4" />
      </>,
      color
    ),

  /** A line, a station, a stop. */
  [IdeaCategory.Transport]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <rect x="6" y="3.5" width="12" height="13" rx="3" />
        <path d="M6 11h12" />
        <path d="M9.5 14h.01M14.5 14h.01" />
        <path d="M9 16.5 7 20.5M15 16.5l2 4" />
      </>,
      color
    ),

  /** Something already there that could be different. */
  [IdeaCategory.Fix]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <path d="M15.4 4.6a4.5 4.5 0 0 0-5.9 5.6L4 15.7 8.3 20l5.5-5.5a4.5 4.5 0 0 0 5.6-5.9l-2.7 2.7-2.5-.7-.7-2.5z" />
      </>,
      color
    ),

  /** Everything else. */
  [IdeaCategory.Other]: ({ size = 16, className, color }) =>
    svg(
      size,
      className,
      <>
        <path d="M9.3 9a2.8 2.8 0 1 1 3.6 2.7c-.6.2-.9.7-.9 1.3v.8" />
        <path d="M12 17.5h.01" />
      </>,
      color
    ),
};

export function CategoryGlyph({
  category,
  size = 16,
  className,
  color,
}: GlyphProps & { category: IdeaCategory }) {
  const Glyph = CATEGORY_GLYPHS[category] ?? CATEGORY_GLYPHS[IdeaCategory.Other];
  return Glyph({ size, className, color });
}

/**
 * The shape that goes with a status. Filled, small, and readable with the colour removed - which
 * is the whole reason it exists.
 */
export function StatusShapeGlyph({
  shape,
  size = 10,
  className,
  color = DEFAULT_INK,
}: GlyphProps & { shape: StatusShape }) {
  const common = {
    width: size,
    height: size,
    viewBox: "0 0 12 12",
    className,
    fill: color,
  };

  switch (shape) {
    case "triangle":
      return (
        <svg {...common}>
          <path d="M6 1.2 11 10.4H1z" />
        </svg>
      );
    case "square":
      return (
        <svg {...common}>
          <rect x="1.6" y="1.6" width="8.8" height="8.8" rx="1.2" />
        </svg>
      );
    case "bar":
      return (
        <svg {...common}>
          <rect x="1.2" y="4.8" width="9.6" height="2.4" rx="1.2" />
        </svg>
      );
    case "diamond":
      return (
        <svg {...common}>
          <path d="M6 0.8 11.2 6 6 11.2 0.8 6z" />
        </svg>
      );
    case "check":
      return (
        <svg {...common} fill="none" stroke={color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round">
          <path d="M2 6.4 4.8 9.2 10 3.2" />
        </svg>
      );
    case "disc":
    default:
      return (
        <svg {...common}>
          <circle cx="6" cy="6" r="4.4" />
        </svg>
      );
  }
}

/** A place on the map. Used only by Welcome Back, to say "you were around here". */
export const PlaceGlyph = ({ size = 16, className, color }: GlyphProps) =>
  svg(
    size,
    className,
    <>
      <path d="M12 21s7-6.1 7-11.5A7 7 0 0 0 5 9.5C5 14.9 12 21 12 21z" />
      <circle cx="12" cy="9.5" r="2.4" />
    </>,
    color
  );
