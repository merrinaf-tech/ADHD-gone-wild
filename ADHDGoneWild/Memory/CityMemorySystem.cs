using System;
using ADHDGoneWild.Core;
using ADHDGoneWild.Persistence;
using Colossal.Serialization.Entities;
using Game;
using Game.City;
using Game.Rendering;
using Game.Simulation;
using Game.UI;
using Unity.Entities;

namespace ADHDGoneWild.Memory
{
    /// <summary>
    /// Opens and closes the city's memory as the player moves between cities and the menu.
    ///
    /// It exists so there is exactly one answer to "which city are we in and what do we remember
    /// about it". Features do not each work that out; they ask this, and listen to
    /// <see cref="CityChanged"/> so it does not matter which of them the game happens to notify
    /// first.
    ///
    /// It has no update: everything it does happens when a city is loaded or left.
    /// </summary>
    public partial class CityMemorySystem : GameSystemBase
    {
        private const string LogPrefix = "[Memory] ";

        private CitySystem _city;
        private CityConfigurationSystem _config;
        private NameSystem _names;
        private CameraUpdateSystem _camera;

        /// <summary>Raised after the memory has been opened or closed. Never during a read.</summary>
        public event Action CityChanged;

        public CityMemory Memory { get; private set; }

        /// <summary>True while a city - not the menu, not the editor - is open.</summary>
        public bool InGame { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();

            Memory = new CityMemory(new JsonCityStore());

            _city = World.GetOrCreateSystemManaged<CitySystem>();
            _config = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            _names = World.GetOrCreateSystemManaged<NameSystem>();
            _camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();

            // Nothing to do per frame.
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            var wasInGame = InGame;
            InGame = mode == GameMode.Game;

            try
            {
                if (!InGame)
                {
                    // Leaving a city - to the menu, not necessarily quitting the app. This is the
                    // moment "where you left off" actually means something, and the only reliable
                    // one: waiting for an action (parking an idea, muting an alert) would leave
                    // LastSeenUnixUtc stale on a visit where the player did neither, which would
                    // make the next Welcome Back judge the wrong gap.
                    if (wasInGame && Memory.IsLoaded)
                    {
                        RememberCurrentCamera();
                        Memory.Flush();
                    }

                    Memory.Close();
                }
                else
                {
                    var cityName = ResolveCityName();
                    Memory.Open(CityKey.FromCityName(cityName), cityName);
                }
            }
            catch (Exception e)
            {
                // A city that cannot be read is a city with no notes, not a city that fails to
                // load. Whatever went wrong stays inside the mod.
                Mod.Log.Error(e, LogPrefix + "Could not open this city's memory; continuing without it.");
                Memory.Close();
            }

            var changed = CityChanged;
            if (changed != null)
            {
                changed();
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                // The player may be quitting straight from a city rather than going to the menu
                // first, in which case the block above never ran for this session.
                if (Memory != null && Memory.IsLoaded)
                {
                    RememberCurrentCamera();
                    Memory.Flush();
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not write this city's memory while shutting down.");
            }

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Where the player actually was when they left, for Welcome Back. Best-effort: if the
        /// camera system is already gone by the time this runs (possible during app shutdown),
        /// this simply leaves the previously recorded position rather than guessing.
        /// </summary>
        private void RememberCurrentCamera()
        {
            WorldPoint pivot;
            if (CameraJump.TryGetPivot(_camera, out pivot))
            {
                Memory.RememberCamera(pivot);
            }
        }

        /// <summary>
        /// The city's display name, which is the only handle the game gives us - see
        /// docs/TECHNICAL_FINDINGS.md for why there is nothing better, and what it costs.
        ///
        /// It comes from <c>CityConfigurationSystem.cityName</c>, which is where the game itself
        /// reads it from: the name in the bottom toolbar is bound as
        /// <c>new GetterValueBinding&lt;string&gt;("toolbarBottom", "cityName", () =&gt;
        /// m_CityConfigurationSystem.cityName ?? "")</c>, and it is the same string written into
        /// every save as <c>SaveInfo.cityName</c>.
        ///
        /// <see cref="NameSystem.GetRenderedLabelName"/> was tried first and returned nothing for
        /// the city entity, so every city shared the key "unnamed-faca275f" - one file, one set
        /// of parked ideas, for all of them. It is kept only as a second chance.
        /// </summary>
        private string ResolveCityName()
        {
            try
            {
                if (_config != null && !string.IsNullOrEmpty(_config.cityName))
                {
                    return _config.cityName;
                }

                if (_names != null && _city != null && _city.City != Entity.Null)
                {
                    var name = _names.GetRenderedLabelName(_city.City);
                    if (!string.IsNullOrEmpty(name))
                    {
                        return name;
                    }
                }
            }
            catch (Exception e)
            {
                Mod.Log.Warn(LogPrefix + "Could not read the city name (" + e.Message +
                             "); falling back to a shared key.");
            }

            return "unnamed";
        }
    }
}
