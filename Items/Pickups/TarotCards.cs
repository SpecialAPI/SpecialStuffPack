using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Pickups
{
    [HarmonyPatch]
    public class TarotCards : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            CreateTarotCard(TarotCardType.TheHeartsI,           "The Hearts I",             "Endurance",                    "Grants half a heart container.");
            CreateTarotCard(TarotCardType.TheHeartsII,          "The Hearts II",            "Vitality",                     "Grants a heart container.");
            CreateTarotCard(TarotCardType.TheHeartsIII,         "The Hearts III",           "Longevity",                    "Grants two heart containers.");
            CreateTarotCard(TarotCardType.WeepingMoon,          "Weeping Moon",             "+1.4 to bullet... sometimes",  "Grants +35% damage on even floors.");
            CreateTarotCard(TarotCardType.NaturesBoon,          "Nature's Boon",            "Leafy Death",                  "Enemies create leaves on death.");
            CreateTarotCard(TarotCardType.TheLoversI,           "The Lovers I",             "Defense",                      "Grants a piece of armor.");
            CreateTarotCard(TarotCardType.TheLoversII,          "The Lovers II",            "Protection",                   "Grants two pieces of armor.");
            CreateTarotCard(TarotCardType.AllSeeingSun,         "All Seeing Sun",           "+1 to bullet... sometimes",    "Grants +25% damage on odd floors.");
            CreateTarotCard(TarotCardType.TrueSight,            "True Sight",               "Chance to crit",               "10% chance for bullets to deal double damage.");
            CreateTarotCard(TarotCardType.Telescope,            "Telescope",                "See beyond",                   "Reveals all rooms.");
            CreateTarotCard(TarotCardType.HandsOfRage,          "Hands of Rage",            "Furious bullets",              "Shooting creates a strong homing bullet every 10 seconds.");
            CreateTarotCard(TarotCardType.BurningDead,          "The Burning Dead",         "Enemies explode",              "Enemies explode on death.");
            CreateTarotCard(TarotCardType.DiseasedHeart,        "Diseased Heart",           "Hurtful armor",                "Grants a piece of armor. Losing a piece of armor hurts all enemies in the room.");
            CreateTarotCard(TarotCardType.TheArachnid,          "The Arachnid",             "Poison shot",                  "30% chance for bullets to inflict poison.");
            CreateTarotCard(TarotCardType.DivineStrength,       "Divine Strength",          "Rate of Fire Up",              "Shoot bullets 25% faster.");
            CreateTarotCard(TarotCardType.MasterOfTheArt,       "Master of the Art",        "+0.8 to bullet",               "Bullets deal 20% more damage.");
            CreateTarotCard(TarotCardType.ThePath,              "The Path",                 "Speed Up",                     "Increases speed by 2.");
            CreateTarotCard(TarotCardType.FervoursHarvest,      "Fervour's Harvest",        "Faster actives",               "Active items charge 35% faster from dealing damage.");
            CreateTarotCard(TarotCardType.SoulSnatcher,         "Soul Snatcher",            "Hurting heals",                "Heal half a heart every 1500 damage dealt.");
            CreateTarotCard(TarotCardType.ShieldOfFaith,        "Shield of Faith",          "Chance to block damage",       "10% chance to negate damage taken.");
            CreateTarotCard(TarotCardType.Bomb,                 "The Bomb",                 "Explosive rolls",              "Dodge rolling creates an explosion.");
            CreateTarotCard(TarotCardType.IchorEarned,          "Ichor Earned",             "Ichor on hit",                 "Getting hit creates poisonous ichor.");
            CreateTarotCard(TarotCardType.IchorLingered,        "Ichor Lingered",           "Ichor on roll",                "Rolling creates poisonous ichor.");
            CreateTarotCard(TarotCardType.PoisonImmunity,       "Mithridatism",             "Poison immunity",              "Can't get damaged by poison-related sources.");
            CreateTarotCard(TarotCardType.BlazingTrail,         "Blazing Trail",            "Rolling burns",                "Dodge roll through enemies, dealing quick damage to them.");
            CreateTarotCard(TarotCardType.DivineCurse,          "Divine Curse",             "Less active cooldown",         "Active items start 25% cooled down.");
            CreateTarotCard(TarotCardType.StrengthFromWithin,   "Strength from Within",     "Auto-cooling actives",         "Slowly cool down actives while in combat.");
            CreateTarotCard(TarotCardType.StrengthFromWithout,  "Strength from Without",    "Cooldown on hit",              "Getting hit cools down actives.");
            CreateTarotCard(TarotCardType.NeptunesCurse,        "Neptune's Curse",          "Fishy Death",                  "Enemies create fish on death.");
            CreateTarotCard(TarotCardType.FortunesBlessing,     "Fortune's Blessing",       "Doubled healing",              "Healing is doubled.");
            CreateTarotCard(TarotCardType.DeathsDoor,           "Death's Door",             "Risk brings reward",           "Dropping to the last half heart deals damage to all enemies in the room.");
            CreateTarotCard(TarotCardType.TheDeal,              "The Deal",                 "Extra life",                   "On death, revive with a full heart of health.");
            CreateTarotCard(TarotCardType.RabbitsFoot,          "Rabbit's Foot",            "Luckier",                      "Grants 1 coolness. Bullet effects are 25% more likely to trigger.");
            CreateTarotCard(TarotCardType.Ambrosia,             "Ambrosia",                 "Hurtful actives",              "Using an active item hurts all enemies in the room.");
            CreateTarotCard(TarotCardType.GiftFromBelow,        "Gift from Below",          "Hurting gives protection",     "Get a piece of armor for every 2000 damage dealt.");
            CreateTarotCard(TarotCardType.FervoursHost,         "Fervour's Host",           "Cooldown on combat",           "Entering a combat room cools down actives.");
            CreateTarotCard(TarotCardType.Intangible,           "The Intangible",           "Blessing of the Pit Lord",     "Grants pit immunity.");
            CreateTarotCard(TarotCardType.Retribution,          "Retribution",              "Make them pay",                "Getting hit creates multiple bombs.");
            CreateTarotCard(TarotCardType.WraithsWill,          "Wraith's Will",            "Hurtful touch",                "Grants contact damage immunity. Touching enemies while not rolling damages them.");
            CreateTarotCard(TarotCardType.GodlyMoment,          "Godly Moment",             "Invulnerability on room",      "Entering a room grants a brief moment of invulnerability.");
            CreateTarotCard(TarotCardType.KinOfTurua,           "Kin of Turua",             "Create vengeful tentacles",    "Getting hit creates a temporary tentacle to hurt enemies.");
            CreateTarotCard(TarotCardType.TheCollector,         "The Collector",            "More actives, capacity up",    "Grants extra item capacity. Active items are twice as likely to show up.");
            CreateTarotCard(TarotCardType.ConsecratedOil,       "Consecrated Oil",          "Faster actives",               "Active items charge 35% faster from dealing damage.");
            var plac = AssetBundleManager.Load<GameObject>("Tarot Card Room Placeable", null, null);
            plac.AddComponent<TarotCardRoomPlaceable>();
            tk2dSprite.AddComponent(plac, tarotCards[0].sprite.Collection, tarotCards[0].sprite.spriteId);
        }

        public static void CreateTarotCard(TarotCardType type, string name, string shortdesc, string longdesc)
        {
            var item = EasyItemInit<TarotCards>($"tarotcard{type}", "tarot_card_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, $"{SpecialStuffModule.globalPrefix}:tarot_card_{name.ToMTGId()}");
            item.encounterTrackable.journalData.SuppressInAmmonomicon = true;
            item.type = type;
            tarotCards.Add(item);
        }

        [HarmonyPatch(typeof(LootEngine), nameof(LootEngine.PostprocessItemSpawn))]
        [HarmonyPostfix]
        public static void RegisterInteractibleCard(DebrisObject item)
        {
            var card = item.GetComponent<TarotCards>();
            if (card != null && !RoomHandler.unassignedInteractableObjects.Contains(card))
            {
                RoomHandler.unassignedInteractableObjects.Add(card);
            }
        }

        public void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            seenTarotCards.Add(type);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!sprite)
            {
                return 1000f;
            }
            Bounds bounds = sprite.GetBounds();
            bounds.SetMinMax(bounds.min + transform.position, bounds.max + transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            sprite.UpdateZDepth();
            TextBoxManager.ShowTextBox(sprite.WorldTopCenter + Vector2.up / 4f, transform, -1f, $"{EncounterNameOrDisplayName}\n{encounterTrackable.journalData.GetAmmonomiconFullEntry(false, false)}", "", true, 
                TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            sprite.UpdateZDepth();
            TextBoxManager.ClearTextBox(transform);
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            TextBoxManager.ClearTextBoxImmediate(transform);
            Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public override void Pickup(PlayerController player)
        {
            if (m_pickedUp)
            {
                return;
            }
            player.Ext().tarotCards.Add(type);
            player.RecalculateStats();
            switch (type)
            {
                case TarotCardType.TheHeartsI:
                    player.healthHaver.ApplyHealing(0.5f);
                    break;
                case TarotCardType.TheHeartsII:
                    player.healthHaver.ApplyHealing(1f);
                    break;
                case TarotCardType.TheHeartsIII:
                    player.healthHaver.ApplyHealing(2f);
                    break;
                case TarotCardType.TheLoversI:
                    player.healthHaver.Armor++;
                    break;
                case TarotCardType.TheLoversII:
                    player.healthHaver.Armor += 2;
                    break;
                case TarotCardType.Telescope:
                    Minimap.Instance.RevealAllRooms(false);
                    break;
                case TarotCardType.PoisonImmunity:
                    player.healthHaver.damageTypeModifiers.Add(new() { damageMultiplier = 0f, damageType = CoreDamageTypes.Poison });
                    break;
                case TarotCardType.NaturesBoon:
                    player.lootModData.Add(new()
                    {
                        AssociatedPickupId = MahogunyId,
                        DropRateMultiplier = 3f
                    });
                    player.lootModData.Add(new()
                    {
                        AssociatedPickupId = BeeHiveId,
                        DropRateMultiplier = 3f
                    });
                    player.lootModData.Add(new()
                    {
                        AssociatedPickupId = DecoyId,
                        DropRateMultiplier = 3f
                    });
                    break;
                case TarotCardType.NeptunesCurse:
                    player.lootModData.Add(new()
                    {
                        AssociatedPickupId = BarrelId,
                        DropRateMultiplier = 3f
                    });
                    player.lootModData.Add(new()
                    {
                        AssociatedPickupId = TridentId,
                        DropRateMultiplier = 3f
                    });
                    break;
                case TarotCardType.Intangible:
                    player.ImmuneToPits.AddOverride("Intangible");
                    break;
                case TarotCardType.WraithsWill:
                    player.IncrementFlag<LiveAmmoItem>();
                    break;
            }
            GameObject original = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Pickup");
            GameObject gameObject = Instantiate(original);
            tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor(sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.UpdateZDepth();
            HandleEncounterable(player);
            m_pickedUp = true;
            AkSoundEngine.PostEvent("Play_OBJ_passive_get_01", player.gameObject);
            Destroy(this.gameObject);
        }

        public static TarotCards GetTarotCardForPlayer()
        {
            var list = tarotCards.FindAll(x => !seenTarotCards.Contains(x.type));
            if(list.Count > 0)
            {
                return list.RandomElement();
            }
            return tarotCards.RandomElement();
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ClearActiveGameData))]
        [HarmonyPostfix]
        public static void ClearSeen()
        {
            seenTarotCards.Clear();
        }

        public TarotCardType type;
        private bool m_pickedUp;
        public static List<TarotCards> tarotCards = new();
        public static List<TarotCardType> seenTarotCards = new();

        public enum TarotCardType
        {
            TheHeartsI,             //done
            TheHeartsII,            //done
            TheHeartsIII,           //done
            WeepingMoon,            //done
            NaturesBoon,            //done
            TheLoversI,             //done
            TheLoversII,            //done
            AllSeeingSun,           //done
            TrueSight,              //done
            Telescope,              //done
            HandsOfRage,            //done
            BurningDead,            //done
            DiseasedHeart,          //done
            TheArachnid,            //done
            DivineStrength,         //done
            MasterOfTheArt,         //done
            ThePath,                //done
            FervoursHarvest,        //done
            SoulSnatcher,           //done
            ShieldOfFaith,          //done
            Bomb,                   //done
            IchorEarned,            //done
            IchorLingered,          //done
            PoisonImmunity,         //done
            BlazingTrail,           //done
            DivineCurse,            //done
            StrengthFromWithin,     //done
            StrengthFromWithout,    //done
            NeptunesCurse,          //done
            FortunesBlessing,       //done
            DeathsDoor,             //done
            TheDeal,                //done
            RabbitsFoot,            //done
            Ambrosia,               //done
            GiftFromBelow,          //done
            FervoursHost,           //done
            Intangible,             //done
            Retribution,            //done
            WraithsWill,            //done
            GodlyMoment,            //done
            KinOfTurua,             //done
            TheCollector,           //done
            ConsecratedOil          //done
        }
    }
}
