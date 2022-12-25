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
            List<int> upgradeGang = new()
            {
                Plus1BulletsId,
                AmuletOfThePitLordId,
            };

            //build synergies
            CreateSynergy("Fufufufufu", new() { ItemIds["woodentoken"] }, new() { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemIds["ratwhistle"] });
            CreateSynergy("Double the Wish!", new() { ItemIds["wishorb"], LifeOrbId });
            CreateSynergy("Wish of Power", new() { ItemIds["wishorb"], SprunId });
            CreateSynergy("Wish of Reflection", new() { ItemIds["wishorb"], RollingEyeId });
            CreateSynergy("25% OFF ON ALL IN-GAME PURCHASES!", new() { ItemIds["paytowin"], MicrotransactionGunId });
            CreateSynergy("Super Blasts", new() { ItemIds["bombammolet"], GoldAmmoletId });
            CreateSynergy("Stunning Blasts", new() { ItemIds["bombammolet"], LodestoneAmmoletId });
            CreateSynergy("Random Blasts", new() { ItemIds["bombammolet"], ChaosAmmoletId });
            CreateSynergy("Poison Blasts", new() { ItemIds["bombammolet"], UraniumAmmoletId });
            CreateSynergy("Hot Blasts", new() { ItemIds["bombammolet"], CopperAmmoletId });
            CreateSynergy("Freezing Blasts", new() { ItemIds["bombammolet"], FrostAmmoletId });
            CreateSynergy("v3.0 The Massive Update", new() { ItemIds["boxofstuff"] }, new() { UtilityBeltId, AmmoBeltId, BackpackId, BottleId, BoxId, MailboxId, BriefcaseOfCashId, ResourcefulSackId });
            CreateSynergy("Also click that bell", new() { ItemIds["subscribebutton"] }, new() { RocketPoweredBulletsId, AgedBellId, ItemIds["glassbell"] });
            CreateSynergy("Also you can leave a comment", new() { ItemIds["subscribebutton"], MailboxId }, null, false);
            CreateSynergy("Chill", new() { ItemIds["ratwhistle"] }, new() { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            CreateSynergy("All At Once", new() { ItemIds["ratwhistle"] }, new() { ElimentalerId, RatBootsId, PartiallyEatenCheeseId, ResourcefulSackId, RingOfTheResourcefulRatId, ItemIds["woodentoken"] });
            CreateSynergy("Shadow Mirror", new() { ItemIds["truthmirror"], ShadowCloneId });
            CreateSynergy("Blessed Mirror", new() { ItemIds["truthmirror"], SilverBulletsId });
            CreateSynergy("Just Your Normal Luck", new() { ItemIds["badluckclover"], SevenLeafCloverId });
            CreateSynergy("Somehow... Luckier?", new() { ItemIds["badluckclover"], ItemIds["truthmirror"] });
            CreateSynergy("why do you keep crashing", new() { ItemIds["binary_gun"], MagnumId });
            CreateSynergy("BURN! BUUURRRNNNN!!!", new() { ItemIds["greencandle"] }, new() { GungeonPepperId, RingOfFireResistanceId, HotLeadId });
            CreateSynergy("Ring It Twice", new() { ItemIds["glassbell"], AgedBellId });
            CreateSynergy("Celestial Rhythm", new() { ItemIds["shooting_star"], CrescentCrossbowId });
            CreateSynergy("Wishing Star", new() { ItemIds["shooting_star"], ItemIds["wishorb"] });
            CreateSynergy("The Initial Idea", new() { ItemIds["shooting_star"], ItemIds["asteroidbelt"] }, null, false, new List<StatModifier> { StatModifier.Create(PlayerStats.StatType.Damage,
                StatModifier.ModifyMethod.MULTIPLICATIVE, 2f) });
            CreateSynergy("Bouncy Throws", new() { ItemIds["butter"], BouncyBulletsId });
            CreateSynergy("Piercing Throws", new() { ItemIds["butter"], GhostBulletsId });
            CreateSynergy("Homing Boomerang Throws", new() { ItemIds["butter"] }, new() { HomingBulletsId, CrutchId });
            CreateSynergy("Take It Before They Notice", new() { ItemIds["legitcoupon"] }, new() { ChaffGrenadeId, AgedBellId, ExplosiveDecoyId, DecoyId, SmokeBombId, BoxId, GrapplingHookId, RingOfEtherealFormId, CharmHornId, ThePredatorId, GreyMauserId });
            CreateSynergy("The zombies are coming", new() { ItemIds["gravediggershovel"] }, new() { ZombieBulletsId, Vertebraek47Id, SkullSpitterId });
            CreateSynergy("Armored Support", new() { ItemIds["frailheart"] }, new() { BionicLegId, LaserSightId, ShockRoundsId, NanomachinesId });
            CreateSynergy("I am YesEngine", new() { ItemIds["consolecontroller"] }, new() { GungineId, AlienEngineId });
            CreateSynergy("Stone x2", new() { ItemIds["catapult"], SlingId });
            CreateSynergy("Super Launch", new() { ItemIds["catapult"], ItemIds["launcher"] }, activeWhenGunsUnequipped: false, statModifiers: new()
            {
                StatModifier.Create(PlayerStats.StatType.ProjectileSpeed, StatModifier.ModifyMethod.MULTIPLICATIVE, 3f),
                StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.MULTIPLICATIVE, 2f)
            });
            CreateSynergy("static readonly", new() { ItemIds["staticroll"] }, new() { BookOfChestAnatomyId, MagazineRackId, GungeonBlueprintId, ItemIds["guardianbook"] });
            CreateSynergy("static void", new() { ItemIds["staticroll"] }, new() { VoidCoreCannonId, VoidShotgunId, VoidCoreAssaultRifleId, VoidMarshalId });
            CreateSynergy("private static", new() { ItemIds["staticroll"] }, new() { GreyMauserId, ThePredatorId, SmokeBombId, BoxId, RingOfEtherealFormId });
            CreateSynergy("Infinite Mirror", new() { ItemIds["mirrorbullet"], ItemIds["truthmirror"] });
            CreateSynergy("Shoulders", new() { ItemIds["club"] }, new() { LichyTriggerFingerId, GunknightGauntletId, RobotsLeftHandId, MegahandId, FlameHandId, LaserSightId });
            CreateSynergy("Cacti Club II", new() { ItemIds["club"] }, new() { Plus1BulletsId, AmuletOfThePitLordId, UtilityBeltId, DuctTapeId, CactusId, BroccoliId });
            CreateSynergy("Fully Unlocked", new() { ItemIds["bosschest"], ShelletonKeyId });
            CreateSynergy("while(true) { }", new() { ItemIds["nank"] }, new() { IceBombId, FrostGiantId, FrostBulletsId, FrostAmmoletId, Cold45Id, PolarisId, HeartOfIceId, IceCubeId, IceBreakerId, FreezeRayId });
            CreateSynergy("NullReferenceException", new() { ItemIds["nank"] }, new() { DragunfireId, FlameHandId, PitchforkId, DemonHeadId, PhoenixId, HotLeadId, RingOfFireResistanceId });
            CreateSynergy("Mono.dll has caused an Access Violation", new() { ItemIds["nank"] }, new() { GrapplingHookId, LilBomberId, BombId, IbombCompanionAppId });
            CreateSynergy("Frogs are Friends", new() { ItemIds["frogun"], ReallySpecialLuteId });
            CreateSynergy("Revenge", new() { ItemIds["frogun"], FaceMelterId });
            CreateSynergy("Gun and Bullets", new() { ItemIds["lichgun"], LichsEyeBulletsId }, ignoreLichsEyeBullets: true);
            CreateSynergy("Flatter Flat Bullets", new() { ItemIds["flatbullets"] }, new(upgradeGang));
            CreateSynergy("SpecialUtils", new() { ItemIds["boxofstuff"] }, new()
            {
                PeaShooterId,
                ThirtyEightSpecialId,
                DerringerId,
                MakarovId,
                DuelingPistolId,
                TurboGunId,
                ScreecherId,
                NailGunId,
                WoodBeamId,
                TheKilnId,
                BuzzkillId,
                TearJerkerId,
                QuadLaserId,
                CactusId,
                KlobbeId,
                TrashcannonId
            });
            CreateSynergy("QoL", new() { ItemIds["boxofstuff"] }, new()
            {
                ArmorSynthesizerId,
                AmmoSynthesizerId,
                ExplosiveRoundsId,
                SprunId,
                ItemIds["mirrorbullet"],
                HighDragunfireId
            });
            CreateSynergy("Hotter Kiln", new() { ItemIds["hotcoal"], TheKilnId }, activeWhenGunsUnequipped: false);
            CreateSynergy("When the shotgun is sus!", new() { ItemIds["sus_shotgun"] }, new() { BackpackId, MoonscraperId, DungeonEagleId, JetpackId, BlastHelmetId, KnifeShieldId });
            CreateSynergy("Rainbuddy", new() { ItemIds["sus_shotgun"] }, new() { BabyGoodShelletonId, BabyGoodMimicId, ChickenFluteId, OwlId, R2g2Id }, false, new()
            {
                CreateStatMod(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.25f),
                CreateStatMod(PlayerStats.StatType.PlayerBulletScale, StatModifier.ModifyMethod.ADDITIVE, 0.25f)
            });
            CreateSynergy("Bow-Bow", new() { ItemIds["sus_shotgun"] }, new() { BowId, CharmedBowId, ShotbowId, CrescentCrossbowId, StickyCrossbowId, TripleCrossbowId, GunbowId }, false, new()
            {
                CreateStatMod(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.125f, false),
                CreateStatMod(PlayerStats.StatType.PlayerBulletScale, StatModifier.ModifyMethod.ADDITIVE, 0.125f, false),
                CreateStatMod(PlayerStats.StatType.ChargeAmountMultiplier, StatModifier.ModifyMethod.ADDITIVE, 0.25f, false),
            });
            CreateSynergy("RAINBOW!!!", new() { ItemIds["sus_shotgun"] }, new() { UnicornHornId, StuffedStarId, ItemIds["totem"], ChaosBulletsId, ItemIds["chaos"], ChaosAmmoletId }, false, new()
            {
                CreateStatMod(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.25f),
                CreateStatMod(PlayerStats.StatType.PlayerBulletScale, StatModifier.ModifyMethod.ADDITIVE, 0.25f),
            });
            CreateSynergy("Bane Gun", new() { ItemIds["red_gun"] }, new()
            {
                TheMembraneId,
                PlaguePistolId,
                PlungerId,
                PoxcannonId,
                GammaRayId,
                RattlerId,
                ShotgunFullOfHateId,
                ShotgrubId,
                PoisonVialId,
                IrradiatedLeadId,
                UraniumAmmoletId,
                MonsterBloodId,
                BugBootsId,
                GasMaskId,
                HazmatSuitId
            });
            CreateSynergy("Vampiric Gun", new() { ItemIds["red_gun"] }, new()
            {
                BloodBroochId,
                PinkGuonStoneId,
                ItemIds["heartpiece"],
                AntibodyId,
                LifeOrbId,
                MutationId,
                SuperMeatGunId,
                BloodiedScarfId
            });
            CreateSynergy("Necromantic Gun", new() { ItemIds["red_gun"] }, new()
            {
                Vertebraek47Id,
                SkullSpitterId,
                GunslingersAshesId,
                MeltedRockId,
                GhostBulletsId,
                ZombieBulletsId,
                ProtonBackpackId,
                ShelletonKeyId,
                BabyGoodShelletonId,
                ShellegunId
            });
            CreateSynergy("Zealous Gun", new() { ItemIds["red_gun"] }, new()
            {
                EyeOfTheBeholsterId,
                RollingEyeId,
                BloodyEyeId,
                TableTechSightId,
                GungeonBlueprintId,
                SunglassesId,
                MimicToothNecklaceId,
                CharmingRoundsId,
                LamentConfigurumId
            });
            CreateSynergy("Merciless Gun", new() { ItemIds["red_gun"] }, new()
            {
                VorpalGunId,
                VorpalBulletsId,
                Bloody9MmId,
                TableTechRageId,
                AngryBulletsId,
                CogOfBattleId,
                ShadowBulletsId,
                GoldAmmoletId,
                WoodBeamId,
                HyperLightBlasterId
            });
            CreateSynergy("Godly Gun", new() { ItemIds["red_gun"] }, new()
            {
                RubensteinsMonsterId,
                HighDragunfireId,
                FinishedGunId,
                CloneId
            }, ignoreLichsEyeBullets: true);
            CreateSynergy("Might of the Devout", new() { ItemIds["red_gun"] }, new()
            {
                Plus1BulletsId,
                BroccoliId,
                RiddleOfLeadId,
                RailgunId,
                LichyTriggerFingerId,
                AmuletOfThePitLordId,
                MagicSweetId
            }, false, new()
            {
                CreateStatMod(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.MULTIPLICATIVE, 1.25f)
            });
            CreateSynergy("Hearts of the Faithful", new() { ItemIds["red_gun"] }, new()
            {
                HeartHolsterId,
                HeartLunchboxId,
                HeartLocketId,
                HeartBottleId,
                HeartPurseId,
                HeartOfIceId
            }, true, new()
            {
                CreateStatMod(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, 0.5f)
            });
            CreateSynergy("Cheap Rituals", new() { ItemIds["red_gun"] }, new()
            {
                ItemIds["legitcoupon"],
                TurkeyId,
                GildedBulletsId,
                MicrotransactionGunId,
                ItemIds["paytowin"],
                IronCoinId,
                SmileysRevolverId,
                RingOfMiserlyProtectionId,
                EscapeRopeId
            });
            CreateSynergy("Dynamite", new() { ItemIds["shredder"] }, new()
            {
                ExplosiveRoundsId,
                Com4nd0Id,
                RpgId,
                YariLauncherId,
                BombId,
                GrenadeLauncherId,
                ClusterMineId,
                ProximityMineId,
                C4Id,
                AirStrikeId
            });
            CreateSynergy("Plastic Explosive", new() { ItemIds["shredder"] }, new()
            {
                FatBulletsId,
                BigShotgunId,
                HeavyBulletsId,
                BlastHelmetId,
                SeriousCannonId,
                RailgunId,
                RcRocketId,
                VulcanCannonId,
                BigBoyId,
                BigIronId
            });
            CreateSynergy("Shredder II", new() { ItemIds["shredder"] }, new() { Plus1BulletsId, AmuletOfThePitLordId, UtilityBeltId, DuctTapeId, BroccoliId });
            CreateSynergy("Good Game Design", new() { ItemIds["infinitycrystal"], MicrotransactionGunId });
            CreateSynergy("Enter the Gungeon 2", new() { ItemIds["dlchest"] }, new()
            {
                BookOfChestAnatomyId,
                RingOfChestFriendshipId,
                ChestTeleporterId,
                RingOfChestVampirismId,
                RingOfMimicFriendshipId,
                MicrotransactionGunId
            });
            CreateSynergy("50% OFF ON ALL DLCHESTS!", new() { ItemIds["dlchest"], ItemIds["paytowin"] });
            CreateSynergy("True Knight", new() { ItemIds["marinehelmet"] }, new()
            {
                GunknightArmorId,
                GunknightGauntletId,
                GunknightGreavesId,
                GunknightHelmetId,
                OldKnightsHelmId,
                OldKnightsShieldId,
                BlastHelmetId,
                ArmorOfThornsId,
                HeavyBootsId,
                KnightsGunId
            });
            CreateSynergy("A Little More", new() { ItemIds["totem"] }, new()
            {
                BulletIdolId,
                SprunId,
                CogOfBattleId
            });
            CreateSynergy("Wrath of the Keys", new() { ItemIds["plushie"] }, new()
            {
                AngryBulletsId,
                HomingBulletsId
            });
            CreateSynergy("Keybag", new() { ItemIds["plushie"] }, new()
            {
                LootBagId,
                BackpackId,
                ResourcefulSackId,
                ItemIds["magicbag"]
            });
            CreateSynergy("Ring of Lock Friendship", new() { ItemIds["plushie"] }, new()
            {
                RingOfChestFriendshipId,
                RingOfChestVampirismId,
                RingOfEtherealFormId,
                RingOfFireResistanceId,
                RingOfMiserlyProtectionId,
                RingOfMimicFriendshipId
            });
            CreateSynergy("Rusty Shovel", new() { ItemIds["rustybullets"], ItemIds["gravediggershovel"] });
            CreateSynergy("Rusty Iron", new() { ItemIds["rustybullets"], BigIronId }, activeWhenGunsUnequipped: false);
            CreateSynergy("Rusty Coin", new() { ItemIds["rustybullets"], IronCoinId });
            CreateSynergy("Snake Eyes", new() { ItemIds["woodendice"], SnakemakerId });
            CreateSynergy("Gambling Addiction", new() { ItemIds["woodendice"], ChanceBulletsId });
            CreateSynergy("Midnight", new() { ItemIds["woodendice"], SixthChamberId });
            CreateSynergy("Recycling at its Finest", new() { ItemIds["rustyammobox"] }, new List<int>() { AmmoSynthesizerId, AmmoBeltId, UtilityBeltId, HipHolsterId });
            CreateSynergy("Magnum Mox", new() { ItemIds["magnificus"], MagnumId });
            CreateSynergy("The Goo Mage", new() { ItemIds["magnificus"] }, new() { PoisonVialId });
            CreateSynergy("The Pike Mage", new() { ItemIds["magnificus"] }, new() { PitchforkId });
            CreateSynergy("The Lonely Mage", new() { ItemIds["magnificus"] }, new() { RingOfEtherealFormId });
            CreateSynergy("Wizard Mentor", new() { ItemIds["magnificus"] }, new() { ChaosBulletsId });

            // add synergy processors
            SetupDualWieldSynergy("Rotato Potato", Guns["revolvever"], Guns["akpi"]);
            Guns["frogun"].AddHoveringGunSynergyProcessor("Frogs are Friends", 506, false, null, HoveringGunController.HoverPosition.CIRCULATE,
                HoveringGunController.AimType.PLAYER_AIM, HoveringGunController.FireType.ON_FIRED_GUN, 0.5f, -1f, false, "", "", "", HoveringGunSynergyProcessor.TriggerStyle.CONSTANT, 1, -1f, false, 0f);
            Guns["frogun"].AddHoveringGunSynergyProcessor("Revenge", 149, false, null, HoveringGunController.HoverPosition.CIRCULATE,
                HoveringGunController.AimType.PLAYER_AIM, HoveringGunController.FireType.ON_RELOAD, 0.2f, 0f, false, "", "", "", HoveringGunSynergyProcessor.TriggerStyle.CONSTANT, 1, -1f, false, 0f);
            var goodgamedesign = MicrotransactionGunObject.AddComponent<MiscSynergyProcessor>();
            goodgamedesign.synergy = "Good Game Design";
            goodgamedesign.toggleFundsToShoot = true;
            MicrotransactionGunObject.ammo = MicrotransactionGunObject.maxAmmo;

            //add this long synergy
            List<StatModifier> sm = new();
            if(DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                sm.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, -3f));
                sm.Add(StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.5f));
            }
            else
            {
                sm.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, 1f));
            }
            CreateSynergy("I Hate Mondays", new() { ItemIds["calendar"] }, new() { ShotgunFullOfHateId, TableTechRageId }, statModifiers: sm);

            //add new items to existing synergies
            AddItemToSynergy(CustomSynergyType.PITCHPERFECT, ItemIds["greencandle"]);
            AddItemToSynergy(CustomSynergyType.LOADED_DICE, ItemIds["greencandle"]);
            AddItemToSynergy(CustomSynergyType.RELODESTAR, ItemIds["bombammolet"]);
            AddItemToSynergy(CustomSynergyType.MINOR_BLANKABLES, ItemIds["bombammolet"]);
            AddItemToSynergy(CustomSynergyType.RELODESTAR, ItemIds["redammolet"]);
            AddItemToSynergy(CustomSynergyType.MINOR_BLANKABLES, ItemIds["redammolet"]);
            AddItemToSynergy(CustomSynergyType.BATTERY_POWERED, ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.LOTUS_BLOOM, ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.MACHINE_PISTOL, ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.LASER_THOMPSON, ItemIds["energydrink"]);
            AddItemToSynergy(CustomSynergyType.RECYCLING, ItemIds["rustyammobox"]);

            //add synergy components
            PickupObjectDatabase.GetById(476).AddComponent<MicrotransactionDiscountSynergyController>().SynergyToCheck = "25% OFF ON ALL IN-GAME PURCHASES!";

            //add synergy hooks
            new Hook(
                typeof(Gun).GetMethod("DecrementAmmoCost", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MicrotransactionDiscountSynergyController).GetMethod("MaybeReduceCost")
            );
        }
    }
}
