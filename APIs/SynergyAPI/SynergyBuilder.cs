using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Gungeon;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Alexandria.ItemAPI;

namespace SpecialStuffPack.SynergyAPI
{
    /// <summary>
    /// The core class of SynergyAPI that allows you to build synergies.
    /// </summary>
    public static class SynergyBuilder
    {
        /// <summary>
        /// Inits SynergyAPI.
        /// </summary>
        public static void InitSynergyBuilder()
        {
            synergies = new List<AdvancedSynergyEntry>();
        }

        /// <summary>
        /// Unloads SynergyAPI.
        /// </summary>
        public static void Unload()
        {
            m_synergyTable = null;
            synergies?.Clear();
            synergies = null;
        }

        /// <summary>
        /// Adds <paramref name="obj"/> to an already existing synergy with the <see cref="CustomSynergyType"/> of <paramref name="type"/>.
        /// </summary>
        /// <param name="obj">The item to add to the synergy.</param>
        /// <param name="type">The <see cref="CustomSynergyType"/> of the synergy.</param>
        public static void AddItemToSynergy(this PickupObject obj, CustomSynergyType type)
        {
            AddItemToSynergy(type, obj.PickupObjectId);
        }

        /// <summary>
        /// Adds <paramref name="obj"/> to an already existing synergy with the name key of <paramref name="nameKey"/>.
        /// </summary>
        /// <param name="obj">The item to add to the synergy.</param>
        /// <param name="nameKey">The name key of the synergy.</param>
        public static void AddItemToSynergy(this PickupObject obj, string nameKey)
        {
            AddItemToSynergy(nameKey, obj.PickupObjectId);
        }

        /// <summary>
        /// Adds an item with the id of <paramref name="id"/> to an already existing synergy with the <see cref="CustomSynergyType"/> of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="CustomSynergyType"/> of the synergy.</param>
        /// <param name="id">The id of the item that will be added to the synergy.</param>
        public static void AddItemToSynergy(CustomSynergyType type, int id)
        {
            foreach (AdvancedSynergyEntry entry in GameManager.Instance.SynergyManager.synergies)
            {
                if (entry.bonusSynergies.Contains(type))
                {
                    if (PickupObjectDatabase.GetById(id) != null)
                    {
                        PickupObject obj = PickupObjectDatabase.GetById(id);
                        if (obj is Gun)
                        {
                            if (entry.OptionalGunIDs != null)
                            {
                                entry.OptionalGunIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalGunIDs = new List<int> { id };
                            }
                        }
                        else
                        {
                            if (entry.OptionalItemIDs != null)
                            {
                                entry.OptionalItemIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalItemIDs = new List<int> { id };
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds an item with the id of <paramref name="id"/> to an already existing synergy with the name key of <paramref name="nameKey"/>.
        /// </summary>
        /// <param name="nameKey">The name key of the synergy.</param>
        /// <param name="id">The id of the item that will be added to the synergy.</param>
        public static void AddItemToSynergy(string nameKey, int id)
        {
            foreach (AdvancedSynergyEntry entry in GameManager.Instance.SynergyManager.synergies)
            {
                if (entry.NameKey == nameKey)
                {
                    if (PickupObjectDatabase.GetById(id) != null)
                    {
                        PickupObject obj = PickupObjectDatabase.GetById(id);
                        if (obj is Gun)
                        {
                            if (entry.OptionalGunIDs != null)
                            {
                                entry.OptionalGunIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalGunIDs = new List<int> { id };
                            }
                        }
                        else
                        {
                            if (entry.OptionalItemIDs != null)
                            {
                                entry.OptionalItemIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalItemIDs = new List<int> { id };
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="self"/>'s owner has a synergy with name of <paramref name="synergyToCheck"/>.
        /// </summary>
        /// <param name="self">The gun whose owner will be checked.</param>
        /// <param name="synergyToCheck">Synergy to check.</param>
        /// <returns><see langword="true"/> if <paramref name="self"/>'s owner has a synergy with name of <paramref name="synergyToCheck"/>.</returns>
        public static bool OwnerHasSynergy(this Gun self, string synergyToCheck)
        {
            return self.CurrentOwner && self.CurrentOwner is PlayerController && (self.CurrentOwner as PlayerController).PlayerHasActiveSynergy(synergyToCheck);
        }

        public static void SetupDualWieldSynergy(string synergyName, Gun first, Gun second)
        {
            CreateSynergy(synergyName, new List<int>() { first.PickupObjectId, second.PickupObjectId });
            AddDualWieldSynergyProcessor(first, second, synergyName);
        }

        /// <summary>
        /// Adds a <see cref="AdvancedDualWieldSynergyProcessor"/> to <paramref name="first"/> and <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first gun in the dual wield synergy.</param>
        /// <param name="second">The second gun in the dual wield synergy.</param>
        /// <param name="requiredSynergy">The synergy required for the dual wield.</param>
        public static void AddDualWieldSynergyProcessor(Gun first, Gun second, string requiredSynergy)
        {
            AdvancedDualWieldSynergyProcessor p1 = first.gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            p1.SynergyNameToCheck = requiredSynergy;
            p1.PartnerGunID = second.PickupObjectId;
            AdvancedDualWieldSynergyProcessor p2 = second.gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            p2.SynergyNameToCheck = requiredSynergy;
            p2.PartnerGunID = first.PickupObjectId;
        }

        /// <summary>
        /// Adds a <see cref="AdvancedGunFormeSynergyProcessor"/> to <paramref name="gun"/>.
        /// </summary>
        /// <param name="gun">The gun to add the <see cref="AdvancedGunFormeSynergyProcessor"/> to.</param>
        /// <returns>The added <see cref="AdvancedGunFormeSynergyProcessor"/>.</returns>
        public static AdvancedGunFormeSynergyProcessor AddGunFormeSynergyProcessor(this Gun gun)
        {
            AdvancedGunFormeSynergyProcessor p = gun.gameObject.AddComponent<AdvancedGunFormeSynergyProcessor>();
            p.Formes = new List<AdvancedGunFormeData>();
            return p;
        }

        /// <summary>
        /// Adds a new <see cref="AdvancedGunFormeData"/> to <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The <see cref="AdvancedGunFormeSynergyProcessor"/> to add the new <see cref="AdvancedGunFormeData"/> to.</param>
        /// <param name="requiresSynergy">If <see langword="true"/>, the gun will only be able to transform into this forme if the owner has the synergy with the name of <paramref name="requiredSynergy"/>.</param>
        /// <param name="requiredSynergy">If <paramref name="requiresSynergy"/> is <see langword="true"/>, the name of the synergy that will be required for the gun to transform into this forme.</param>
        /// <param name="formeGunId">The id of the gun that the forme will transform the original gun into.</param>
        /// <param name="addedForme">The added <see cref="AdvancedGunFormeData"/>.</param>
        /// <returns><paramref name="processor"/></returns>
        public static AdvancedGunFormeSynergyProcessor AddForme(this AdvancedGunFormeSynergyProcessor processor, bool requiresSynergy, string requiredSynergy, int formeGunId, out AdvancedGunFormeData addedForme)
        {
            AdvancedGunFormeData forme = ScriptableObject.CreateInstance<AdvancedGunFormeData>();
            forme.RequiresSynergy = requiresSynergy;
            forme.RequiredSynergyString = requiredSynergy;
            forme.FormeID = formeGunId;
            processor.Formes.Add(forme);
            addedForme = forme;
            return processor;
        }

        /// <summary>
        /// Adds a new <see cref="AdvancedGunFormeData"/> to <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The <see cref="AdvancedGunFormeSynergyProcessor"/> to add the new <see cref="AdvancedGunFormeData"/> to.</param>
        /// <param name="requiresSynergy">If <see langword="true"/>, the gun will only be able to transform into this forme if the owner has the synergy with the name of <paramref name="requiredSynergy"/>.</param>
        /// <param name="requiredSynergy">If <paramref name="requiresSynergy"/> is <see langword="true"/>, the name of the synergy that will be required for the gun to transform into this forme.</param>
        /// <param name="formeGunId">The id of the gun that the forme will transform the original gun into.</param>
        /// <returns><paramref name="processor"/></returns>
        public static AdvancedGunFormeSynergyProcessor AddForme(this AdvancedGunFormeSynergyProcessor processor, bool requiresSynergy, string requiredSynergy, int formeGunId)
        {
            return processor.AddForme(requiresSynergy, requiredSynergy, formeGunId, out _);
        }

        /// <summary>
        /// Adds a <see cref="AdvancedHoveringGunSynergyProcessor"/> to <paramref name="gun"/>.
        /// </summary>
        /// <param name="gun">The gun to add the <see cref="AdvancedHoveringGunSynergyProcessor"/> to.</param>
        /// <param name="requiredSynergy">The synergy required for the hovering gun to be created.</param>
        /// <param name="targetGunId">The id of the hovering gun.</param>
        /// <param name="usesMultipleGuns">If <see langword="true"/>, instead of creating one gun with id of <paramref name="targetGunId"/> it will instead create multiple guns from all ids in <paramref name="targetGunIDs"/></param>
        /// <param name="targetGunIDs">If <paramref name="usesMultipleGuns"/> is <see langword="true"/>, ids of the guns it will create. This array's length must be the same as <paramref name="numToTrigger"/>.</param>
        /// <param name="positionType">How the gun will move when created.</param>
        /// <param name="aimType">Where the gun will aim when created.</param>
        /// <param name="fireType">When the gun will fire when created.</param>
        /// <param name="fireCooldown">Firing cooldown of the gun.</param>
        /// <param name="fireDuration">Firing duration of the gun. If zero or less, the gun will just shoot a single time every time the fire condition is met. If greater than zero, after the fire condition is met the gun will shoot for this duration.</param>
        /// <param name="onlyOnEmptyReload">If <paramref name="fireType"/> is <see cref="HoveringGunController.FireType.ON_RELOAD"/>, the gun will only fire on empty reload.</param>
        /// <param name="shootAudioEvent">Sound that the gun makes when shooting. If <paramref name="fireDuration"/> is greater than zero, the sound will only play at the start.</param>
        /// <param name="onEveryShotAudioEvent">Sound that the gun makes when shooting. If <paramref name="fireDuration"/> is greater than zero, unlike <paramref name="shootAudioEvent"/>, the sound will play every shot.</param>
        /// <param name="onFinishedShootingAudioEvent">If <paramref name="fireDuration"/> is greater than zero, this sound will play when the firing duration finishes.</param>
        /// <param name="trigger">Condition that creates the gun.</param>
        /// <param name="numToTrigger">Number of guns that will be created.</param>
        /// <param name="triggerDuration">If <paramref name="trigger"/> is <see cref="HoveringGunSynergyProcessor.TriggerStyle.ON_DAMAGE"/>, the gun will only stay for this long (in seconds).</param>
        /// <param name="consumesTargetGunAmmo">If <see langword="true"/> and the owner has the hovering gun as a normal gun, the hovering gun will have a chance to consume that gun's ammo equal to <paramref name="chanceToConsumeTargetGunAmmo"/>.</param>
        /// <param name="chanceToConsumeTargetGunAmmo">If <paramref name="chanceToConsumeTargetGunAmmo"/> is <see langword="true"/>, the chance that the hovering gun will consume the "original" gun's ammo. Ranges from 0 to 1 (0 = 0%, 1 = 100%)</param>
        /// <returns>The added <see cref="AdvancedHoveringGunSynergyProcessor"/>.</returns>
        public static AdvancedHoveringGunSynergyProcessor AddHoveringGunSynergyProcessor(this Gun gun, string requiredSynergy, int targetGunId, bool usesMultipleGuns, List<int> targetGunIDs, HoveringGunController.HoverPosition positionType,
            HoveringGunController.AimType aimType, HoveringGunController.FireType fireType, float fireCooldown, float fireDuration, bool onlyOnEmptyReload, string shootAudioEvent, string onEveryShotAudioEvent, string onFinishedShootingAudioEvent,
            AdvancedHoveringGunSynergyProcessor.TriggerStyle trigger, int numToTrigger, float triggerDuration, bool consumesTargetGunAmmo, float chanceToConsumeTargetGunAmmo)
        {
            AdvancedHoveringGunSynergyProcessor p = gun.gameObject.AddComponent<AdvancedHoveringGunSynergyProcessor>();
            p.RequiredSynergy = requiredSynergy;
            p.IDsToSpawn = targetGunIDs?.ToArray() ?? new int[] { targetGunId };
            p.PositionType = positionType;
            p.AimType = aimType;
            p.FireType = fireType;
            p.FireCooldown = fireCooldown;
            p.BeamFireDuration = fireDuration;
            p.OnlyOnEmptyReload = onlyOnEmptyReload;
            p.ShootAudioEvent = shootAudioEvent;
            p.OnEveryShotAudioEvent = onEveryShotAudioEvent;
            p.FinishedShootingAudioEvent = onFinishedShootingAudioEvent;
            p.Trigger = trigger;
            p.TriggerDuration = triggerDuration;
            p.ChanceToConsumeTargetGunAmmo = consumesTargetGunAmmo ? chanceToConsumeTargetGunAmmo : 0f;
            return p;
        }

        /// <summary>
        /// Creates a new synergy.
        /// </summary>
        /// <param name="name">The name of the synergy.</param>
        /// <param name="mandatoryIds">Ids of items that are always required for the completion of the synergy.</param>
        /// <param name="optionalIds">Ids of "filler items" that will be needed to fill empty spaces in list of synergy-completing items.</param>
        /// <param name="activeWhenGunsUnequipped">If true, the synergy will still be active when the player is not holding the guns required for it's completion.</param>
        /// <param name="statModifiers">Stat modifiers that will be applied to the player when the synergy is active.</param>
        /// <param name="ignoreLichsEyeBullets">If true, Lich's Eye Bullets will not be able to activate the synergy.</param>
        /// <param name="numberObjectsRequired">Number of items required for the synergy's completion.</param>
        /// <param name="suppressVfx">If true, the synergy arrow VFX will not appear when the synergy is completed.</param>
        /// <param name="requiresAtLeastOneGunAndOneItem">If true, the player will have to have at least one item AND gun from either/both <paramref name="mandatoryIds"/> and <paramref name="optionalIds"/>.</param>
        /// <param name="bonusSynergies">List of "bonus synergies" for the synergy. Bonus synergies are used by base game items to detect if a synergy is active, but for modded synergies you don't need them.</param>
        /// <returns>The built synergy</returns>
        public static AdvancedSynergyEntry CreateSynergy(string name, List<int> mandatoryIds, List<int> optionalIds = default, bool activeWhenGunsUnequipped = true, List<StatModifier> statModifiers = default, bool ignoreLichsEyeBullets = false,
            int numberObjectsRequired = 2, bool suppressVfx = false, bool requiresAtLeastOneGunAndOneItem = false, List<CustomSynergyType> bonusSynergies = default)
        {
            if (synergies == null)
            {
                InitSynergyBuilder();
            }
            AdvancedSynergyEntry entry = new AdvancedSynergyEntry();
            string key = "#" + name.ToUpper().Replace(" ", "_").Replace("'", "").Replace(",", "").Replace(".", "");
            entry.NameKey = key;
            ETGMod.Databases.Strings.Synergy.Set(key, name);
            if (mandatoryIds != null)
            {
                foreach (int id in mandatoryIds)
                {
                    PickupObject po = PickupObjectDatabase.GetById(id);
                    if (po is Gun)
                    {
                        entry.MandatoryGunIDs.Add(id);
                    }
                    else if (po is PassiveItem || po is PlayerItem)
                    {
                        entry.MandatoryItemIDs.Add(id);
                    }
                }
            }
            if (optionalIds != null)
            {
                foreach (int id in optionalIds)
                {
                    PickupObject po = PickupObjectDatabase.GetById(id);
                    if (po is Gun)
                    {
                        entry.OptionalGunIDs.Add(id);
                    }
                    else if (po is PassiveItem || po is PlayerItem)
                    {
                        entry.OptionalItemIDs.Add(id);
                    }
                }
            }
            entry.ActiveWhenGunUnequipped = activeWhenGunsUnequipped;
            entry.statModifiers = new List<StatModifier>();
            if (statModifiers != null)
            {
                foreach (StatModifier mod in statModifiers)
                {
                    if (mod != null)
                    {
                        entry.statModifiers.Add(mod);
                    }
                }
            }
            entry.IgnoreLichEyeBullets = ignoreLichsEyeBullets;
            entry.NumberObjectsRequired = numberObjectsRequired;
            entry.SuppressVFX = suppressVfx;
            entry.RequiresAtLeastOneGunAndOneItem = requiresAtLeastOneGunAndOneItem;
            entry.bonusSynergies = new List<CustomSynergyType>();
            if (bonusSynergies != null)
            {
                foreach (CustomSynergyType type in bonusSynergies)
                {
                    entry.bonusSynergies.Add(type);
                }
            }
            synergies.Add(entry);
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { entry }).ToArray();
            return entry;
        }

        /// <summary>
        /// Changes <paramref name="original"/> to a name that can be used for checking synergies with <see cref="PlayerHasActiveSynergy(PlayerController, string)"/>. This is done automatically at the start of <see cref="PlayerHasActiveSynergy(PlayerController, string)"/>, so you don't need to do it manually
        /// </summary>
        /// <param name="original">The original synergy name.</param>
        /// <returns>The changed <paramref name="original"/> that can be used for checking synergies with <see cref="PlayerHasActiveSynergy(PlayerController, string)"/>.</returns>
        public static string FixSynergyAPISynergyName(string original)
        {
            string key = "#" + original.ToUpper().Replace(" ", "_").Replace("'", "").Replace(",", "").Replace(".", "");
            foreach(var s in synergies)
            {
                if(s.NameKey == key)
                {
                    return key;
                }
            }
            return original;
        }

        public static bool SynergyIsMatchingAndActive(this AdvancedSynergyEntry synergy, string nameToCheck)
        {
            if (synergy.NameKey == nameToCheck)
            {
                if (synergy.SynergyIsActive(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if <paramref name="player"/> has an active synergy with a name of <paramref name="synergyNameToCheck"/>. It's recommended to use this only with synergies added by this SynergyAPI, for base game synergies use <see cref="PlayerController.HasActiveBonusSynergy(CustomSynergyType, bool)"/>
        /// </summary>
        /// <param name="player">The player that will get checked.</param>
        /// <param name="synergyNameToCheck">The name of the synergy that will be checked.</param>
        /// <returns><see langword="true"/> if any matching active synergies were found, <see langword="false"/> otherwise.</returns>
        public static bool PlayerHasActiveSynergy(this PlayerController player, string synergyNameToCheck)
        {
            string actualSynergyName = FixSynergyAPISynergyName(synergyNameToCheck);
            foreach (int index in player.ActiveExtraSynergies)
            {
                AdvancedSynergyEntry synergy = GameManager.Instance.SynergyManager.synergies[index];
                if (synergy.SynergyIsMatchingAndActive(actualSynergyName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if anyone has an active synergy with the name of <paramref name="synergy"/>. It's recommended to use this only with synergies added by this SynergyAPI, for base game synergies use <see cref="PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType, out int)"/>
        /// </summary>
        /// <param name="synergy">The name of the synergy that will be checked.</param>
        /// <param name="count">The final count of matching synergies found.</param>
        /// <returns><see langword="true"/> if any matching active synergies were found, <see langword="false"/> otherwise.</returns>
        public static bool AnyoneHasActiveSynergy(string synergy, out int count)
        {
            string actualSynergy = FixSynergyAPISynergyName(synergy);
            count = 0;
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                if (!GameManager.Instance.AllPlayers[i].IsGhost)
                {
                    count += GameManager.Instance.AllPlayers[i].CountActiveBonusSynergies(actualSynergy);
                }
            }
            return count > 0;
        }

        /// <summary>
        /// Checks if anyone has an active synergy with the name of <paramref name="synergy"/>. It's recommended to use this only with synergies added by this SynergyAPI, for base game synergies use <see cref="PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType, out int)"/>
        /// </summary>
        /// <param name="synergy">The name of the synergy that will be checked.</param>
        /// <returns><see langword="true"/> if any matching active synergies were found, <see langword="false"/> otherwise.</returns>
        public static bool AnyoneHasActiveSynergy(string synergy)
        {
            return AnyoneHasActiveSynergy(synergy, out _);
        }

        /// <summary>
        /// Counts the active synergies of all players with the name of <paramref name="synergy"/>. It's recommended to use this only with synergies added by this SynergyAPI.
        /// </summary>
        /// <param name="synergy">The name of the synergies that will be counted.</param>
        /// <returns>The count of <paramref name="player"/>'s synergies that are named <paramref name="synergy"/>.</returns>
        public static int CountAllActiveMatchingSynergies(string synergy)
        {
            AnyoneHasActiveSynergy(synergy, out var num);
            return num;
        }

        /// <summary>
        /// Counts <paramref name="player"/>'s active synergies with the name of <paramref name="synergy"/>. It's recommended to use this only with synergies added by this SynergyAPI, for base game synergies use <see cref="PlayerController.CountActiveBonusSynergies(CustomSynergyType)"/>
        /// </summary>
        /// <param name="player">The player whose matching synergies will be counted.</param>
        /// <param name="synergy">The name of the synergies that will be counted.</param>
        /// <returns>The count of <paramref name="player"/>'s synergies that are named <paramref name="synergy"/>.</returns>
        public static int CountActiveBonusSynergies(this PlayerController player, string synergy)
        {
            if (player == null)
            {
                return 0;
            }
            string actualSynergy = FixSynergyAPISynergyName(synergy);
            int num = 0;
            foreach (int index in player.ActiveExtraSynergies)
            {
                AdvancedSynergyEntry entry = GameManager.Instance.SynergyManager.synergies[index];
                if (entry.SynergyIsMatchingAndActive(actualSynergy))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// Synergies that were added by SynergyAPI;
        /// </summary>
        public static List<AdvancedSynergyEntry> synergies;
        /// <summary>
        /// <see cref="FieldInfo"/> for the synergy table.
        /// </summary>
        public static FieldInfo m_synergyTable;
    }
}
