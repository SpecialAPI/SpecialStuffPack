using ETGGUI.Inspector;
using SpecialStuffPack.Components;
using SpecialStuffPack.GungeonAPI;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.Items;
using SpecialStuffPack.SaveAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Text;
using Gungeon;
using MonoMod.RuntimeDetour;
using UnityEngine;
using System.Diagnostics;
using SpecialStuffPack.Placeables;
using SpecialStuffPack.Enemies;
using Dungeonator;
using SpecialStuffPack.SoundAPI;
using SpecialStuffPack.Controls;

namespace SpecialStuffPack
{
    public class SpecialStuffModule : ETGModule
    {
        public static int? GetActiveItemUICount(PlayerItem input)
        {
            if (input is GreenCandle)
            {
                return (input as GreenCandle).Flames;
            }
            return null;
        }

        public override void Init()
        {
            //asset bundle setup
            AssetBundleManager.LoadBundle();
            SoundManager.LoadBanksFromModProject();

            //saveapi setup
            SaveAPIManager.Setup("spapistuff");
        }

        public override void Start()
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
            LightningHornController.Init();
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
            //PastsRewardItem.Init();
            //ForceOfTwilight.Init();

            //init synergies
            SpecialSynergies.Init();

            //init enemies
            SpecialEnemies.AddAdvancedDragunAmmonomiconEntry();
            SpecialEnemies.SetupWizardKnightPrefab();
            SpecialEnemies.SetupAk47VeteranPrefab();

            //init placeables
            SpecialPlaceables.InitLockUnlockPedestal();
            SpecialPlaceables.InitDiamondShrine();
            SpecialPlaceables.InitSomethingSpecialPlaceable();

            //add other stuff
            EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();

            //add hooks
            new Hook(typeof(GameUIItemController).GetMethod("UpdateItem"), typeof(SpecialStuffModule).GetMethod("ItemUIUpdateHook"));
            //new Hook(typeof(Gun).GetMethod("HandleSpecificEndGunShoot", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialStuffModule).GetMethod("HandleChargeBurst"));

            //add console commands
            ItemAutocompletion = new AutocompletionSettings(delegate (string input)
            {
                List<string> list = new List<string>();
                foreach (string text in Game.Items.IDs)
                {
                    if (text.AutocompletionMatch(input.ToLower()))
                    {
                        Console.WriteLine(string.Concat(new string[]
                        {
                        "INPUT ",
                        input,
                        " KEY ",
                        text,
                        " MATCH!"
                        }));
                        list.Add(text.Replace("gungeon:", ""));
                    }
                    else
                    {
                        Console.WriteLine(string.Concat(new string[]
                        {
                        "INPUT ",
                        input,
                        " KEY ",
                        text,
                        " NO MATCH!"
                        }));
                    }
                }
                return list.ToArray();
            });
            ETGModConsole.Commands.GetGroup("spawn").AddUnit("item", SpawnItem, ItemAutocompletion);
            ETGModConsole.Commands.AddUnit("play_sound", (string[] args) => ETGModConsole.Log(AkSoundEngine.PostEvent(args[0], GameManager.Instance.PrimaryPlayer.gameObject).ToString()));
            ETGModConsole.Commands.AddUnit("set_state", (string[] args) => ETGModConsole.Log(AkSoundEngine.SetState(args[0], args[1]).ToString()));
            ETGModConsole.Commands.AddUnit("componentsinroom", delegate(string[] args) { foreach (Component com in GameManager.Instance.PrimaryPlayer.CurrentRoom.GetComponentsInRoom<Component>()) { ETGModConsole.Log(com.GetType() + com.name); } });
            ETGModConsole.Commands.AddUnit("use", UseItem, ItemAutocompletion);
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
                GameObject item = UnityEngine.Object.Instantiate(Game.Items[args[0]].gameObject, GameManager.Instance.PrimaryPlayer.CenterPosition, Quaternion.identity);
                typeof(PlayerItem).GetMethod("DoEffect", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(item.GetComponent<PlayerItem>(), new object[] { GameManager.Instance.PrimaryPlayer });
                UnityEngine.Object.Destroy(item);
            }
        }

        public override void Exit()
        {
        }

        public static string globalPrefix = "chmb";
        public static AutocompletionSettings ItemAutocompletion;
    }
}
