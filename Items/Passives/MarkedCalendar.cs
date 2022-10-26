using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class MarkedCalendar : PassiveItem
    {
        public static bool IsTime;

        public static void Init()
        {
            var name = "Marked Calendar";
            var shortdesc = "September 29 2022";
            var longdesc = "A calendar left in the breach, to be forever forgotten.";
            var item = EasyItemInit<MarkedCalendar>("MarkedCalendar", "marked_calendar_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            item.encounterTrackable.journalData.SuppressInAmmonomicon = true;
            item.encounterTrackable.DatabaseEntry().journalData.SuppressInAmmonomicon = true;
            item.CanBeDropped = false;
            var now = DateTime.Now;
            IsTime = now.Day == 29 && now.Month == 9;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.IncrementFlag<MarkedCalendar>();
            if (GameManager.HasInstance && !GameManager.Instance.IsFoyer)
            {
                GameManager.Instance.StartCoroutine(HandleWrathOfGod());
            }
        }

        public static IEnumerator HandleWrathOfGod()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.StartCoroutine(HandleReapers());
                GameManager.Instance.StartCoroutine(DelayedKillPlayers());
            }
            while (true)
            {
                if (GameManager.HasInstance)
                {
                    foreach(var p in GameManager.Instance.AllPlayers)
                    {
                        p.healthHaver.NextShotKills = true;
                        if(p.CurrentRoom != null)
                        {
                            var enemies = p.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                            if(enemies != null)
                            {
                                foreach(var e in enemies)
                                {
                                    if(e.healthHaver != null)
                                    {
                                        e.healthHaver.IsVulnerable = false;
                                        e.healthHaver.PreventAllDamage = true;
                                        e.healthHaver.ForceSetCurrentHealth(e.healthHaver.GetMaxHealth());
                                    }
                                    if (!e.IsBlackPhantom)
                                    {
                                        e.BecomeBlackPhantom();
                                    }
                                }
                            }
                        }
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator HandleReapers()
        {
            while (true)
            {
                if (!GameManager.HasInstance || GameManager.Instance.Dungeon == null)
                {
                    yield return null;
                    continue;
                }
                GameManager.Instance.Dungeon.SpawnCurseReaper();
                yield return new WaitForSeconds(1f);
            }
        }

        public static IEnumerator DelayedKillPlayers()
        {
            yield return new WaitForSeconds(60f);
            if (GameManager.HasInstance)
            {
                foreach (var p in GameManager.Instance.AllPlayers)
                {
                    p.healthHaver.NextShotKills = true;
                    p.healthHaver.ApplyDamage(1000, Vector2.zero, "WRATH OF GOD", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                    Exploder.DoDefaultExplosion(p.sprite.WorldBottomCenter, Vector2.zero, null, true, CoreDamageTypes.None, true);
                }
                yield return null;
            }
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }
    }
}
