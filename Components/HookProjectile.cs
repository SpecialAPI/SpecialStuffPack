using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class HookProjectile : LobbedProjectile
    {
        public override void Start()
        {
            base.Start();
            pitTimeRequired = requiredPitTime + Random.Range(-requiredPitTimeVariance, requiredPitTimeVariance);
            if (source != null)
            {
                m_cable = gameObject.AddComponent<ArbitraryCableDrawer>();
                m_cable.Attach1Offset = Vector2.zero;
                m_cable.Attach2Offset = sprite.WorldCenter - sprite.transform.position.XY();
                m_cable.Initialize(source.barrelOffset, sprite.transform);
            }
        }

        public void HookingTime()
        {
            if (!m_hookingTime)
            {
                FishUpReward(finalPit);
                if (!m_isDestroying)
                {
                    hookingStartTime = m_elapsedBounceTime;
                }
                m_hookingTime = true;
                hookStartPos = transform.position;
            }
        }

        public void FishUpReward(IntVector2? finalPit = null)
        {
            if(reward == null)
            {
                return;
            }
            var startPosition = finalPit?.ToVector2() ?? specRigidbody.UnitCenter;
            var endPosition = Vector2.zero;
            var room = startPosition.GetAbsoluteRoom() ?? this.PlayerOwner()?.CurrentRoom;
            if(room == null)
            {
                return;
            }
            IntVector2? nearestAvailableCell = room.GetNearestAvailableCell(startPosition, new IntVector2?(new IntVector2(2, 2)), new CellTypes?(CellTypes.FLOOR), false, null);
            if (nearestAvailableCell != null)
            {
                endPosition = nearestAvailableCell.Value.ToVector2();
            }
            else
            {
                endPosition = room.GetBestRewardLocation(IntVector2.One, RoomHandler.RewardLocationStyle.CameraCenter, true).ToVector2();
            }
            endPosition += Vector2.up;
            if (reward.isRandomItem)
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomItem(endPosition);
            }
            else if (reward.isRoomReward)
            {
                var reward = GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight(false);
                LootEngine.SpawnItem(reward, endPosition, Vector2.down, 0f, true, true, false);
            }
            else if (!string.IsNullOrEmpty(reward.FishEnemyGUID) && EnemyDatabase.GetOrLoadByGuid(reward.FishEnemyGUID) != null)
            {
                var ai = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(reward.FishEnemyGUID), endPosition, room, false, AIActor.AwakenAnimationType.Spawn);
                room.SealRoom();
                ai.behaviorSpeculator?.Stun(0.5f);
                PhysicsEngine.Instance?.RegisterOverlappingGhostCollisionExceptions(ai.specRigidbody);
            }
            else if(PickupObjectDatabase.GetById(reward.FishItemId) != null && ((PickupObjectDatabase.GetById(reward.FishItemId) is not Gun) || (this.PlayerOwner() != null && !this.PlayerOwner().HasPickupID(reward.FishItemId))))
            {
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(reward.FishItemId).gameObject, endPosition, Vector2.down, 0f, true, true, false);
            }
            else
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomItem(endPosition);
            }
        }

        public override void HandleHeight(SpeculativeRigidbody body)
        {
            base.HandleHeight(body);
            if(reward != null)
            {
                var value = Mathf.Lerp(-0.5f, 0f, Mathf.Min(1f, (pitRestTime - pitTimeRequired) / rewardKeepTime * 2f));
                sprite.transform.localPosition += value * Vector3.up;
                sprite.HeightOffGround += value;
                sprite.UpdateZDepth();
            }
        }

        public void Drag()
        {
            isDragging = true;
            everDragged = true;
        }

        public override void Update()
        {
            base.Update();
            if(source == null || this.PlayerOwner() == null || this.PlayerOwner().CurrentGun != source || ((isDragging || m_hookingTime) && m_isDestroying && 
                Vector2.Distance(source.barrelOffset.position, specRigidbody.UnitCenter) < 0.75f) || this.PlayerOwner().CurrentRoom != specRigidbody.UnitCenter.GetAbsoluteRoom())
            {
                DieInAir(false, true, true, true);
            }
            else if(m_hookingTime)
            {
                SendInDirection(source.barrelOffset.position.XY() - specRigidbody.UnitCenter, true, false);
                EnsureSimulatedTime();
                transform.position = Vector2.Lerp(hookStartPos, source.barrelOffset.position, (m_elapsedBounceTime - hookingStartTime) / (simulatedTime.GetValueOrDefault() - hookingStartTime));
                specRigidbody.Reinitialize();
            }
            else if (everDragged && !m_isDestroying)
            {
                if (isDragging)
                {
                    baseData.speed = dragSpeed;
                    UpdateSpeed();
                    SendInDirection(source.barrelOffset.position.XY() - specRigidbody.UnitCenter, true, false);
                    isDragging = false;
                }
                else
                {
                    baseData.speed = 0f;
                    UpdateSpeed();
                }
            }
            if (m_shouldResetBounces)
            {
                this.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 0;
            }
        }

        private float hookingStartTime;
        private bool m_hookingTime;
        private bool isDragging;
        private bool everDragged;
        public bool IsHookingTime => m_hookingTime;
        private Vector3 hookStartPos;
        private bool m_shouldResetBounces;
        public float dragSpeed;
        public float pitRestTime;
        public float requiredPitTime;
        public float requiredPitTimeVariance;
        public float rewardKeepTime;
        private float pitTimeRequired;
        [NonSerialized]
        public Gun source;
        private ArbitraryCableDrawer m_cable;
        private FishingReward reward;
        private IntVector2? finalPit;
        public WeightedFishingRewardCollection rewards;
        public override bool ShouldBeDestroyed => m_hookingTime;
        public override void OnFloorLinger()
        {
            base.OnFloorLinger();
            if(pitRestTime > pitTimeRequired + rewardKeepTime)
            {
                DieInAir(false, true, true, false);
                return;
            }
            if (isDragging)
            {
                baseData.speed = dragSpeed;
                pitRestTime = 0f;
                UpdateSpeed();
                SendInDirection(source.barrelOffset.position.XY() - specRigidbody.UnitCenter, true, false);
                isDragging = false;
            }
            else if (GameManager.Instance?.Dungeon?.data != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)) &&
                GameManager.Instance.Dungeon.data[specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)].type == CellType.PIT)
            {
                finalPit = specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                pitRestTime += BraveTime.DeltaTime;
                if(pitRestTime >= pitTimeRequired && reward == null)
                {
                    reward = rewards.SelectByWeight();
                }
            }
            else
            {
                pitRestTime = 0f;
            }
            if(!(GameManager.Instance?.Dungeon?.data != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)) &&
                GameManager.Instance.Dungeon.data[specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)].type == CellType.PIT) && reward != null)
            {
                HookingTime();
            }
        }
        public override void DoBounceReset()
        {
            base.DoBounceReset();
            this.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 0;
            m_shouldResetBounces = true;
        }
    }
}
