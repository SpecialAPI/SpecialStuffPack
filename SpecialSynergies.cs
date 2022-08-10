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
            SynergyBuilder.CreateSynergy("Fufufufufu", new List<int> { ItemBuilder.ItemIds["woodentoken"] }, new List<int> { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemBuilder.ItemIds["ratwhistle"] });
            SynergyBuilder.CreateSynergy("Double the Wish!", new List<int> { ItemBuilder.ItemIds["wishorb"], LifeOrbId });
            SynergyBuilder.CreateSynergy("Wish of Power", new List<int> { ItemBuilder.ItemIds["wishorb"], SprunId });
            SynergyBuilder.CreateSynergy("Wish of Reflection", new List<int> { ItemBuilder.ItemIds["wishorb"], RollingEyeId });
            SynergyBuilder.CreateSynergy("50% OFF ON ALL IN-GAME PURCHASES!", new List<int> { ItemBuilder.ItemIds["paytowin"], MicrotransactionGunId });
            SynergyBuilder.CreateSynergy("Super Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], GoldAmmoletId });
            SynergyBuilder.CreateSynergy("Stunning Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], LodestoneAmmoletId });
            SynergyBuilder.CreateSynergy("Random Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], ChaosAmmoletId });
            SynergyBuilder.CreateSynergy("Poison Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], UraniumAmmoletId });
            SynergyBuilder.CreateSynergy("Hot Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], CopperAmmoletId });
            SynergyBuilder.CreateSynergy("Freezing Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], FrostAmmoletId });
            SynergyBuilder.CreateSynergy("v3.0 The Massive Update", new List<int> { ItemBuilder.ItemIds["boxofstuff"] }, new List<int> { UtilityBeltId, AmmoBeltId, BackpackId, BottleId, BoxId, MailboxId, BriefcaseOfCashId, ResourcefulSackId });
            SynergyBuilder.CreateSynergy("Also click that bell", new List<int> { ItemBuilder.ItemIds["subscribebutton"] }, new List<int> { RocketPoweredBulletsId, AgedBellId, ItemBuilder.ItemIds["glassbell"] });
            SynergyBuilder.CreateSynergy("Also you can leave a comment", new List<int> { ItemBuilder.ItemIds["subscribebutton"], MailboxId }, null, false);
            SynergyBuilder.CreateSynergy("Chill", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            SynergyBuilder.CreateSynergy("All At Once", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemBuilder.ItemIds["woodentoken"] });
            SynergyBuilder.CreateSynergy("Shadow Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], ShadowCloneId });
            SynergyBuilder.CreateSynergy("Blessed Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], SilverBulletsId });
            SynergyBuilder.CreateSynergy("Just Your Normal Luck", new List<int> { ItemBuilder.ItemIds["badluckclover"], SevenLeafCloverId });
            SynergyBuilder.CreateSynergy("Somehow... Luckier?", new List<int> { ItemBuilder.ItemIds["badluckclover"], ItemBuilder.ItemIds["truthmirror"] });
            SynergyBuilder.CreateSynergy("why do you keep crashing", new List<int> { ItemBuilder.ItemIds["binary_gun"], MagnumId });
            SynergyBuilder.CreateSynergy("BURN! BUUURRRNNNN!!!", new List<int> { ItemBuilder.ItemIds["greencandle"] }, new List<int> { GungeonPepperId, RingOfFireResistanceId, HotLeadId });
            SynergyBuilder.CreateSynergy("Ring It Twice", new List<int> { ItemBuilder.ItemIds["glassbell"], AgedBellId });
            SynergyBuilder.CreateSynergy("Celestial Rhythm", new List<int> { ItemBuilder.ItemIds["shooting_star"], CrescentCrossbowId });
            SynergyBuilder.CreateSynergy("Wishing Star", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["wishorb"] });
            SynergyBuilder.CreateSynergy("The Initial Idea", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["asteroidbelt"] }, null, false, new List<StatModifier> { StatModifier.Create(PlayerStats.StatType.Damage, 
                StatModifier.ModifyMethod.MULTIPLICATIVE, 2f) });
            SynergyBuilder.CreateSynergy("Bouncy Throws", new List<int> { ItemBuilder.ItemIds["butter"], BouncyBulletsId });
            SynergyBuilder.CreateSynergy("Piercing Throws", new List<int> { ItemBuilder.ItemIds["butter"], GhostBulletsId });
            SynergyBuilder.CreateSynergy("Homing Boomerang Throws", new List<int> { ItemBuilder.ItemIds["butter"] }, new List<int> { HomingBulletsId, CrutchId });
            SynergyBuilder.CreateSynergy("Take It Before They Notice", new List<int> { ItemBuilder.ItemIds["legitcoupon"] }, new List<int> { ChaffGrenadeId, AgedBellId, ExplosiveDecoyId, DecoyId, SmokeBombId, BoxId, GrapplingHookId, RingOfEtherealFormId, CharmHornId, ThePredatorId, GreyMauserId });
            SynergyBuilder.CreateSynergy("The zombies are coming", new List<int> { ItemBuilder.ItemIds["gravediggershovel"] }, new List<int> { ZombieBulletsId, Vertebraek47Id, SkullSpitterId });
            SynergyBuilder.CreateSynergy("Armored Support", new List<int> { ItemBuilder.ItemIds["frailheart"] }, new List<int> { BionicLegId, LaserSightId, ShockRoundsId, NanomachinesId });
            SynergyBuilder.CreateSynergy("I am YesEngine", new List<int> { ItemBuilder.ItemIds["consolecontroller"] }, new List<int> { GungineId, AlienEngineId });
            SynergyBuilder.CreateSynergy("Stone x2", new List<int>() { ItemBuilder.ItemIds["catapult"], SlingId });
            SynergyBuilder.CreateSynergy("Super Launch", new List<int>() { ItemBuilder.ItemIds["catapult"], ItemBuilder.ItemIds["launcher"] }, activeWhenGunsUnequipped: false, statModifiers: new()
            {
                StatModifier.Create(PlayerStats.StatType.ProjectileSpeed, StatModifier.ModifyMethod.MULTIPLICATIVE, 3f),
                StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.MULTIPLICATIVE, 2f)
            });
            SynergyBuilder.CreateSynergy("static readonly", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { BookOfChestAnatomyId, MagazineRackId, GungeonBlueprintId });
            SynergyBuilder.CreateSynergy("static void", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { VoidCoreCannonId, VoidShotgunId, VoidCoreAssaultRifleId, VoidMarshalId });
            SynergyBuilder.CreateSynergy("private static", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { GreyMauserId, ThePredatorId, SmokeBombId, BoxId, RingOfEtherealFormId });
            SynergyBuilder.CreateSynergy("Infinite Mirror", new List<int>() { ItemBuilder.ItemIds["mirrorbullet"], ItemBuilder.ItemIds["truthmirror"] });
            SynergyBuilder.CreateSynergy("Shoulders", new List<int>() { ItemBuilder.ItemIds["club"] }, new List<int>() { LichyTriggerFingerId, GunknightGauntletId, RobotsLeftHandId, MegahandId, FlameHandId, LaserSightId });
            SynergyBuilder.CreateSynergy("Cacti Club II", new List<int>() { ItemBuilder.ItemIds["club"] }, new List<int>() { Plus1BulletsId, AmuletOfThePitLordId, UtilityBeltId, DuctTapeId, CactusId, BroccoliId });
            SynergyBuilder.CreateSynergy("Fully Unlocked", new List<int>() { ItemBuilder.ItemIds["bosschest"], ShelletonKeyId });
            SynergyBuilder.CreateSynergy("while(true) { }", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            SynergyBuilder.CreateSynergy("NullReferenceException", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { DragunfireId, FlameHandId, PitchforkId, DemonHeadId, PhoenixId, HotLeadId, RingOfFireResistanceId });
            SynergyBuilder.CreateSynergy("Mono.dll has caused an Access Violation", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { 250, 332, 108, 234 });
            SynergyBuilder.CreateSynergy("Frogs are Friends", new List<int>() { ItemBuilder.ItemIds["frogun"], ReallySpecialLuteId });
            SynergyBuilder.CreateSynergy("Revenge", new List<int>() { ItemBuilder.ItemIds["frogun"], FaceMelterId });
            SynergyBuilder.CreateSynergy("Gun and Bullets", new List<int>() { ItemBuilder.ItemIds["lichgun"], LichsEyeBulletsId }, ignoreLichsEyeBullets: true);
            SynergyBuilder.CreateSynergy("Flatter Flat Bullets", new List<int>() { ItemBuilder.ItemIds["flatbullets"], 815 }, ignoreLichsEyeBullets: true);

            // add synergy processors
            SynergyBuilder.SetupDualWieldSynergy("Rotato Potato", ItemBuilder.Guns["revolvever"], ItemBuilder.Guns["akpi"]);
            SynergyBuilder.AddHoveringGunSynergyProcessor(ItemBuilder.Guns["frogun"], "Frogs are Friends", 506, false, null, HoveringGunController.HoverPosition.CIRCULATE,
                HoveringGunController.AimType.PLAYER_AIM, HoveringGunController.FireType.ON_FIRED_GUN, 0.5f, -1f, false, "", "", "", HoveringGunSynergyProcessor.TriggerStyle.CONSTANT, 1, -1f, false, 0f);
            SynergyBuilder.AddHoveringGunSynergyProcessor(ItemBuilder.Guns["frogun"], "Revenge", 149, false, null, HoveringGunController.HoverPosition.CIRCULATE,
                HoveringGunController.AimType.PLAYER_AIM, HoveringGunController.FireType.ON_RELOAD, 0.2f, 0f, false, "", "", "", HoveringGunSynergyProcessor.TriggerStyle.CONSTANT, 1, -1f, false, 0f);

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
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.BATTERY_POWERED, ItemBuilder.ItemIds["energydrink"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.LOTUS_BLOOM, ItemBuilder.ItemIds["energydrink"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.MACHINE_PISTOL, ItemBuilder.ItemIds["energydrink"]);
            SynergyBuilder.AddItemToSynergy(CustomSynergyType.LASER_THOMPSON, ItemBuilder.ItemIds["energydrink"]);

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
