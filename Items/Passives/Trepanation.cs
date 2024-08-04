using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Trepanation : PassiveItem
    {
        public static void Init()
        {
            var name = "Trepanation";
            var shortdesc = "A real head scratcher";
            var longdesc = "Chance to apply Linked to enemies when entering a new room.";
            var item = EasyItemInit<Trepanation>("trepanation", "trepanation_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.chance = 0.5f;
            link = LinkedStatusEffect.LinkedGenerator(5f);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnEnteredCombat += LinkEnemies;
        }

        public void LinkEnemies()
        {
            if(Owner != null && Owner.CurrentRoom != null)
            {
                var enemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if(enemies != null)
                {
                    foreach(var enemy in enemies)
                    {
                        if(enemy != null && BraveUtility.RandomBool())
                        {
                            enemy.ApplyEffect(link);
                        }
                    }
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player != null)
            {
                player.OnEnteredCombat -= LinkEnemies;
            }
        }

        public static LinkedStatusEffect link;
        public float chance;
    }
}
