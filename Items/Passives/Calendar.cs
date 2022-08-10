using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class Calendar
    {
        public static void Init()
        {
            string name = "Calendar";
            string shortdesc = "It's... wait, what day is it?";
            string longdesc = "Increases different stats depending on the current day of the week.";
            float statIncrease = 0f;
            PlayerStats.StatType statType = PlayerStats.StatType.Damage;
            StatModifier.ModifyMethod modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    shortdesc = "It's Monday";
                    longdesc = "Increases the owner's speed.";
                    statIncrease = 2f;
                    statType = PlayerStats.StatType.MovementSpeed;
                    modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
                    break;
                case DayOfWeek.Tuesday:
                    shortdesc = "It's Tuesday";
                    longdesc = "Increases the owner's dodge powers.";
                    statIncrease = 1.25f;
                    statType = PlayerStats.StatType.DodgeRollSpeedMultiplier;
                    modifyMethod = StatModifier.ModifyMethod.MULTIPLICATIVE;
                    break;
                case DayOfWeek.Wednesday:
                    shortdesc = "It's Wednesday";
                    longdesc = "Increases the owner's health.";
                    statIncrease = 1f;
                    statType = PlayerStats.StatType.Health;
                    modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
                    break;
                case DayOfWeek.Thursday:
                    shortdesc = "It's Thursday";
                    longdesc = "Increases the owner's damage.";
                    statIncrease = 0.33f;
                    statType = PlayerStats.StatType.Damage;
                    modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
                    break;
                case DayOfWeek.Friday:
                    shortdesc = "It's Friday";
                    longdesc = "Increases the owner's rate of fire.";
                    statIncrease = 0.5f;
                    statType = PlayerStats.StatType.RateOfFire;
                    modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
                    break;
                case DayOfWeek.Saturday:
                    shortdesc = "It's Saturday";
                    longdesc = "Increases the owner's max ammo.";
                    statIncrease = 1.5f;
                    statType = PlayerStats.StatType.AmmoCapacityMultiplier;
                    modifyMethod = StatModifier.ModifyMethod.MULTIPLICATIVE;
                    break;
                case DayOfWeek.Sunday:
                    shortdesc = "It's Sunday";
                    longdesc = "Decreases the speed of enemy bullets.";
                    statIncrease = 0.8f;
                    statType = PlayerStats.StatType.EnemyProjectileSpeedMultiplier;
                    modifyMethod = StatModifier.ModifyMethod.MULTIPLICATIVE;
                    break;
            }
            longdesc += "\n\nWhile calendars can be useful in other places, they aren't really that useful in gungeon, because of how time in gungeon works.";
            PassiveItem item = EasyInitItem<PassiveItem>("items/calendar", "sprites/calendar_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.B, 524, null);
            item.AddPassiveStatModifier(statType, statIncrease, modifyMethod);
        }
    }
}
