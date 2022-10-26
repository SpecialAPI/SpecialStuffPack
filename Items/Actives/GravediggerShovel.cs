using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class GravediggerShovel : PlayerItem
    {
        public static void Init()
        {
            string name = "Gravedigger's Shovel";
            string shortdesc = "A Grave Mistake";
            string longdesc = "Turns all enemies in the room into Tombstoners. Destroys all tombstoners that are already in the room.\n\nA steel shovel used for digging graves, still effective in Gungeon!";
            GravediggerShovel item = EasyItemInit<GravediggerShovel>("items/gravediggershovel", "sprites/gravedigger_shovel_idle_001", name, shortdesc, longdesc, ItemQuality.C, null, null);
            item.TransformEnemyGuid = "cf27dd464a504a428d87a8b2560ad40a";
            item.TransformEnemyGuidSynergy = "249db525a9464e5282d02162c88e0357";
            item.SetCooldownType(CooldownType.Damage, 250);
        }

        public override void DoEffect(PlayerController user)
        {
            if(user != null && user != null && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All) != null)
            {
                List<AIActor> enemies = user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All);
                foreach(AIActor aiactor in enemies)
                {
                    if(aiactor != null)
                    {
                        if (aiactor.EnemyGuid == (user.PlayerHasActiveSynergy("The zombies are coming") ? TransformEnemyGuidSynergy : TransformEnemyGuid))
                        {
                            aiactor.healthHaver?.ApplyDamage(99999999, Vector2.zero, "Gravedigger's Shovel", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                        }
                        else if (!aiactor.healthHaver.IsBoss)
                        {
                            aiactor.Transmogrify(EnemyDatabase.GetOrLoadByGuid((user.PlayerHasActiveSynergy("The zombies are coming") ? TransformEnemyGuidSynergy : TransformEnemyGuid)), (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
                        }
                    }
                }
                if(user.PlayerHasActiveSynergy("The zombies are coming"))
                {
                    List<AIActor> enemies2 = user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All);
                    foreach (AIActor aiactor in enemies2)
                    {
                        if (aiactor != null)
                        {
                            if (!aiactor.healthHaver.IsBoss && aiactor.GetComponent<SpawnEnemyOnDeath>() != null)
                            {
                                Destroy(aiactor.GetComponent<SpawnEnemyOnDeath>());
                            }
                        }
                    }
                }
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All) != null && user.IsInCombat &&
                user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All).Count > 0;
        }

        public string TransformEnemyGuid;
        public string TransformEnemyGuidSynergy;
    }
}
