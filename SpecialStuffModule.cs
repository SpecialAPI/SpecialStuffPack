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

            //init synergies
            SpecialSynergies.Init();
            
            //add other stuff
            EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").AddComponent<DragunDeathChecks>();
        }

        public override void Exit()
        {
        }

        public static string globalPrefix = "chmb";
    }
}
