using Dungeonator;
using HutongGames.PlayMaker.Actions;
using MonoMod.RuntimeDetour;
using SpecialStuffPack.Behaviors;
using SpecialStuffPack.BulletScripts.AdvancedRat;
using SpecialStuffPack.BulletScripts.AdvancedRatMetalGear;
using SpecialStuffPack.BulletScripts;
using SpecialStuffPack.BulletScripts.UltimateDragun;
using SpecialStuffPack.Components;
using SpecialStuffPack.Enemies;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using SpecialStuffPack.Behaviors.AdvancedRatMetalGear;
using SpecialStuffPack.BulletScripts.AdvancedHelicopter;

namespace SpecialStuffPack.Items
{
    public class GoldKey : PassiveItem
    {
        public static void Init()
        {
            string name = "Golden Key";
            string shortdesc = "Old and Long Forgotten";
            string longdesc = "A shiny golden key found in the walls of the Abbey. Resonates with power.\n\nThere are directions written on it:\n\"FIRST, TO THE LAYER OF THE BOTHERSOME RAT\"\n\"THEN, TO THE GREAT CELL BENEATH THE HOLLOW\"\n" +
                "\"THEN, TO THE HEART OF THE FORGE\"\n\"AND FINALLY, TO THE VOID OF BULLETS, YOUR DESTINATION\"";
            GoldKey item = EasyItemInit<GoldKey>("items/goldkey", "sprites/golden_key_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            AddSpriteToCollection("sprites/key_bit_001", item.sprite.Collection);
            AddSpriteToCollection("sprites/emerald_bit_001", item.sprite.Collection);
            item.ShatterVFX = (GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab");
            GoldKeyId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            new Hook(typeof(InteractableLock).GetMethod("OnEnteredRange", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("UnlockRatLock", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(SellCellController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance), typeof(GoldKey).GetMethod("UnlockSellCell", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(ProceduralFlowModifierData).GetProperty("PrerequisitesMet", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), typeof(GoldKey).GetMethod("EnableSellCreepInjection", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(DraGunController).GetMethod("MaybeConvertToGold", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("ForceConvertDragun", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(ResourcefulRatIntroDoer).GetMethod("StartIntro", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("StartGoldKeyRatIntro", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(SpecificIntroDoer).GetMethod("PlayerWalkedIn", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("MakeRatIntroUnskippable", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(MetalGearRatIntroDoer).GetMethod("Init", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("ChangeMetalGearRat", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(HelicopterIntroDoer).GetMethod("PlayerWalkedIn", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("MakeHelicopterIntroUnskippable", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(HelicopterIntroDoer).GetMethod("StartIntro", BindingFlags.Public | BindingFlags.Instance), typeof(GoldKey).GetMethod("StartGoldKeyHelicopterIntro", BindingFlags.Public | BindingFlags.Static));
            SpecialEnemies.InitLockGhostPrefab();
            ETGMod.Databases.Strings.Core.SetComplex("#GOLD_KEY_RAT_TALK", "Hold on.", "That golden key you have.", "It shines with power.", "Mind if I have it?");
            ETGMod.Databases.Strings.Core.SetComplex("#GOLD_KEY_HELICOPTERAGUNIM_TALK", "Wait a minute....", "That gem and that key you have...", "They are full of power!", "Now, can I have them for a moment?");
            ETGMod.Databases.Strings.Enemies.Set("#GOLD_KEY_POWEREDUPANDREADY", "POWERED UP AND READY...");
        }

        public static void StartGoldKeyHelicopterIntro(Action<HelicopterIntroDoer, List<tk2dSpriteAnimator>> orig, HelicopterIntroDoer self, List<tk2dSpriteAnimator> animators)
        {
            if (IsFlagSetAtAll(typeof(GoldKey)) && IsFlagSetAtAll(typeof(EmeraldItem)))
            {
                self.StartCoroutine(GoldKeyHelicopterIntro(self));
            }
            else
            {
                orig(self, animators);
            }
        }

        public static IEnumerator GoldKeyHelicopterIntro(HelicopterIntroDoer cop)
        {
            cop.GetComponent<GenericIntroDoer>().portraitSlideSettings = new PortraitSlideSettings
            {
                bgColor = new Color(1f, 0.75f, 0f),
                bossSubtitleString = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossSubtitleString,
                bossArtSprite = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossArtSprite,
                bossNameString = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossNameString,
                bossQuoteString = "#GOLD_KEY_POWEREDUPANDREADY",
                bossSpritePxOffset = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossSpritePxOffset,
                bottomRightTextPxOffset = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.bottomRightTextPxOffset,
                topLeftTextPxOffset = cop.GetComponent<GenericIntroDoer>().portraitSlideSettings.topLeftTextPxOffset
            };
            cop.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            TextBoxManager.TIME_INVARIANT = true;
            yield return cop.StartCoroutine(cop.GetComponent<VoiceOverer>().HandleIntroVO());
            cop.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            cop.GetComponent<AIAnimator>().EndAnimation();
            cop.GetComponent<tk2dSpriteAnimator>().Play("idle_south");
            yield return WaitRealtimeAndUpdatePlayers(1.5f);
            cop.GetComponent<AIAnimator>().PlayUntilCancelled("intro_talk", false, null, -1f, false);
            yield return cop.StartCoroutine(DoHelicopterTalk(cop.GetComponent<VoiceOverer>(), "#GOLD_KEY_HELICOPTERAGUNIM_TALK", true));
            cop.GetComponent<AIAnimator>().EndAnimation();
            cop.GetComponent<tk2dSpriteAnimator>().Play("idle_south");
            cop.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            TextBoxManager.TIME_INVARIANT = false;
            PlayerController keyHaver = GameManager.Instance.AllPlayers.ToList().Find((PlayerController p) => IsFlagSetForCharacter(p, typeof(GoldKey)));
            PlayerController emeraldHaver = GameManager.Instance.AllPlayers.ToList().Find((PlayerController p) => IsFlagSetForCharacter(p, typeof(EmeraldItem)));
            CutsceneMotion mot = new CutsceneMotion(GameManager.Instance.MainCameraController.transform, Vector2.Lerp(keyHaver.CenterPosition, emeraldHaver.CenterPosition, 0.5f), cop.GetComponent<GenericIntroDoer>().cameraMoveSpeed, 0f);
            mot.camera = GameManager.Instance.MainCameraController;
            while (mot.lerpProgress < 1f)
            {
                Vector2? lerpEnd = mot.lerpEnd;
                Vector2 vector = (lerpEnd == null) ? GameManager.Instance.MainCameraController.GetIdealCameraPosition() : mot.lerpEnd.Value;
                float num = Vector2.Distance(vector, mot.lerpStart);
                float num2 = mot.speed * GameManager.INVARIANT_DELTA_TIME;
                float num3 = num2 / num;
                mot.lerpProgress = Mathf.Clamp01(mot.lerpProgress + num3);
                float t = mot.lerpProgress;
                if (mot.isSmoothStepped)
                {
                    t = Mathf.SmoothStep(0f, 1f, t);
                }
                Vector2 vector2 = Vector2.Lerp(mot.lerpStart, vector, t);
                if (mot.camera != null)
                {
                    mot.camera.OverridePosition = vector2.ToVector3ZUp(mot.zOffset);
                }
                else
                {
                    mot.transform.position = BraveUtility.QuantizeVector(vector2.ToVector3ZUp(mot.zOffset), (float)PhysicsEngine.Instance.PixelsPerUnit);
                }
                foreach(PlayerController play in GameManager.Instance.AllPlayers)
                {
                    foreach(tk2dSpriteAnimator anim in play.GetComponentsInChildren<tk2dSpriteAnimator>())
                    {
                        anim.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                }
                yield return null;
            }
            yield return WaitRealtimeAndUpdatePlayers(0.5f);
            Vector2 keyTarget;
            Vector2 emeraldTarget;
            if(keyHaver != emeraldHaver)
            {
                keyTarget = keyHaver.CenterPosition + new Vector2(0f, 4f);
                emeraldTarget = emeraldHaver.CenterPosition + new Vector2(0f, 4f);
            }
            else
            {
                keyTarget = keyHaver.CenterPosition + new Vector2(-1.5f, 4f);
                emeraldTarget = emeraldHaver.CenterPosition + new Vector2(1.5f, 4f);
            }
            float ela = 0f;
            float dura = 1f;
            emeraldHaver.RemovePassiveItem(EmeraldItem.EmeraldId);
            keyHaver.RemovePassiveItem(GoldKeyId);
            if (cop.GetComponent<GemDropper>() != null)
            {
                if(cop.GetComponent<GemDropper>().AdditionalItems == null)
                {
                    cop.GetComponent<GemDropper>().AdditionalItems = new List<int>();
                }
                cop.GetComponent<GemDropper>().AdditionalItems.Add(EmeraldItem.EmeraldId);
                cop.GetComponent<GemDropper>().AdditionalItems.Add(GoldKeyId);
            }
            Transform keySprite = CreateEmptySprite(PickupObjectDatabase.GetById(GoldKeyId), keyHaver.CenterPosition);
            keySprite.localScale = Vector3.zero;
            Transform emeraldSprite = CreateEmptySprite(PickupObjectDatabase.GetById(EmeraldItem.EmeraldId), emeraldHaver.CenterPosition);
            emeraldSprite.localScale = Vector3.zero;
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                keySprite.position = Vector2Extensions.SmoothStep(keyHaver.CenterPosition, keyTarget, ela / dura);
                keySprite.rotation = Quaternion.Euler(0f, 0f, Mathf.SmoothStep(0f, -360f, ela / dura));
                keySprite.localScale = Vector2Extensions.SmoothStep(Vector2.zero, Vector2.one, ela / dura);
                emeraldSprite.position = Vector2Extensions.SmoothStep(emeraldHaver.CenterPosition, emeraldTarget, ela / dura);
                emeraldSprite.rotation = Quaternion.Euler(0f, 0f, Mathf.SmoothStep(0f, 360f, ela / dura));
                emeraldSprite.localScale = Vector2Extensions.SmoothStep(Vector2.zero, Vector2.one, ela / dura);
                foreach (PlayerController play in GameManager.Instance.AllPlayers)
                {
                    foreach (tk2dSpriteAnimator anim in play.GetComponentsInChildren<tk2dSpriteAnimator>())
                    {
                        anim.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                }
                yield return null;
            }
            yield return WaitRealtimeAndUpdatePlayers(1f);
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", keyHaver.gameObject);
            if(keyHaver != emeraldHaver)
            {
                AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", emeraldHaver.gameObject);
            }
            Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab"), keySprite.position, Quaternion.identity).GetComponentInChildren<tk2dSpriteAnimator>().AnimateDuringBossIntros = true;
            Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab"), emeraldSprite.position, Quaternion.identity).GetComponentInChildren<tk2dSpriteAnimator>().AnimateDuringBossIntros = true;
            Destroy(keySprite.gameObject);
            Destroy(emeraldSprite.gameObject);
            List<GameObject> bits = new List<GameObject>();
            List<Vector2> bitTargets = new List<Vector2>();
            for(int i = 0; i < 30; i++)
            {
                GameObject keyBit = new GameObject("Key Bit " + i);
                keyBit.transform.position = keyTarget;
                tk2dSprite.AddComponent(keyBit, PickupObjectDatabase.GetById(GoldKeyId).sprite.Collection, PickupObjectDatabase.GetById(GoldKeyId).sprite.Collection.GetSpriteIdByName("key_bit_001")).HeightOffGround = 10f;
                keyBit.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                Vector2 keyBitTarget = keyTarget + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(2f, 2.5f);
                bits.Add(keyBit);
                bitTargets.Add(keyBitTarget);
                GameObject emeraldBit = new GameObject("Emerald Bit " + i);
                emeraldBit.transform.position = emeraldTarget;
                tk2dSprite.AddComponent(emeraldBit, PickupObjectDatabase.GetById(GoldKeyId).sprite.Collection, PickupObjectDatabase.GetById(GoldKeyId).sprite.Collection.GetSpriteIdByName("emerald_bit_001")).HeightOffGround = 10f;
                emeraldBit.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                Vector2 emeraldBitTarget = emeraldTarget + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(2f, 2.5f);
                bits.Add(emeraldBit);
                bitTargets.Add(emeraldBitTarget);
            }
            ela = 0f;
            dura = 0.5f;
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                for(int i = 0; i < bits.Count; i++)
                {
                    GameObject bit = bits[i];
                    Vector2 target = bit.transform.position;
                    if(i < bitTargets.Count)
                    {
                        target = bitTargets[i];
                    }
                    bit.transform.position = Vector2Extensions.SmoothStep(bit.name.Contains("Key") ? keyTarget : emeraldTarget, target, ela / dura);
                    bit.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                }
                foreach (PlayerController play in GameManager.Instance.AllPlayers)
                {
                    foreach (tk2dSpriteAnimator anim in play.GetComponentsInChildren<tk2dSpriteAnimator>())
                    {
                        anim.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                }
                yield return null;
            }
            yield return WaitRealtimeAndUpdatePlayers(0.5f);
            ela = 0f;
            dura = 1f;
            CutsceneMotion mot2 = new CutsceneMotion(GameManager.Instance.MainCameraController.transform, cop.GetComponent<GenericIntroDoer>().BossCenter, cop.GetComponent<GenericIntroDoer>().cameraMoveSpeed * 1.2f, 0f);
            mot2.camera = GameManager.Instance.MainCameraController;
            cop.StartCoroutine(HandleMotionProgress(mot2));
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                for (int i = 0; i < bits.Count; i++)
                {
                    GameObject bit = bits[i];
                    Vector2 start = bit.transform.position;
                    if (i < bitTargets.Count)
                    {
                        start = bitTargets[i];
                    }
                    bit.transform.position = Vector2Extensions.SmoothStep(start, Vector2.Lerp(cop.GetComponentInChildren<tk2dBaseSprite>().WorldCenter, cop.GetComponentInChildren<tk2dBaseSprite>().WorldBottomCenter, 0.5f), ela / dura);
                    bit.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                }
                foreach (PlayerController play in GameManager.Instance.AllPlayers)
                {
                    foreach (tk2dSpriteAnimator anim in play.GetComponentsInChildren<tk2dSpriteAnimator>())
                    {
                        anim.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                }
                yield return null;
            }
            foreach(GameObject bit in bits)
            {
                Destroy(bit);
            }
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Thunder_01", cop.gameObject);
            yield return WaitRealtimeAndUpdatePlayers(0.75f);
            typeof(HelicopterIntroDoer).GetField("m_isFinished", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(cop, true);
            (cop.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem i) => i.NickName == "Magic").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(HelicopterLightning2));
            (cop.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem i) => i.NickName == "Missiles").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(HelicopterMissiles2));
            (cop.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem i) => i.NickName == "Quick Fire").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(HelicopterRandomSimple2));
            (cop.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem i) => i.NickName == "Shoot Flames (arc)").Behavior as ShootBehavior).BulletScript = 
                new CustomBulletScriptSelector(typeof(HelicopterFlames2));
            yield break;
        }

        public static IEnumerator WaitRealtimeAndUpdatePlayers(float waitTime)
        {
            float ela = 0f;
            while(ela < waitTime)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                foreach (PlayerController play in GameManager.Instance.AllPlayers)
                {
                    foreach (tk2dSpriteAnimator anim in play.GetComponentsInChildren<tk2dSpriteAnimator>())
                    {
                        anim.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                }
                yield return null;
            }
            yield break;
        }

        public static IEnumerator HandleMotionProgress(CutsceneMotion mot)
        {
            while (mot.lerpProgress < 1f)
            {
                Vector2? lerpEnd = mot.lerpEnd;
                Vector2 vector = (lerpEnd == null) ? GameManager.Instance.MainCameraController.GetIdealCameraPosition() : mot.lerpEnd.Value;
                float num = Vector2.Distance(vector, mot.lerpStart);
                float num2 = mot.speed * GameManager.INVARIANT_DELTA_TIME;
                float num3 = num2 / num;
                mot.lerpProgress = Mathf.Clamp01(mot.lerpProgress + num3);
                float t = mot.lerpProgress;
                if (mot.isSmoothStepped)
                {
                    t = Mathf.SmoothStep(0f, 1f, t);
                }
                Vector2 vector2 = Vector2.Lerp(mot.lerpStart, vector, t);
                if (mot.camera != null)
                {
                    mot.camera.OverridePosition = vector2.ToVector3ZUp(mot.zOffset);
                }
                else
                {
                    mot.transform.position = BraveUtility.QuantizeVector(vector2.ToVector3ZUp(mot.zOffset), (float)PhysicsEngine.Instance.PixelsPerUnit);
                }
                yield return null;
            }
        }

        public static IEnumerator DoHelicopterTalk(VoiceOverer v, string stringKey, bool multiline)
        {
            v.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            if (multiline)
            {
                int numLines = StringTableManager.GetNumStrings(stringKey);
                for (int i = 0; i < numLines; i++)
                {
                    yield return v.StartCoroutine(TalkRaw(v, StringTableManager.GetExactString(stringKey, i)));
                }
            }
            else
            {
                yield return v.StartCoroutine(TalkRaw(v, StringTableManager.GetString(stringKey)));
            }
            yield return null;
            yield break;
        }

        private static IEnumerator TalkRaw(VoiceOverer v, string plaintext, float maxDuration = -1f)
        {
            TextBoxManager.ShowTextBox(v.transform.position + new Vector3(2.25f, 7.5f, 0f), v.transform, 5f, plaintext, "agunim", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
            float elapsed = 0f;
            bool advancedPressed = false;
            while (!advancedPressed)
            {
                advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (maxDuration > 0f)
                {
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    if (elapsed > maxDuration)
                    {
                        break;
                    }
                }
                yield return null;
            }
            TextBoxManager.ClearTextBox(v.transform);
            yield break;
        }

        public static void MakeHelicopterIntroUnskippable(Action<HelicopterIntroDoer, PlayerController, List<tk2dSpriteAnimator>> orig, HelicopterIntroDoer self, PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            if (IsFlagSetAtAll(typeof(GoldKey)) && IsFlagSetAtAll(typeof(EmeraldItem)))
            {
                self.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            }
            orig(self, player, animators);
        }

        public static void ChangeMetalGearRat(Action<MetalGearRatIntroDoer> orig, MetalGearRatIntroDoer self)
        {
            orig(self);
            if (Components.PlayerControllerExt.AnyoneHasBeenKeyRobbed() && self.GetComponent<ModifiedMetalGearRatBehavior>() == null)
            {
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Jump Pound").Behavior as ShootBehavior).BulletScript =
                           new CustomBulletScriptSelector(typeof(MetalGearRatJumpPound2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Pound Left").Behavior as ShootBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatSidePoundLeft2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Pound Right").Behavior as ShootBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatSidePoundRight2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Magic Circle").Behavior as ShootBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatSpinners2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Missiles").Behavior as ShootBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatMissiles2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Tailgun").Behavior as ShootBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatTailgun2));
                (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Center)").Behavior as MetalGearRatBeamsBehavior).BulletScript =
                    new CustomBulletScriptSelector(typeof(MetalGearRatLaserBullets2));
                ShootBeamBehavior laserRight = self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Right)").Behavior as ShootBeamBehavior;
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Right)").Behavior = new ScriptedShootBeamBehavior
                {
                    AccumulateHealthThresholds = laserRight.AccumulateHealthThresholds,
                    AttackCooldown = laserRight.AttackCooldown,
                    degreeCatchUpSpeed = laserRight.degreeCatchUpSpeed,
                    degTurnRateAcceleration = laserRight.degTurnRateAcceleration,
                    maxDegTurnRate = laserRight.maxDegTurnRate,
                    minDegreesForCatchUp = laserRight.minDegreesForCatchUp,
                    MinWallDistance = laserRight.MinWallDistance,
                    resetCooldownOnDamage = laserRight.resetCooldownOnDamage,
                    useDegreeCatchUp = laserRight.useDegreeCatchUp,
                    beamLengthOFfset = laserRight.beamLengthOFfset,
                    beamLengthSinMagnitude = laserRight.beamLengthSinMagnitude,
                    beamLengthSinPeriod = laserRight.beamLengthSinPeriod,
                    beamSelection = (ScriptedShootBeamBehavior.BeamSelection)laserRight.beamSelection,
                    BulletScript = new CustomBulletScriptSelector(typeof(MetalGearRatSweepLaserBullets2)),
                    Cooldown = laserRight.Cooldown,
                    CooldownVariance = laserRight.CooldownVariance,
                    FireAnimation = laserRight.FireAnimation,
                    firingTime = laserRight.firingTime,
                    GlobalCooldown = laserRight.GlobalCooldown,
                    GroupCooldown = laserRight.GroupCooldown,
                    GroupName = laserRight.GroupName,
                    HealthThresholds = laserRight.HealthThresholds,
                    initialAimOffset = laserRight.initialAimOffset,
                    initialAimType = (ScriptedShootBeamBehavior.InitialAimType)laserRight.initialAimType,
                    InitialCooldown = laserRight.InitialCooldown,
                    InitialCooldownVariance = laserRight.InitialCooldownVariance,
                    IsBlackPhantom = laserRight.IsBlackPhantom,
                    MaxEnemiesInRoom = laserRight.MaxEnemiesInRoom,
                    RequiresLineOfSight = laserRight.RequiresLineOfSight,
                    MaxHealthThreshold = laserRight.MaxHealthThreshold,
                    maxUnitForCatchUp = laserRight.maxUnitForCatchUp,
                    maxUnitTurnRate = laserRight.maxUnitTurnRate,
                    MaxUsages = laserRight.MaxUsages,
                    restrictBeamLengthToAim = laserRight.restrictBeamLengthToAim,
                    MinHealthThreshold = laserRight.MinHealthThreshold,
                    MinRange = laserRight.MinRange,
                    minUnitForCatchUp = laserRight.minUnitForCatchUp,
                    minUnitForOvershoot = laserRight.minUnitForOvershoot,
                    minUnitRadius = laserRight.minUnitRadius,
                    PostFireAnimation = laserRight.PostFireAnimation,
                    randomInitialAimOffsetSign = laserRight.randomInitialAimOffsetSign,
                    Range = laserRight.Range,
                    ShootPoint = (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Center)").Behavior as MetalGearRatBeamsBehavior).ShootPoint,
                    specificBeamShooter = laserRight.specificBeamShooter,
                    stopWhileFiring = laserRight.stopWhileFiring,
                    targetAreaStyle = laserRight.targetAreaStyle,
                    TellAnimation = laserRight.TellAnimation,
                    trackingType = (ScriptedShootBeamBehavior.TrackingType)laserRight.trackingType,
                    unitCatchUpSpeed = laserRight.unitCatchUpSpeed,
                    unitOvershootSpeed = laserRight.unitOvershootSpeed,
                    unitOvershootTime = laserRight.unitOvershootTime,
                    unitTurnRateAcceleration = laserRight.unitTurnRateAcceleration,
                    useUnitCatchUp = laserRight.useUnitCatchUp,
                    useUnitOvershoot = laserRight.useUnitOvershoot
                };
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Right)").Behavior.Init(self.gameObject, self.aiActor, self.aiShooter);
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Right)").Behavior.Start();
                ShootBeamBehavior laserLeft = self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Left)").Behavior as ShootBeamBehavior;
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Left)").Behavior = new ScriptedShootBeamBehavior
                {
                    AccumulateHealthThresholds = laserLeft.AccumulateHealthThresholds,
                    AttackCooldown = laserLeft.AttackCooldown,
                    degreeCatchUpSpeed = laserLeft.degreeCatchUpSpeed,
                    degTurnRateAcceleration = laserLeft.degTurnRateAcceleration,
                    maxDegTurnRate = laserLeft.maxDegTurnRate,
                    minDegreesForCatchUp = laserLeft.minDegreesForCatchUp,
                    MinWallDistance = laserLeft.MinWallDistance,
                    resetCooldownOnDamage = laserLeft.resetCooldownOnDamage,
                    useDegreeCatchUp = laserLeft.useDegreeCatchUp,
                    beamLengthOFfset = laserLeft.beamLengthOFfset,
                    beamLengthSinMagnitude = laserLeft.beamLengthSinMagnitude,
                    beamLengthSinPeriod = laserLeft.beamLengthSinPeriod,
                    beamSelection = (ScriptedShootBeamBehavior.BeamSelection)laserLeft.beamSelection,
                    BulletScript = new CustomBulletScriptSelector(typeof(MetalGearRatSweepLaserBullets2)),
                    Cooldown = laserLeft.Cooldown,
                    CooldownVariance = laserLeft.CooldownVariance,
                    FireAnimation = laserLeft.FireAnimation,
                    firingTime = laserLeft.firingTime,
                    GlobalCooldown = laserLeft.GlobalCooldown,
                    GroupCooldown = laserLeft.GroupCooldown,
                    GroupName = laserLeft.GroupName,
                    HealthThresholds = laserLeft.HealthThresholds,
                    initialAimOffset = laserLeft.initialAimOffset,
                    initialAimType = (ScriptedShootBeamBehavior.InitialAimType)laserLeft.initialAimType,
                    InitialCooldown = laserLeft.InitialCooldown,
                    InitialCooldownVariance = laserLeft.InitialCooldownVariance,
                    IsBlackPhantom = laserLeft.IsBlackPhantom,
                    MaxEnemiesInRoom = laserLeft.MaxEnemiesInRoom,
                    RequiresLineOfSight = laserLeft.RequiresLineOfSight,
                    MaxHealthThreshold = laserLeft.MaxHealthThreshold,
                    maxUnitForCatchUp = laserLeft.maxUnitForCatchUp,
                    maxUnitTurnRate = laserLeft.maxUnitTurnRate,
                    MaxUsages = laserLeft.MaxUsages,
                    restrictBeamLengthToAim = laserLeft.restrictBeamLengthToAim,
                    MinHealthThreshold = laserLeft.MinHealthThreshold,
                    MinRange = laserLeft.MinRange,
                    minUnitForCatchUp = laserLeft.minUnitForCatchUp,
                    minUnitForOvershoot = laserLeft.minUnitForOvershoot,
                    minUnitRadius = laserLeft.minUnitRadius,
                    PostFireAnimation = laserLeft.PostFireAnimation,
                    randomInitialAimOffsetSign = laserLeft.randomInitialAimOffsetSign,
                    Range = laserLeft.Range,
                    ShootPoint = (self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Center)").Behavior as MetalGearRatBeamsBehavior).ShootPoint,
                    specificBeamShooter = laserLeft.specificBeamShooter,
                    stopWhileFiring = laserLeft.stopWhileFiring,
                    targetAreaStyle = laserLeft.targetAreaStyle,
                    TellAnimation = laserLeft.TellAnimation,
                    trackingType = (ScriptedShootBeamBehavior.TrackingType)laserLeft.trackingType,
                    unitCatchUpSpeed = laserLeft.unitCatchUpSpeed,
                    unitOvershootSpeed = laserLeft.unitOvershootSpeed,
                    unitOvershootTime = laserLeft.unitOvershootTime,
                    unitTurnRateAcceleration = laserLeft.unitTurnRateAcceleration,
                    useUnitCatchUp = laserLeft.useUnitCatchUp,
                    useUnitOvershoot = laserLeft.useUnitOvershoot
                };
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Left)").Behavior.Init(self.gameObject, self.aiActor, self.aiShooter);
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Laser Face (Left)").Behavior.Start();
                SpawnReinforcementsBehavior mousers = self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Spawn Mousers").Behavior as SpawnReinforcementsBehavior;
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Spawn Mousers").Behavior = new SpawnAndPoisonifyReinforcementsBehavior
                {
                    AccumulateHealthThresholds = mousers.AccumulateHealthThresholds,
                    AttackCooldown = mousers.AttackCooldown,
                    DelayAfterSpawn = mousers.DelayAfterSpawn,
                    DelayAfterSpawnMinOccupancy = mousers.DelayAfterSpawnMinOccupancy,
                    DirectionalAnimation = mousers.DirectionalAnimation,
                    DisableDrops = mousers.DisableDrops,
                    MinWallDistance = mousers.MinWallDistance,
                    resetCooldownOnDamage = mousers.resetCooldownOnDamage,
                    staggerDelay = mousers.staggerDelay,
                    StopDuringAnimation = mousers.StopDuringAnimation,
                    Cooldown = mousers.Cooldown,
                    CooldownVariance = mousers.CooldownVariance,
                    GlobalCooldown = mousers.GlobalCooldown,
                    GroupCooldown = mousers.GroupCooldown,
                    GroupName = mousers.GroupName,
                    HealthThresholds = mousers.HealthThresholds,
                    HideGun = mousers.HideGun,
                    indexType = (SpawnAndModifyReinforcementsBehavior.IndexType)mousers.indexType,
                    InitialCooldown = mousers.InitialCooldown,
                    InitialCooldownVariance = mousers.InitialCooldownVariance,
                    IsBlackPhantom = mousers.IsBlackPhantom,
                    MaxEnemiesInRoom = mousers.MaxEnemiesInRoom,
                    MaxHealthThreshold = mousers.MaxHealthThreshold,
                    MaxRoomOccupancy = mousers.MaxRoomOccupancy,
                    MaxUsages = mousers.MaxUsages,
                    MinHealthThreshold = mousers.MinHealthThreshold,
                    MinRange = mousers.MinRange,
                    OverrideMaxOccupancyToSpawn = mousers.OverrideMaxOccupancyToSpawn,
                    Range = mousers.Range,
                    ReinforcementIndices = mousers.ReinforcementIndices,
                    RequiresLineOfSight = mousers.RequiresLineOfSight,
                    staggerMode = (SpawnAndModifyReinforcementsBehavior.StaggerMode)mousers.staggerMode,
                    StaggerSpawns = mousers.StaggerSpawns,
                    StaticCooldown = mousers.StaticCooldown,
                    targetAreaStyle = mousers.targetAreaStyle
                };
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Spawn Mousers").Behavior.Init(self.gameObject, self.aiActor, self.aiShooter);
                self.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem d) => d.NickName == "Spawn Mousers").Behavior.Start();
                self.aiActor.SetResistance(EffectResistanceType.Fire, 1f);
                self.aiActor.SetResistance(EffectResistanceType.Poison, 1f);
                self.AddComponent<ModifiedMetalGearRatBehavior>();
            }
        }

        public static void MakeRatIntroUnskippable(Action<SpecificIntroDoer, PlayerController, List<tk2dSpriteAnimator>> orig, SpecificIntroDoer self, PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            if (IsFlagSetAtAll(typeof(GoldKey)) && self is ResourcefulRatIntroDoer)
            {
                self.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            }
            orig(self, player, animators);
        }

        public static void StartGoldKeyRatIntro(Action<ResourcefulRatIntroDoer, List<tk2dSpriteAnimator>> orig, ResourcefulRatIntroDoer self, List<tk2dSpriteAnimator> animators)
        {
            if (IsFlagSetAtAll(typeof(GoldKey)))
            {
                self.StartCoroutine(GoldKeyRatIntro(self));
            }
            else
            {
                orig(self, animators);
            }
        }

        public static IEnumerator GoldKeyRatIntro(ResourcefulRatIntroDoer rat)
        {
            rat.GetComponent<GenericIntroDoer>().portraitSlideSettings = new PortraitSlideSettings
            {
                bgColor = new Color(1f, 0.75f, 0f),
                bossSubtitleString = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossSubtitleString,
                bossArtSprite = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossArtSprite,
                bossNameString = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossNameString,
                bossQuoteString = "#GOLD_KEY_POWEREDUPANDREADY",
                bossSpritePxOffset = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossSpritePxOffset,
                bottomRightTextPxOffset = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.bottomRightTextPxOffset,
                topLeftTextPxOffset = rat.GetComponent<GenericIntroDoer>().portraitSlideSettings.topLeftTextPxOffset
            };
            rat.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            TextBoxManager.TIME_INVARIANT = true;
            var args = new object[] { false };
            string introKey = (string)typeof(ResourcefulRatIntroDoer).GetMethod("SelectIntroString", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(rat, args);
            yield return rat.StartCoroutine(rat.DoRatTalk(introKey, (bool)args[0]));
            rat.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            yield return new WaitForSecondsRealtime(1.5f);
            yield return rat.StartCoroutine(rat.DoRatTalk("#GOLD_KEY_RAT_TALK", true));
            rat.GetComponent<GenericIntroDoer>().SuppressSkipping = true;
            TextBoxManager.TIME_INVARIANT = false;
            Transform copySprite = CreateEmptySprite(PickupObjectDatabase.GetById(GoldKeyId), GameManager.Instance.PrimaryPlayer.CenterPosition);
            foreach(PlayerController p in GameManager.Instance.AllPlayers)
            {
                if (p.HasPassiveItem(GoldKeyId))
                {
                    p.RemovePassiveItem(GoldKeyId);
                    p.Ext().HasBeenKeyRobbed = true;
                }
            }
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 2f;
            while (elapsed < duration)
            {
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                if (copySprite)
                {
                    Vector3 position = rat.sprite.WorldCenter + new Vector2(0f, -3f);
                    float t = elapsed / duration;
                    copySprite.position = Vector2Extensions.SmoothStep(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, Mathf.SmoothStep(0f, 360f, t));
                }
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1f);
            tk2dSprite sprite = copySprite.GetComponentInChildren<tk2dSprite>();
            sprite.usesOverrideMaterial = true;
            sprite.renderer.material = new Material(sprite.renderer.material) { shader = ShaderCache.Acquire("Brave/LitBlendUber") };
            sprite.renderer.material.SetFloat("_VertexColor", 1f);
            AkSoundEngine.PostEvent("Play_CHR_ghost_appear_01", rat.gameObject);
            yield return rat.StartCoroutine(HandleFadeSpritesAdvanced(new List<Tuple<tk2dSprite, bool>> { new Tuple<tk2dSprite, bool>(sprite, false), new Tuple<tk2dSprite, bool>(CreateEmptySprite(SpecialEnemies.LockGhostPrefab.GetComponent<tk2dBaseSprite>().Collection, 
                SpecialEnemies.LockGhostPrefab.GetComponent<tk2dBaseSprite>().spriteId, rat.sprite.WorldCenter).GetComponentInChildren<tk2dSprite>(), true)}));
            GameObject fadeObject = new GameObject(rat.sprite.name + "_fade");
            tk2dSprite fade = tk2dSprite.AddComponent(fadeObject, rat.sprite.Collection, rat.sprite.spriteId);
            fade.usesOverrideMaterial = true;
            fade.renderer.material = new Material(fade.renderer.material);
            fade.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            fade.renderer.material.SetFloat("_VertexColor", 1f);
            fade.renderer.material.SetColor("_OverrideColor", new Color(99, 99, 99, 1f));
            fade.color = new Color(99, 99, 99, 1f);
            fade.HeightOffGround = rat.sprite.HeightOffGround + 10f;
            fade.PlaceAtPositionByAnchor(rat.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            fade.UpdateZDepth();
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Thunder_01", rat.gameObject);
            yield return rat.StartCoroutine(HandleFadeSprites(new List<tk2dSprite> { fade }, 5f));
            yield return new WaitForSecondsRealtime(0.5f);
            rat.GetComponent<GenericIntroDoer>().SuppressSkipping = false;
            (rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Tail Whip").Behavior as ShootBehavior).BulletScript = 
                new CustomBulletScriptSelector(typeof(ResourcefulRatTail2));
            (rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Quick Spin").Behavior as ShootBehavior).BulletScript = 
                new CustomBulletScriptSelector(typeof(ResourcefulRatSpinFire2));
            (rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Mouse Traps").Behavior as ShootBehavior).BulletScript =
                new CustomBulletScriptSelector(typeof(ResourcefulRatMouseTraps2));
            (rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Daggers").Behavior as ShootBehavior).BulletScript =
                new CustomBulletScriptSelector(typeof(ResourcefulRatDaggers2));
            (rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Quick Throw Dagger").Behavior as ShootBehavior).BulletScript =
                new CustomBulletScriptSelector(typeof(ResourcefulRatQuickDaggers2));
            ((rat.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Cheese Wheel").Behavior as SequentialAttackBehaviorGroup).AttackBehaviors[1] as 
                ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ResourcefulRatCheeseWheel2));
            rat.aiActor.SetResistance(EffectResistanceType.Poison, 1f);
            typeof(ResourcefulRatIntroDoer).GetField("m_isFinished", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(rat, true);
            yield break;
        }

        public static bool ForceConvertDragun(Func<DraGunController, bool> orig, DraGunController self)
        {
            if (IsFlagSetAtAll(typeof(GoldKey)) && IsFlagSetAtAll(typeof(EmeraldItem)) && IsFlagSetAtAll(typeof(AquamarineItem)))
            {
                self.StartCoroutine(ConvertToGold(self));
                return true;
            }
            return orig(self);
        }

        public static IEnumerator ConvertToGold(DraGunController self)
        {
            self.enabled = false;
            self.head.OverrideDesiredPosition = null;
            PlayerController target = GameManager.Instance.AllPlayers.Where((PlayerController play) => IsFlagSetForCharacter(play, typeof(GoldKey))).ToList()[0];
            self.healthHaver.IsVulnerable = false;
            self.aiAnimator.PlayVfx("heart_heal", null, null, null);
            if (GameManager.HasInstance && GameManager.Instance.MainCameraController)
            {
                CameraController mainCameraController = GameManager.Instance.MainCameraController;
                mainCameraController.OverridePosition = self.specRigidbody.UnitCenter + new Vector2(0f, 4f);
                mainCameraController.SetManualControl(true, true);
            }
            GameUIRoot.Instance.HideCoreUI("dragun_transition");
            GameUIRoot.Instance.ToggleLowerPanels(false, false, "dragun_transition");
            yield return new WaitForSeconds(3f);
            self.behaviorSpeculator.InterruptAndDisable();
            StaticReferenceManager.DestroyAllEnemyProjectiles();
            self.aiAnimator.StopVfx("heart_heal");
            self.head.aiAnimator.PlayUntilFinished("open_mouth");
            while (self.head.aiAnimator.IsPlaying("open_mouth"))
            {
                self.head.TargetX = self.sprite.WorldCenter.x;
                self.head.UpdateHead();
                yield return null;
            }
            self.head.aiAnimator.PlayUntilCancelled("open_mouth_idle");
            List<int> itemsToSuck = new List<int>
            {
                GoldKeyId,
                EmeraldItem.EmeraldId,
                AquamarineItem.AquamarineId
            };
            List<int> additionalItemsToDrop = new List<int>();
            foreach(int id in itemsToSuck)
            {
                target = GameManager.Instance.AllPlayers.Where((PlayerController play) => play.HasPassiveItem(id)).ToList()[0];
                if(target == null)
                {
                    target = GameManager.Instance.PrimaryPlayer;
                }
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if (p.HasPassiveItem(id))
                    {
                        p.RemovePassiveItem(id);
                        additionalItemsToDrop.Add(id);
                    }
                }
                Transform copySprite = CreateEmptySprite(PickupObjectDatabase.GetById(id), target.CenterPosition);
                Vector3 startPosition = copySprite.transform.position;
                float elapsed = 0f;
                float duration = 0.5f;
                while (elapsed < duration)
                {
                    self.head.TargetX = self.sprite.WorldCenter.x;
                    self.head.UpdateHead();
                    elapsed += BraveTime.DeltaTime;
                    if (copySprite)
                    {
                        Vector3 position = self.head.sprite.WorldCenter;
                        float t = elapsed / duration * (elapsed / duration);
                        copySprite.position = Vector3.Lerp(startPosition, position, t);
                        copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                        copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    }
                    yield return null;
                }
                if (copySprite)
                {
                    Destroy(copySprite.gameObject);
                }
                AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", self.gameObject);
            }
            self.head.aiAnimator.PlayUntilFinished("close_mouth");
            while (self.head.aiAnimator.IsPlaying("close_mouth"))
            {
                self.head.TargetX = null;
                self.head.UpdateHead();
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            List<tk2dSprite> fadeSprites = new List<tk2dSprite>();
            foreach (tk2dSprite sprite in self.GetComponentsInChildren<tk2dSprite>())
            {
                GameObject fadeObject = new GameObject(sprite.name + "_fade");
                tk2dSprite fade = tk2dSprite.AddComponent(fadeObject, sprite.Collection, sprite.spriteId);
                fade.usesOverrideMaterial = true;
                fade.renderer.material = new Material(fade.renderer.material);
                fade.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
                fade.renderer.material.SetFloat("_VertexColor", 1f);
                fade.renderer.material.SetColor("_OverrideColor", new Color(99, 99, 99, 1f));
                fade.color = new Color(99, 99, 99, 1f);
                fade.HeightOffGround = sprite.HeightOffGround + 10f;
                fade.PlaceAtPositionByAnchor(sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                fadeSprites.Add(fade);
            }
            StaticReferenceManager.DestroyAllEnemyProjectiles();
            List<AIActor> enemies = self.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].name.Contains("knife", true))
                {
                    enemies[i].healthHaver.ApplyDamage(1000f, Vector2.zero, "Dragun Near-Death", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, false);
                }
            }
            GameManager.Instance.PreventPausing = true;
            StaticReferenceManager.DestroyAllEnemyProjectiles();
            self.aiActor.ToggleRenderers(false);
            self.head.OverrideDesiredPosition = new Vector2?(self.transform.position + new Vector3(3.63f, 10.8f));
            GameManager.Instance.DungeonMusicController.SwitchToDragunTwo();
            GameManager.Instance.PreventPausing = false;
            AIActor advancedDraGun = AIActor.Spawn(self.AdvancedDraGunPrefab, self.specRigidbody.UnitBottomLeft, self.aiActor.ParentRoom, false, AIActor.AwakenAnimationType.Default, false);
            advancedDraGun.transform.position = self.transform.position;
            advancedDraGun.specRigidbody.Reinitialize();
            self.healthHaver.EndBossState(false);
            Destroy(self.gameObject);
            GameUIRoot.Instance.ShowCoreUI("dragun_transition");
            GameUIRoot.Instance.ToggleLowerPanels(true, true, "dragun_transition");
            advancedDraGun.GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT, true);
            DestroyImmediate(advancedDraGun.GetComponent<SpecificIntroDoer>());
            typeof(GenericIntroDoer).GetField("m_specificIntroDoer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(advancedDraGun.GetComponent<GenericIntroDoer>(), advancedDraGun.AddComponent<UltimateDraGunIntroDoer>());
            advancedDraGun.GetComponent<GenericIntroDoer>().TriggerSequence(target);
            GemDropper dragunGemDropper = advancedDraGun.GetComponent<GemDropper>();
            if(dragunGemDropper != null)
            {
                if(dragunGemDropper.AdditionalItems == null)
                {
                    dragunGemDropper.AdditionalItems = new List<int>();
                }
                dragunGemDropper.AdditionalItems.AddRange(additionalItemsToDrop);
            }
            AttackBehaviorGroup group = advancedDraGun.behaviorSpeculator.AttackBehaviorGroup;
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Head Sweep").Behavior as DraGunHeadShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunSweepFlameBreath3));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Flame Blast").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunFlameBreath3));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Big Nose Shot").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunBigNoseShot3));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "RPG").Behavior as DraGunRPGBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunRocket3));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Grenade").Behavior as DraGunGrenadeBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunGrenade2));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Spotlight").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunSpotlight2));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Roar").Behavior as DraGunRoarBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunRoar2));
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "NEGATRON PLATFORMON Mk. II").Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(DraGunNegativeSpace3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Mac10 (Both)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunMac10Behavior).BulletScript = 
                new CustomBulletScriptSelector(typeof(DraGunMac10Burst3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Mac10 (Both)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunMac10Behavior).BulletScript =
                new CustomBulletScriptSelector(typeof(DraGunMac10Burst3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[0].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[1].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[2].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[0].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardRight3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[1].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardRight3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Alternating, Fixed Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[2].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedHardRight3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[0].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[1].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunGlockBehavior).attacks[2].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedLeft3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[0].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedRight3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[1].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedRight3));
            ((group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Glock (Aim Fire)").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunGlockBehavior).attacks[2].bulletScript =
                new CustomBulletScriptSelector(typeof(DraGunGlockDirectedRight3));
            DraGunThrowKnifeBehavior knifeThrow1 = (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Knives").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] as DraGunThrowKnifeBehavior;
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Knives").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] = new UltimateDraGunThrowKnifeBehavior
            {
                AccumulateHealthThresholds = knifeThrow1.AccumulateHealthThresholds,
                aiAnimator = knifeThrow1.aiAnimator,
                aiShootAnim = knifeThrow1.aiShootAnim,
                angle = knifeThrow1.angle,
                AttackCooldown = knifeThrow1.AttackCooldown,
                BulletName = knifeThrow1.BulletName,
                Cooldown = knifeThrow1.Cooldown,
                CooldownVariance = knifeThrow1.CooldownVariance,
                delay = knifeThrow1.delay,
                GlobalCooldown = knifeThrow1.GlobalCooldown,
                GroupCooldown = knifeThrow1.GroupCooldown,
                GroupName = knifeThrow1.GroupName,
                HealthThresholds = knifeThrow1.HealthThresholds,
                InitialCooldown = knifeThrow1.InitialCooldown,
                InitialCooldownVariance = knifeThrow1.InitialCooldownVariance,
                IsBlackPhantom = knifeThrow1.IsBlackPhantom,
                MaxEnemiesInRoom = knifeThrow1.MaxEnemiesInRoom,
                MaxHealthThreshold = knifeThrow1.MaxHealthThreshold,
                MaxUsages = knifeThrow1.MaxUsages,
                MinHealthThreshold = knifeThrow1.MaxEnemiesInRoom,
                MinRange = knifeThrow1.MinRange,
                MinWallDistance = knifeThrow1.MinWallDistance,
                Range = knifeThrow1.Range,
                RequiresLineOfSight = knifeThrow1.RequiresLineOfSight,
                resetCooldownOnDamage = knifeThrow1.resetCooldownOnDamage,
                ShootPoint = knifeThrow1.ShootPoint,
                targetAreaStyle = knifeThrow1.targetAreaStyle,
                unityAnimation = knifeThrow1.unityAnimation,
                unityShootAnim = knifeThrow1.unityShootAnim
            };
            DraGunThrowKnifeBehavior knifeThrow2 = (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Knives").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] as DraGunThrowKnifeBehavior;
            (group.AttackBehaviors.Find((AttackBehaviorGroup.AttackGroupItem item) => item.NickName == "Throw Knives").Behavior as SimultaneousAttackBehaviorGroup).AttackBehaviors[1] = new UltimateDraGunThrowKnifeBehavior
            {
                AccumulateHealthThresholds = knifeThrow2.AccumulateHealthThresholds,
                aiAnimator = knifeThrow2.aiAnimator,
                aiShootAnim = knifeThrow2.aiShootAnim,
                angle = knifeThrow2.angle,
                AttackCooldown = knifeThrow2.AttackCooldown,
                BulletName = knifeThrow2.BulletName,
                Cooldown = knifeThrow2.Cooldown,
                CooldownVariance = knifeThrow2.CooldownVariance,
                delay = knifeThrow2.delay,
                GlobalCooldown = knifeThrow2.GlobalCooldown,
                GroupCooldown = knifeThrow2.GroupCooldown,
                GroupName = knifeThrow2.GroupName,
                HealthThresholds = knifeThrow2.HealthThresholds,
                InitialCooldown = knifeThrow2.InitialCooldown,
                InitialCooldownVariance = knifeThrow2.InitialCooldownVariance,
                IsBlackPhantom = knifeThrow2.IsBlackPhantom,
                MaxEnemiesInRoom = knifeThrow2.MaxEnemiesInRoom,
                MaxHealthThreshold = knifeThrow2.MaxHealthThreshold,
                MaxUsages = knifeThrow2.MaxUsages,
                MinHealthThreshold = knifeThrow2.MaxEnemiesInRoom,
                MinRange = knifeThrow2.MinRange,
                MinWallDistance = knifeThrow2.MinWallDistance,
                Range = knifeThrow2.Range,
                RequiresLineOfSight = knifeThrow2.RequiresLineOfSight,
                resetCooldownOnDamage = knifeThrow2.resetCooldownOnDamage,
                ShootPoint = knifeThrow2.ShootPoint,
                targetAreaStyle = knifeThrow2.targetAreaStyle,
                unityAnimation = knifeThrow2.unityAnimation,
                unityShootAnim = knifeThrow2.unityShootAnim
            };
            advancedDraGun.GetComponent<DraGunController>().SpotlightRadius *= 1.35f;
            if (self.HasConvertedBaby)
            {
                BabyDragunController babyDragunController = FindObjectOfType<BabyDragunController>();
                if (babyDragunController)
                {
                    Destroy(babyDragunController.gameObject);
                }
            }
            if(fadeSprites.Count > 0)
            {
                GameManager.Instance.Dungeon.StartCoroutine(HandleFadeSprites(fadeSprites));
            }
            yield break;
        }

        public static IEnumerator HandleFadeSprites(List<tk2dSprite> fadeSprites, float overrideDestination = 2f)
        {
            float ela = 0f;
            float dura = 0.5f;
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                foreach(tk2dSprite s in fadeSprites)
                {
                    Vector2 center = s.WorldCenter;
                    s.scale = Vector2.one * Mathf.Lerp(1f, overrideDestination, ela / dura);
                    s.sprite.color = s.sprite.color.WithAlpha(Mathf.Lerp(1f, 0f, ela / dura));
                    s.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                }
                yield return null;
            }
            for (int i = 0; i < fadeSprites.Count; i++)
            {
                Destroy(fadeSprites[i].gameObject);
            }
            yield break;
        }

        public static IEnumerator HandleFadeSpritesAdvanced(List<Tuple<tk2dSprite, bool>> fadeSprites)
        {
            float ela = 0f;
            float dura = 0.5f;
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                foreach (Tuple<tk2dSprite, bool> tu in fadeSprites)
                {
                    tk2dSprite s = tu.First;
                    float t = (tu.Second ? dura - ela : ela) / dura;
                    Vector2 center = s.WorldCenter;
                    s.scale = Vector2.one * Mathf.Lerp(1f, tu.Second ? 5f : 2f, t);
                    s.sprite.color = s.sprite.color.WithAlpha(Mathf.Lerp(1f, 0f, t));
                    s.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                }
                yield return null;
            }
            for (int i = 0; i < fadeSprites.Count; i++)
            {
                Destroy(fadeSprites[i].First.gameObject);
            }
            yield break;
        }

        public static Transform CreateEmptySprite(PickupObject target, Vector2 startpos)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = startpos;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            return gameObject2.transform;
        }

        public static Transform CreateEmptySprite(tk2dSpriteCollectionData coll, int id, Vector2 startpos)
        {
            GameObject gameObject = new GameObject("suck image");
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(coll, id);
            tk2dSprite.transform.position = startpos;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            return gameObject2.transform;
        }

        public static bool EnableSellCreepInjection(Func<ProceduralFlowModifierData, bool> orig, ProceduralFlowModifierData self)
        {
            bool result = orig(self);
            if(self.annotation == "Sell Creep (Nakatomi)" && !result)
            {
                for (int i = 0; i < self.prerequisites.Length; i++)
                {
                    if (!self.prerequisites[i].CheckConditionsFulfilled())
                    {
                        return false;
                    }
                }
                return !self.RequiresMasteryToken || IsFlagSetAtAll(typeof(GoldKey));
            }
            return result;
        }

        public static void UnlockSellCell(Action<SellCellController> orig, SellCellController self)
        {
            orig(self);
            if(self.SellPitDweller != null && !self.SellPitDweller.PreventInteraction && GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
            {
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if (IsFlagSetForCharacter(p, typeof(GoldKey)) && Vector2.Distance(self.specRigidbody.UnitCenter, p.CenterPosition) < 3f)
                    {
                        self.StartCoroutine((IEnumerator)typeof(SellCellController).GetMethod("HandleSellPitOpening", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[0]));
                    }
                }
            }
        }

        public static void UnlockRatLock(Action<InteractableLock, PlayerController> orig, InteractableLock self, PlayerController interactor)
        {
            orig(self, interactor);
            if(self.lockMode == InteractableLock.InteractableLockMode.RESOURCEFUL_RAT && IsFlagSetForCharacter(interactor, typeof(GoldKey)))
            {
                self.ForceUnlock();
                Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab"), self.sprite != null ? self.sprite.WorldCenter.ToVector3ZUp(0f) : self.transform.position, Quaternion.identity);
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", interactor.gameObject);
                AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", interactor.gameObject);
            }
        }

        public void RecreateGhostOnPickup()
        {
            m_recreateGhostInstantly = true;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(GoldKey));
            player.OnNewFloorLoaded += RecreateGhostOnNewFloor;
            player.OnEnteredCombat += CombatEntered;
            if (m_recreateGhostInstantly)
            {
                RecreateGhostOnNewFloor(player);
                m_recreateGhostInstantly = false;
            }
        }

        public void CombatEntered()
        {
            if(Owner != null && Owner.CurrentRoom != null && Owner.CurrentRoom.area != null && Owner.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && LockGhostController.Instance != null)
            {
                if(GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.tileIndices != null && (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON ||
                    GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON))
                {
                    LockGhostController.Instance.Fade();
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.OnNewFloorLoaded -= RecreateGhostOnNewFloor;
            player.OnEnteredCombat -= CombatEntered;
            DecrementFlag(player, typeof(GoldKey));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            result.GetComponent<GoldKey>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<GoldKey>().Shatter;
            if(LockGhostController.Instance != null)
            {
                LockGhostController.Instance.Fade();
            }
            return result;
        }

        public override void Update()
        {
            base.Update();
            if(Owner != null && PickedUp)
            {
                if (GameManager.HasInstance && GameManager.Instance.Dungeon != null)
                {
                    if(LockGhostController.Instance != null && GameManager.Instance.Dungeon.IsEndTimes)
                    {
                        ProcessUnlockQuestFailure(LockGhostController.Instance, Owner);
                    }
                    if(Owner.CurrentRoom != null)
                    {
                        if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
                        {
                            if (StaticReferenceManager.AllRatTrapdoors != null)
                            {
                                foreach (ResourcefulRatMinesHiddenTrapdoor trapdoor in StaticReferenceManager.AllRatTrapdoors)
                                {
                                    if (trapdoor != null && trapdoor.GetAbsoluteParentRoom() == Owner.CurrentRoom)
                                    {
                                        trapdoor.RevealPercentage = 1f;
                                        trapdoor.OnNearbyExplosion(trapdoor.transform.position.XY() + new Vector2(2f, 2f));
                                        trapdoor.Lock.Suppress = false;
                                        trapdoor.Lock.ForceUnlock();
                                    }
                                }
                            }
                            if (Owner.CurrentRoom.PreventMinimapUpdates && !string.IsNullOrEmpty(Owner.CurrentRoom.GetRoomName()) && Owner.CurrentRoom.GetRoomName().Contains("Rat") && Owner.CurrentRoom.connectedRooms != null &&
                                Owner.CurrentRoom.connectedRooms.Where((RoomHandler r) => r.IsSecretRoom).ToList().Count > 0)
                            {
                                Owner.ForceBlank();
                            }
                        }
                    }
                }
            }
        }

        public void RecreateGhostOnNewFloor(PlayerController play)
        {
            if(LockGhostController.Instance == null && !Dungeon.IsGenerating && play != null)
            {
                GameManager.Instance.Dungeon.StartCoroutine(DelayedCreateGhost(play));
            }
        }

        public IEnumerator DelayedCreateGhost(PlayerController play)
        {
            while(play.CurrentRoom == null)
            {
                yield return null;
            }
            GameObject ghost = Instantiate(SpecialEnemies.LockGhostPrefab, play.CurrentRoom.GetRandomVisibleClearSpot(2, 2).ToVector3(), Quaternion.identity);
            ghost.GetComponent<Renderer>().enabled = false;
            ProcessUnlockQuestFailure(ghost.GetComponent<LockGhostController>(), play);
            yield break;
        }

        public void ProcessUnlockQuestFailure(LockGhostController ghost, PlayerController play)
        {
            bool unlockQuestFailed = false;
            GlobalDungeonData.ValidTilesets tileset = GameManager.Instance.Dungeon.tileIndices.tilesetId;
            if ((tileset == GlobalDungeonData.ValidTilesets.CATACOMBGEON || tileset == GlobalDungeonData.ValidTilesets.FORGEGEON || tileset == GlobalDungeonData.ValidTilesets.HELLGEON) && !IsFlagSetForCharacter(play, typeof(EmeraldItem)))
            {
                unlockQuestFailed = true;
            }
            if((tileset == GlobalDungeonData.ValidTilesets.FORGEGEON || tileset == GlobalDungeonData.ValidTilesets.HELLGEON) && !IsFlagSetForCharacter(play, typeof(AquamarineItem)))
            {
                unlockQuestFailed = true;
            }
            if((GameManager.Instance.Dungeon.IsEndTimes || tileset == GlobalDungeonData.ValidTilesets.HELLGEON) && !IsFlagSetForCharacter(play, typeof(RubyItem)))
            {
                unlockQuestFailed = true;
            }
            if (unlockQuestFailed)
            {
                ghost.QuestFailed(GameManager.Instance.Dungeon.IsEndTimes);
            }
            else if (GameManager.Instance.Dungeon.IsEndTimes)
            {
                SaveAPIManager.SetFlag("LockUnlocked", true);
                ghost.Fade();
                Owner.RemovePassiveItem(PickupObjectId);
            }
        }

        public override void OnDestroy()
        {
            if (Owner != null)
            {
                DecrementFlag(Owner, typeof(GoldKey));
                Owner.OnNewFloorLoaded -= RecreateGhostOnNewFloor;
                Owner.OnEnteredCombat -= CombatEntered;
            }
            base.OnDestroy();
        }

        public void Shatter(DebrisObject debris)
        {
            GameObject soundSource = new GameObject("sound source");
            soundSource.transform.position = sprite.WorldCenter;
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", soundSource);
            Destroy(soundSource, 3f);
            Instantiate(ShatterVFX, debris.sprite.WorldCenter, Quaternion.identity);
            Destroy(debris.gameObject);
        }

        public GameObject ShatterVFX;
        public static int GoldKeyId;
        private bool m_recreateGhostInstantly;
    }
}
