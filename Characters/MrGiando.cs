using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    [HarmonyPatch]
    public class MrGiando : PlayerController
    {
        public static void Init()
        {
            var giando = CharacterBuilder.BuildCharacter<MrGiando>("PlayerGiando", "mr_giando", "GiandoCollection", "GiandoAnimation", "playersprites/giando/sprites/", PlayableCharactersE.CustomCharacterGiando,
                    new(12, 0, 14, 4), new(14, 5, 8, 10),
                    "player_giando_portrait", "playersprites/giando/bosscard/", 10, "GiandoMinimapIcon", "giando_minimap_001", "GiandoStats", new() { { PlayerStats.StatType.MovementSpeed, 10f } }, 3f, 0, 2, 1, new(), new(), 0, 0, false, "giando", "giando_hand_001", "giando_hand_001", false, false, "bug");

            var afterimage = giando.AddComponent<AfterImageTrailController>();
            afterimage.dashColor = new(1f, 0f, 0f);

            giando.maxSpeed = 2.75f;
            giando.timeToGetToMaxSpeed = 1.5f;
            giando.percentageForPartialBonus = 0.5f;

            var crate = AssetBundleManager.Load<GameObject>("guncrate");
            var coll = EasyCollectionSetup("PlaceableCollection");
            tk2dSprite.AddComponent(crate, coll, AddSpriteToCollection("guncrate_001", coll));
            var shadow = crate.transform.Find("Shadow").gameObject;
            var ssprite = tk2dSprite.AddComponent(shadow, coll, SpriteBuilder.AddSpriteToCollection("guncrate_shadow_001", coll, "tk2d/BlendVertexColorTilted"));
            ssprite.HeightOffGround = -3;
            ssprite.UpdateZDepth();
            var cratecomp = crate.AddComponent<FreeGunCrate>();
            cratecomp.itemToGive = Item["gshotgun"].gameObject;
            cratecomp.unsealRoom = true;
            cratecomp.startGiandoCountdown = true;
            giando.freegunCrate = cratecomp;

            var skull = giando.skullprefab = AssetBundleManager.Load<GameObject>("GiandoTimerSkullIcon");
            var sprite = skull.SetupDfSpriteFromTexture<dfSprite>(AssetBundleManager.Load<Texture2D>("certain_death_001"), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            sprite.pivot = dfPivotPoint.MiddleCenter;

            var mrdeathando = giando.giandoTimesupSprite = AssetBundleManager.Load<GameObject>("GiandoTimeupSprite");
            var deathandosprite = mrdeathando.GetOrAddComponent<dfSprite>();
            var atlas = mrdeathando.GetOrAddComponent<dfAtlas>();
            atlas.Material = new(ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            atlas.Material.mainTexture = AssetBundleManager.Load<Texture2D>("giando_timeup");
            atlas.Items.Clear();
            atlas.AddItem(new()
            {
                border = new RectOffset(),
                deleted = false,
                name = "1",
                region = new Rect(Vector2.zero, new Vector2(0.5f, 1f)),
                rotated = false,
                sizeInPixels = new Vector2(31, 27),
                texture = null,
                textureGUID = "1"
            });
            atlas.AddItem(new()
            {
                border = new RectOffset(),
                deleted = false,
                name = "2",
                region = new Rect(new(32f / 62f, 0f), new Vector2(30f / 62f, 1f)),
                rotated = false,
                sizeInPixels = new Vector2(31, 27),
                texture = null,
                textureGUID = "2"
            });
            deathandosprite.Atlas = atlas;
            deathandosprite.SpriteName = "1";
            deathandosprite.zindex = 3;
            deathandosprite.pivot = dfPivotPoint.BottomCenter;

            var timeuptext = giando.giantTimesupText = AssetBundleManager.Load<GameObject>("GiandoTimeupText");
            var timeupsprite = timeuptext.SetupDfSpriteFromTexture<dfSprite>(AssetBundleManager.Load<Texture2D>("times_up_001"), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            timeupsprite.pivot = dfPivotPoint.BottomCenter;

            giando.AddToBreach("GiandoBreachCharacter", new(12.5f, 24f), new(), new(8, 1, 10, 4), new(7, 1, 12, 19), new(-1, 0), "GiandoBreachCharacterPanel", "Mr. Giando", "giando", 1, "giando_items");
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.CheckSpawnEmergencyCrate))]
        [HarmonyPrefix]
        public static bool NoNinjaCrate(PlayerController __instance)
        {
            if (__instance is MrGiando)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.HandleAnimations))]
        [HarmonyPrefix]
        public static void ChangeNinjaGunAngle(PlayerController __instance, ref float gunAngle)
        {
            if (__instance is MrGiando)
            {
                bool showGun = __instance.m_playerCommandedDirection == Vector2.zero || (__instance.CurrentGun != null && __instance.CurrentGun.spriteAnimator != null && (__instance.CurrentGun.spriteAnimator.IsPlaying(__instance.CurrentGun.shootAnimation) || __instance.CurrentGun.spriteAnimator.IsPlaying(__instance.CurrentGun.introAnimation)));
                __instance.ToggleGunRenderers(showGun, "giando_animation");
                __instance.ToggleHandRenderers(showGun, "giando_animation");
                if (!showGun)
                {
                    gunAngle = BraveMathCollege.Atan2Degrees(__instance.m_playerCommandedDirection);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.HandleFlipping))]
        [HarmonyPrefix]
        public static void ChangeNinjaFlipping(PlayerController __instance, ref float gunAngle)
        {
            if (__instance is MrGiando)
            {
                bool showGun = __instance.m_playerCommandedDirection == Vector2.zero || (__instance.CurrentGun != null && __instance.CurrentGun.spriteAnimator != null && (__instance.CurrentGun.spriteAnimator.IsPlaying(__instance.CurrentGun.shootAnimation) || __instance.CurrentGun.spriteAnimator.IsPlaying(__instance.CurrentGun.introAnimation)));
                if (!showGun)
                {
                    gunAngle = BraveMathCollege.Atan2Degrees(__instance.m_playerCommandedDirection);
                }
            }
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToBossMusic))]
        [HarmonyPrefix]
        public static bool NoMusic1()
        {
            if(GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.StartArcadeGame))]
        [HarmonyPrefix]
        public static bool NoMusic2()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToCustomMusic))]
        [HarmonyPrefix]
        public static bool NoMusic3()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToVictoryMusic))]
        [HarmonyPrefix]
        public static bool NoMusic4()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToEndTimesMusic))]
        [HarmonyPrefix]
        public static bool NoMusic5()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToDragunTwo))]
        [HarmonyPrefix]
        public static bool NoMusic6()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.SwitchToBossMusic))]
        [HarmonyPrefix]
        public static bool NoMusic7()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.EndBossMusic))]
        [HarmonyPrefix]
        public static bool NoMusic8()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.EndBossMusicNoVictory))]
        [HarmonyPrefix]
        public static bool NoMusic10()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.EndVictoryMusic))]
        [HarmonyPrefix]
        public static bool NoMusic11()
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DungeonFloorMusicController), nameof(DungeonFloorMusicController.ResetForNewFloor))]
        [HarmonyPostfix]
        public static void OverrideNewFloorMusic(DungeonFloorMusicController __instance)
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer is MrGiando gnd && gnd.hasCountdownStarted)
            {
                GameManager.Instance.FlushMusicAudio();
                //AkSoundEngine.PostEvent("PizzaTowerThousandMarch", __instance.gameObject);
            }
        }

        public override void Die(Vector2 finalDamageDirection)
        {
            skullInstance.IsVisible = timer.IsVisible = lastAlive = false;
            if (useSpecialDeath)
            {
                CurrentInputState = PlayerInputState.NoInput;
                if (CurrentGun)
                {
                    CurrentGun.CeaseAttack(true, null);
                }
                Transform transform = GameManager.Instance.MainCameraController.transform;
                Vector3 position = transform.position;
                GameManager.Instance.MainCameraController.OverridePosition = position;
                GameManager.Instance.MainCameraController.SetManualControl(true, false);
                HandleDeathPhotography();
                GameManager.Instance.FlushMusicAudio();
                GameManager.Instance.PauseRaw(true);
                AkSoundEngine.PostEvent("Stop_SND_All", gameObject);
                GameManager.Instance.FlushAudio();
                AkSoundEngine.PostEvent("PizzaTowerYourFatAssSlowsYouDown", gameObject);
                BraveTime.RegisterTimeScaleMultiplier(0f, GameManager.Instance.gameObject);
                GameManager.Instance.StartCoroutine(GetCrushedByTimesUpText());
            }
            else
            {
                base.Die(finalDamageDirection);
            }
        }

        public IEnumerator GetCrushedByTimesUpText()
        {
            Pixelator.Instance.fade = 0f;
            Pixelator.Instance.m_fadeMaterial.SetColor(Pixelator.Instance.m_fadeColorID, Color.black);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.NUMBER_DEATHS, 1f);
            AmmonomiconDeathPageController.LastKilledPlayerPrimary = IsPrimaryPlayer;
            var sp = GameUIRoot.Instance.Manager.AddPrefab(giandoTimesupSprite) as dfSprite;
            sp.transform.localPosition = Vector3.zero;
            sp.transform.localScale = new(3f, 3f);
            yield return new WaitForSecondsRealtime(2.5f);
            var txt = GameUIRoot.Instance.Manager.AddPrefab(giantTimesupText) as dfSprite;
            txt.transform.localScale = new(2f, 2f);
            txt.zindex = sp.zindex - 1;
            var opos = txt.transform.localPosition = Vector3.up * 1.75f;
            sp.transform.localScale = new(3f, 3f);
            var ela = 0f;
            var dura = 0.4f;
            while(ela < dura)
            {
                txt.transform.localPosition = Vector3.Lerp(opos, Vector3.zero, ela / dura);
                ela += GameManager.INVARIANT_DELTA_TIME;
                yield return null;
            }
            txt.transform.localPosition = Vector3.zero;
            sp.SpriteName = "2";
            ela = 0f;
            dura = 2.5f;
            Vector3 lastVelocity = new(-1.5f, 2.5f, 0f);
            while (ela < dura)
            {
                float dt = GameManager.INVARIANT_DELTA_TIME;
                ela += dt;
                lastVelocity += new Vector3(0f, -5f, 0f) * dt;
                sp.transform.localPosition = lastVelocity * dt + sp.transform.localPosition;
                if (sp.transform.localPosition.y < -2f)
                {
                    break;
                }
                yield return null;
            }
            ela = 0f;
            dura = 0.5f;
            while (ela < dura)
            {
                ela += GameManager.INVARIANT_DELTA_TIME;
                txt.Opacity = Mathf.Lerp(1f, 0f, ela / dura);
                yield return null;
            }
            txt.Opacity = 0f;
            yield return new WaitForSecondsRealtime(0.25f);
            GameManager.Instance.DoGameOver(healthHaver.lastIncurredDamageSource);
            yield break;
        }

        public override void Start()
        {
            base.Start();
            afterimage = GetComponent<AfterImageTrailController>();
            speedModifier = StatModifier.Create(PlayerStats.StatType.MovementSpeed, ModifyMethodE.TrueMultiplicative, 1f);
            ownerlessStatModifiers.Add(speedModifier);
            specRigidbody.OnPreRigidbodyCollision += OnPreRigidbodyCollision;
            almostMaxSpeed = Mathf.Lerp(1f, maxSpeed, percentageForPartialBonus);
            OnAboutToFall += RunOverThePit;
            //specRigidbody.OnPreTileCollision += BreakWalls;
            OnNewFloorLoaded += GunAcquireSequence;
        }

        public void GunAcquireSequence(PlayerController play)
        {
            if (play.inventory.AllGuns.Count <= 0)
            {
                StartCoroutine(AcquireGun());
                OnNewFloorLoaded -= GunAcquireSequence;
            }
        }

        public IEnumerator AcquireGun()
        {
            while (Dungeon.IsGenerating || CurrentRoom == null)
            {
                yield return null;
            }
            var room = CurrentRoom;
            room.SealRoom();
            var unsealEvents = room.area.runtimePrototypeData.roomEvents.Where(x => x.condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && x.action == RoomEventTriggerAction.UNSEAL_ROOM).ToList();
            foreach (var roomEvent in unsealEvents)
            {
                room.area.runtimePrototypeData.roomEvents.Remove(roomEvent);
            }
            var crate = freegunCrate.InstantiateObject(CurrentRoom, room.area.dimensions / 2 + IntVector2.Right * 2, false).GetComponent<FreeGunCrate>();
            crate.unsealEvents = unsealEvents;
            crate.theRoomThatWillBeUnsealed = CurrentRoom;
            CurrentRoom.RegisterInteractable(crate);
            yield break;
        }

        public bool RunOverThePit(bool partial)
        {
            return speedModifier.amount < almostMaxSpeed;
        }

        public override void OnPreRigidbodyCollision(SpeculativeRigidbody thisRB, PixelCollider thisPC, SpeculativeRigidbody otherRB, PixelCollider otherPC)
        {
            base.OnPreRigidbodyCollision(thisRB, thisPC, otherRB, otherPC);
            if (otherRB.aiActor != null)
            {
                if (speedModifier.amount >= almostMaxSpeed)
                {
                    PhysicsEngine.SkipCollision = true;
                    otherRB.RegisterTemporaryCollisionException(thisRB, 0.01f, 0.1f);
                    if (otherRB.knockbackDoer != null)
                    {
                        otherRB.knockbackDoer.ApplyKnockback(thisRB.Velocity.normalized, 50f, false);
                    }
                }
                if (speedModifier.amount >= maxSpeed && otherRB.healthHaver != null)
                {
                    otherRB.healthHaver.ApplyDamage(30f, thisRB.Velocity.normalized, "Weeeeeeeeeeeeeeeeeeee", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }
            else if (otherRB.projectile != null && speedModifier.amount >= almostMaxSpeed)
            {
                PhysicsEngine.SkipCollision = true;
                otherRB.RegisterTemporaryCollisionException(thisRB, 0.01f, 0.1f);
            }
            else if (otherRB.majorBreakable != null)
            {
                if (speedModifier.amount >= almostMaxSpeed)
                {
                    PhysicsEngine.SkipCollision = true;
                    otherRB.RegisterTemporaryCollisionException(thisRB, 0.01f, 0.1f);
                }
                if (speedModifier.amount >= maxSpeed)
                {
                    otherRB.majorBreakable.Break(thisRB.Velocity.normalized);
                }
            }
        }

        public void ActivateCountdown()
        {
            hasCountdownStarted = true;
            if (timer == null)
            {
                var time = GameUIRoot.Instance.Manager.AddControl<dfLabel>();
                time.gameObject.SetActive(false);
                timer = time;
                timer.TextAlignment = TextAlignment.Center;
                timer.Pivot = dfPivotPoint.MiddleLeft;
                timer.AutoSize = true;
                timer.TextScale = 3f;
                var roundedtime = Mathf.Max(Mathf.FloorToInt(timeLeft), 0);
                var seconds = (roundedtime % 60).ToString();
                if (seconds.Length == 1)
                {
                    seconds = $"0{seconds}";
                }
                var minutes = (Mathf.RoundToInt(roundedtime / 60)).ToString();
                if (minutes.Length == 1)
                {
                    minutes = $"0{minutes}";
                }
                timer.Text = $"{minutes}:{seconds}";
                LastRoundedTime = roundedtime;
                var follow = time.AddComponent<dfFollowObject>();
                follow.mainCamera = GameManager.Instance.MainCameraController.Camera;
                follow.attach = gameObject;
                follow.anchor = dfPivotPoint.MiddleRight;
                follow.offset = new(0.8f, 2.5f);
                follow.enabled = true;
                time.gameObject.SetActive(true);

                var skull = GameUIRoot.Instance.Manager.AddPrefab(skullprefab) as dfSprite;
                skull.gameObject.SetActive(false);
                var sfollow = skull.AddComponent<dfFollowObject>();
                sfollow.mainCamera = GameManager.Instance.MainCameraController.Camera;
                sfollow.attach = gameObject;
                sfollow.anchor = dfPivotPoint.MiddleRight;
                sfollow.offset = new(0.4f, 2.475f);
                sfollow.enabled = true;
                skull.gameObject.SetActive(true);
                skullInstance = skull;

                AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.DungeonMusicController.gameObject);
                AkSoundEngine.PostEvent("PizzaTowerThousandMarch", GameManager.Instance.DungeonMusicController.gameObject);
            }
        }

        public override void Update()
        {
            base.Update();
            if (speedModifier.amount < maxSpeed && (IsDodgeRolling || Velocity.magnitude > 0.05f))
            {
                if (IsDodgeRolling)
                {
                    speedModifier.amount = maxSpeed;
                }
                else
                {
                    speedModifier.amount = Mathf.Min(maxSpeed, speedModifier.amount + BraveTime.DeltaTime * (maxSpeed - 1f) / timeToGetToMaxSpeed);
                }
                stats.RecalculateStats(this, false, false);
            }
            else if (speedModifier.amount != 1f && !IsDodgeRolling && Velocity.magnitude <= 0.05f)
            {
                speedModifier.amount = 1f;
                stats.RecalculateStats(this, false, false);
            }
            afterimage.spawnShadows = speedModifier.amount >= almostMaxSpeed;
            if (hasCountdownStarted && healthHaver.IsAlive)
            {
                timeLeft -= BraveTime.DeltaTime;
                if(timer != null)
                {
                    var roundedtime = Mathf.Max(Mathf.FloorToInt(timeLeft), 0);
                    if(roundedtime != LastRoundedTime)
                    {
                        var seconds = (roundedtime % 60).ToString();
                        if(seconds.Length == 1)
                        {
                            seconds = $"0{seconds}";
                        }
                        var minutes = (Mathf.RoundToInt(roundedtime / 60)).ToString();
                        if (minutes.Length == 1)
                        {
                            minutes = $"0{minutes}";
                        }
                        timer.Text = $"{minutes}:{seconds}";
                        LastRoundedTime = roundedtime;
                    }
                }
                if(timeLeft <= -1f && healthHaver.IsAlive)
                {
                    useSpecialDeath = true;
                    healthHaver.ApplyDamage(100000f, Vector2.zero, "Time's Up!", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }
            if (healthHaver.IsAlive != lastAlive)
            {
                skullInstance.IsVisible = timer.IsVisible = lastAlive = healthHaver.IsAlive;
            }
        }

        public float percentageForPartialBonus;
        public float almostMaxSpeed;
        public float maxSpeed;
        public float timeToGetToMaxSpeed;
        public float timeLeft = 40f;
        public int LastRoundedTime = -1;
        public dfLabel timer;
        public GameObject skullprefab;
        public GameObject giandoTimesupSprite;
        public GameObject giantTimesupText;
        public dfSprite skullInstance;
        public bool hasCountdownStarted;
        public StatModifier speedModifier;
        public AfterImageTrailController afterimage;
        public FreeGunCrate freegunCrate;
        public bool useSpecialDeath;
        public bool lastAlive = true;
    }
}
