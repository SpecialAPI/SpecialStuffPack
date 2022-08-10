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
using System.IO;
using System.Globalization;

namespace SpecialStuffPack
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class SpecialStuffModule : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.specialstuffpack";
        public const string NAME = "SpecialAPI's Stuff";
        public const string VERSION = "1.0.0";

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
            //SoundManager.Init();

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
                //init apis
                InitItemBuilder();
                InitSynergyBuilder();
                GungeonAPIMain.Init();
                GoopDatabase.Init();

                GetItemById<HealPlayerItem>(412).healingAmount = 1f;
                GetItemById(326).AddComponent<SPCultistBandana>();
                ETGMod.Databases.Strings.Core.Set("#PLAYER_NAME_" + PlayableCharactersE.SPCultist.ToString().ToUpper(), "Cultist");
                ETGMod.Databases.Strings.Core.Set("#PLAYER_NICK_" + PlayableCharactersE.SPCultist.ToString().ToUpper(), "child");
                ETGMod.Databases.Strings.Core.Set("#SPAPI_RAT_NOTE_SP_CULTIST", 
                    "Don't know how to fight, do you? You also don't have a main character for me to beat up, so have these keys as a consolation prize instead.\n\n" +
                    "Later, %INSULT! - R.R.");

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
                Items.Calendar.Init();
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
                //SoulGun.Init();
                //PastsRewardItem.Init();
                //ForceOfTwilight.Init();

                //init synergies
                SpecialSynergies.Init();

                //init enemies
                SpecialEnemies.AddAdvancedDragunAmmonomiconEntry();
                SpecialEnemies.AddNinja();
                SpecialEnemies.SetupAk47VeteranPrefab();

                //init placeables
                SpecialPlaceables.InitLockUnlockPedestal();
                SpecialPlaceables.InitDiamondShrine();
                SpecialPlaceables.InitSomethingSpecialPlaceable();
                SpecialPlaceables.InitDoor();

                //add other stuff
                EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();

                //add hooks
                new Hook(typeof(GameUIItemController).GetMethod("UpdateItem"), typeof(SpecialStuffModule).GetMethod("ItemUIUpdateHook"));
                new Hook(
                     typeof(RoomHandler).GetMethod("PostGenerationCleanup", BindingFlags.Instance | BindingFlags.Public),
                     typeof(SpecialStuffModule).GetMethod("FixBadDodgerollCode")
                 );
                //new Hook(typeof(Gun).GetMethod("HandleSpecificEndGunShoot", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialStuffModule).GetMethod("HandleChargeBurst"));
                ETGModConsole.Commands.AddUnit("play_sound", (string[] args) => ETGModConsole.Log(AkSoundEngine.PostEvent(args[0], GameManager.Instance.PrimaryPlayer.gameObject).ToString()));
                ETGModConsole.Commands.AddUnit("set_state", (string[] args) => ETGModConsole.Log(AkSoundEngine.SetState(args[0], args[1]).ToString()));
                var pitch = new float[0];
                ETGModConsole.Commands.AddUnit("set_pitch", x => pitch = x.Select(x2 => float.Parse(x2)).ToArray());
                ETGModConsole.Commands.AddUnit("switch", x => ETGModConsole.Log(Game.Items[x[0]].GetComponent<Gun>().gunSwitchGroup), ETGModConsole.GiveAutocompletionSettings);
                ETGModConsole.Commands.AddUnit("play", x => StartCoroutine(Play(pitch)));
                ETGModConsole.Commands.AddUnit("use", UseItem, ETGModConsole.GiveAutocompletionSettings);
                ETGModConsole.Commands.AddUnit("soundtest", x => Instantiate(AssetBundleManager.Load<GameObject>("testsound")));
                ETGModConsole.Commands.AddUnit("showhitboxes", x => ETGModConsole.SwitchValue(x.FirstOrDefault(), ref showHitboxes, "Show Hitboxes"));
                ETGModConsole.Commands.AddUnit("reset_ss_completion", delegate (string[] s)
                {
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_AMETHYST, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_OPAL, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_EMERALD, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_AQUAMARINE, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_RUBY, false);
                });
                ETGModConsole.Commands.AddUnit("decryptSave", x => { SaveManager.GameSave.encrypted = false; GameStatsManager.Save(); });
                ETGModConsole.Commands.AddUnit("setflag1", x => { GameStatsManager.Instance.SetFlag(GungeonFlagsE.TEST_FLAG_1, bool.Parse(x[0])); GameStatsManager.Save(); });
                ETGModConsole.Commands.AddUnit("setflag2", x => { GameStatsManager.Instance.SetFlag(GungeonFlagsE.TEST_FLAG_2, bool.Parse(x[0])); GameStatsManager.Save(); });
                ETGModConsole.Commands.AddUnit("setflag3", x => { GameStatsManager.Instance.SetFlag(GungeonFlagsE.TEST_FLAG_3, bool.Parse(x[0])); GameStatsManager.Save(); });
                ETGModConsole.Commands.AddUnit("setflag4", x => { GameStatsManager.Instance.SetFlag(GungeonFlagsE.TEST_FLAG_4, bool.Parse(x[0])); GameStatsManager.Save(); });
                ETGModConsole.Commands.AddUnit("getflag1", x => { ETGModConsole.Log(GameStatsManager.Instance.GetFlag(GungeonFlagsE.TEST_FLAG_1)); });
                ETGModConsole.Commands.AddUnit("getflag2", x => { ETGModConsole.Log(GameStatsManager.Instance.GetFlag(GungeonFlagsE.TEST_FLAG_2)); });
                ETGModConsole.Commands.AddUnit("getflag3", x => { ETGModConsole.Log(GameStatsManager.Instance.GetFlag(GungeonFlagsE.TEST_FLAG_3)); });
                ETGModConsole.Commands.AddUnit("getflag4", x => { ETGModConsole.Log(GameStatsManager.Instance.GetFlag(GungeonFlagsE.TEST_FLAG_4)); });
                ETGModConsole.Commands.AddUnit("gettracked", x => { GameStatsManager.Instance.GetPlayerMaximum(TrackedMaximums.MOST_KEYS_HELD); });
                TCultistHandler.Init();
                SpecialOptions.Setup();
                SpecialInput.Setup();
                ETGModConsole.Log($"{NAME} loaded successfully.");
            }
            catch (Exception ex)
            {
                ETGModConsole.Log($"Something bad happened while loading {NAME}: " + ex);
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

        public static IEnumerator Play(float[] pitch)
        {
            foreach(var f in pitch)
            {
                AkSoundEngine.SetRTPCValue("Pitch_Metronome", f);
                AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", GameManager.Instance.PrimaryPlayer.gameObject);
                yield return new WaitForSecondsRealtime(0.75f);
            }
            yield break;
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

        public void SpawnItem(string[] args)
        {
            if(args.Length < 1)
            {
                ETGModConsole.Log("Item not given!");
                return;
            }
            if (!Game.Items.ContainsID(args[0]))
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
            if(args.Length > 1)
            {
                int.TryParse(args[1], out numToSpawn);
            }
            for(int i = 0; i < numToSpawn; i++)
            {
                LootEngine.SpawnItem(Game.Items[args[0]].gameObject, GameManager.Instance.PrimaryPlayer.CenterPosition, Vector2.down, 0f, true, false, false);
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
            if (!(Game.Items[args[0]] is PlayerItem))
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
                typeof(PlayerItem).GetMethod("DoEffect", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(item.GetComponent<PlayerItem>(), new object[] { GameManager.Instance.PrimaryPlayer });
                Destroy(item);
            }
        }

        public static string globalPrefix = "spapi";
        public static bool showHitboxes;
    }
}
