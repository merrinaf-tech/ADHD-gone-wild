# ADHD gone wild

A Cities: Skylines II mod that lets your brain play however it wants, and takes care of what it
doesn't want to keep in mind.

Start a highway. Suddenly build a metro station. Abandon it. Decorate a park. Remember an
industrial district. Spend forty minutes landscaping. Never go back to the highway.

None of that is a problem to be fixed. The mod exists so none of it has to be held in your head.

It is not a productivity tool, and it never will be — see
[docs/PRODUCT_PRINCIPLES.md](docs/PRODUCT_PRINCIPLES.md).

## What works today

One small owl joins the vanilla row in the top-left. It shows a count of alerts that need
attention; everything else is behind one click.

**Smart alerts.** Cities: Skylines II will put the same warning over forty buildings. This gathers
them into one short list: one row per kind, worst first, with the count beside it. It also watches
electricity, water and sewage across the whole city.

Every row says what is true — *"Part of the city is without electricity"* — and stops there. It
never tells you what to build. **View** takes you to the affected places one at a time.
**Mute** silences a row in that city, for good, without pretending the thing went away.

**Brain parking.** Press the shortcut (`Ctrl+I` by default), click anywhere on the map, and the
thought is saved there. No dialogue, no typing, no deadline. You are back to whatever you were
doing on the next click.

The same panel lists what you have parked in this city. Click one to fly to it, change what kind
of thing it was, or forget it. Ideas are memories, not tasks: nothing here has a state, a
percentage, or a due date.

Players have confirmed that the released Go Back flow, reminders and toolbar control help them
play. Changes in the current source still need an in-game check before a new version is published.

## Building

Two halves: a C# code mod and a React UI module. Build the UI first, then the code mod.

**The game must be closed** — deploying overwrites the mod folder, and a running game holds it.

First time only, from `UI/`:

```bash
npm install
```

Then, from the repository root:

```bash
npm --prefix UI run build
```

```bash
dotnet build ADHDGoneWild.sln -c Release
```

That deploys to `%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\ADHDGoneWild`. Start
the game and enable the mod.

The toolchain's post-processor targets .NET 6. With only a newer SDK installed, set
`DOTNET_ROLL_FORWARD=Major` before building.

## Tests

Pure logic only — anything needing the game is kept thin and tested by playing it.

```bash
dotnet test tests/ADHDGoneWild.Tests
```

The rules for alerts, reminders and the attention trail are tested separately from the game UI.

## Options

Under **Options → Mods → ADHD gone wild**. The finer settings of each feature appear once the
game's **Show Advanced** switch is on.

- **Smart alerts** — the feature's own switch
- **Tell me about** — only what is immediate, immediate and important, or everything
- **Brain parking** — the feature's own switch
- **Park an idea** — the shortcut, rebindable
- **New ideas are filed as** — what a freshly parked idea starts out as
- **Toolbar fold, Welcome Back, safety net and hyperfocus reminders** — independent switches
- **Attention trail** — an optional, session-only list of places you spent time in
- **Colour palette** — standard, or colour-blind friendly

Every feature the mod grows will get its own switch. There is no single correct ADHD experience,
so nothing here is load-bearing: switch something off and the game is exactly as it shipped.

## Where things are

| | |
|---|---|
| [docs/PRODUCT_PRINCIPLES.md](docs/PRODUCT_PRINCIPLES.md) | what this is for, and the things it will never do |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | systems, the UI bridge, persistence |

## Privacy

Local, always. No accounts, no telemetry, no external services, no analytics. The mod's notes
live in a plain JSON file beside the game's user data, and never inside a save — a city saved
with this mod installed opens perfectly well without it. The optional attention trail stays in
memory for the current city session and is not written to the log or the city file.

## Licence

MIT. See [LICENSE](LICENSE).
