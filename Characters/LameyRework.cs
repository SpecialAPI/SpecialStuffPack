using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    public static class LameyRework
    {
        public static void Init()
        {
            //fix up lamey gun
            LameyGunObject.quality = PickupObject.ItemQuality.SPECIAL;
            LameyGunObject.encounterTrackable.journalData.PrimaryDisplayName = "#SPAPI_LAMEY_GUN_NAME";
            LameyGunObject.encounterTrackable.journalData.NotificationPanelDescription = "#SPAPI_LAMEY_GUN_SHORTDESC";
            LameyGunObject.encounterTrackable.journalData.AmmonomiconFullEntry = "#SPAPI_LAMEY_GUN_LONGDESC";
            LameyGunObject.encounterTrackable.journalData.AmmonomiconSprite = "spapi_lamey_gun_idle_001";
            LameyGunObject.encounterTrackable.journalData.SuppressInAmmonomicon = false;
            EncounterDatabase.GetEntry(LameyGunObject.encounterTrackable.EncounterGuid).journalData.PrimaryDisplayName = "#SPAPI_LAMEY_GUN_NAME";
            EncounterDatabase.GetEntry(LameyGunObject.encounterTrackable.EncounterGuid).journalData.NotificationPanelDescription = "#SPAPI_LAMEY_GUN_SHORTDESC";
            EncounterDatabase.GetEntry(LameyGunObject.encounterTrackable.EncounterGuid).journalData.AmmonomiconFullEntry = "#SPAPI_LAMEY_GUN_LONGDESC";
            EncounterDatabase.GetEntry(LameyGunObject.encounterTrackable.EncounterGuid).journalData.AmmonomiconSprite = "spapi_lamey_gun_idle_001";
            EncounterDatabase.GetEntry(LameyGunObject.encounterTrackable.EncounterGuid).journalData.SuppressInAmmonomicon = false;
            AddSpriteToCollection("spapi_lamey_gun_idle_001", AmmonomiconController.ForceInstance.EncounterIconCollection);
            ETGMod.Databases.Strings.Items.Set("#SPAPI_LAMEY_GUN_NAME", "Lamey Gun");
            ETGMod.Databases.Strings.Items.Set("#SPAPI_LAMEY_GUN_SHORTDESC", "Detective's Weapon");
            ETGMod.Databases.Strings.Items.Set("#SPAPI_LAMEY_GUN_LONGDESC", "Lamey's trusty gun, its perfect accuracy and high range allows to easily hit enemies even from long distances.");

            //fix up lamey
            var lamey = (GameObject)BraveResources.Load("PlayerLamey");
            SpecialAssets.assets.Add(lamey);
            var lameyPlayer = lamey.GetComponent<PlayerController>();
            lameyPlayer.startingGunIds = new() { LameyGunId };
            lameyPlayer.startingActiveItemIds = new() { ItemIds["disguisehat"] };
            lameyPlayer.startingPassiveItemIds = new() { ItemIds["magnifyingglass"] };
            lameyPlayer.uiPortraitName = AddToAtlas("lamey_portrait");
            lameyPlayer.characterIdentity = PlayableCharactersE.Lamey;
            lameyPlayer.characterAudioSpeechTag = "convict";
            var library = lameyPlayer.transform.Find("PlayerSprite").GetComponent<tk2dSpriteAnimator>().Library;
            library.clips = library.clips.AddToArray(new(library.GetClipByName("idle_twohands")) { name = "select_idle" });
            library.clips = library.clips.AddToArray(new(library.GetClipByName("item_get")) { name = "select_choose" });
            library.GetClipByName("jetpack_front_right").name = "jetpack_right";
            library.GetClipByName("jetpack_back_right").name = "jetpack_right_bw";
            library.GetClipByName("jetpack_front_right_hand").name = "jetpack_right_hand";
            Dictionary<string, Vector2> jetpackOffsets = new()
            {
                { "jetpack_down", new(0.6875f, 0.875f) },
                { "jetpack_down_hand", new(0.6875f, 0.875f) },
                { "jetpack_right", new(0.3125f, 0.6875f) },
                { "jetpack_right_hand", new(0.3125f, 0.6875f) },
                { "jetpack_right_bw", new(0.375f, 0.5625f) },
                { "jetpack_up", new(0.6875f, 0.625f) }
            };
            foreach(var kvp in jetpackOffsets)
            {
                var clippy = library.GetClipByName(kvp.Key);
                for(int i = 0; i < clippy.frames.Length; i++)
                {
                    clippy.frames[i].spriteCollection.SetAttachPoints(clippy.frames[i].spriteId, new tk2dSpriteDefinition.AttachPoint[] { new() { name = "jetpack", position = kvp.Value, angle = 0f } });
                }
            }
            List<string> dog = new()
            {
                "dodge",
                "dodge_bw",
                "dodge_left",
                "dodge_left_bw",
            };
            foreach(var d in dog)
            {
                var clippy = library.GetClipByName(d);
                if(clippy.frames.Length > 0)
                {
                    clippy.frames[0].triggerEvent = true;
                    clippy.frames[0].eventAudio = "Play_Leap";
                    var ground = clippy.frames.Where(x => x.groundedFrame && x.invulnerableFrame).FirstOrDefault();
                    if(ground != null)
                    {
                        ground.triggerEvent = true;
                        ground.eventAudio = "Play_Roll";
                    }
                }
            }
            List<string> walk = new()
            {
                "run_right",
                "run_right_bw",
                "run_down",
                "run_up",
                "run_right_hand",
                "run_down_hand",
                "run_up_hand",
                "run_down_twohands",
                "run_right_twohands",
                "run_up_twohands",
                "run_right_bw_twohands"
            };
            foreach (var w in walk)
            {
                var clippy = library.GetClipByName(w);
                if (clippy.frames.Length > 0)
                {
                    clippy.frames[clippy.frames.Length - 1].triggerEvent = true;
                    clippy.frames[clippy.frames.Length - 1].eventAudio = "Play_FS";
                    clippy.frames[Mathf.RoundToInt(clippy.frames.Length / 2f) - 1].triggerEvent = true;
                    clippy.frames[Mathf.RoundToInt(clippy.frames.Length / 2f) - 1].eventAudio = "Play_FS";
                }
            }
            lamey.AddComponent<DefaultLootMods>().lootMods = new()
            {
                new()
                {
                    AssociatedPickupId = GreyMauserId,
                    DropRateMultiplier = 2f
                },
                new()
                {
                    AssociatedPickupId = ThePredatorId,
                    DropRateMultiplier = 2f
                },
                new()
                {
                    AssociatedPickupId = BoxId,
                    DropRateMultiplier = 2f
                },
                new()
                {
                    AssociatedPickupId = VorpalGunId,
                    DropRateMultiplier = 2f
                },
                new()
                {
                    AssociatedPickupId = SuperSpaceTurtleId,
                    DropRateMultiplier = 2f
                }
            };
            ETGMod.Databases.Strings.Core.Set($"#PLAYER_NAME_{PlayableCharactersE.Lamey.ToString().ToUpperInvariant()}", "Lamey");
            ETGMod.Databases.Strings.Core.Set($"#PLAYER_NICK_{PlayableCharactersE.Lamey.ToString().ToUpperInvariant()}", "detective");

            //add lamey to breach
            lameyPlayer.AddToBreach("lameybreachflag", new(30.5f, 28.75f), new(), new(8, 1, 10, 4), new(7, 1, 12, 19), new(-1, 0), "lameyoverheadpanel", "The Detective (WIP)", "lamey", 5, "lamey_items");
        }
    }
}
