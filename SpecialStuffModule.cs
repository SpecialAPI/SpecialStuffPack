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
using UnityEngine;

namespace SpecialStuffPack
{
    public class SpecialStuffModule : ETGModule
    {
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
            WishingOrb.Init();
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

            //init synergies
            SpecialSynergies.Init();
            
            //add other stuff
            EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();

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
