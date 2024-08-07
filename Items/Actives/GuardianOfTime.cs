﻿using Dungeonator;
using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class GuardianOfTime : PlayerItem
    {
        public static void Init()
        {
            string name = "The Guardian of Time";
            string shortdesc = "Solution? Book!";
            string longdesc = "Can open doors and solve puzzles.\n\nThis book contains a solution to every problem: hit it with the book.";
            GuardianOfTime item = EasyItemInit<GuardianOfTime>("items/guardianbook", "sprites/guardian_of_time_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.SetCooldownType(CooldownType.PerRoom, 1f);
            AdditionalBraveLight light = item.transform.Find("Light Source").AddComponent<AdditionalBraveLight>();
            light.transform.position = item.sprite.WorldCenter;
            light.LightColor = new Color(0.5f, 0f, 1f);
            light.LightIntensity = 4.25f;
            light.LightRadius = 5.4375f;
            item.SynergySlow = new()
            {
                DoesCirclePass = false,
                DoesSepia = true,
                audioEvent = "Play_OBJ_time_bell_01",
                EffectRadius = 100f,
                UpdatesForNewEnemies = true,
                RadialSlowInTime = 0f,
                RadialSlowHoldTime = 1f,
                RadialSlowOutTime = 0.5f,
                RadialSlowTimeModifier = 0.25f
            };
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (GetComponentInChildren<AdditionalBraveLight>() != null)
            {
                GetComponentInChildren<AdditionalBraveLight>().LightIntensity = 0f;
                GetComponentInChildren<AdditionalBraveLight>().LightRadius = 0f;
            }
        }

        public override void OnPreDrop(PlayerController player)
        {
            GetComponentInChildren<AdditionalBraveLight>().LightIntensity = 4.25f;
            GetComponentInChildren<AdditionalBraveLight>().LightRadius = 5.4375f;
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            if(user.CurrentRoom != null)
            {
                if (user.PlayerHasActiveSynergy("It Guards Time!"))
                {
                    SynergySlow.DoRadialSlow(user.CenterPosition, user.CurrentRoom);
                }
                if (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) is DungeonDoorController && 
                    (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as DungeonDoorController).isLocked)
                {
                    (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as DungeonDoorController).Unlock();
                }
                else if (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) is InteractableLock &&
                    user.CurrentRoom.GetRoomInteractables().Where((IPlayerInteractable interact) => interact is InteractableDoorController && (interact as InteractableDoorController).WorldLocks != null &&
                    (interact as InteractableDoorController).WorldLocks.Contains(user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as InteractableLock)).ToList().Count > 0)
                {
                    (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as InteractableLock).ForceUnlock();
                }
                else if (user.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
                {
                    roomToReset = user.CurrentRoom;
                    if (user.CurrentRoom.IsSealed)
                    {
                        user.CurrentRoom.UnsealRoom();
                        GameObject roomResetter = new("room resetter");
                        roomResetter.transform.position = user.CurrentRoom.GetCenterCell().ToVector2();
                        roomResetter.AddComponent<LeaveRoomResetter>().parentRoom = user.CurrentRoom;
                    }
                    user.CurrentRoom.UnsealOneWayDoors();
                }
            }
        }

        public void ResetRoomOnLeave()
        {
            roomToReset?.ResetPredefinedRoomLikeDarkSouls();
            roomToReset = null;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user == null || user.CurrentRoom == null || !base.CanBeUsed(user))
            {
                return false;
            }
            bool anythingToUnlock = false;
            if (user.CurrentRoom.IsSealed && user.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
            {
                anythingToUnlock |= true;
            }
            if (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) is DungeonDoorController && 
                (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as DungeonDoorController).isLocked)
            {
                anythingToUnlock |= true;
            }
            if (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) is InteractableLock && 
                user.CurrentRoom.GetRoomInteractables().Where((IPlayerInteractable interact) => interact is InteractableDoorController && (interact as InteractableDoorController).WorldLocks != null && 
                (interact as InteractableDoorController).WorldLocks.Contains(user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as InteractableLock)).ToList().Count > 0)
            {
                anythingToUnlock |= true;
            }
            if (user.CurrentRoom.connectedDoors != null && user.CurrentRoom.connectedDoors.Where((DungeonDoorController door) => door.OneWayDoor).ToList().Count > 0)
            {
                anythingToUnlock |= true;
            }
            return anythingToUnlock;
        }

        private static RoomHandler roomToReset;
        public RadialSlowInterface SynergySlow;
    }
}
