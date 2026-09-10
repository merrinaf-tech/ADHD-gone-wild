using System;
using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ADHDGoneWild.Calm
{
    /// <summary>
    /// Stops the game's in-world notification icons from pulsing.
    ///
    /// This is the first piece of Calm UI, and it was chosen first on purpose. For someone with
    /// ADHD the expensive thing is rarely the amount of information on screen - it is how much of
    /// it grabs attention without being asked. A pulsing icon in peripheral vision pulls the eye
    /// whether or not the player decided to look, which is exactly the cost this mod exists to
    /// remove.
    ///
    /// So it takes away the movement and nothing else. Every icon stays exactly where it was, at
    /// exactly the size it was, saying exactly what it said. Nothing is hidden, so nothing can be
    /// missed - which is what makes this the cheapest possible calm: it has no information cost
    /// at all.
    ///
    /// The values live on the notification icon *prefab* entities - a few dozen, one per kind -
    /// so applying this is a handful of writes, not a walk over the city.
    /// </summary>
    public partial class CalmIconsSystem : GameSystemBase
    {
        private const string LogPrefix = "[Calm] ";

        /// <summary>
        /// The pulse amplitude sits in the y of each params pair; x is the display size and is
        /// never touched. Confirmed against NotificationIconPrefabSystem, which fills them as
        /// (m_DisplaySize.min, m_PulsateAmplitude.min) and (.max, .max).
        /// </summary>
        private const float Still = 0f;

        private EntityQuery _iconPrefabs;

        /// <summary>
        /// What each prefab looked like before we touched it, so switching the option off puts
        /// the game back exactly as it was rather than to a guess at the defaults.
        /// </summary>
        private readonly Dictionary<Entity, float4> _original = new Dictionary<Entity, float4>();

        private bool _applied;

        protected override void OnCreate()
        {
            base.OnCreate();

            // IgnoreComponentEnabledState: NotificationIconDisplayData is enableable, and the
            // icons a player has switched off are disabled rather than removed. Without this they
            // drop out of the query and would keep pulsing the moment they came back.
            _iconPrefabs = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<NotificationIconDisplayData>() },
                Options = EntityQueryOptions.IgnoreComponentEnabledState
            });

            // Nothing to do per frame: this reacts to a loaded game and to the option changing.
            Enabled = false;

            Mod.RegisterCalm(this);
            Mod.Log.Info("[Calm] Icon calm bridge initialised.");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            // The record of the untouched values is deliberately *not* cleared here.
            //
            // It used to be, on the assumption that prefabs are rebuilt with the game. They are
            // not: loading a second city logged "0 of 161 kinds changed", which can only mean the
            // prefab components still carried the values written during the first one. Clearing
            // then meant the next Apply() recorded our own zeroes as the originals, and switching
            // the option off afterwards restored zeroes - the icons stayed still for the rest of
            // the session with no way back short of restarting the game.
            //
            // Losing the only copy of something the player can no longer recover is the one
            // failure this mod is least allowed. So the record is kept for the life of the
            // process, and Apply() adds to it only what it has not already seen.
            _applied = false;

            if (mode == GameMode.Game)
            {
                ApplySettings();
            }
        }

        /// <summary>Called when the player changes the option.</summary>
        public void ApplySettings()
        {
            var settings = Mod.Settings;
            var wanted = settings != null && settings.CalmNotificationIcons;

            if (wanted == _applied)
            {
                return;
            }

            try
            {
                if (wanted)
                {
                    Apply();
                }
                else
                {
                    Restore();
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not change how the notification icons move.");
            }
        }

        private void Apply()
        {
            var entities = _iconPrefabs.ToEntityArray(Allocator.TempJob);

            try
            {
                var changed = 0;

                for (var i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    var data = EntityManager.GetComponentData<NotificationIconDisplayData>(entity);

                    // Remember the whole pair, not just what we change: restoring half of a value
                    // is worse than never touching it.
                    if (!_original.ContainsKey(entity))
                    {
                        _original.Add(entity, new float4(data.m_MinParams, data.m_MaxParams));
                    }

                    if (data.m_MinParams.y == Still && data.m_MaxParams.y == Still)
                    {
                        continue;
                    }

                    data.m_MinParams.y = Still;
                    data.m_MaxParams.y = Still;
                    EntityManager.SetComponentData(entity, data);
                    changed++;
                }

                _applied = true;
                Mod.Log.Info(LogPrefix + "Notification icons held still (" + changed + " of " +
                             entities.Length + " kinds changed).");
            }
            finally
            {
                entities.Dispose();
            }
        }

        private void Restore()
        {
            var restored = 0;

            foreach (var pair in _original)
            {
                var entity = pair.Key;

                // A prefab can disappear between switching this on and off - a mod unloading, for
                // instance. Skipping one is fine; throwing here would strand the rest.
                if (!EntityManager.Exists(entity) ||
                    !EntityManager.HasComponent<NotificationIconDisplayData>(entity))
                {
                    continue;
                }

                var data = EntityManager.GetComponentData<NotificationIconDisplayData>(entity);
                data.m_MinParams = pair.Value.xy;
                data.m_MaxParams = pair.Value.zw;
                EntityManager.SetComponentData(entity, data);
                restored++;
            }

            _original.Clear();
            _applied = false;
            Mod.Log.Info(LogPrefix + "Notification icons moving again (" + restored + " restored).");
        }

        protected override void OnUpdate()
        {
        }
    }
}
