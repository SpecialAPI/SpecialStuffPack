global using SpecialStuffPack.Tools;
global using SpecialStuffPack.Components;
global using SpecialStuffPack.GungeonAPI;
global using SpecialStuffPack.ItemAPI;
global using SpecialStuffPack.Items;
global using SpecialStuffPack.SaveAPI;
global using SpecialStuffPack.SynergyAPI;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Collections;
global using System.Reflection;
global using System.Text;
global using Gungeon;
global using MonoMod.RuntimeDetour;
global using UnityEngine;
global using SpecialStuffPack.Placeables;
global using SpecialStuffPack.Enemies;
global using Dungeonator;
global using SpecialStuffPack.SoundAPI;
global using SpecialStuffPack.Controls;
global using Random = UnityEngine.Random;
global using Object = UnityEngine.Object;
global using SpecialStuffPack.Items.Passives;
global using SpecialStuffPack.Items.Guns;
global using SpecialStuffPack.Items.Actives;
global using BepInEx;
global using HarmonyLib;
global using SpecialStuffPack.EnumExtensions;
global using HutongGames.PlayMaker;
global using HutongGames.PlayMaker.Actions;
global using InControl;
global using static SpecialStuffPack.ItemAPI.ItemBuilder;
global using static SpecialStuffPack.ItemAPI.GunBuilder;
global using static SpecialStuffPack.SynergyAPI.SynergyBuilder;
global using ModifyMethod = StatModifier.ModifyMethod;
using UnityEngine.Networking;
using System.IO;
using SpecialStuffPack.Feedback;
using SpecialStuffPack.CursorAPI;
using SpecialStuffPack.Characters;
using SpecialStuffPack.Items.Pickups;
using System.Diagnostics;

namespace SpecialStuffPack
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class SpecialStuffModule : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.specialstuffpack";
        public const string NAME = "SpecialAPI's Stuff";
        public const string VERSION = "2.0.0";
        public const string LOG_VERSION = $"{VERSION}b-1";
        public static readonly Color LogColor = new Color32(217, 57, 106, 255);
        public static Texture2D spCultistBosscard;
        public static int EverhoodCursorId = -1;

        [HarmonyPatch(typeof(AkSoundEngine), nameof(AkSoundEngine.PostEvent), typeof(string), typeof(GameObject))]
        [HarmonyPrefix]
        public static void Funny(ref string in_pszEventName)
        {
            if(GameManager.Options != null && GameManager.Options.CurrentCursorIndex == EverhoodCursorId)
            {
                if(in_pszEventName == "Play_UI_menu_confirm_01" || in_pszEventName == "Play_UI_menu_cancel_01" || in_pszEventName == "Play_UI_menu_back_01" || in_pszEventName == "Play_UI_menu_unpause_01")
                {
                    in_pszEventName = "EverhoodMenuConfirm";
                }
                if(in_pszEventName == "Play_UI_menu_select_01" || in_pszEventName == "Play_UI_menu_pause_01")
                {
                    in_pszEventName = "EverhoodMenuSelect";
                }
            }
        }

        public static int? GetActiveItemUICount(PlayerItem input)
        {
            if (input is GreenCandle)
            {
                return (input as GreenCandle).Flames;
            }
            return null;
        }

        public void Awake()
        {
            BepInEx.Logging.Logger.Listeners.Add(new LogRecorder());
            var asm = Assembly.GetExecutingAssembly();
            foreach(var type in asm.GetTypes())
            {
                var custom = type.GetCustomAttributes(false);
                if (custom != null)
                {
                    var extension = custom.OfType<EnumExtensionAttribute>().FirstOrDefault();
                    if(extension != null && extension.type != null && extension.type.IsEnum)
                    {
                        foreach(var f in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                        {
                            f.SetValue(null, ETGModCompatibility.ExtendEnum(GUID, f.Name, extension.type));
                        }
                    }
                }
            }
            new Harmony(GUID).PatchAll();
            //asset bundle setup
            AssetBundleManager.LoadBundle();
            SoundManager.Init();
            spCultistBosscard = AssetBundleManager.Load<Texture2D>("cultist_bosscard");

            //saveapi setup
            SaveAPIManager.Setup("spapistuff");
        }

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            try
            {
                ETGModConsole.Log($"{NAME} {LOG_VERSION} started loading.").Foreground = LogColor;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                InitStatics();
                SoundManager.LoadBankFromModProject("SpecialStuffPack.SPECIAL_SFX.bnk");

                //init apis
                InitItemBuilder();
                InitSynergyBuilder();
                GungeonAPIMain.Init();
                GoopDatabase.Init();
                VFXDatabase.Init();

                GetItemById<HealPlayerItem>(412).healingAmount = 1f;
                GetItemById(326).AddComponent<SPCultistBandana>();
                ETGMod.Databases.Strings.Core.Set("#PLAYER_NAME_" + PlayableCharactersE.SPCultist.ToString().ToUpper(), "Cultist");
                ETGMod.Databases.Strings.Core.Set("#PLAYER_NICK_" + PlayableCharactersE.SPCultist.ToString().ToUpper(), "child");
                ETGMod.Databases.Strings.Core.Set("#SPAPI_RAT_NOTE_SP_CULTIST",
                    "Don't know how to fight, do you? You also don't have a main character for me to beat up, so have these keys as a consolation prize instead.\n\n" +
                    "Later, %INSULT! - R.R.");

                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_INTRODUCTION", "I break mods and am a stupid feature.");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_AGREE", "Save and Quit (and probably break everything)");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_DISAGREE", "Pick This Option");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_BRINGING_CRYOELEVATOR", "*sounds of mods breaking*");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_NOT_BRINGING_CRYOELEVATOR", "Good.");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_OUCH", "Oh no... It's arriving...");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_CHANGEDMIND", "I am a dumb design decition and shouldn't have been in the game to begin with.");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_DIDCHANGEMIND", "Unbreak everything");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_DIDNTCHANGEMIND", "Remain in the broken spaghetti land");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_BRINGING_ELEVATOR", "Is that mass of frozen spaghetti gone? Good.");
                ETGMod.Databases.Strings.Core.Set("#CRYOBUTTON_ANNOYED", "I wouldn't recommend wasting your time on a dumb feature such as me.");

                RedGun.globalIndoctrinateInteractable = AssetBundleManager.Load<GameObject>("IndoctrinateFollowerInteractable");
                var interactable = RedGun.globalIndoctrinateInteractable.AddComponent<IndoctrinateFollowerInteractable>();
                interactable.AddComponent<tk2dSprite>().SetSprite(BulletKinEnemy.sprite.Collection, BulletKinEnemy.sprite.spriteId);
                interactable.AddComponent<tk2dSpriteAnimator>().Library = BulletKinEnemy.spriteAnimator.Library;
                interactable.noRedgunKey = SetString("#INDOCTRINATE_NOREDGUN", "I can only be converted by the Red Gun.");
                interactable.notEnoughAmmoKey = SetString("#INDOCTRINATE_NOTENOUGH", "You don't have enough resources to convert me!");
                interactable.validKey = SetString("#INDOCTRINATE_VALID", "Convert me to your cult, I will follow your teachings faithfully.");
                interactable.yesKey = SetString("#INDOCTRINATE_YES", "<Indoctrinate follower <Lose %AMMO ammo>>");
                interactable.yesHealthKey = SetString("#INDOCTRINATE_YESHEALTH", "<Indoctrinate follower <Lose %HEART_SYMBOL>>");
                interactable.noKey = SetString("#INDOCTRINATE_NO", "<Walk away>");

                //init items
                WoodenToken.Init();
                PayToWin.Init();
                FusedAmmolet.Init();
                BoxOfStuff.Init();
                ProtectiveArmorItem.Init();
                ShacklesItem.Init();
                RatWhistle.Init();
                SubscribeButton.Init();
                ExtraChestItem.Init();
                MirrorOfTruth.Init();
                BadLuckClover.Init();
                Calendar.Init();
                GreenCandle.Init();
                GlassBell.Init();
                ShootingStar.Init();
                WishingOrb.Init();
                Butter.Init();
                LegitCoupon.Init();
                BinaryGun.Init();
                AsteroidBelt.Init();
                GravediggerShovel.Init();
                HandheldCatapult.Init();
                GoldKey.Init();
                AmethystItem.Init();
                OpalItem.Init();
                EmeraldItem.Init();
                AquamarineItem.Init();
                RubyItem.Init();
                DiamondItem.Init();
                OtherworldlyAssistance.Init();
                HotCoal.Init();
                GuardianOfTime.Init();
                MarblesItem.Init();
                FrailHeart.Init();
                UndyingTotem.Init();
                Watermelon.Init();
                ConsoleController.Init();
                StaticRoll.Init();
                MirroredBullet.Init();
                BloodyScales.Init();
                Plushie.Init();
                BossChest.Init();
                Chaos.Init();
                Launcher.Init();
                BalancingPole.Init();
                InfinityCrystal.Init();
                EnergyDrink.Init();
                AmmoFlower.Init();
                FlatBullets.Init();
                AkNAN.Init();
                CactiClub.Init();
                HelloWorld.Init();
                Frogun.Init();
                Revolvever.Init();
                AkPI.Init();
                RoundBullets.Init();
                OrbOfKnowledge.Init();
                AkSoundEngineGun.Init();
                EnderGun.Init();
                LeatherWhip.Init();
                PistolWhip.Init();
                WhipCream.Init();
                SynergyCompletionGun.Init();
                SusShotgun.Init();
                RedGun.Init();
                LeadFists.Init();
                HeartPiece.Init();
                HotSauce.Init();
                MagicBag.Init();
                BananaJam.Init();
                CrownBullets.Init();
                RedAmmolet.Init();
                MarkedCalendar.Init();
                Shredder.Init();
                RustyBullets.Init();
                RustyAmmoBox.Init();
                WoodenDice.Init();
                MagnifyingGlass.Init();
                DisguiseHat.Init();
                Bug.Init();
                AlexandriaBook.Init();
                BrokenCalculator.Init();
                BrokenMask.Init();
                TarotCards.Init();
                LeaderDeck.Init();
                Magnificus.Init();
                FossilFuel.Init();
                SniperGun.Init();
                ChamberChamberChamber.Init();
                CCCAbstractions.Init();
                GoldenShotgun.Init();
                UglyGun.Init();
                HDGun.Init();
                //Evergun.Init();
                //SoulGun.Init();
                //PastsRewardItem.Init();
                //ForceOfTwilight.Init();

                CursorMaker.BuildCursor(AssetBundleManager.Load<Texture2D>("Mouse"));
                EverhoodCursorId = CursorMaker.BuildCursor(AssetBundleManager.Load<Texture2D>("EverhoodHand"));
                CursorMaker.BuildCursor(AssetBundleManager.Load<Texture2D>("SuperliminalCursor"));
                CursorMaker.BuildCursor(AssetBundleManager.Load<Texture2D>("SuperliminalSmiley"));

                Item["boxofstuff"].associatedItemChanceMods = Item.Where(x => x.Value != null && x.Value.quality >= PickupObject.ItemQuality.D && x.Value.quality <= PickupObject.ItemQuality.S).Select(x => new LootModData
                {
                    AssociatedPickupId = x.Value.PickupObjectId,
                    DropRateMultiplier = 1.25f
                }).ToArray();

                LilChest.SetupChests();

                //init synergies
                SpecialSynergies.Init();

                //init characters
                CharacterBuilder.BuildCharacter<PlayerController>("PlayerExample", "example", "PlayerExampleCollection", "PlayerExampleAnimation", "playersprites/example/sprites/", PlayableCharactersE.CustomCharacterExample,
                    new(5, 0, 14, 4), new(7, 5, 8, 10),
                    "player_example_portrait", "playersprites/example/bosscard/", 10, "PlayerExampleMinimapIcon", "example_minimap_001", "PlayerExampleStats", new()
                    {
                        { PlayerStats.StatType.Health, 10f },
                        { PlayerStats.StatType.Damage, 100000f }
                    }, 5f, 10, 10, 5, new()
                    {
                        GuntherId,
                        CaseyId,
                        MakeshiftCannonId,
                        YariLauncherId,
                        CloneId,
                        GundromedaStrainId,
                        BlankBulletsId
                    }, new()
                    {
                        KlobbeId
                    }, 99999, 99, false, "guide", "guide_hand_001", "guide_hand_001", true, false, "bug");

                LameyRework.Init();
                //NinjaRework.Init();
                MrGiando.Init();
                //CosmonautRework.Init();

                //init enemies
                SpecialEnemies.AddAdvancedDragunAmmonomiconEntry();
                SpecialEnemies.AddNinja();
                SpecialEnemies.SetupAk47VeteranPrefab();

                //init placeables
                SpecialPlaceables.InitLockUnlockPedestal();
                SpecialPlaceables.InitDiamondShrine();
                SpecialPlaceables.InitSomethingSpecialPlaceable();
                //SpecialPlaceables.InitDoor(); //bye bye stupid door
                CCCChallenge.Init();

                //add other stuff
                EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();

                //add hooks
                new Hook(typeof(GameUIItemController).GetMethod("UpdateItem"), typeof(SpecialStuffModule).GetMethod("ItemUIUpdateHook"));
                new Hook(
                     typeof(RoomHandler).GetMethod("PostGenerationCleanup", BindingFlags.Instance | BindingFlags.Public),
                     typeof(SpecialStuffModule).GetMethod("FixBadDodgerollCode")
                 );

                //add sewer grate rooms
                foreach (var asset in AssetBundleManager.specialeverything.GetAllAssetNames())
                {
                    if (asset.ToLowerInvariant().StartsWith("assets/rooms/sewerentrance"))
                    {
                        //RoomFactory.AddRoomToSewerGratePool(RoomFactory.BuildFromTextAsset(AssetBundleManager.Load<TextAsset>(asset)));
                    }
                }

                //new Hook(typeof(Gun).GetMethod("HandleSpecificEndGunShoot", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialStuffModule).GetMethod("HandleChargeBurst"));

                ETGModConsole.Commands.AddGroup(globalPrefix);
                var group = ETGModConsole.Commands.GetGroup(globalPrefix);
                ETGModConsole.CommandDescriptions.Add(globalPrefix, $"Group used by commands added by {NAME}.");

                group.AddUnit("use_active", UseItem, ETGModConsole.GiveAutocompletionSettings);
                ETGModConsole.CommandDescriptions.Add($"{globalPrefix} use_active", "Uses the given active item once. If a second argument is given, it specifies how many times the item should be used.");
                group.AddUnit("showhitboxes", x => ETGModConsole.SwitchValue(x.FirstOrDefault(), ref showHitboxes, "Show Hitboxes"));
                ETGModConsole.CommandDescriptions.Add($"{globalPrefix} showhitboxes", "Toggles Hitbox display.");
                group.AddUnit("decrypt_save", x => { SaveManager.GameSave.encrypted = false; GameStatsManager.Save(); });
                ETGModConsole.CommandDescriptions.Add($"{globalPrefix} decrypt_save", "Temporarily decrypts the current save file for debug purposes.");

                gameObject.AddComponent<FeedbackCore>();

                TCultistHandler.Init();
                SpecialOptions.Setup();
                SpecialInput.Setup();

                //WindowsWindowNamer.RenameWindow();

                stopwatch.Stop();
                ETGModConsole.Log($"{NAME} {LOG_VERSION} loaded successfully.").Foreground = LogColor;
                ETGModConsole.Log($"{NAME} took {stopwatch.Elapsed} to load").Foreground = LogColor;
                ETGModConsole.Log($"Use the feedback form in the F8 menu to send feedback and report bugs.").Foreground = LogColor;
                ETGModConsole.Log($"You can also use the command \"{globalPrefix} feedback\".").Foreground = LogColor;
            }
            catch (Exception ex)
            {
                ETGModConsole.Log($"Something bad happened while loading {NAME}: " + ex).Foreground = LogColor;
            }
        }

        public void Update()
        {
            if (showHitboxes && PhysicsEngine.Instance != null && PhysicsEngine.Instance.AllRigidbodies != null)
            {
                foreach (var obj in PhysicsEngine.Instance.AllRigidbodies)
                {
                    if (obj && obj.sprite && Vector2.Distance(obj.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter) < 8)
                    {
                        HitboxMonitor.DisplayHitbox(obj);
                    }
                }
            }
        }

        public static void FixBadDodgerollCode(Action<RoomHandler> orig, RoomHandler room)
        {
            PrototypeDungeonRoom old = room.area.prototypeRoom;
            orig(room);
            room.area.prototypeRoom ??= old;
        }

        public static bool HandleChargeBurst(Func<Gun, ProjectileModule, bool, ProjectileData, bool, bool> orig, Gun self, ProjectileModule module, bool canAttack, ProjectileData overrideProjectileData, bool playEffects)
        {
            bool result = orig(self, module, canAttack, overrideProjectileData, playEffects);
            return result;
        }

        public static void ItemUIUpdateHook(Action<GameUIItemController, PlayerItem, List<PlayerItem>> orig, GameUIItemController self, PlayerItem current, List<PlayerItem> items)
        {
            orig(self, current, items);
            if (!GameUIRoot.Instance.ForceHideItemPanel && !self.temporarilyPreventVisible && current != null)
            {
                if (!((current.canStack && current.numberOfUses > 1 && current.consumable) || (current.numberOfUses > 1 && current.UsesNumberOfUsesBeforeCooldown && !current.IsOnCooldown)) && !(current is EstusFlaskItem) && (!(current is RatPackItem) || 
                    current.IsOnCooldown) && GetActiveItemUICount(current) != null)
                {
                    self.ItemCountLabel.IsVisible = true;
                    self.ItemCountLabel.Text = GetActiveItemUICount(current).Value.ToString();
                }
            }
        }

        public void UseItem(string[] args)
        {
            if (args.Length < 1)
            {
                ETGModConsole.Log("Item not given!");
                return;
            }
            if (!Game.Items.ContainsID(args[0]))
            {
                ETGModConsole.Log("Invalid item " + args[0] + "!");
                return;
            }
            if (Game.Items[args[0]] is not PlayerItem)
            {
                ETGModConsole.Log("Invalid item " + args[0] + "!");
                return;
            }
            if (!GameManager.Instance.PrimaryPlayer)
            {
                ETGModConsole.Log("Player doesn't exist!");
                return;
            }
            int numToSpawn = 1;
            if (args.Length > 1)
            {
                if(!int.TryParse(args[1], out numToSpawn))
                {
                    numToSpawn = 1;
                }
            }
            for(int i = 0; i < numToSpawn; i++)
            {
                GameObject item = Instantiate(Game.Items[args[0]].gameObject, GameManager.Instance.PrimaryPlayer.CenterPosition, Quaternion.identity);
                item.GetComponent<PlayerItem>().DoEffect(GameManager.Instance.PrimaryPlayer);
                Destroy(item);
            }
        }

        public static string globalPrefix = "spapi";
        public static bool showHitboxes;
    }
}
