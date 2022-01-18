using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using System.Collections;
using SpecialStuffPack.SaveAPI;
using SpecialStuffPack.Components;

namespace SpecialStuffPack.Items
{
    public class PastsRewardItem : PassiveItem, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Past's Reward";
            string shortdesc = "Worth the Shot";
            string longdesc = "Has a different effect for different gungeoneers.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsreward", "sprites/past_reward_idle_001", name, shortdesc, longdesc, ItemQuality.S, SpecialStuffModule.globalPrefix, null, null);
            int convictFormId = InitConvictForm();
            int pilotFormId = InitPilotForm();
            int hunterFormId = InitHunterForm();
            int marineFormId = InitMarineForm();
            int robotFormId = InitRobotForm();
            int bulletFormId = InitBulletForm();
            int paradoxFormId = InitParadoxForm();
            int gunslingerFormId = InitGunslingerForm();
            int lockFormId = InitLockForm();
            int modularFormId = InitModularForm();
            int bulwarkFormId = InitBulwarkForm();
            int shadeFormId = InitShadeForm();
            int mistakeFormId = InitMistakeForm();
            item.baseCharacterToItemIds = new Dictionary<PlayableCharacters, int> { { PlayableCharacters.Convict, convictFormId }, { PlayableCharacters.Pilot, pilotFormId }, { PlayableCharacters.Guide, hunterFormId }, { PlayableCharacters.Soldier, 
                    marineFormId }, { PlayableCharacters.Robot, robotFormId }, { PlayableCharacters.Bullet, bulletFormId } }.ToList();
            item.overrideBasegameCharacterToItemIds = new Dictionary<PlayableCharacters, int> { { PlayableCharacters.Eevee, paradoxFormId }, { PlayableCharacters.Gunslinger, gunslingerFormId } }.ToList();
            item.overrideModdedCharacterToItemIds = new Dictionary<string, int> { { "lock", lockFormId }, { "modular", modularFormId }, { "bulwark", bulwarkFormId }, { "shade", shadeFormId }, { "mistake", mistakeFormId } }.ToList();
            item.SetupUnlockOnFlag(GungeonFlags.ACHIEVEMENT_COMPLETE_FOUR_PASTS, true);
        }

        public static int InitConvictForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Rage Up!";
            string longdesc = "Increases the duration and power of the owner's rage.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardconvict", "sprites/past_reward_convict_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null, 
                SpecialStuffModule.globalPrefix + ":pasts_reward_convict");
            item.currentForm = PastsRewardForm.CONVICT;
            return item.PickupObjectId;
        }

        public static int InitPilotForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Support From Space";
            string longdesc = "Summons a spaceship to orbit the owner.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardpilot", "sprites/past_reward_pilot_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_pilot");
            item.currentForm = PastsRewardForm.PILOT;
            return item.PickupObjectId;
        }

        public static int InitMarineForm()
        {
            string name = "Past's Reward";
            string shortdesc = "";
            string longdesc = "Summons demons from hell in each room and makes them fight for the owner.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardmarine", "sprites/past_reward_marine_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_marine");
            item.currentForm = PastsRewardForm.MARINE;
            return item.PickupObjectId;
        }

        public static int InitHunterForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Better Dog";
            string longdesc = "Significanly improves items found by the Dog.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardhunter", "sprites/past_reward_hunter_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_hunter");
            item.currentForm = PastsRewardForm.HUNTER;
            return item.PickupObjectId;
        }

        public static int InitBulletForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Shinier Sword";
            string longdesc = "Improves the functionality of Blasphemy.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardbullet", "sprites/past_reward_bullet_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_bullet");
            item.currentForm = PastsRewardForm.BULLET;
            return item.PickupObjectId;
        }

        public static int InitRobotForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Quicker Repair";
            string longdesc = "Turns useless hearts into armor, doubles all normal armor drops and grants a damage up for every health up item the owner has.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardrobot", "sprites/past_reward_robot_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_robot");
            item.currentForm = PastsRewardForm.BULLET;
            return item.PickupObjectId;
        }

        public static int InitParadoxForm()
        {
            string name = "Past's Reward";
            string shortdesc = "?????????";
            string longdesc = "??????????????????????????????";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardparadox", "sprites/past_reward_paradox_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_paradox");
            item.currentForm = PastsRewardForm.PARADOX;
            return item.PickupObjectId;
        }

        public static int InitGunslingerForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Synergies = Damage";
            string longdesc = "Improves the owner's damage for every synergy.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardgunslinger", "sprites/past_reward_gunslinger_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_gunslinger");
            item.currentForm = PastsRewardForm.GUNSLINGER;
            return item.PickupObjectId;
        }

        public static int InitLockForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Indestructible";
            string longdesc = "Improves the owner's longevity and fortune.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardlock", "sprites/past_reward_lock_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_lock");
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.currentForm = PastsRewardForm.LOCK;
            return item.PickupObjectId;
        }

        public static int InitModularForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Modultimate";
            string longdesc = "Significantly improves the owner's firepower.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardmodular", "sprites/past_reward_modular_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_modular");
            item.currentForm = PastsRewardForm.MODULAR;
            return item.PickupObjectId;
        }

        public static int InitBulwarkForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Kaliber Blesses You!";
            string longdesc = "Some bullets may not hurt the owner. Increases the invulnerability time after taking damage.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardbulwark", "sprites/past_reward_bulwark_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_bulwark");
            item.currentForm = PastsRewardForm.BULWARK;
            return item.PickupObjectId;
        }

        public static int InitShadeForm()
        {
            string name = "Past's Reward";
            string shortdesc = "Die and Come Back Stronger";
            string longdesc = "Grants the owner an extra life and increases the owner's strength after dying.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardshade", "sprites/past_reward_shade_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_shade");
            item.currentForm = PastsRewardForm.SHADE;
            return item.PickupObjectId;
        }

        public static int InitMistakeForm()
        {
            string name = "Past's Reward";
            string shortdesc = "kill me kill me kill me";
            string longdesc = "Slowly kills the owner.";
            PastsRewardItem item = ItemBuilder.EasyInit<PastsRewardItem>("items/pastsrewardmistake", "sprites/past_reward_mistake_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, SpecialStuffModule.globalPrefix, null,
                SpecialStuffModule.globalPrefix + ":pasts_reward_mistake");
            item.currentForm = PastsRewardForm.SHADE;
            item.CanBeDropped = false;
            return item.PickupObjectId;
        }

        public bool IsCharacterSpecific => currentForm != PastsRewardForm.NONE;

        public void Start()
        {
            m_baseCharacterToItemIds = baseCharacterToItemIds.ToDictionary();
            m_overrideBasegameCharacterToItemIds = overrideBasegameCharacterToItemIds.ToDictionary();
            m_overrideModdedCharacterToItemIds = overrideModdedCharacterToItemIds.ToDictionary();
        }

        public new void OnEnteredRange(PlayerController interactor)
        {
            if (!IsCharacterSpecific && !m_isTransforming)
            {
                if (!this)
                {
                    return;
                }
                if (!RoomHandler.unassignedInteractableObjects.Contains(this))
                {
                    return;
                }
                m_isTransforming = true;
                sprite.UpdateZDepth();
                SquishyBounceWiggler component = GetComponent<SquishyBounceWiggler>();
                if (component != null)
                {
                    component.WiggleHold = true;
                }
                m_pickedUp = true;
                int? itemId = GetItemToTransformTo(interactor);
                StartCoroutine(HandleTransformation(itemId));
            }
        }

        public IEnumerator HandleTransformation(int? id)
        {
            if(sprite != null)
            {
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material = new Material(sprite.renderer.material) { shader = ShaderCache.Acquire("Brave/PlayerShaderEevee") };
            }
            float ela = 0f;
            while(ela < 1f)
            {
                ela += BraveTime.DeltaTime;
                if(sprite != null)
                {
                    sprite.scale = Vector2.Lerp(Vector2.one, Vector2.zero, ela);
                }
                else
                {
                    transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, ela);
                }
                yield return null;
            }
            if (id.HasValue)
            {
                PickupObject obj = PickupObjectDatabase.GetById(id.Value);
                if (obj != null && obj.gameObject != null)
                {
                    if(obj.sprite != null)
                    {
                        sprite.SetSprite(obj.sprite.Collection, obj.sprite.spriteId);
                        ela = 0f;
                        while (ela < 1f)
                        {
                            ela += BraveTime.DeltaTime;
                            sprite.scale = Vector2.Lerp(Vector2.one, Vector2.zero, ela);
                            yield return null;
                        }
                    }
                    if (encounterTrackable != null)
                    {
                        GameStatsManager.Instance?.HandleEncounteredObject(encounterTrackable);
                    }
                    Destroy(gameObject);
                    LootEngine.SpawnItem(obj.gameObject, transform.position, Vector2.down, 0f, false, false, false);
                    yield break;
                }
            }
            if (encounterTrackable != null)
            {
                GameStatsManager.Instance?.HandleEncounteredObject(encounterTrackable);
            }
            Destroy(gameObject);
            if (GameManager.HasInstance && GameManager.Instance.RewardManager != null && GameManager.Instance.RewardManager.S_Chest != null)
            {
                Vector2 pos = Vector2.zero;
                if (transform != null)
                {
                    pos = transform.position;
                }
                Chest c = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, pos, pos.GetAbsoluteRoom(), true);
                c.ForceUnlock();
                c.BecomeGlitchChest();
            }
            yield break;
        }

        public override void Pickup(PlayerController player)
        {
            if (!IsCharacterSpecific)
            {
                Destroy(gameObject);
                if (encounterTrackable != null)
                {
                    GameStatsManager.Instance?.HandleEncounteredObject(encounterTrackable);
                }
                int? itemId = GetItemToTransformTo(player);
                bool successful = false;
                if (itemId.HasValue)
                {
                    PickupObject obj = PickupObjectDatabase.GetById(itemId.Value);
                    if (obj != null && obj.gameObject != null)
                    {
                        LootEngine.GivePrefabToPlayer(obj.gameObject, player);
                        successful = true;
                    }
                }
                if (!successful)
                {
                    Vector2 pos = Vector2.zero;
                    if (transform != null)
                    {
                        pos = transform.position;
                    }
                    if (GameManager.HasInstance && GameManager.Instance.RewardManager != null && GameManager.Instance.RewardManager.S_Chest != null)
                    {
                        Chest c = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, pos, pos.GetAbsoluteRoom(), true);
                        c.ForceUnlock();
                        c.BecomeGlitchChest();
                    }
                }
            }
            else
            {
                if(!m_pickedUpThisRun && currentForm == PastsRewardForm.PARADOX)
                {
                    if (GameManager.HasInstance && GameManager.Instance.RewardManager != null && GameManager.Instance.RewardManager.S_Chest != null)
                    {
                        Chest c = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, player.CenterPosition + new Vector2(-1f, -2.5f), player.CurrentRoom, true);
                        c.ForceUnlock();
                        Chest c2 = Chest.Spawn(GameManager.Instance.RewardManager.B_Chest, player.CenterPosition + new Vector2(-3.5f, 0f), player.CurrentRoom, true);
                        c2.ForceUnlock();
                        c2.BecomeGlitchChest();
                        Chest c3 = Chest.Spawn(GameManager.Instance.RewardManager.Synergy_Chest, player.CenterPosition + new Vector2(1.5f, 0f), player.CurrentRoom, true);
                        c3.ForceUnlock();
                        c3.PreventFuse = true;
                        c.AddComponent<ChestChoiceComponent>().OtherChoiceChests = new List<Chest> { c2, c3 };
                        c2.AddComponent<ChestChoiceComponent>().OtherChoiceChests = new List<Chest> { c, c3 };
                        c3.AddComponent<ChestChoiceComponent>().OtherChoiceChests = new List<Chest> { c, c2 };
                    }
                }
                base.Pickup(player);
                if (player.healthHaver != null)
                {
                    player.healthHaver.OnPreDeath += ReviveShade;
                    if (currentForm == PastsRewardForm.MISTAKE)
                    {
                        player.StartCoroutine(KillPlayer(player.healthHaver));
                    }
                }
                player.PostProcessProjectile += GunslingerEffect;
            }
        }

        public static IEnumerator KillPlayer(HealthHaver target)
        {
            while(target != null && target.IsAlive)
            {
                target.ApplyDamage(0.5f, Vector2.zero, "", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                yield return null;
            }
            yield break;
        }

        public void ReviveShade(Vector2 finalDamageDirection)
        {
            if(Owner != null && currentForm == PastsRewardForm.SHADE)
            {
                if (!m_didReviveShade)
                {
                    m_didReviveShade = true;
                    if (Owner.ForceZeroHealthState)
                    {
                        Owner.healthHaver.Armor++;
                    }
                    else
                    {
                        Owner.healthHaver.ApplyHealing(0.5f);
                    }
                }
                List<StatModifier> deathStats = new List<StatModifier>
                {
                    StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.35f),
                    StatModifier.Create(PlayerStats.StatType.ReloadSpeed, StatModifier.ModifyMethod.ADDITIVE, -0.05f),
                    StatModifier.Create(PlayerStats.StatType.RateOfFire, StatModifier.ModifyMethod.ADDITIVE, 0.1f),
                    StatModifier.Create(PlayerStats.StatType.ShadowBulletChance, StatModifier.ModifyMethod.ADDITIVE, 5f),
                    StatModifier.Create(PlayerStats.StatType.EnemyProjectileSpeedMultiplier, StatModifier.ModifyMethod.ADDITIVE, -0.05f)
                };
                Owner.ownerlessStatModifiers.AddRange(deathStats);
                Owner.stats.RecalculateStats(Owner, false, false);
            }
        }

        public void GunslingerEffect(Projectile proj, float f)
        {
            if(Owner != null && currentForm == PastsRewardForm.GUNSLINGER && Owner.ActiveExtraSynergies != null)
            {
                proj.baseData.damage *= 1f + Owner.ActiveExtraSynergies.Count * 0.02f;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (IsCharacterSpecific)
            {
                if (player.healthHaver != null)
                {
                    player.healthHaver.OnPreDeath -= ReviveShade;
                }
                player.PostProcessProjectile -= GunslingerEffect;
            }
            return base.Drop(player);
        }

        public int? GetItemToTransformTo(PlayerController player)
        {
            if(player == null)
            {
                return null;
            }
            if(m_overrideModdedCharacterToItemIds != null && m_overrideModdedCharacterToItemIds.ContainsKey(player.name.Replace("(Clone)", "")))
            {
                return m_overrideModdedCharacterToItemIds[player.name.ToLower().Replace("(clone)", "")];
            }
            if (m_overrideBasegameCharacterToItemIds != null && m_overrideBasegameCharacterToItemIds.ContainsKey(player.characterIdentity))
            {
                return m_overrideBasegameCharacterToItemIds[player.characterIdentity];
            }
            if(GameStatsManager.Instance != null && !GameStatsManager.Instance.GetCharacterSpecificFlag(player.characterIdentity, CharacterSpecificGungeonFlags.KILLED_PAST))
            {
                return null;
            }
            if (m_baseCharacterToItemIds != null && m_baseCharacterToItemIds.ContainsKey(player.characterIdentity))
            {
                return m_baseCharacterToItemIds[player.characterIdentity];
            }
            return null;
        }

        private bool m_isTransforming;
        private Dictionary<PlayableCharacters, int> m_baseCharacterToItemIds;
        private Dictionary<PlayableCharacters, int> m_overrideBasegameCharacterToItemIds;
        private Dictionary<string, int> m_overrideModdedCharacterToItemIds;
        [SerializeField]
        private bool m_didReviveShade;
        public List<KeyValuePair<PlayableCharacters, int>> baseCharacterToItemIds;
        public List<KeyValuePair<PlayableCharacters, int>> overrideBasegameCharacterToItemIds;
        public List<KeyValuePair<string, int>> overrideModdedCharacterToItemIds;
        public PastsRewardForm currentForm;
        public enum PastsRewardForm
        {
            NONE,
            CONVICT,
            PILOT,
            HUNTER,
            MARINE,
            BULLET,
            ROBOT,
            PARADOX,
            GUNSLINGER,
            LOCK,
            MISTAKE,
            MODULAR,
            BULWARK,
            SHADE,
        }
    }
}
