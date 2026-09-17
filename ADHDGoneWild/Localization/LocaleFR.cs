using System.Collections.Generic;
using ADHDGoneWild.Alerts;
using ADHDGoneWild.BrainParking;
using ADHDGoneWild.Settings;
using Colossal;

namespace ADHDGoneWild.Localization
{
    /// <summary>
    /// The fr-FR source.
    ///
    /// House style carries over from <see cref="LocaleEN"/> and matters more than literalness:
    /// short, calm, never about the player. The alert lines state what is true and stop, without
    /// sliding into the advice French makes so easy ("il faut construire une centrale"), and the
    /// body notes stay a direct ask with a warm reason rather than an order.
    ///
    /// The player is addressed as "tu" throughout. "Vous" would be more formal and less like
    /// something on your side, which is the whole voice of the mod.
    ///
    /// Anything missing here falls back to English rather than showing a key, so a gap is a
    /// cosmetic loss. <see cref="LocaleCoverage"/> logs what is missing at load.
    /// </summary>
    public class LocaleFR : IDictionarySource
    {
        private readonly AdhdSettings _settings;

        public LocaleFR(AdhdSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // ---- Page d'options ---------------------------------------------------------
                { _settings.GetSettingsLocaleID(), Mod.Name },
                { _settings.GetOptionTabLocaleID(AdhdSettings.MainSection), "Principal" },

                { _settings.GetOptionGroupLocaleID(AdhdSettings.InformationGroup), "Informations" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.MemoryGroup), "Mémoire" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.CreativityGroup), "Créativité" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.WellbeingGroup), "Bien-être" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.AboutGroup), "À propos" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "Figer les icônes sur la carte"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "Les icônes d'alerte du jeu clignotent, et l'œil y va que tu le veuilles ou non. Ceci arrête le mouvement. Rien n'est caché : elles restent où elles sont, de la même taille, à dire la même chose."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "Bouton pour replier la barre"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "Une ligne dans le panneau de ce mod qui replie la barre de construction et la ramène. Elle y vit plutôt que de flotter sur le jeu, où aucune résolution ni aucun autre mod ne peut la rendre inatteignable."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Alertes regroupées"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Rassemble les alertes de la ville en une seule liste courte, regroupées au lieu d'être répétées."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "Parle-moi de"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "Ce qui arrive jusqu'à la liste. Ce qui reste dehors se produit quand même : ce n'est simplement pas à l'écran."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.OnlyImmediate), "Uniquement l'immédiat" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.ImportantAndUp), "Immédiat et important" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.Everything), "Tout" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Garer une pensée"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Laisse une pensée sur la carte avec un raccourci et un clic, puis continue ce que tu faisais."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Garer une idée"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Appuie, puis clique là où la pensée a sa place. Appuie encore pour laisser tomber."
                },
                { _settings.GetBindingKeyLocaleID(AdhdSettings.ParkIdeaActionName), "Garer une idée" },
                { _settings.GetBindingMapLocaleID(), Mod.Name },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "Les nouvelles idées sont du type"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "Ce qu'est une idée fraîchement garée. Tu peux les changer plus tard, ou jamais."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "Proposer d'ajouter une note"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "Après avoir garé une pensée, propose d'en choisir le type et d'ajouter un titre. La pensée est déjà enregistrée dans tous les cas : désactive ceci pour raccourci, clic, terminé."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowIdeaAge)),
                    "Afficher quand une idée a été garée"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowIdeaAge)),
                    "Ajoute « il y a 4 jours » sous chacune. Désactivé par défaut : rien d'une idée garée ne change avec le temps, rien n'expire et rien n'est jamais relancé - et un nombre qui monte est la seule chose capable de faire ressembler une note à quelque chose qui vous attend. Utile tout de même pour distinguer les pensées de cette session de celles d'une ville plus ancienne."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Montrer les idées sur la carte"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Laisse un petit anneau là où chaque pensée a été garée, pour que l'endroit lui-même te le rappelle. Cliquer l'anneau ouvre l'idée."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "Bon retour"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "Un petit bonjour quand tu reviens dans une ville après une vraie pause, avec assez de contexte pour ne pas avoir à le reconstituer toi-même."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "Seulement après une absence de"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "Un bref alt-tab ne mérite pas de salutations. Voici la durée minimale de la pause."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.TenMinutes), "10 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThirtyMinutes), "30 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneHour), "1 heure" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThreeHours), "3 heures" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneDay), "Un jour ou plus" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AttentionTrailEnabled)),
                    "J'en étais où ?"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AttentionTrailEnabled)),
                    "Une courte trace des endroits où vous avez passé du temps pendant cette session, du plus récent au plus ancien. Ouvrez-en un pour retrouver la vue que vous aviez. Rien n'est enregistré, rien ne survit à la fermeture de la ville, et rien n'y est jamais qualifié d'inachevé."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "Filet de sécurité"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "Un retour en arrière après un essai, en un clic. Le mod enregistre la partie à ta place, séparément de tes propres sauvegardes, pour que tu puisses tenter quelque chose et changer d'avis sans rien nommer ni rien chercher."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "Rappeler l'heure de temps en temps"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "Une petite carte dit quelle heure il est et depuis combien de temps tu es là, puis s'en va. Elle ne suggère jamais d'arrêter : la regarder et continuer à construire est un très bon résultat."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "À quelle fréquence"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "Combien de jeu s'écoule entre deux rappels. Le temps passé dans le menu principal ne compte pas."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "Ajouter un mot sur le corps"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "Une courte deuxième ligne sous l'horloge : eau, épaules, se lever. Il n'y a jamais de suite et rien n'est compté. Désactivé, il te reste l'heure seule."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoMinutes), "Toutes les 2 minutes (pour voir ce que ça donne)" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThirtyMinutes), "Toutes les 30 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.OneHour), "Toutes les heures" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.NinetyMinutes), "Toutes les 90 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoHours), "Toutes les 2 heures" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThreeHours), "Toutes les 3 heures" },

                { _settings.GetOptionGroupLocaleID(AdhdSettings.AppearanceGroup), "Apparence" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.InterfaceHue)),
                    "Couleur des panneaux"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.InterfaceHue)),
                    "La couleur des panneaux de ce mod. Seule la teinte change : ils restent assez sombres pour qu'on y lise, où que tu places le curseur. Par défaut, celle des panneaux du jeu."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.Version)),
                    "Version"
                },

                { _settings.GetEnumValueLocaleID(IdeaCategory.Idea), "Idée" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Build), "Construire quelque chose" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Decoration), "Décoration" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Transport), "Transports" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Fix), "Corriger ou changer" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Other), "Autre" },

                // ---- Panneau en jeu ---------------------------------------------------------
                { L10n.PanelTitle, "Idées garées" },
                { L10n.ToolbarTooltip, Mod.Name },

                { L10n.ParkIdea, "Garer une idée" },
                { L10n.ParkingHint, "Clique n'importe où pour y laisser une pensée." },
                { L10n.ParkingCancel, "Laisse tomber" },

                { L10n.Empty, "Rien de garé ici pour l'instant." },
                { L10n.EmptyHint, "Appuie sur le raccourci, clique la carte. C'est tout." },

                { L10n.View, "Y aller" },
                { L10n.Forget, "Oublier" },
                { L10n.NotePlaceholder, "Un mot, si tu veux" },

                { L10n.Disabled, "Garer une pensée est désactivé dans les options." },
                { L10n.Close, "Fermer" },
                { L10n.JustParkedSaved, "Enregistré. Quelque chose à ajouter ?" },
                { L10n.ToolbarFold, "Replier la barre" },
                { L10n.ToolbarUnfold, "Ramener la barre" },

                // ---- Filet de sécurité ------------------------------------------------------
                // Une réassurance, jamais une corvée. Rien ici ne compte quoi que ce soit et rien
                // ne demande à être terminé.
                { L10n.SafetyNetTitle, "Filet de sécurité" },
                { L10n.SafetyNetCreate, "Enregistrer un retour possible" },
                { L10n.SafetyNetHint, "Pour tenter quelque chose et pouvoir changer d'avis." },
                { L10n.SafetyNetHave, "Tu peux revenir à ce point" },
                { L10n.SafetyNetRestore, "Revenir" },
                { L10n.SafetyNetKeep, "Je garde" },
                { L10n.SafetyNetWorking, "Un instant..." },
                { L10n.SafetyNetSaveMarker, "filet de sécurité" },

                // L'heure, et rien d'autre. Aucun conseil, aucune invitation à arrêter, aucun avis
                // sur la durée d'une session.
                { L10n.HyperfocusHereFor, "Tu es ici depuis" },
                { L10n.HyperfocusHourOne, "heure" },
                { L10n.HyperfocusHoursMany, "heures" },
                { L10n.HyperfocusMinutesMany, "minutes" },
                { L10n.HyperfocusThanks, "Merci" },
                { L10n.HyperfocusLater, "Plus tard" },

                // Une demande directe, plus une raison chaleureuse ou une offre. Jamais un ordre
                // sec, jamais une remarque ironique : dite de biais, elle sonnerait comme un
                // commentaire sur la personne. S'il y a une blague, elle vise un dos ou une ville,
                // jamais celui qui lit.
                { L10n.BodyNote[0], "De l'eau. Vas-y, je surveille la ville." },
                { L10n.BodyNote[1], "Mange quelque chose. Bâtir des villes, ça creuse." },
                { L10n.BodyNote[2], "Lève-toi et bouge une minute. Je ne demande rien de plus." },
                { L10n.BodyNote[3], "Regarde quelque chose au loin. Tes yeux te diront merci." },
                { L10n.BodyNote[4], "Desserre la mâchoire. Baisse les épaules. Ouvre les poings." },
                { L10n.BodyNote[5], "Va dormir. Cette route sera encore là demain." },

                // ---- Bon retour -------------------------------------------------------------
                { L10n.WelcomeBackTitle, "Bon retour à" },
                { L10n.WelcomeBackViewPlace, "Tu étais par ici" },
                { L10n.WelcomeBackIdeasOne, "1 idée enregistrée ici" },
                { L10n.WelcomeBackIdeasMany, "idées enregistrées ici" },

                // ---- Alertes ----------------------------------------------------------------
                // Chaque ligne dit quelque chose sur la ville. Aucune ne dit au joueur quoi en
                // faire : cette décision reste la sienne.
                { L10n.AlertsTitle, "En ce moment" },
                { L10n.AlertsQuiet, "Rien à signaler." },
                { L10n.AlertsQuietHint, "La ville tourne comme avant." },
                { L10n.AlertsDisabled, "Les alertes regroupées sont désactivées dans les options." },
                { L10n.AlertsMute, "Masquer" },
                { L10n.AlertsUnmute, "Réafficher" },
                { L10n.AlertsMutedCount, "masquées" },
                { L10n.AffectedOne, "1 bâtiment" },
                { L10n.AffectedMany, "bâtiments" },

                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Immediate),
                    "Une partie de la ville est sans électricité."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Important),
                    "La demande d'électricité a rejoint la production."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Monitor),
                    "La demande d'électricité approche la production."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Immediate),
                    "Une partie de la ville est sans eau."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Important),
                    "La demande en eau a rejoint la capacité."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Monitor),
                    "La demande en eau approche la capacité."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Immediate),
                    "Une partie des eaux usées n'est pas traitée."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Important),
                    "Le traitement des eaux usées a rejoint la capacité."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Monitor),
                    "Le traitement des eaux usées approche la capacité."
                },

                { L10n.IdeaDescriptionPlaceholder, "Quelque chose à retenir" },
                { L10n.HasDescription, "a des notes" },
                { L10n.IdeaAddNote, "Ajouter une ligne" },

                { L10n.TrailTitle, "J'en étais où ?" },
                { L10n.TrailEmpty, "Nulle part ailleurs pour l'instant" },
                { L10n.TrailEmptyHint, "Les lieux que vous quittez apparaissent ici." },
                { L10n.TrailBack, "Ramène-moi là-bas" },
                { L10n.TrailActivity[0], "En train de regarder" },
                { L10n.TrailActivity[1], "Tracé" },
                { L10n.TrailActivity[2], "Placement" },
                { L10n.TrailActivity[3], "Zonage" },
                { L10n.TrailActivity[4], "Démolition" },
                { L10n.TrailActivity[5], "Terrassement" },
                { L10n.TrailActivity[6], "Zones" },
                { L10n.TrailActivity[7], "Lignes de transport" },
                { L10n.TrailActivity[8], "Amélioration" },

                { L10n.CategoryIdea, "Idée" },
                { L10n.CategoryBuild, "Construire quelque chose" },
                { L10n.CategoryDecoration, "Décoration" },
                { L10n.CategoryTransport, "Transports" },
                { L10n.CategoryFix, "Corriger ou changer" },
                { L10n.CategoryOther, "Autre" }
            };
        }

        public void Unload()
        {
        }
    }
}
