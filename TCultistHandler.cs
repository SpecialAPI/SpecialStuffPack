using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text;
using Dungeonator;
using MonoMod.RuntimeDetour;
using InControl;

namespace SpecialStuffPack
{
    public class TCultistHandler
    {
        public static void Init()
        {
            ETGModConsole.Commands.AddUnit("tcultist_test", delegate (string[] args)
            {
                try
                {
                    foreach (PlayerController controller in GameManager.Instance.AllPlayers)
                    {
                        string a = "Gunslinger";
                        if(args.Length > 0)
                        {
                            a = args[0];
                        }
                        GameObject go = UnityEngine.Object.Instantiate((GameObject)BraveResources.Load("Player" + a), controller.transform.position, controller.transform.rotation);
                        go.transform.position += new Vector3(1f, 0f, 0f);
                        PlayerController pc = go.GetComponent<PlayerController>();
                        controller.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider));
                        controller.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox));
                        pc.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider));
                        pc.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox));
                        TCultistController tcc = ReplacePlayerControllerWithTCC(pc);
                        tcc.PlayerIDX = 1;
                        go.SetActive(true);
                        GameManager.Instance.RefreshAllPlayers();
                        GameManager.Instance.SecondaryPlayer = tcc;
                        GameManager.Instance.PrimaryPlayer = controller;
                        tcc.canHold = true;
                        GameUIRoot.Instance.ConvertCoreUIToCoopMode();
                        typeof(BraveInput).GetField("m_playerID", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(BraveInput.GetInstanceForPlayer(1), 0);
                    }
                }
                catch (Exception ex)
                {
                    ETGModConsole.Log("error: " + ex);
                }
            });
            new Hook(typeof(GameManager).GetProperty("SecondaryPlayer").GetGetMethod(), typeof(TCultistHandler).GetMethod("SecondaryPlayerButGood"));
            new Hook(typeof(GameManager).GetMethod("GetOtherPlayer"), typeof(TCultistHandler).GetMethod("GetOtherPlayerButGood"));
            new Hook(typeof(GameUIRoot).GetMethod("UpdateGunData"), typeof(TCultistHandler).GetMethod("UpdateGunDataButGood"));
            new Hook(typeof(GameUIRoot).GetMethod("UpdateItemData"), typeof(TCultistHandler).GetMethod("UpdateItemDataButGood"));
            new Hook(typeof(PlayerController).GetMethod("ReuniteWithOtherPlayer"), typeof(TCultistHandler).GetMethod("GhostCollisionExceptionOnReunite"));
            new Hook(typeof(RoomHandler).GetMethod("OnEntered", BindingFlags.NonPublic | BindingFlags.Instance), typeof(TCultistHandler).GetMethod("ReuniteWithOtherPlayerOnEnterWithEnemies"));
            new Hook(typeof(PlayerStats).GetMethod("RecalculateStatsInternal", BindingFlags.Public | BindingFlags.Instance), typeof(TCultistHandler).GetMethod("RecalculateStatsRecursive"));
            new Hook(typeof(DodgeRollStats).GetMethod("GetModifiedTime", BindingFlags.Public | BindingFlags.Instance), typeof(TCultistHandler).GetMethod("BalanceModifiedTime"));
            new Hook(typeof(DodgeRollStats).GetMethod("GetModifiedDistance", BindingFlags.Public | BindingFlags.Instance), typeof(TCultistHandler).GetMethod("BalanceModifiedDistance"));
            new Hook(typeof(OneAxisInputControl).GetProperty("WasPressed", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), typeof(TCultistHandler).GetMethod("WasPressedOverride"));
            new Hook(typeof(OneAxisInputControl).GetProperty("IsPressed", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), typeof(TCultistHandler).GetMethod("IsPressedOverride"));
            new Hook(typeof(OneAxisInputControl).GetProperty("WasReleased", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), typeof(TCultistHandler).GetMethod("WasReleasedOverride"));
            new Hook(typeof(PlayerController).GetProperty("CurrentInputState").GetSetMethod(), typeof(TCultistHandler).GetMethod("LogAllInputSource"));
        }

        public static void LogAllInputSource(Action<PlayerController, PlayerInputState> orig, PlayerController self, PlayerInputState state)
        {
            orig(self, state);
            if(state == PlayerInputState.AllInput)
            {
                ETGModConsole.Log("AllInput state source: " + Environment.StackTrace);
            }
        }

        public static bool CheckOverridePressedValue(OneAxisInputControl self, out bool value)
        {
            bool succes = false;
            bool isSecondPlayer = false;
            if (self != null && BraveInput.HasInstanceForPlayer(1) && BraveInput.HasInstanceForPlayer(0))
            {
                if (BraveInput.GetInstanceForPlayer(1) != null && BraveInput.GetInstanceForPlayer(1).ActiveActions != null && BraveInput.GetInstanceForPlayer(0) != null && BraveInput.GetInstanceForPlayer(0).ActiveActions != null) 
                {
                    if (GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
                    {
                        GungeonActions aa = BraveInput.GetInstanceForPlayer(0).ActiveActions;
                        GungeonActions aa2 = BraveInput.GetInstanceForPlayer(1).ActiveActions;
                        succes = self == aa.EquipmentMenuAction || self == aa.UseItemAction || self == aa.ItemUpAction || self == aa.ItemDownAction || self == aa.GunQuickEquipAction || self == aa.GunDownAction || self == aa.GunUpAction ||
                            self == aa.BlankAction;
                        if (!succes)
                        {
                            if (self == aa2.PauseAction || self == aa2.CancelAction)
                            {
                                value = false;
                                return true;
                            }
                            succes = self == aa2.EquipmentMenuAction || self == aa2.UseItemAction || self == aa2.ItemUpAction || self == aa2.ItemDownAction || self == aa2.GunQuickEquipAction || self == aa2.GunDownAction || self == aa2.GunUpAction || 
                                self == aa2.BlankAction;
                            isSecondPlayer = succes;
                        }
                    } 
                }
            }
            value = succes && (BraveInput.HasInstanceForPlayer(0) && BraveInput.GetInstanceForPlayer(0) != null && BraveInput.GetInstanceForPlayer(0).SpecialInput() != null && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions != null
                && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions.HoldSecondPlayerAction.IsPressed) == isSecondPlayer;
            return succes;
        }

        public static bool WasReleasedOverride(Func<OneAxisInputControl, bool> orig, OneAxisInputControl self)
        {
            bool result = orig(self);
            if (result && CheckOverridePressedValue(self, out var succes))
            {
                return succes;
            }
            return result;
        }

        public static bool IsPressedOverride(Func<OneAxisInputControl, bool> orig, OneAxisInputControl self)
        {
            bool result = orig(self);
            if(result && CheckOverridePressedValue(self, out var succes))
            {
                return succes;
            }
            return result;
        }

        public static bool WasPressedOverride(Func<OneAxisInputControl, bool> orig, OneAxisInputControl self)
        {
            bool result = orig(self);
            if (result && CheckOverridePressedValue(self, out var succes))
            {
                return succes;
            }
            return result;
        }

        public static float BalanceModifiedDistance(Func<DodgeRollStats, PlayerController, float> orig, DodgeRollStats self, PlayerController owner)
        {
            float result = orig(self, owner);
            if (GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(owner);
                if (otherPlayer != null && otherPlayer.rollStats != null)
                {
                    float result2 = orig(otherPlayer.rollStats, otherPlayer);
                    return Mathf.Lerp(result, result2, 0.5f);
                }
            }
            return result;
        }

        public static float BalanceModifiedTime(Func<DodgeRollStats, PlayerController, float> orig, DodgeRollStats self, PlayerController owner)
        {
            float result = orig(self, owner);
            if (GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(owner);
                if (otherPlayer != null && otherPlayer.rollStats != null)
                {
                    float result2 = orig(otherPlayer.rollStats, otherPlayer);
                    return Mathf.Lerp(result, result2, 0.5f);
                }
            }
            return result;
        }

        public static void RecalculateStatsRecursive(Action<PlayerStats, PlayerController> orig, PlayerStats self, PlayerController owner)
        {
            orig(self, owner);
            if(GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(owner);
                if (otherPlayer != null && otherPlayer.stats != null)
                {
                    orig(otherPlayer.stats, otherPlayer);
                    FieldInfo statValuesInfo = typeof(PlayerStats).GetField("StatValues", BindingFlags.NonPublic | BindingFlags.Instance);
                    List<float> thisStats = (List<float>)statValuesInfo.GetValue(self);
                    List<float> otherStats = (List<float>)statValuesInfo.GetValue(otherPlayer.stats);
                    try
                    {
                        float thisSpeed = thisStats[(int)PlayerStats.StatType.MovementSpeed];
                        float otherSpeed = otherStats[(int)PlayerStats.StatType.MovementSpeed];
                        float sharedSpeed = Mathf.Lerp(thisSpeed, otherSpeed, 0.5f);
                        thisStats[(int)PlayerStats.StatType.MovementSpeed] = sharedSpeed;
                        otherStats[(int)PlayerStats.StatType.MovementSpeed] = sharedSpeed;
                    }
                    catch { }
                }
            }
        }

        public static void GhostCollisionExceptionOnReunite(Action<PlayerController, PlayerController, bool> orig, PlayerController self, PlayerController other, bool useDefaultVFX)
        {
            orig(self, other, useDefaultVFX);
            if((self is TCultistController || other is TCultistController) && self.specRigidbody != null && other.specRigidbody != null)
            {
                self.specRigidbody.RegisterGhostCollisionException(other.specRigidbody);
                other.specRigidbody.RegisterGhostCollisionException(self.specRigidbody);
            }
        }

        public static void ReuniteWithOtherPlayerOnEnterWithEnemies(Action<RoomHandler, PlayerController> orig, RoomHandler self, PlayerController play)
        {
            orig(self, play);
            if((play != null && play is TCultistController) || (GameManager.HasInstance && GameManager.Instance.GetOtherPlayer(play) != null && GameManager.Instance.GetOtherPlayer(play) is TCultistController))
            {
                List<AIActor> list = self.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                if (list != null)
                {
                    if (list.Exists((AIActor a) => !a.healthHaver.IsDead))
                    {
                        GameManager.Instance.GetOtherPlayer(play).ReuniteWithOtherPlayer(play, false);
                    }
                }
            }
        }

        public static void UpdateItemDataButGood(Action<GameUIRoot, PlayerController, PlayerItem, List<PlayerItem>> orig, GameUIRoot root, PlayerController targetPlayer, PlayerItem item, List<PlayerItem> items)
        {
            GameManager.GameType? whatItWasBefore = null;
            if (GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
            {
                whatItWasBefore = GameManager.Instance.CurrentGameType;
                GameManager.Instance.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
            }
            orig(root, targetPlayer, item, items);
            if (whatItWasBefore.HasValue)
            {
                GameManager.Instance.CurrentGameType = whatItWasBefore.Value;
            }
        }

        public static void UpdateGunDataButGood(Action<GameUIRoot, GunInventory, int, PlayerController> orig, GameUIRoot root, GunInventory inventory, int inventoryShift, PlayerController sourcePlayer)
        {
            GameManager.GameType? whatItWasBefore = null;
            if (GameManager.HasInstance && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameManager.Instance) is TCultistController)
            {
                whatItWasBefore = GameManager.Instance.CurrentGameType;
                GameManager.Instance.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
            }
            orig(root, inventory, inventoryShift, sourcePlayer);
            if (whatItWasBefore.HasValue)
            {
                GameManager.Instance.CurrentGameType = whatItWasBefore.Value;
            }
        }

        public static PlayerController GetOtherPlayerButGood(Func<GameManager, PlayerController, PlayerController> orig, GameManager man, PlayerController play)
        {
            GameManager.GameType? whatItWasBefore = null;
            if (typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(man) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(man) is TCultistController)
            {
                whatItWasBefore = man.CurrentGameType;
                man.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
            }
            PlayerController res = orig(man, play);
            if (whatItWasBefore.HasValue)
            {
                man.CurrentGameType = whatItWasBefore.Value;
            }
            return res;
        }

        public static PlayerController SecondaryPlayerButGood(Func<GameManager, PlayerController> orig, GameManager man)
        {
            GameManager.GameType? whatItWasBefore = null;
            if (typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(man) != null && typeof(GameManager).GetField("m_secondaryPlayer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(man) is TCultistController)
            {
                whatItWasBefore = man.CurrentGameType;
                man.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
            }
            PlayerController res = orig(man);
            if(whatItWasBefore.HasValue)
            {
                man.CurrentGameType = whatItWasBefore.Value;
            }
            return res;
        }

        public static TCultistController ReplacePlayerControllerWithTCC(PlayerController pc)
        {
            pc.gameObject.SetActive(false);
            TCultistController tcc = null;
            try
            {
                tcc = pc.gameObject.AddComponent<TCultistController>();
            }
            catch { }
            foreach (PropertyInfo fi in typeof(PlayerController).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fi.CanRead && fi.CanWrite)
                {
                    try
                    {
                        fi.SetValue(tcc, fi.GetValue(pc, new object[0]), new object[0]);
                    }
                    catch { }
                }
            }
            foreach (FieldInfo fi in typeof(PlayerController).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!fi.IsLiteral)
                {
                    fi.SetValue(tcc, fi.GetValue(pc));
                }
            }
            tcc.stats = ((GameObject)BraveResources.Load("PlayerGunslinger")).GetComponent<PlayerController>().stats;
            tcc.Awake();
            UnityEngine.Object.DestroyImmediate(pc);
            tcc.gameObject.SetActive(true);
            return tcc;
        }
    }
}
