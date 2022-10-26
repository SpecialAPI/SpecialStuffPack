using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class RedAmmolet : CustomBlankModificationItem
    {
        public static void Init()
        {
            var name = "Red Blammolet";
            var shortdesc = "Blanks Indoctrinate";
            var longdesc = "Blanks can indoctrinate enemies.\n\nThis ammolet was used by the great Blam to quickly find new followers.";
            var item = EasyItemInit<RedAmmolet>("RedAmmolet", "red_ammolet_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f);
            item.EnemiesPerCharm = 3;
            item.MaxCharmedEnemies = 3;
            item.BlankStunTime = 0f;
            item.AddToCursulaShop();
        }

        public override void DoCustomBlankEffect(SilencerInstance silencer, Vector2 blankCenter, PlayerController user)
        {
            var room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(blankCenter.ToIntVector2(VectorConversions.Round));
            if(room != null)
            {
                var enemies = room.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                if(enemies != null && enemies.Count > 0)
                {
                    var enemiesToCharm = Mathf.Clamp(enemies.Count / EnemiesPerCharm, 1, MaxCharmedEnemies);
                    for(int i = 0; i < enemiesToCharm && enemies.Count > 0; i++)
                    {
                        var enemy = enemies.RandomElement();
                        if(enemy == null)
                        {
                            continue;
                        }
                        CultCharmEffect.ConvertToIndoctrinate(enemy, true, 0.5f);
                        enemies.Remove(enemy);
                    }
                }
            }
        }

        public int EnemiesPerCharm;
        public int MaxCharmedEnemies;
    }
}
