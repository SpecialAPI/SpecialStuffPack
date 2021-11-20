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
using System.Reflection;
using System.Text;
using Gungeon;
using MonoMod.RuntimeDetour;
using UnityEngine;

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
            //saveapi setup
            SaveAPIManager.Setup("spapistuff");
        }

        public override void Start()
        {
            //init apis
            ItemBuilder.Init();
            SynergyBuilder.Init();
            GungeonAPIMain.Init();

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

            //init synergies
            SpecialSynergies.Init();
            
            //add other stuff
            EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();

            //add hooks
            new Hook(typeof(GameUIItemController).GetMethod("UpdateItem"), typeof(SpecialStuffModule).GetMethod("ItemUIUpdateHook"));

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
            LootEngine.SpawnItem(Game.Items[args[0]].gameObject, GameManager.Instance.PrimaryPlayer.CenterPosition, Vector2.down, 0f, true, false, false);
        }

        public override void Exit()
        {
        }

        public static string globalPrefix = "chmb";
        public static AutocompletionSettings ItemAutocompletion;
    }
}
