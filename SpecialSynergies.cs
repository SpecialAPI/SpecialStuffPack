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
            SynergyBuilder.CreateSynergy("Shadow Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], 820 });
            SynergyBuilder.CreateSynergy("Blessed Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], 538 });
            SynergyBuilder.CreateSynergy("Just Your Normal Luck", new List<int> { ItemBuilder.ItemIds["badluckclover"], 289 });
            SynergyBuilder.CreateSynergy("Somehow... Luckier?", new List<int> { ItemBuilder.ItemIds["badluckclover"], ItemBuilder.ItemIds["truthmirror"] });
            SynergyBuilder.CreateSynergy("why do you keep crashing", new List<int> { ItemBuilder.ItemIds["binary_gun"], 38 });
            SynergyBuilder.CreateSynergy("BURN! BUUURRRNNNN!!!", new List<int> { ItemBuilder.ItemIds["greencandle"] }, new List<int> { 253, 191, 295 });
            SynergyBuilder.CreateSynergy("Ring It Twice", new List<int> { ItemBuilder.ItemIds["glassbell"], 237 });
            SynergyBuilder.CreateSynergy("Celestial Rhythm", new List<int> { ItemBuilder.ItemIds["shooting_star"], 52 });
            SynergyBuilder.CreateSynergy("Wishing Star", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["wishorb"] });
            SynergyBuilder.CreateSynergy("The Initial Idea", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["asteroidbelt"] }, null, false, new List<StatModifier> { StatModifier.Create(PlayerStats.StatType.Damage, 
                StatModifier.ModifyMethod.MULTIPLICATIVE, 2f) });
            SynergyBuilder.CreateSynergy("Bouncy Throws", new List<int> { ItemBuilder.ItemIds["butter"], 288 });
            SynergyBuilder.CreateSynergy("Piercing Throws", new List<int> { ItemBuilder.ItemIds["butter"], 172 });
            SynergyBuilder.CreateSynergy("Homing Boomerang Throws", new List<int> { ItemBuilder.ItemIds["butter"] }, new List<int> { 240, 284 });
            SynergyBuilder.CreateSynergy("Take It Before They Notice", new List<int> { ItemBuilder.ItemIds["legitcoupon"] }, new List<int> { 460, 237, 438, 71, 462, 216, 250, 458, 206, 543, 182 });

            //add this long synergy
            List<StatModifier> sm = new List<StatModifier>();
            if(DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                sm.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, -3f));
                sm.Add(StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.5f));
            }
            else
            {
                sm.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, 1f));
            }
            SynergyBuilder.CreateSynergy("I Hate Mondays", new List<int> { ItemBuilder.ItemIds["calendar"] }, new List<int> { 143, 399 }, statModifiers: sm);

            //add new items to existing synergies
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.PITCHPERFECT, ItemBuilder.ItemIds["greencandle"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.LOADED_DICE, ItemBuilder.ItemIds["greencandle"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.RELODESTAR, ItemBuilder.ItemIds["bombammolet"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.MINOR_BLANKABLES, ItemBuilder.ItemIds["bombammolet"]);

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
