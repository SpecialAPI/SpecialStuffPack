using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class HeartPiece
    {
        public static void Init()
        {
            string name = "Heart Piece";
            string shortdesc = "Better than nothing";
            string longdesc = "Grants half a heart container. That's it.\n\nA piece of an ancient artifact, only containing a small portion of its former power.";
            var obtainable = EasyInitItem<PassiveItem>("HeartPiece", "heart_piece_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.D, null, null);
            obtainable.AddPassiveStatModifier(PlayerStats.StatType.Health, 0.5f);
            var unobtainable = EasyInitItem<PassiveItem>("UnobtainableHeartPiece", "heart_piece_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.EXCLUDED, null, $"{SpecialStuffModule.globalPrefix}:unobtainable_heart_piece");
            unobtainable.encounterTrackable.ProxyEncounterGuid = obtainable.encounterTrackable.EncounterGuid;
            unobtainable.encounterTrackable.m_hasCheckedForProxy = false;
            unobtainable.quality = PickupObject.ItemQuality.SPECIAL;
            unobtainable.AddPassiveStatModifier(PlayerStats.StatType.Health, 0.5f);
        }
    }
}
