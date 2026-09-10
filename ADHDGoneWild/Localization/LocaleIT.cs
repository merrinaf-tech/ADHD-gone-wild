using System.Collections.Generic;
using ADHDGoneWild.Alerts;
using ADHDGoneWild.BrainParking;
using ADHDGoneWild.Settings;
using Colossal;

namespace ADHDGoneWild.Localization
{
    /// <summary>
    /// The it-IT source.
    ///
    /// House style carries over from <see cref="LocaleEN"/> and matters more than literalness:
    /// short, calm, never about the player. Two places where a literal translation would be
    /// wrong, and where this deliberately is not one:
    ///
    /// - The alert lines report what is true and stop. Italian invites an implied instruction
    ///   ("serve una centrale") that the English carefully avoids; it is avoided here too.
    /// - The body notes are a direct ask plus a warm reason, and the joke - where there is one -
    ///   is aimed at a spine or a city, never at the reader. "Alzati e muoviti un minuto. Non
    ///   chiedo altro" keeps the small ask of the English rather than the words.
    ///
    /// Anything missing here falls back to English rather than showing a key, so a gap is a
    /// cosmetic loss. <see cref="LocaleCoverage"/> logs what is missing at load.
    /// </summary>
    public class LocaleIT : IDictionarySource
    {
        private readonly AdhdSettings _settings;

        public LocaleIT(AdhdSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // ---- Pagina opzioni ---------------------------------------------------------
                { _settings.GetSettingsLocaleID(), Mod.Name },
                { _settings.GetOptionTabLocaleID(AdhdSettings.MainSection), "Principale" },

                { _settings.GetOptionGroupLocaleID(AdhdSettings.InformationGroup), "Informazioni" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.MemoryGroup), "Memoria" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.CreativityGroup), "Creatività" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.WellbeingGroup), "Benessere" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.AccessibilityGroup), "Accessibilità" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.AboutGroup), "Il mod" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "Ferma le icone sulla mappa"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "Le icone di avviso del gioco pulsano, e l'occhio ci va che tu voglia o no. Questo ferma il movimento. Non nasconde niente: restano dove sono, della stessa dimensione, a dire la stessa cosa."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "Comando per chiudere la barra"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "Un piccolo comando accanto alla barra di costruzione che la chiude e la riapre. I pulsanti sono sempre a un clic dal tornare."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Avvisi raggruppati"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Raccoglie gli avvisi della città in un solo elenco breve, raggruppati invece che ripetuti."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "Dimmi di"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "Quanto arriva all'elenco. Quello che resta fuori sta comunque accadendo: semplicemente non è a schermo."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.OnlyImmediate), "Solo ciò che è immediato" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.ImportantAndUp), "Immediato e importante" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.Everything), "Tutto" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Parcheggio dei pensieri"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Lascia un pensiero sulla mappa con una scorciatoia e un clic, e continua quello che stavi facendo."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Parcheggia un'idea"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Premi questo, poi clicca dove il pensiero appartiene. Premilo di nuovo per lasciar perdere."
                },
                { _settings.GetBindingKeyLocaleID(AdhdSettings.ParkIdeaActionName), "Parcheggia un'idea" },
                { _settings.GetBindingMapLocaleID(), Mod.Name },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "Le nuove idee sono di tipo"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "Come nasce un'idea appena parcheggiata. Puoi cambiarle tutte dopo, oppure mai."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "Proponi di aggiungere una nota"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "Dopo aver parcheggiato un pensiero, propone di scegliere il tipo e aggiungere un titolo. Il pensiero è già salvato in ogni caso: spegnilo per avere scorciatoia, clic, fatto."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Mostra le idee sulla mappa"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Lascia un piccolo anello dove ogni pensiero è stato parcheggiato, così è il posto stesso a ricordartelo. Cliccando l'anello si apre l'idea."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "Bentornato"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "Un piccolo saluto quando torni in una città dopo una pausa vera, con abbastanza contesto da non doverlo ricostruire da solo."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "Solo dopo un'assenza di"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "Un alt-tab breve non merita un saluto. Questa è la durata minima della pausa."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.TenMinutes), "10 minuti" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThirtyMinutes), "30 minuti" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneHour), "1 ora" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThreeHours), "3 ore" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneDay), "Un giorno o più" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "Rete di sicurezza"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "Un modo per tornare indietro da un esperimento, con un clic. Salva la partita al posto tuo, separatamente dai tuoi salvataggi, così puoi provare qualcosa e cambiare idea senza dare nomi né cercare niente."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "Ricorda che ore sono, ogni tanto"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "Una piccola scheda dice che ore sono e da quanto sei qui, poi se ne va. Non suggerisce mai di smettere: guardarla e continuare a costruire è un esito perfettamente buono."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "Ogni quanto"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "Quanto gioco passa tra un promemoria e l'altro. Il tempo nel menu principale non conta."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "Aggiungi una riga sul corpo"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "Una seconda riga breve sotto l'orologio: acqua, spalle, alzarsi. Non c'è mai un seguito e non viene contato niente. Spenta, ti resta solo l'ora."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoMinutes), "Ogni 2 minuti (per vedere com'è fatta)" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThirtyMinutes), "Ogni 30 minuti" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.OneHour), "Ogni ora" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.NinetyMinutes), "Ogni 90 minuti" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoHours), "Ogni 2 ore" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThreeHours), "Ogni 3 ore" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ColourPalette)),
                    "Palette dei colori"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ColourPalette)),
                    "Qui il colore non porta mai il significato da solo: ogni stato ha anche un'icona e una forma."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.Version)),
                    "Versione"
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.Palette.Standard), "Standard" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Palette.ColourBlindFriendly), "Adatta al daltonismo" },

                { _settings.GetEnumValueLocaleID(IdeaCategory.Idea), "Idea" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Build), "Costruire qualcosa" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Decoration), "Decorazione" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Transport), "Trasporti" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Fix), "Sistemare o cambiare" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Other), "Altro" },

                // ---- Pannello in gioco ------------------------------------------------------
                { L10n.PanelTitle, "Idee parcheggiate" },
                { L10n.ToolbarTooltip, Mod.Name },

                { L10n.ParkIdea, "Parcheggia un'idea" },
                { L10n.ParkingHint, "Clicca dove vuoi per lasciarci un pensiero." },
                { L10n.ParkingCancel, "Lascia stare" },

                { L10n.Empty, "Qui non c'è ancora niente." },
                { L10n.EmptyHint, "Premi la scorciatoia, clicca la mappa. È tutto qui." },

                { L10n.View, "Vai" },
                { L10n.Forget, "Dimentica" },
                { L10n.NotePlaceholder, "Una parola, se ti va" },

                { L10n.Disabled, "Il parcheggio dei pensieri è spento nelle opzioni." },
                { L10n.Close, "Chiudi" },
                { L10n.JustParkedSaved, "Salvato. Vuoi aggiungere qualcosa?" },
                { L10n.ToolbarFold, "Chiudi la barra" },
                { L10n.ToolbarUnfold, "Riapri la barra" },

                // ---- Rete di sicurezza ------------------------------------------------------
                // Rassicurazione, mai una commissione. Niente qui conta nulla o chiede di essere
                // chiuso.
                { L10n.SafetyNetTitle, "Rete di sicurezza" },
                { L10n.SafetyNetCreate, "Salva una via di ritorno" },
                { L10n.SafetyNetHint, "Così puoi provare qualcosa e cambiare idea." },
                { L10n.SafetyNetHave, "Puoi tornare a questo punto" },
                { L10n.SafetyNetRestore, "Torna indietro" },
                { L10n.SafetyNetKeep, "Tengo quello che ho fatto" },
                { L10n.SafetyNetWorking, "Un momento..." },
                { L10n.SafetyNetSaveMarker, "rete di sicurezza" },

                // L'orologio, e nient'altro. Nessun consiglio, nessun invito a smettere, nessuna
                // opinione sulla durata della sessione.
                { L10n.HyperfocusHereFor, "Sei qui da" },
                { L10n.HyperfocusHourOne, "ora" },
                { L10n.HyperfocusHoursMany, "ore" },
                { L10n.HyperfocusMinutesMany, "minuti" },
                { L10n.HyperfocusThanks, "Grazie" },
                { L10n.HyperfocusLater, "Più tardi" },

                // Richiesta diretta più un motivo caldo o un'offerta. Mai un comando secco, mai
                // un'osservazione ironica: detta di traverso suonerebbe come un commento sulla
                // persona. Se c'è una battuta, il bersaglio è una schiena o una città, mai chi legge.
                { L10n.BodyNote[0], "Acqua. Vai pure, alla città ci penso io." },
                { L10n.BodyNote[1], "Mangia qualcosa. Costruire città mette appetito." },
                { L10n.BodyNote[2], "Alzati e muoviti un minuto. Non chiedo altro." },
                { L10n.BodyNote[3], "Guarda qualcosa di lontano. I tuoi occhi ringraziano." },
                { L10n.BodyNote[4], "Rilassa la mascella. Abbassa le spalle. Apri le mani." },
                { L10n.BodyNote[5], "Vai a dormire. Quella strada domani è ancora lì." },

                // ---- Bentornato -------------------------------------------------------------
                { L10n.WelcomeBackTitle, "Bentornato a" },
                { L10n.WelcomeBackViewPlace, "Eri da queste parti" },
                { L10n.WelcomeBackIdeasOne, "1 idea salvata qui" },
                { L10n.WelcomeBackIdeasMany, "idee salvate qui" },

                // ---- Avvisi -----------------------------------------------------------------
                // Ogni riga dice qualcosa sulla città. Nessuna dice al giocatore cosa farci: quella
                // decisione resta sua. L'italiano invita a scivolare nel consiglio ("serve una
                // centrale") ed è esattamente ciò che qui si evita.
                { L10n.AlertsTitle, "Adesso" },
                { L10n.AlertsQuiet, "Niente da segnalare." },
                { L10n.AlertsQuietHint, "La città va come prima." },
                { L10n.AlertsDisabled, "Gli avvisi raggruppati sono spenti nelle opzioni." },
                { L10n.AlertsMute, "Silenzia" },
                { L10n.AlertsUnmute, "Riattiva" },
                { L10n.AlertsMutedCount, "silenziati" },
                { L10n.AffectedOne, "1 edificio" },
                { L10n.AffectedMany, "edifici" },

                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Immediate),
                    "Una parte della città è senza elettricità."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Important),
                    "La domanda di elettricità ha raggiunto la produzione."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Monitor),
                    "La domanda di elettricità è vicina alla produzione."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Immediate),
                    "Una parte della città è senza acqua."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Important),
                    "La domanda d'acqua ha raggiunto la capacità."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Monitor),
                    "La domanda d'acqua è vicina alla capacità."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Immediate),
                    "Una parte delle acque reflue non viene trattata."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Important),
                    "Il trattamento delle acque reflue ha raggiunto la capacità."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Monitor),
                    "Il trattamento delle acque reflue è vicino alla capacità."
                },

                { L10n.CategoryIdea, "Idea" },
                { L10n.CategoryBuild, "Costruire qualcosa" },
                { L10n.CategoryDecoration, "Decorazione" },
                { L10n.CategoryTransport, "Trasporti" },
                { L10n.CategoryFix, "Sistemare o cambiare" },
                { L10n.CategoryOther, "Altro" }
            };
        }

        public void Unload()
        {
        }
    }
}
