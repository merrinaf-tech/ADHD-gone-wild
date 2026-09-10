# Architecture

## Shape

```
ADHDGoneWild.sln
ADHDGoneWild/                  the code mod (net48, references the game assemblies)
  Mod.cs                       entry point, lifecycle, logging, system registration
  Core/
    Semantics.cs               the Status vocabulary. No colours live here.
    ModPaths.cs                where the mod keeps its own files
  Settings/AdhdSettings.cs     one option per implemented feature, and the keybinding
  Alerts/
    AlertSubject.cs  Alert.cs   what an alert is about, and one alert
    AlertClassifier.cs          the only place "is this worth attention" is decided. Pure.
    AlertAggregator.cs          merging, muting, filtering, ordering. Pure.
    AlertSet.cs                 what the panel is shown, and the per-status counts
    UtilityAlerts.cs            electricity / water / sewage, from three readings. Pure.
    NotificationTally.cs        counts in-world icons by kind. Pure.
    NotificationSubjects.cs     guesses a subject from the game's name for a notification
    AlertsUISystem.cs           the only part that touches ECS: reads numbers and icons
  Memory/
    CityMemory.cs               the single owner of one city's stored data. Pure.
    CityMemorySystem.cs         opens and closes it as cities load
  Localization/
    L10n.cs                    key constants for the in-game panel
    LocaleEN.cs                the en-US source
  Persistence/
    ICityStore.cs              read/write one city's data
    JsonCityStore.cs           the sidecar JSON implementation
    CityData.cs                the on-disk shape, versioned
    CityKey.cs                 city name -> filename
  BrainParking/
    IdeaCategory.cs  Idea.cs   the domain
    IdeaRepository.cs          the rules: ordering, load, save. No ECS, no game types but float3.
    IdeaParkingTool.cs         a real ToolBaseSystem: point, click, done
    BrainParkingUISystem.cs    the bridge: bindings, triggers, camera jump, shortcut
  UI/ThemeUISystem.cs          which palette the UI should draw in

UI/                            the React side (official CS:II UI template)
  src/theme/
    tokens.ts                  semantic design tokens. The only place a colour is decided.
    glyphs.tsx                 every icon, as inline SVG
    l10n.ts                    the key list, mirroring L10n.cs
  src/mods/brain-entry.tsx     the one button, and the panel it opens
  src/mods/alerts/             what is true about the city, in as few rows as possible
  src/mods/brain-parking/      parked ideas, and the placing hint

tests/ADHDGoneWild.Tests/      pure logic only, net8.0 + xunit
docs/                          this, and the rest
```

## The rule the layout exists to enforce

**Domain logic does not know about the UI, and the UI does not know about the game.**

The alert pipeline is the clearest example, and the reason the split earns its indirection:

```
game state -> collector -> classifier -> status -> aggregator -> grouped UI
              (ECS, thin)   (pure)                 (pure)
```

Only the collectors touch ECS, and they do nothing but read. `AlertClassifier` decides when the
mod is allowed to spend the player's attention, and it does that from three integers — so the
thresholds can be argued about in a test rather than rediscovered by playing. `AlertAggregator`
turns forty identical warnings into one row. Neither can see a component, an entity or a colour.

`CityMemory` is the same idea on the storage side: it takes an `ICityStore` and a city key and
holds the ordering, muting and persistence rules, with no `Entity` and no Unity type in sight.
Positions cross that boundary as `Core.WorldPoint`, converted once at the edge.

**One owner per city file.** Parked ideas and muted alerts live in the same file, so exactly one
object writes it. `CityMemorySystem` owns it, opens and closes it as cities load, and raises
`CityChanged`. Features listen to that rather than each working out which city is open — which
also means the order the game notifies systems in does not matter.

## UI ↔ backend

One binding group, `"adhd"`, over the game's own `Colossal.UI.Binding`. No Harmony, no patching.

| Name | Kind | Direction |
|---|---|---|
| `ideas` | raw value | C# → UI. The whole list, rewritten on any change. |
| `parking` | bool | C# → UI. Whether the placing tool is live. |
| `brainParkingEnabled` | bool | C# → UI. The feature's own switch. |
| `palette` | int | C# → UI. Which token set to draw in. |
| `startParking` / `cancelParking` | trigger | UI → C# |
| `forgetIdea(id)` | trigger | UI → C# |
| `jumpToIdea(id)` | trigger | UI → C# |
| `setIdeaCategory(id, category)` | trigger | UI → C# |
| `setIdeaNote(id, note)` | trigger | UI → C#. Exists; no UI uses it yet — see ROADMAP. |
| `alerts` | raw value | C# → UI. Counts per status, plus the visible rows. |
| `smartAlertsEnabled` | bool | C# → UI. |
| `setAlertsOpen(open)` | trigger | UI → C#. Lets the collector slow down when nobody is looking. |
| `viewAlert(id)` | trigger | UI → C#. Visits the affected places one at a time. |
| `muteAlert(id, muted)` | trigger | UI → C#. |

The UI holds no state about ideas. The repository is the only copy, so a marker cannot exist on
screen without existing on disk.

Three enums are duplicated across the bridge and must be kept in step: `Core.Status`,
`BrainParking.IdeaCategory` and `Alerts.AlertSubject`, each against its twin in
`theme/tokens.ts`. All are numbered, and the category numbers are written to disk, so they are
appended to and never renumbered.

## Persistence

`<EnvPath.kUserDataPath>/ModsData/ADHDGoneWild/cities/<key>.json`, one file per city, schema
versioned from v1. Nothing is written into a save game. Full reasoning, and the city-key
limitation, in [TECHNICAL_FINDINGS.md](TECHNICAL_FINDINGS.md).

Failure is always survivable: a missing or corrupt file yields empty data plus a log line; a
failed write logs and returns false. Neither ever throws at the player.

## Performance

The city is large and the simulation is already expensive, so:

- Brain Parking reads one input action per UI frame and nothing else. The idea list is pushed on
  change, not polled.
- Alert collection is the one expensive thing in the mod, and it runs on a timer rather than per
  frame: 1.5s while the panel is open, 5s while it is not. The UI tells the C# side which it is.
- Utility readings are three property reads on systems the game already maintains. Nothing is
  summed or sampled.
- Counting notification icons walks a query that can hold thousands of entries. Both native arrays
  are released in a `finally`, prefab names are resolved once and cached by entity, and the
  locations kept per alert are capped at 24 — the view button visits one at a time, so the rest
  would be memory spent on nothing.
- Relative times are computed on render, not by a ticking timer.
- The placing tool exists but is disabled until the player activates it.

Still to watch: whether the icon walk is cheap enough in a large city. If not, `OpenInterval` and
`ClosedInterval` in `AlertsUISystem` are the two numbers to raise. Abandoned-project detection,
when it comes, must be event-driven or scheduled from the start.

## Logging

`[ADHDGoneWild]` is the logger name; messages carry a subsystem prefix:

```
[Memory]       Opened 'riverside-069508ba': 3 parked ideas, 1 muted alerts.
[Memory]       Marker created at 412, -88 (Idea, id 8f3a...).
[Persistence]  Loaded schema version 1 for 'riverside-069508ba' (3 ideas).
[Alerts]       Could not collect the city's alerts this tick.
[UI]           Alerts bridge initialised.
```

Nothing logs per frame. Errors name the subsystem, the operation, the id or state involved, and
carry the exception.

## Known API dependencies

Listed with their verification status in [TECHNICAL_FINDINGS.md](TECHNICAL_FINDINGS.md). The
short version: `ToolBaseSystem`, `ToolRaycastSystem`, `CameraUpdateSystem`, `CitySystem`,
`NameSystem`, `PrefabSystem`, `ElectricityStatisticsSystem`, `WaterStatisticsSystem`,
`Game.Notifications.Icon`, `ModSetting` + `ProxyAction`, `EnvPath`. No Harmony. No patched
methods.
