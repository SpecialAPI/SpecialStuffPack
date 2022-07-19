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
using BepInEx;

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
            //asset bundle setup
            AssetBundleManager.LoadBundle();
            SoundManager.LoadBanksFromModProject();

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
                ItemBuilder.Init();
                SynergyBuilder.Init();
                GungeonAPIMain.Init();
                GoopDatabase.Init();

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
                ETGModConsole.Commands.AddUnit("use", UseItem, ETGModConsole.GiveAutocompletionSettings);
                ETGModConsole.Commands.AddUnit("reset_ss_completion", delegate (string[] s)
                {
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_AMETHYST, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_OPAL, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_EMERALD, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_AQUAMARINE, false);
                    SaveAPIManager.SetFlag(CustomDungeonFlags.SOMETHINGSPECIAL_RUBY, false);
                });
                TCultistHandler.Init();
                SpecialOptions.Setup();
                SpecialInput.Setup();
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("Something bad happened while loading SpecialAPI's Stuff Reloaded: " + ex);
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

        public static string globalPrefix = "chmb";
    }
}
