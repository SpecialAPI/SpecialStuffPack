using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class BrokenCalculator : PassiveItem
    {
        public static void Init()
        {
            string name = "Broken Calculator";
            string shortdesc = "1 x 1 = 5";
            string longdesc = "Messes up the owner's stats until recalculation.\n\nA calculator that has been completely broken, becoming absolutely unusable.";
            BrokenCalculator item = EasyItemInit<BrokenCalculator>("items/brokencalculator", "sprites/broken_calculator_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.ArmorToGainOnInitialPickup = 2;
            item.DamageMultiplier = 5f;
            item.ReloadMultiplier = 0f;
            item.ClipMultiplier = 50f;
            item.FirerateMultiplier = 5f;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                List<float> stats = (List<float>)typeof(PlayerStats).GetField("StatValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(player.stats);
                stats[(int)PlayerStats.StatType.Damage] *= DamageMultiplier;
                stats[(int)PlayerStats.StatType.ReloadSpeed] *= ReloadMultiplier;
                stats[(int)PlayerStats.StatType.AdditionalClipCapacityMultiplier] *= ClipMultiplier;
                stats[(int)PlayerStats.StatType.RateOfFire] *= FirerateMultiplier;
            }
            base.Pickup(player);
            if (player.healthHaver != null)
            {
                if(player.healthHaver.damageTypeModifiers == null)
                {
                    player.healthHaver.damageTypeModifiers = new List<DamageTypeModifier>();
                }
                m_electricImmunity = new DamageTypeModifier { damageMultiplier = 0f, damageType = CoreDamageTypes.Electric };
                player.healthHaver.damageTypeModifiers.Add(m_electricImmunity);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player != null && player.healthHaver != null && player.healthHaver.damageTypeModifiers != null && m_electricImmunity != null)
            {
                player.healthHaver.damageTypeModifiers.Add(m_electricImmunity);
            }
            m_electricImmunity = null;
        }

        public float DamageMultiplier;
        public float ReloadMultiplier;
        public float ClipMultiplier;
        public float FirerateMultiplier;
        private DamageTypeModifier m_electricImmunity;
    }
}
