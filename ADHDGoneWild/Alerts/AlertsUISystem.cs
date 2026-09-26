using System;
using System.Collections.Generic;
using ADHDGoneWild.Core;
using ADHDGoneWild.Memory;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.UI;
using Unity.Collections;
using Unity.Entities;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// Collects what is true about the city, hands it to the rules, and publishes the result.
    ///
    /// The rules themselves are elsewhere and are pure - see <see cref="AlertClassifier"/> and
    /// <see cref="AlertAggregator"/>. This class only does the two things that need the game:
    /// reading the numbers, and reading the in-world notification icons.
    ///
    /// Cost is the design constraint. Counting icons means walking a query that can hold
    /// thousands of entries in a busy city, so it happens on a five-second timer rather than per
    /// frame. Opening the panel uses the last collected snapshot and never forces another walk.
    /// </summary>
    public partial class AlertsUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[Alerts] ";

        /// <summary>The list and compact badge share one deliberately unhurried refresh rate.</summary>
        private const float RefreshInterval = 5f;

        private CityMemorySystem _memory;
        private CameraUpdateSystem _camera;
        private PrefabSystem _prefabs;
        private ElectricityStatisticsSystem _electricity;
        private WaterStatisticsSystem _water;

        private EntityQuery _iconQuery;

        private readonly NotificationTally _tally = new NotificationTally();
        private readonly List<Alert> _collected = new List<Alert>();

        /// <summary>Prefab entity to name. Names never change, so this is filled once and kept.</summary>
        private readonly Dictionary<Entity, string> _prefabNames = new Dictionary<Entity, string>();

        /// <summary>Which location the next "view" on an alert should visit. See ViewAlert.</summary>
        private readonly Dictionary<string, int> _viewCursor = new Dictionary<string, int>();

        private AlertSet _set = AlertSet.Empty;

        private RawValueBinding _alertsBinding;
        private ValueBinding<bool> _enabledBinding;

        private float _nextRefresh;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();
            _memory.CityChanged += OnCityChanged;

            _camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();
            _prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
            _electricity = World.GetOrCreateSystemManaged<ElectricityStatisticsSystem>();
            _water = World.GetOrCreateSystemManaged<WaterStatisticsSystem>();

            // Deleted and Temp are excluded because they are not really on screen. Hidden is not:
            // another mod hiding the in-world icons is a display choice, and it must not quietly
            // empty this list as well.
            _iconQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Game.Notifications.Icon>(),
                    ComponentType.ReadOnly<PrefabRef>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Game.Common.Deleted>(),
                    ComponentType.ReadOnly<Game.Tools.Temp>()
                }
            });

            _alertsBinding = new RawValueBinding(Group, "alerts", WriteAlerts);
            AddBinding(_alertsBinding);

            _enabledBinding = new ValueBinding<bool>(Group, "smartAlertsEnabled", true);
            AddBinding(_enabledBinding);

            AddBinding(new TriggerBinding<string>(Group, "viewAlert", ViewAlert));
            AddBinding(new TriggerBinding<string, bool>(Group, "muteAlert", MuteAlert));

            Mod.RegisterAlerts(this);
            Mod.Log.Info("[UI] Alerts bridge initialised.");
        }

        private void OnCityChanged()
        {
            _set = AlertSet.Empty;
            _viewCursor.Clear();
            _nextRefresh = 0f;
            _alertsBinding.Update();
            ApplySettings();
        }

        public void ApplySettings()
        {
            var settings = Mod.Settings;
            var on = settings == null || settings.SmartAlertsEnabled;

            _enabledBinding.Update(on);

            if (!on)
            {
                _set = AlertSet.Empty;
                _alertsBinding.Update();
            }
            else
            {
                // A change of severity floor should be visible at once, not in five seconds.
                _nextRefresh = 0f;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var settings = Mod.Settings;
            if (!_memory.InGame || (settings != null && !settings.SmartAlertsEnabled))
            {
                return;
            }

            var now = UnityEngine.Time.time;
            if (now < _nextRefresh)
            {
                return;
            }

            _nextRefresh = now + RefreshInterval;

            try
            {
                Refresh();
            }
            catch (Exception e)
            {
                // A failed collection means a stale list for a few seconds, which is survivable.
                // Throwing here would surface as a mod error over the player's city.
                Mod.Log.Error(e, LogPrefix + "Could not collect the city's alerts this tick.");
            }
        }

        private void Refresh()
        {
            _collected.Clear();

            CollectUtilities();
            CollectNotifications();

            var settings = Mod.Settings;
            var floor = settings == null ? Status.Monitor : settings.AlertFloor();

            _set = AlertAggregator.Build(_collected, _memory.Memory.MutedAlerts, floor);
            _alertsBinding.Update();
        }

        /// <summary>
        /// The city's supply networks. Three cheap property reads - these systems keep the current
        /// figures, so nothing is being summed or sampled here.
        /// </summary>
        private void CollectUtilities()
        {
            UtilityAlerts.Collect(
                _collected,
                new UtilityReading(
                    _electricity.production,
                    _electricity.consumption,
                    _electricity.fulfilledConsumption),
                new UtilityReading(
                    _water.freshCapacity,
                    _water.freshConsumption,
                    _water.fulfilledFreshConsumption),
                new UtilityReading(
                    _water.sewageCapacity,
                    _water.sewageConsumption,
                    _water.fulfilledSewageConsumption));
        }

        /// <summary>
        /// Every notification icon in the world, counted by kind.
        ///
        /// This is the expensive half, and it is why the whole system runs on a timer. The two
        /// arrays come from one query so their order matches, and both are released before
        /// returning even if grouping throws.
        /// </summary>
        private void CollectNotifications()
        {
            _tally.Reset();

            if (_iconQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            var icons = _iconQuery.ToComponentDataArray<Game.Notifications.Icon>(Allocator.TempJob);
            var refs = _iconQuery.ToComponentDataArray<PrefabRef>(Allocator.TempJob);

            try
            {
                var count = Math.Min(icons.Length, refs.Length);
                for (var i = 0; i < count; i++)
                {
                    var icon = icons[i];
                    _tally.Add(
                        NameOf(refs[i].m_Prefab),
                        (int)icon.m_Priority,
                        new WorldPoint(icon.m_Location.x, icon.m_Location.y, icon.m_Location.z));
                }
            }
            finally
            {
                icons.Dispose();
                refs.Dispose();
            }

            _tally.Fill(_collected);
        }

        private string NameOf(Entity prefab)
        {
            if (prefab == Entity.Null)
            {
                return null;
            }

            string name;
            if (_prefabNames.TryGetValue(prefab, out name))
            {
                return name;
            }

            try
            {
                name = _prefabs.GetPrefabName(prefab);
            }
            catch
            {
                // An icon whose prefab will not resolve still gets counted, just without a name.
                name = null;
            }

            _prefabNames[prefab] = name;
            return name;
        }

        /// <summary>
        /// Takes the player to one of the places this alert is about, and remembers where it left
        /// them so pressing it again moves on to the next rather than showing the same building
        /// over and over.
        /// </summary>
        private void ViewAlert(string id)
        {
            try
            {
                var alert = Find(id);
                if (alert == null || alert.Locations.Count == 0)
                {
                    return;
                }

                int cursor;
                _viewCursor.TryGetValue(id, out cursor);

                var index = cursor % alert.Locations.Count;
                _viewCursor[id] = (index + 1) % alert.Locations.Count;

                CameraJump.To(_camera, alert.Locations[index]);
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not move the camera to alert '" + id + "'.");
            }
        }

        private void MuteAlert(string id, bool muted)
        {
            try
            {
                if (_memory.Memory.SetAlertMuted(id, muted))
                {
                    // Rebuild now: silencing something should take effect on the click, not on the
                    // next tick. Push the timer out too, or the next frame would redo this work.
                    Refresh();
                    _nextRefresh = UnityEngine.Time.time + RefreshInterval;
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not change the muted state of '" + id + "'.");
            }
        }

        private Alert Find(string id)
        {
            foreach (var alert in _set.Visible)
            {
                if (alert.Id == id)
                {
                    return alert;
                }
            }

            return null;
        }

        private void WriteAlerts(IJsonWriter writer)
        {
            writer.TypeBegin("adhd.Alerts");

            writer.PropertyName("peak");
            writer.Write((int)_set.Peak);

            writer.PropertyName("immediate");
            writer.Write(_set.ImmediateCount);

            writer.PropertyName("important");
            writer.Write(_set.ImportantCount);

            writer.PropertyName("monitor");
            writer.Write(_set.MonitorCount);

            writer.PropertyName("muted");
            writer.Write(_set.MutedCount);

            writer.PropertyName("items");
            WriteAlertArray(writer, _set.Visible);

            // The same rows again, for the ones the player silenced. Sent whether or not the
            // panel is showing them: a mute the player cannot find their way back to is not a
            // mute, it is something quietly taken away.
            writer.PropertyName("mutedItems");
            WriteAlertArray(writer, _set.Muted);

            writer.TypeEnd();
        }

        private static void WriteAlertArray(IJsonWriter writer, List<Alert> alerts)
        {
            writer.ArrayBegin((uint)alerts.Count);

            foreach (var alert in alerts)
            {
                writer.TypeBegin("adhd.Alert");

                writer.PropertyName("id");
                writer.Write(alert.Id);

                writer.PropertyName("subject");
                writer.Write((int)alert.Subject);

                writer.PropertyName("status");
                writer.Write((int)alert.Status);

                writer.PropertyName("titleKey");
                writer.Write(alert.TitleKey);

                writer.PropertyName("rawName");
                writer.Write(alert.RawName ?? string.Empty);

                writer.PropertyName("icon");
                writer.Write(alert.Icon ?? AlertIcons.Fallback);

                writer.PropertyName("affected");
                writer.Write(alert.AffectedCount);

                writer.PropertyName("canView");
                writer.Write(alert.Locations.Count > 0);

                writer.TypeEnd();
            }

            writer.ArrayEnd();
        }
    }
}
