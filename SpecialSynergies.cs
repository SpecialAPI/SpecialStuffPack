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
            CreateSynergy("Fufufufufu", new List<int> { ItemBuilder.ItemIds["woodentoken"] }, new List<int> { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemBuilder.ItemIds["ratwhistle"] });
            CreateSynergy("Double the Wish!", new List<int> { ItemBuilder.ItemIds["wishorb"], LifeOrbId });
            CreateSynergy("Wish of Power", new List<int> { ItemBuilder.ItemIds["wishorb"], SprunId });
            CreateSynergy("Wish of Reflection", new List<int> { ItemBuilder.ItemIds["wishorb"], RollingEyeId });
            CreateSynergy("50% OFF ON ALL IN-GAME PURCHASES!", new List<int> { ItemBuilder.ItemIds["paytowin"], MicrotransactionGunId });
            CreateSynergy("Super Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], GoldAmmoletId });
            CreateSynergy("Stunning Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], LodestoneAmmoletId });
            CreateSynergy("Random Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], ChaosAmmoletId });
            CreateSynergy("Poison Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], UraniumAmmoletId });
            CreateSynergy("Hot Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], CopperAmmoletId });
            CreateSynergy("Freezing Blasts", new List<int> { ItemBuilder.ItemIds["bombammolet"], FrostAmmoletId });
            CreateSynergy("v3.0 The Massive Update", new List<int> { ItemBuilder.ItemIds["boxofstuff"] }, new List<int> { UtilityBeltId, AmmoBeltId, BackpackId, BottleId, BoxId, MailboxId, BriefcaseOfCashId, ResourcefulSackId });
            CreateSynergy("Also click that bell", new List<int> { ItemBuilder.ItemIds["subscribebutton"] }, new List<int> { RocketPoweredBulletsId, AgedBellId, ItemBuilder.ItemIds["glassbell"] });
            CreateSynergy("Also you can leave a comment", new List<int> { ItemBuilder.ItemIds["subscribebutton"], MailboxId }, null, false);
            CreateSynergy("Chill", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            CreateSynergy("All At Once", new List<int> { ItemBuilder.ItemIds["ratwhistle"] }, new List<int> { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemBuilder.ItemIds["woodentoken"] });
            CreateSynergy("Shadow Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], ShadowCloneId });
            CreateSynergy("Blessed Mirror", new List<int> { ItemBuilder.ItemIds["truthmirror"], SilverBulletsId });
            CreateSynergy("Just Your Normal Luck", new List<int> { ItemBuilder.ItemIds["badluckclover"], SevenLeafCloverId });
            CreateSynergy("Somehow... Luckier?", new List<int> { ItemBuilder.ItemIds["badluckclover"], ItemBuilder.ItemIds["truthmirror"] });
            CreateSynergy("why do you keep crashing", new List<int> { ItemBuilder.ItemIds["binary_gun"], MagnumId });
            CreateSynergy("BURN! BUUURRRNNNN!!!", new List<int> { ItemBuilder.ItemIds["greencandle"] }, new List<int> { GungeonPepperId, RingOfFireResistanceId, HotLeadId });
            CreateSynergy("Ring It Twice", new List<int> { ItemBuilder.ItemIds["glassbell"], AgedBellId });
            CreateSynergy("Celestial Rhythm", new List<int> { ItemBuilder.ItemIds["shooting_star"], CrescentCrossbowId });
            CreateSynergy("Wishing Star", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["wishorb"] });
            CreateSynergy("The Initial Idea", new List<int> { ItemBuilder.ItemIds["shooting_star"], ItemBuilder.ItemIds["asteroidbelt"] }, null, false, new List<StatModifier> { StatModifier.Create(PlayerStats.StatType.Damage, 
                StatModifier.ModifyMethod.MULTIPLICATIVE, 2f) });
            CreateSynergy("Bouncy Throws", new List<int> { ItemBuilder.ItemIds["butter"], BouncyBulletsId });
            CreateSynergy("Piercing Throws", new List<int> { ItemBuilder.ItemIds["butter"], GhostBulletsId });
            CreateSynergy("Homing Boomerang Throws", new List<int> { ItemBuilder.ItemIds["butter"] }, new List<int> { HomingBulletsId, CrutchId });
            CreateSynergy("Take It Before They Notice", new List<int> { ItemBuilder.ItemIds["legitcoupon"] }, new List<int> { ChaffGrenadeId, AgedBellId, ExplosiveDecoyId, DecoyId, SmokeBombId, BoxId, GrapplingHookId, RingOfEtherealFormId, CharmHornId, ThePredatorId, GreyMauserId });
            CreateSynergy("The zombies are coming", new List<int> { ItemBuilder.ItemIds["gravediggershovel"] }, new List<int> { ZombieBulletsId, Vertebraek47Id, SkullSpitterId });
            CreateSynergy("Armored Support", new List<int> { ItemBuilder.ItemIds["frailheart"] }, new List<int> { BionicLegId, LaserSightId, ShockRoundsId, NanomachinesId });
            CreateSynergy("I am YesEngine", new List<int> { ItemBuilder.ItemIds["consolecontroller"] }, new List<int> { GungineId, AlienEngineId });
            CreateSynergy("Stone x2", new List<int>() { ItemBuilder.ItemIds["catapult"], SlingId });
            CreateSynergy("Super Launch", new List<int>() { ItemBuilder.ItemIds["catapult"], ItemBuilder.ItemIds["launcher"] }, activeWhenGunsUnequipped: false, statModifiers: new()
            {
                StatModifier.Create(PlayerStats.StatType.ProjectileSpeed, StatModifier.ModifyMethod.MULTIPLICATIVE, 3f),
                StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.MULTIPLICATIVE, 2f)
            });
            CreateSynergy("static readonly", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { BookOfChestAnatomyId, MagazineRackId, GungeonBlueprintId });
            CreateSynergy("static void", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { VoidCoreCannonId, VoidShotgunId, VoidCoreAssaultRifleId, VoidMarshalId });
            CreateSynergy("private static", new List<int>() { ItemBuilder.ItemIds["staticroll"] }, new() { GreyMauserId, ThePredatorId, SmokeBombId, BoxId, RingOfEtherealFormId });
            CreateSynergy("Infinite Mirror", new List<int>() { ItemBuilder.ItemIds["mirrorbullet"], ItemBuilder.ItemIds["truthmirror"] });
            CreateSynergy("Shoulders", new List<int>() { ItemBuilder.ItemIds["club"] }, new List<int>() { LichyTriggerFingerId, GunknightGauntletId, RobotsLeftHandId, MegahandId, FlameHandId, LaserSightId });
            CreateSynergy("Cacti Club II", new List<int>() { ItemBuilder.ItemIds["club"] }, new List<int>() { Plus1BulletsId, AmuletOfThePitLordId, UtilityBeltId, DuctTapeId, CactusId, BroccoliId });
            CreateSynergy("Fully Unlocked", new List<int>() { ItemBuilder.ItemIds["bosschest"], ShelletonKeyId });
            CreateSynergy("while(true) { }", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            CreateSynergy("NullReferenceException", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { DragunfireId, FlameHandId, PitchforkId, DemonHeadId, PhoenixId, HotLeadId, RingOfFireResistanceId });
            CreateSynergy("Mono.dll has caused an Access Violation", new List<int>() { ItemBuilder.ItemIds["nank"] }, new List<int>() { 250, 332, 108, 234 });
            CreateSynergy("Frogs are Friends", new List<int>() { ItemBuilder.ItemIds["frogun"], ReallySpecialLuteId });
            CreateSynergy("Revenge", new List<int>() { ItemBuilder.ItemIds["frogun"], FaceMelterId });
            CreateSynergy("Gun and Bullets", new List<int>() { ItemBuilder.ItemIds["lichgun"], LichsEyeBulletsId }, ignoreLichsEyeBullets: true);
            CreateSynergy("Flatter Flat Bullets", new List<int>() { ItemBuilder.ItemIds["flatbullets"], 815 }, ignoreLichsEyeBullets: true);

            // add synergy processors
            SetupDualWieldSynergy("Rotato Potato", ItemBuilder.Guns["revolvever"], ItemBuilder.Guns["akpi"]);
            Guns["frogun"].AddHoveringGunSynergyProcessor("Frogs are Friends", 506, false, null, HoveringGunController.HoverPosition.CIRCULATE,
                HoveringGunController.AimType.PLAYER_AIM, HoveringGunController.FireType.ON_FIRED_GUN, 0.5f, -1f, false, "", "", "", HoveringGunSynergyProcessor.TriggerStyle.CONSTANT, 1, -1f, false, 0f);
            Guns["frogun"].AddHoveringGunSynergyProcessor("Revenge", 149, false, null, HoveringGunController.HoverPosition.CIRCULATE,
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
            CreateSynergy("I Hate Mondays", new List<int> { ItemBuilder.ItemIds["calendar"] }, new List<int> { 143, 399 }, statModifiers: sm);

            //add new items to existing synergies
            AddItemToSynergy(CustomSynergyType.PITCHPERFECT, ItemBuilder.ItemIds["greencandle"]);
            AddItemToSynergy(CustomSynergyType.LOADED_DICE, ItemBuilder.ItemIds["greencandle"]);
            AddItemToSynergy(CustomSynergyType.RELODESTAR, ItemBuilder.ItemIds["bombammolet"]);
            AddItemToSynergy(CustomSynergyType.MINOR_BLANKABLES, ItemBuilder.ItemIds["bombammolet"]);
            AddItemToSynergy(CustomSynergyType.BATTERY_POWERED, ItemBuilder.ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.LOTUS_BLOOM, ItemBuilder.ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.MACHINE_PISTOL, ItemBuilder.ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.LASER_THOMPSON, ItemBuilder.ItemIds["energydrink"]);

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
