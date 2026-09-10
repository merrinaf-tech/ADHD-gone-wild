# Product principles

> Let your brain play however it wants. The mod takes care of what it doesn't want to keep in
> mind.

## What this is

A player starts a highway, suddenly builds a metro station, abandons it, decorates a park,
remembers an industrial district, spends forty minutes landscaping, and never goes back to the
highway.

**That is not a failure condition.** The mod must never try to correct it.

## What the mod is for

- Reducing unnecessary cognitive load
- Reducing information overload
- Externalising memory
- Making forgotten ideas easy to recover
- Letting players jump freely between projects
- Protecting creativity
- Making returning to a city easier
- Making experimentation feel safe
- Gently protecting the player from unhealthy hyperfocus
- Preserving the freedom and pleasure of sandbox play

## What the mod is not for

Making the player productive, focused, disciplined, organised or efficient. Making them finish
things. Teaching them workflows. Optimising their session.

## The test every feature must pass

> Does this give the player more freedom, reduce cognitive burden, preserve information, or
> protect their wellbeing?

If yes, it probably belongs.

> Does this try to make the player more disciplined, productive, organised, focused or efficient?

If yes, it does not belong.

When uncertain, prefer player freedom.

## Rejected outright

These are not "later". They are decisions, and reversing one is a product change, not a feature
request:

- Mandatory Focus Tasks, "One Thing Mode", or anything that forces one project at a time
- Productivity dashboards, productivity scores, session efficiency metrics
- Mandatory checklists, "finish this before starting another thing" mechanics
- Completion pressure of any kind
- Task streaks, daily streaks, XP for being organised, badges for completing things
- Guilt-inducing reminders, giant todo backlogs
- Notifications implying an unfinished project must be completed
- Gamification whose purpose is productivity
- Conventional Session Goals

A sandbox game must remain a sandbox.

## Four distinctions to keep in mind

| | Good | Bad |
|---|---|---|
| **Information vs instruction** | "Electricity demand is higher than production." | "Build a power plant now." |
| **Memory vs obligation** | "You saved an idea here." | "You still have 7 unfinished ideas." |
| **Creativity vs productivity** | "Want an idea?" | "Complete three city improvements this session." |
| **Wellbeing vs interruption** | a small water indicator after long play | a modal window stopping the game |
| **Freedom vs correction** | the player jumps between activities | the mod asks them to go back |

## Visual language

Colour and icons are a cognitive tool, not decoration. The player should understand the type and
importance of something before reading a word.

**Colour represents status. Icon represents subject.** Electricity is not "yellow"; an
electricity problem worth watching is.

| Status | Meaning |
|---|---|
| Immediate | happening now, likely to matter |
| Important | real, not on fire |
| Monitor | worth a glance later |
| Muted | present, deliberately quiet |
| Personal | something the player made |
| Resolved | settled, used only where saying so helps |

Never assign colour by city system. Never let colour be the only carrier of meaning: every
status also has a shape, and everything showing a status also shows a subject icon. Alternative
palettes are a first-class setting, not an afterthought.

All of this lives in `UI/src/theme/tokens.ts`. Nothing else may write a colour.

## UX

The mod behaves like a quiet assistant sitting next to the player. What it keeps saying, without
saying it:

> You can keep doing whatever you want. I'll remember the rest.

Prefer passive assistance, ambient information, small icons, contextual information, one-click
interactions, optional panels, automatic memory, progressive disclosure.

Avoid modal windows, frequent popups, long text, mandatory interaction, configuration during
normal play, repeated confirmations.

## Copy style

Short, calm, non-judgemental, not medicalised where it need not be, never patronising.

Good: "Saved." · "You may have been doing something here." · "Water?" · "Welcome back."

Bad: "You forgot to finish this project." · "You should focus on this." · "You have been
distracted." · "You need to be more organised."

The mod never shames the player for ADHD behaviour.

## Honesty about detection

Several planned features rest on heuristics that may not be reliable — abandoned-project
detection, judging whether a warning matters, identifying a "meaningful" action.

Do not fake intelligence with fragile heuristics and present it as reliable. For every heuristic:
define confidence, prefer false negatives over annoying false positives, make detection
dismissible, allow disabling the subsystem, and log why a classification happened during
development. If it cannot be done reliably, leave a clean interface and document the limitation.

## Privacy

Local-first, always. No accounts, no telemetry, no external APIs, no cloud, no analytics.
