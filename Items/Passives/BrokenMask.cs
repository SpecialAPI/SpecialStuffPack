using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BrokenMask : PassiveItem
    {
        public static void Init()
        {
            string name = "Broken Mask";
            string shortdesc = "Everyone is dead.";
            string longdesc = "Clears the current room upon taking damage.\n\nA mysterious broken mask, holding it makes you feel death.";
            var item = EasyItemInit<BrokenMask>("brokenmask", "broken_mask_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += EveryoneIsDead;
        }

        public void EveryoneIsDead(PlayerController p)
        {
            if(p.CurrentRoom != null)
            {
                var goldpig = p.PlayerHasActiveSynergy("Ignorance");
                var mage = p.PlayerHasActiveSynergy("Humour");
                var professor = p.PlayerHasActiveSynergy("Nihilism");
                var robzob = p.PlayerHasActiveSynergy("Scared");
                var frog = p.PlayerHasActiveSynergy("Stoic");
                bool killedSomething = false;
                if (p.CurrentRoom.remainingReinforcementLayers != null && p.CurrentRoom.remainingReinforcementLayers.Count > 0)
                {
                    p.CurrentRoom.ClearReinforcementLayers();
                    killedSomething = true;
                }
                var enemies = p.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                foreach(var e in enemies)
                {
                    if(e.healthHaver != null && !e.healthHaver.IsBoss)
                    {
                        var ondeath = e.GetComponents<OnDeathBehavior>();
                        foreach(var effect in ondeath)
                        {
                            DestroyImmediate(effect);
                        }
                        if (goldpig)
                        {
                            LootEngine.SpawnCurrency(e.CenterPosition, 1, false);
                        }
                        if (mage && 0.05f.RandomChance())
                        {
                            LootEngine.SpawnItem(GlassGuonStoneObject.gameObject, e.CenterPosition, Vector2.down, 0f, false, true, false);
                        }
                        if (professor && p.activeItems != null)
                        {
                            foreach(var active in p.activeItems)
                            {
                                active.DidDamage(p, e.healthHaver.GetCurrentHealth() * 0.5f);
                            }
                        }
                        if (robzob && p.inventory != null && p.inventory.AllGuns != null)
                        {
                            foreach (var g in p.inventory.AllGuns)
                            {
                                g.GainAmmo(1);
                            }
                        }
                        if (frog)
                        {
                            p.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.005f));
                        }
                        e.EraseFromExistenceWithRewards();
                        killedSomething = true;
                    }
                }
                List<Projectile> list = StaticReferenceManager.m_allProjectiles.FindAll(x => x.Owner == null || x.Owner is not PlayerController);
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].DieInAir(false, false, false, false);
                    killedSomething = true;
                }
                if (killedSomething)
                {
                    AkSoundEngine.PostEvent("EverhoodCombatDeathHit", p.gameObject);
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player)
            {
                player.OnReceivedDamage -= EveryoneIsDead;
            }
            base.DisableEffect(player);
        }
    }
}
