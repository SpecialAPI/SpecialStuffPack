using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialStuffPack.GungeonAPI;
using SpecialStuffPack.Components;
using SpecialStuffPack.Items;

namespace SpecialStuffPack.Placeables
{
    public static class SpecialPlaceables
    {
        public static void InitLockUnlockPedestal()
        {
            GameObject lockUnlockPedestal = AssetBundleManager.Load<GameObject>("placeables/lockunlockpedestal");
            GameObject pedestal = LoadHelper.LoadAssetFromAnywhere<GameObject>("boss_reward_pedestal");
            tk2dSpriteCollectionData coll = pedestal.GetComponent<tk2dBaseSprite>().Collection;
            tk2dSprite sprite = tk2dSprite.AddComponent(lockUnlockPedestal.transform.Find("Sprite").gameObject, coll, coll.GetSpriteIdByName("pedestal_gold_001"));
            tk2dSprite shadowSprite = tk2dSprite.AddComponent(lockUnlockPedestal.transform.Find("Shadow").gameObject, coll, coll.GetSpriteIdByName("pedestal_gun_shadow_002"));
            shadowSprite.HeightOffGround = 0f;
            sprite.HeightOffGround = -1f;
            sprite.AttachRenderer(shadowSprite);
            sprite.UpdateZDepth();
            SpeculativeRigidbody body = lockUnlockPedestal.AddComponent<SpeculativeRigidbody>();
            body.PixelColliders = new List<PixelCollider>
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.LowObstacle,
                    ManualOffsetX = 2,
                    ManualOffsetY = 2,
                    ManualWidth = 26,
                    ManualHeight = 20
                },
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.HighObstacle,
                    ManualOffsetX = 2,
                    ManualOffsetY = 13,
                    ManualWidth = 26,
                    ManualHeight = 20
                }
            };
            lockUnlockPedestal.transform.Find("Shadow").localPosition = new Vector3(0.125f, -0.0625f, -1.1f);
            lockUnlockPedestal.transform.Find("Sprite").localPosition = new Vector3(0.0625f, 0f, 0f);
            lockUnlockPedestal.transform.Find("Spawn").localPosition = new Vector3(0.8f, 1.5f, 0f);
            LockUnlockPedestal pedestalComponent = lockUnlockPedestal.AddComponent<LockUnlockPedestal>();
            pedestalComponent.itemId = GoldKey.GoldKeyId;
            pedestalComponent.spawnTransform = lockUnlockPedestal.transform.Find("Spawn");
            GameObject bookPage = AssetBundleManager.Load<GameObject>("placeables/bookpage");
            tk2dSprite pageSprite = tk2dSprite.AddComponent(bookPage, coll, SpriteBuilder.AddSpriteToCollection("placeablesprites/book_page_idle_001", coll, "Brave/LitTk2dCustomFalloffTintableTilted"));
            pageSprite.HeightOffGround = -1;
            pageSprite.UpdateZDepth();
            NoteDoer note = bookPage.AddComponent<NoteDoer>();
            note.noteBackgroundType = NoteDoer.NoteBackgroundType.NOTE;
            ETGMod.Databases.Strings.Core.Set("#MYSTERIOUS_BOOK_PAGE", "And then, he was cast away, his body and spirit separated, never to be seen again.\n\n      - Fall of the Lock Kingdom, Chapter 4");
            note.stringKey = "#MYSTERIOUS_BOOK_PAGE";
            note.useAdditionalStrings = false;
            note.additionalStrings = new string[0];
            note.isNormalNote = true;
            note.useItemsTable = false;
            note.textboxSpawnPoint = bookPage.transform.Find("SpawnPoint");
            note.DestroyedOnFinish = true;
            bookPage.AddComponent<OutlineAdder>().outlineColor = Color.black;
            pedestalComponent.bookPagePrefab = bookPage;
            RoomFactory.AddInjection(RoomFactory.BuildFromResource("SpecialStuffPack.Rooms.Abbey Super Secret.room").room, "Abbey Super Secret Room", new List<ProceduralFlowModifierData.FlowModifierPlacementType> { ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD }, 0f, new List<DungeonPrerequisite> {
                new DungeonPrerequisite { prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET, requireTileset = true, requiredTileset = GlobalDungeonData.ValidTilesets.CATHEDRALGEON } }, "Abbey Super Secret Injection");
            SpecialAssets.assets.Add(lockUnlockPedestal);
            SpecialAssets.assets.Add(lockUnlockPedestal.transform.Find("Shadow").gameObject);
            SpecialAssets.assets.Add(lockUnlockPedestal.transform.Find("Spawn").gameObject);
            SpecialAssets.assets.Add(bookPage);
        }

        public static void InitDiamondShrine()
        {
            GameObject diamondPedestal = AssetBundleManager.Load<GameObject>("placeables/diamondpedestal");
            GameObject pedestal = LoadHelper.LoadAssetFromAnywhere<GameObject>("boss_reward_pedestal");
            tk2dSpriteCollectionData coll = pedestal.GetComponent<tk2dBaseSprite>().Collection;
            tk2dSprite sprite = tk2dSprite.AddComponent(diamondPedestal.transform.Find("Sprite").gameObject, coll, coll.GetSpriteIdByName("pedestal_gun_001"));
            tk2dSprite shadowSprite = tk2dSprite.AddComponent(diamondPedestal.transform.Find("Shadow").gameObject, coll, coll.GetSpriteIdByName("pedestal_gun_shadow_002"));
            shadowSprite.HeightOffGround = -2f;
            sprite.HeightOffGround = -1f;
            sprite.AttachRenderer(shadowSprite);
            sprite.UpdateZDepth();
            SpeculativeRigidbody body = diamondPedestal.AddComponent<SpeculativeRigidbody>();
            body.PixelColliders = new List<PixelCollider>
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.LowObstacle,
                    ManualOffsetX = 2,
                    ManualOffsetY = 2,
                    ManualWidth = 26,
                    ManualHeight = 20
                },
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.HighObstacle,
                    ManualOffsetX = 2,
                    ManualOffsetY = 13,
                    ManualWidth = 26,
                    ManualHeight = 20
                }
            };
            diamondPedestal.transform.Find("Shadow").localPosition = new Vector3(0.125f, -0.0625f, -1.1f);
            diamondPedestal.transform.Find("Sprite").localPosition = new Vector3(0.0625f, 0f, 0f);
            diamondPedestal.transform.Find("Spawn").localPosition = new Vector3(0.8f, 1.5f, 0f);
            DiamondShrine pedestalComponent = diamondPedestal.AddComponent<DiamondShrine>();
            pedestalComponent.ShrineDescription = "#DIAMOND_SHRINE_DESCRIPTION";
            pedestalComponent.AcceptString = "#DIAMOND_SHRINE_ACCEPT";
            pedestalComponent.DeclineString = "#DIAMOND_SHRINE_DECLINE";
            pedestalComponent.DiamondId = DiamondItem.DiamondId;
            pedestalComponent.MinimapIcon = AssetBundleManager.Load<GameObject>("placeables/minimap/diamondshrineicon");
            pedestalComponent.TalkPoint = diamondPedestal.transform.Find("TalkPoint");
            pedestalComponent.DiamondConjureTime = 2f;
            pedestalComponent.DiamondSpawnPoint = diamondPedestal.transform.Find("Spawn");
            //AssetBundleManager.Load<Material>("particles/materials/cosmicparticle").shader = ShaderCache.Acquire("Brave/LitBlendUber");
            tk2dSprite.AddComponent(pedestalComponent.MinimapIcon, coll, SpriteBuilder.AddSpriteToCollection("placeablesprites/minimap/diamond_shrine_minimap_icon_idle_001", coll, "tk2d/CutoutVertexColorTintableTilted"));
            ETGMod.Databases.Strings.Core.Set("#DIAMOND_SHRINE_DESCRIPTION", "A shrine to both Nothing and Everything. Both Light and Dark. Both Good and Evil. A shrine to the Universe. A shrine to Time.");
            ETGMod.Databases.Strings.Core.Set("#DIAMOND_SHRINE_ACCEPT", "<Make a bloody sacrifice <Lose %HEART_SYMBOL>>");
            ETGMod.Databases.Strings.Core.Set("#DIAMOND_SHRINE_DECLINE", "<Walk away>");
            RoomFactory.AddInjection(RoomFactory.BuildFromResource("SpecialStuffPack.Rooms.Diamond Shrine Room.room").room, "Forge Diamond Shrine", new List<ProceduralFlowModifierData.FlowModifierPlacementType> { 
                ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN }, 0f, new List<DungeonPrerequisite> { new DungeonPrerequisite { prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET, requireTileset = true, 
                    requiredTileset = GlobalDungeonData.ValidTilesets.FORGEGEON } }, "Forge Diamond Shrine Injection");
            SpecialAssets.assets.Add(diamondPedestal);
            SpecialAssets.assets.Add(diamondPedestal.transform.Find("Shadow").gameObject);
            SpecialAssets.assets.Add(diamondPedestal.transform.Find("Spawn").gameObject);
            SpecialAssets.assets.Add(AssetBundleManager.Load<Material>("particles/materials/cosmicparticle"));
        }

        public static void InitDoor()
        {
            GameObject pedestal = LoadHelper.LoadAssetFromAnywhere<GameObject>("boss_reward_pedestal");
            tk2dSpriteCollectionData coll = pedestal.GetComponent<tk2dBaseSprite>().Collection;
            GameObject door = AssetBundleManager.Load<GameObject>("placeables/door", coll, "Brave/LitTk2dCustomFalloffTintableTilted");
            door.AddComponent<DoorPlaceable>().talkpoint = door.transform.Find("TalkPoint");
            door.GetComponent<tk2dBaseSprite>().HeightOffGround = -1f;
            ETGMod.Databases.Strings.Core.Set("#STRANGE_DOOR_DESC", "A rigidly locked door. Seems impossible to open.");
            RoomFactory.AddInjection(RoomFactory.BuildFromResource("SpecialStuffPack.Rooms.Door.room").room, "The Door", new List<ProceduralFlowModifierData.FlowModifierPlacementType> {
                ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN }, 0f, new List<DungeonPrerequisite> { new DungeonPrerequisite { prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET, requireTileset = true,
                    requiredTileset = GlobalDungeonData.ValidTilesets.FORGEGEON } }, "The Door Injection");
        }

        public static void InitSomethingSpecialPlaceable()
        {
            GameObject somethingSpecial = AssetBundleManager.Load<GameObject>("placeables/somethingspecialplaceable");
            tk2dSpriteCollectionData foyerColl = Resources.FindObjectsOfTypeAll<tk2dSpriteCollectionData>().ToList().Find((tk2dSpriteCollectionData c) => c.spriteCollectionName == "Foyer_Collection");
            tk2dSpriteCollectionData hellColl = Resources.FindObjectsOfTypeAll<tk2dSpriteCollectionData>().ToList().Find((tk2dSpriteCollectionData c) => c.spriteCollectionName == "Environment_Hell_Collection");
            int baseId = SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_001", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_002", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_003", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_004", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_005", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_006", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            SpriteBuilder.AddSpriteToCollection("placeablesprites/something_special_shrine_idle_007", hellColl, "Brave/LitTk2dCustomFalloffTintableTilted");
            tk2dSprite sprite = tk2dSprite.AddComponent(somethingSpecial, hellColl, baseId);
            tk2dSprite shadowSprite = tk2dSprite.AddComponent(somethingSpecial.transform.Find("Shadow").gameObject, foyerColl, foyerColl.GetSpriteIdByName("gun_shrine_shadow_001"));
            shadowSprite.HeightOffGround = -2f;
            sprite.HeightOffGround = -1f;
            sprite.AttachRenderer(shadowSprite);
            sprite.UpdateZDepth();
            SpeculativeRigidbody body = somethingSpecial.AddComponent<SpeculativeRigidbody>();
            body.PixelColliders = new List<PixelCollider>
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.HighObstacle,
                    ManualOffsetX = 3,
                    ManualOffsetY = 0,
                    ManualWidth = 14,
                    ManualHeight = 20
                }
            };
            SomethingSpecialPlaceable somethingSpecialComponent = somethingSpecial.AddComponent<SomethingSpecialPlaceable>();
            somethingSpecialComponent.ShrineDescription = "#SOMETHING_SPECIAL_DESCRIPTION";
            somethingSpecialComponent.AcceptString = "#SOMETHING_SPECIAL_ACCEPT";
            somethingSpecialComponent.DeclineString = "#SOMETHING_SPECIAL_DECLINE";
            somethingSpecialComponent.AreYouSureString = "#SOMETHING_SPECIAL_AYS";
            somethingSpecialComponent.AmethystReplacement = "#SOMETHING_SPECIAL_AMETHYST";
            somethingSpecialComponent.OpalReplacement = "#SOMETHING_SPECIAL_OPAL";
            somethingSpecialComponent.EmeraldReplacement = "#SOMETHING_SPECIAL_EMERALD";
            somethingSpecialComponent.AquamarineReplacement = "#SOMETHING_SPECIAL_AQUAMARINE";
            somethingSpecialComponent.RubyReplacement = "#SOMETHING_SPECIAL_RUBY";
            somethingSpecialComponent.DiamondReplacement = "#SOMETHING_SPECIAL_DIAMOND";
            somethingSpecialComponent.BaseSpriteId = baseId;
            somethingSpecialComponent.TalkPoint = somethingSpecial.transform.Find("TalkPoint");
            somethingSpecialComponent.DustPoofVFX = GetItemById<PaydayDrillItem>(625).VFXDustPoof;
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_DESCRIPTION", "A remarkably special shrine. It has six holes in it.");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_ACCEPT", "<Insert the %GEM>");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_DECLINE", "<Walk away>");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_AYS", "Something terrible might happen if you insert the Diamond. Are you sure?");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_AMETHYST", "Amethyst");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_OPAL", "Opal");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_EMERALD", "Emerald");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_AQUAMARINE", "Aquamarine");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_RUBY", "Ruby");
            ETGMod.Databases.Strings.Core.Set("#SOMETHING_SPECIAL_DIAMOND", "Diamond");
            SpecialAssets.assets.Add(somethingSpecial);
            SpecialAssets.assets.Add(somethingSpecial.transform.Find("Shadow").gameObject);
            //PrototypeDungeonRoom room = DungeonDatabase.GetOrLoadByName("base_bullethell").PatternSettings.flows[0].FirstNode.overrideExactRoom;
            //RoomFactory.AddPlaceableToRoom(room, new Vector2(10f, 10f), "somethingspecialplaceable");
            //SpecialAssets.assets.Add(room);
        }
    }
}
