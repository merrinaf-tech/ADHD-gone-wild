using System;
using System.Threading.Tasks;
using ADHDGoneWild.Localization;
using ADHDGoneWild.Memory;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Assets;
using Game.City;
using Game.Prefabs.Modes;
using Game.SceneFlow;
using Game.UI;
using Game.UI.Menu;
using UnityEngine;

namespace ADHDGoneWild.Creativity
{
    /// <summary>
    /// The creative safety net: a way back from an experiment, so trying something is cheap.
    ///
    /// The game has no undo and no rollback - a search of its assemblies turns up nothing of the
    /// kind - so a checkpoint here is a real save, made through the game's own save path. Nothing
    /// writes a bespoke format, which is why this cannot corrupt anything.
    ///
    /// What makes it worth having over simply saving the game, which the player could already do:
    ///
    /// - **No naming.** Saving asks for a name, and that is a decision demanded exactly when the
    ///   player is about to experiment - the worst possible moment to interrupt them.
    /// - **The way back stays visible.** A save you have forgotten gives no courage; the point of
    ///   a safety net is not the file, it is knowing it is there while you are being brave.
    /// - **Nothing to identify.** Going back is not "pick the right one out of a list", which is
    ///   memory work. It is this one, and one click.
    ///
    /// And what it must never become: a pending task. There is no count, nothing nags, and
    /// "keep" is not required - a forgotten checkpoint simply stops being mentioned.
    ///
    /// The player's own saves are never touched. This writes one save of its own per city and
    /// overwrites only that.
    /// </summary>
    public partial class CheckpointSystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[SafetyNet] ";

        private CityMemorySystem _memory;

        private RawValueBinding _checkpointBinding;
        private ValueBinding<bool> _enabledBinding;

        /// <summary>Saving and loading are slow and asynchronous; the UI must not offer both twice.</summary>
        private bool _busy;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();
            _memory.CityChanged += OnCityChanged;

            _checkpointBinding = new RawValueBinding(Group, "checkpoint", WriteCheckpoint);
            AddBinding(_checkpointBinding);

            _enabledBinding = new ValueBinding<bool>(Group, "safetyNetEnabled", Enabled_());
            AddBinding(_enabledBinding);

            AddBinding(new TriggerBinding(Group, "createCheckpoint", CreateCheckpoint));
            AddBinding(new TriggerBinding(Group, "restoreCheckpoint", RestoreCheckpoint));
            AddBinding(new TriggerBinding(Group, "forgetCheckpoint", ForgetCheckpoint));

            Mod.RegisterCheckpoints(this);
            Mod.Log.Info("[UI] Safety net bridge initialised.");
        }

        private void OnCityChanged()
        {
            _busy = false;
            ForgetTheCheckpointIfItIsNotOurs();
            _checkpointBinding.Update();
            ApplySettings();
        }

        /// <summary>
        /// Makes sure the way back this city is about to be offered is real, and is this city's.
        ///
        /// Two things can leave a promise that cannot be kept. The player may have deleted the
        /// save themselves - it is an ordinary save in their own list, and they are entitled to.
        /// Or the record may have arrived with notes carried over from before every city had its
        /// own file, in which case the save it names belongs to some other city.
        ///
        /// Both end the same way: stop saying there is a way back. A panel that offers one and
        /// cannot deliver is worse than one that offers nothing, because the whole feature is a
        /// promise made in advance.
        ///
        /// A save that turns out to belong elsewhere is deleted rather than left alone. It is one
        /// of ours, named by us, and after this nothing will ever refer to it again - leaving it
        /// would be abandoning a file in the player's list under a name only we understand.
        /// </summary>
        private void ForgetTheCheckpointIfItIsNotOurs()
        {
            if (!_memory.InGame || !_memory.Memory.IsLoaded || !_memory.Memory.HasCheckpoint)
            {
                return;
            }

            var name = _memory.Memory.CheckpointName;

            try
            {
                var database = AssetDatabase.user;

                SaveGameMetadata metadata;
                if (!database.Exists(SaveHelpers.GetAssetDataPath<SaveGameMetadata>(database, name), out metadata) ||
                    metadata == null || metadata.target == null)
                {
                    Mod.Log.Info(LogPrefix + "The checkpoint save '" + name + "' is gone; forgetting it.");
                    _memory.Memory.ClearCheckpoint();
                    return;
                }

                // The save records the city it was taken in, from the same source we read the
                // current one from, so this is a like-for-like comparison.
                if (!string.Equals(metadata.target.cityName, _memory.Memory.CityName, StringComparison.Ordinal))
                {
                    Mod.Log.Info(LogPrefix + "The checkpoint '" + name + "' was taken in '" +
                                 metadata.target.cityName + "', not '" + _memory.Memory.CityName +
                                 "'; letting it go.");
                    DeleteSave(database, name);
                    _memory.Memory.ClearCheckpoint();
                }
            }
            catch (Exception e)
            {
                // Leaving the record alone is the safe way to fail: the save may well be fine,
                // and restoring already copes with one that is not.
                Mod.Log.Error(e, LogPrefix + "Could not check whether the checkpoint belongs to this city.");
            }
        }

        public void ApplySettings()
        {
            _enabledBinding.Update(Enabled_());
            _checkpointBinding.Update();
        }

        private static bool Enabled_()
        {
            var settings = Mod.Settings;
            return settings == null || settings.CreativeSafetyNetEnabled;
        }

        /// <summary>
        /// The name the player will read in their own list of saves. Built in
        /// <see cref="CheckpointName"/>, which is kept apart so it can be tested without the
        /// game running.
        /// </summary>
        private string NewSaveName()
        {
            return CheckpointName.Build(
                _memory.Memory.CityName,
                Localised(L10n.SafetyNetSaveMarker, "safety net"),
                DateTime.Now);
        }

        /// <summary>
        /// The mod's own wording, in the player's language, read the way the game reads its own.
        /// </summary>
        private static string Localised(string key, string fallback)
        {
            try
            {
                var manager = GameManager.instance != null ? GameManager.instance.localizationManager : null;

                string value;
                if (manager != null && manager.activeDictionary != null &&
                    manager.activeDictionary.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                Mod.Log.Warn(LogPrefix + "Could not read the localised save marker (" + e.Message + ").");
            }

            return fallback;
        }

        // ---- Making one ---------------------------------------------------------------------

        private void CreateCheckpoint()
        {
            if (_busy || !_memory.InGame || !_memory.Memory.IsLoaded || !Enabled_())
            {
                return;
            }

            _busy = true;
            _checkpointBinding.Update();

            // Deliberately not awaited: a trigger handler cannot block the UI thread for the
            // seconds a save of a large city takes. Everything that can fail is caught inside.
            var ignored = CreateCheckpointAsync();
        }

        private async Task CreateCheckpointAsync()
        {
            var name = NewSaveName();
            var previous = _memory.Memory.CheckpointName;
            RenderTexture preview = null;

            try
            {
                var menu = World.GetExistingSystemManaged<MenuUISystem>();
                if (menu == null)
                {
                    Mod.Log.Warn(LogPrefix + "No menu system; cannot take a checkpoint.");
                    return;
                }

                // The same sequence the game's own autosave uses, including the preview it
                // captures for the save list.
                preview = ScreenCaptureHelper.CreateRenderTarget("ADHDGoneWild-Checkpoint", 680, 383);
                ScreenCaptureHelper.CaptureScreenshot(Camera.main, preview, new MenuHelpers.SaveGamePreviewSettings());

                var database = AssetDatabase.user;

                // Replacing our own previous checkpoint for this city, never anyone else's save.
                //
                // The old one is found by the name recorded in the city's memory, not by
                // rebuilding it: the name now carries the time it was taken, so it is different
                // every time and cannot be worked out again. Deleting only the new name - which
                // is what this did while the name was fixed - would quietly leave a checkpoint
                // behind on every single use.
                DeleteSave(database, previous);
                DeleteSave(database, name);

                // autoSave: true keeps it out of the player's own list of saves. It is our
                // scaffolding, not something they made and have to tidy up.
                var info = menu.GetSaveInfo(autoSave: true);

                await GameManager.instance.Save(name, info, database, preview);

                _memory.Memory.SetCheckpoint(name, DateTime.UtcNow);
                Mod.Log.Info(LogPrefix + "Checkpoint taken, saved as '" + name + "'.");
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not take a checkpoint.");
            }
            finally
            {
                if (preview != null)
                {
                    UnityEngine.Object.Destroy(preview);
                }

                _busy = false;
                _checkpointBinding.Update();
            }
        }

        // ---- Going back ---------------------------------------------------------------------

        private void RestoreCheckpoint()
        {
            if (_busy || !_memory.InGame || !_memory.Memory.HasCheckpoint)
            {
                return;
            }

            _busy = true;
            _checkpointBinding.Update();

            var ignored = RestoreCheckpointAsync(_memory.Memory.CheckpointName);
        }

        /// <summary>
        /// Loads our checkpoint back, mirroring what MenuUISystem does around its own load: the
        /// map metadata, the configuration overrides and the game mode all have to be in place
        /// before GameManager.Load, or the city comes back subtly wrong.
        ///
        /// This is the riskiest thing the mod does, and the reason it is survivable: it loads a
        /// save we made minutes ago in this same session, and the player's own saves are never
        /// written to, so the worst case is a bad load they can walk away from.
        /// </summary>
        private async Task RestoreCheckpointAsync(string name)
        {
            try
            {
                var database = AssetDatabase.user;

                // The metadata is what Load actually takes, and it carries the SaveInfo we need
                // for the setup below - so it is fetched directly rather than via the package.
                SaveGameMetadata metadata;
                if (!database.Exists(SaveHelpers.GetAssetDataPath<SaveGameMetadata>(database, name), out metadata) ||
                    metadata == null)
                {
                    Mod.Log.Warn(LogPrefix + "The checkpoint save is gone; forgetting it.");
                    _memory.Memory.ClearCheckpoint();
                    _checkpointBinding.Update();
                    return;
                }

                var saveInfo = metadata.target;
                if (saveInfo == null)
                {
                    Mod.Log.Warn(LogPrefix + "The checkpoint holds no save details; forgetting it.");
                    _memory.Memory.ClearCheckpoint();
                    _checkpointBinding.Update();
                    return;
                }

                var maps = World.GetOrCreateSystemManaged<MapMetadataSystem>();
                var config = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
                var modes = World.GetOrCreateSystemManaged<GameModeSystem>();

                maps.mapName = saveInfo.mapName;
                maps.prefabReferences = saveInfo.prefabReferences;
                config.overrideLoadedOptions = true;
                config.overrideThemeName = null;
                config.overrideCityName = saveInfo.cityName;
                // Taken from the save rather than assumed: it is what the game does when it
                // loads one of its own.
                modes.overrideMode = saveInfo.gameMode;

                Mod.Log.Info(LogPrefix + "Restoring the checkpoint for '" + _memory.Memory.CityKey + "'.");

                await GameManager.instance.Load(GameMode.Game, Purpose.LoadGame, metadata);
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not restore the checkpoint.");
            }
            finally
            {
                _busy = false;
            }
        }

        // ---- Letting it go ------------------------------------------------------------------

        /// <summary>
        /// "I am keeping what I did." Deletes the save and forgets it - the safety net has done
        /// its job and should stop occupying any space, on disk or on screen.
        /// </summary>
        private void ForgetCheckpoint()
        {
            try
            {
                DeleteSave(AssetDatabase.user, _memory.Memory.CheckpointName);

                _memory.Memory.ClearCheckpoint();
                _checkpointBinding.Update();
                Mod.Log.Info(LogPrefix + "Checkpoint let go.");
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not let go of the checkpoint.");
            }
        }

        /// <summary>
        /// Removes one of our own saves, by the name we recorded for it. A name we never wrote is
        /// never passed here, so this cannot reach a save the player made.
        /// </summary>
        private static void DeleteSave(ILocalAssetDatabase database, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            PackageAsset asset;
            if (database.Exists<PackageAsset>(SaveHelpers.GetAssetDataPath<SaveGameMetadata>(database, name), out asset))
            {
                database.DeleteAsset(asset);
            }
        }

        private void WriteCheckpoint(IJsonWriter writer)
        {
            var memory = _memory.Memory;

            writer.TypeBegin("adhd.Checkpoint");

            writer.PropertyName("exists");
            writer.Write(memory.HasCheckpoint);

            writer.PropertyName("takenUnixUtc");
            writer.Write(memory.CheckpointUnixUtc);

            writer.PropertyName("busy");
            writer.Write(_busy);

            writer.TypeEnd();
        }
    }
}
