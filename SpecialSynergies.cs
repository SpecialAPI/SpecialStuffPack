using MonoMod.RuntimeDetour;
using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecialStuffPack
{
    public static class SpecialSynergies
    {
        public static void Init()
        {
            //build synergies
            SynergyBuilder.CreateSynergy("Fufufufufu", new List<int> { ItemBuilder.ItemIds["woodentoken"] }, new List<int> { 626, 667, 662, 663, 463, ItemBuilder.ItemIds["ratwhistle"] });
            SynergyBuilder.CreateSynergy("Double the Wish!", new List<int> { ItemBuilder.ItemIds["wishorb"], 595 });
            SynergyBuilder.CreateSynergy("Wish of Power", new List<int> { ItemBuilder.ItemIds["wishorb"], 578 });
            SynergyBuilder.CreateSynergy("Wish of Reflection", new List<int> { ItemBuilder.ItemIds["wishorb"], 190 });
            SynergyBuilder.CreateSynergy("50% OFF ON ALL IN-GAME PURCHASES!", new List<int> { ItemBuilder.ItemIds["paytowin"], 476 });
            SynergyBuilder.CreateSynergy("Super Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 321 });
            SynergyBuilder.CreateSynergy("Stunning Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 322 });
            SynergyBuilder.CreateSynergy("Random Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 325 });
            SynergyBuilder.CreateSynergy("Poison Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 342 });
            SynergyBuilder.CreateSynergy("Hot Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 343 });
            SynergyBuilder.CreateSynergy("Freezing Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], 344 });
            SynergyBuilder.CreateSynergy("v3.0 The Massive Update", new List<int> { ItemBuilder.ItemIds["boxofstuff"] }, new List<int> { 131, 134, 133, 558, 216, 28, 493, 663 });
            SynergyBuilder.CreateSynergy("Also click that bell", new List<int> { ItemBuilder.ItemIds["subscribebutton"] }, new List<int> { 113, 237 });
            SynergyBuilder.CreateSynergy("Also you can leave a comment", new List<int> { ItemBuilder.ItemIds["subscribebutton"], 28 }, null, false);
            SynergyBuilder.CreateSynergy("Chill", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { 109, 387, 278, 344, 223, 97, 364, 170, 225, 40 });
            SynergyBuilder.CreateSynergy("All At Once", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { 626, 667, 662, 663, 463, ItemBuilder.ItemIds["woodentoken"] });

            //add synergy components
            PickupObjectDatabase.GetById(476).AddComponent<MicrotransactionDiscountSynergyController>().SynergyToCheck = "50% OFF ON ALL IN-GAME PURCHASES!";

            //add synergy hooks
            new Hook(
                typeof(Gun).GetMethod("DecrementAmmoCost", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MicrotransactionDiscountSynergyController).GetMethod("MaybeReduceCost")
            );
        }
    }
}
