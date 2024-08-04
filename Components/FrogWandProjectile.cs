using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class FrogWandProjectile : BraveBehaviour
    {
        public Projectile destroyBeam;

        public void Start()
        {
            if(projectile != null)
            {
                projectile.OnHitEnemy += GemEffect;
            }
        }

        public void GemEffect(Projectile proj, SpeculativeRigidbody rb, bool fatal)
        {
            if(rb != null && rb.aiActor != null && rb.GetComponent<GemifiedEnemy>() != null)
            {
                var gem = rb.GetComponent<GemifiedEnemy>();
                var e = rb.aiActor;
                var type = gem.type;
                Destroy(gem);
                switch (type)
                {
                    case GemifiedEnemy.GemType.Teleport:
                        if (e.ParentRoom != null)
                        {
                            var enemies = e.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != e && x.GetComponent<GemifiedEnemy>() != null);
                            if (enemies.Count > 0)
                            {
                                AIActor target = null;
                                var closestDist = float.MaxValue;
                                foreach (var enemy in enemies)
                                {
                                    if (enemy != null && Vector2.Distance(e.CenterPosition, enemy.CenterPosition) < closestDist)
                                    {
                                        target = enemy;
                                        closestDist = Vector2.Distance(e.CenterPosition, enemy.CenterPosition);
                                    }
                                }
                                if (target != null)
                                {
                                    var targetpos = target.transform.position;
                                    var enemypos = e.transform.position;
                                    LootEngine.DoDefaultItemPoof(e.CenterPosition);
                                    e.transform.position = targetpos;
                                    e.specRigidbody.Reinitialize();
                                    e.specRigidbody.RecheckTriggers = true;
                                    LootEngine.DoDefaultItemPoof(target.CenterPosition);
                                    target.transform.position = enemypos;
                                    target.specRigidbody.Reinitialize();
                                    target.specRigidbody.RecheckTriggers = true;
                                    e.healthHaver.ApplyDamage(10f, (targetpos - enemypos).normalized, "Shattering", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                                    break;
                                }
                            }
                        }
                        e.healthHaver.ApplyDamage(10f, Vector2.zero, "Shattering", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        break;
                    case GemifiedEnemy.GemType.Destroy:
                        OwnedShootProjectile(destroyBeam, e.CenterPosition, proj.Direction.ToAngle(), proj.Owner).specRigidbody.RegisterTemporaryCollisionException(rb, 0.01f, 0.5f);
                        break;
                    case GemifiedEnemy.GemType.Mirror:
                        if(e.ParentRoom != null)
                        {
                            var enemies = e.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != e && x.GetComponent<GemifiedEnemy>() != null);
                            if(enemies.Count > 0)
                            {
                                AIActor target = null;
                                var closestDist = float.MaxValue;
                                foreach(var enemy in enemies)
                                {
                                    if(enemy != null && Vector2.Distance(e.CenterPosition, enemy.CenterPosition) < closestDist)
                                    {
                                        target = enemy;
                                        closestDist = Vector2.Distance(e.CenterPosition, enemy.CenterPosition);
                                    }
                                }
                                if(target != null)
                                {
                                    proj.GetOrAddPierce().penetration++;
                                    proj.m_hasPierced = false;
                                    proj.SendInDirection(target.CenterPosition - proj.specRigidbody.UnitCenter, false, true);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
