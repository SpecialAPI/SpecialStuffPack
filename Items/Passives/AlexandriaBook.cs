using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class AlexandriaBook : PassiveItem
    {
        public static void Init()
        {
            string name = "Book of Alexandria";
            string shortdesc = "Has bugs in it";
            string longdesc = "Makes the owner stronger, creates bugs.\n\nA book that contains all knowledge in the universe, including books like \"How to level up your bullets\", \"How to eat lichy trigger fingers\"," +
                " \"How to make your shoes into a rocket\", \"How to clip your drums\" and of course, \"Top 10 best spaghetti recipes\". It also has bugs living in it, mostly near the spaghetti section.";
            var item = EasyItemInit<AlexandriaBook>("alexandria", "alexandria_idle_001", name, shortdesc, longdesc, ItemQuality.S, null, null);

            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.10f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 1.10f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, 1.10f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, 0.90f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.PlayerBulletScale, 1.10f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.MoneyMultiplierFromEnemies, 1.10f, ModifyMethodE.TrueMultiplicative);

            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Coolness, 1f, ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 0.5f, ModifyMethod.ADDITIVE);

            item.amountOfBugsToSpawn = 3;
            item.bugItemId = ItemIds["bug"];
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                for(int i = 0; i < amountOfBugsToSpawn; i++)
                {
                    LootEngine.SpawnItem(GetItemById(bugItemId).gameObject, player.CenterPosition, Random.insideUnitCircle, 5f, false, false, false);
                }
            }
            base.Pickup(player);
        }

        public int amountOfBugsToSpawn;
        public int bugItemId;
    }
}
