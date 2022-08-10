using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    public class SPCultistBandana : MonoBehaviour
    {
        public void Awake()
        {
            item = GetComponent<CoopPassiveItem>();
            if(item != null && (cachedCoopModifiers == null || cachedCoopModifiers.Count <= 0))
            {
                cachedCoopModifiers = item.modifiers;
            }
        }

        public void Update()
        {
            if (item != null && (cachedCoopModifiers == null || cachedCoopModifiers.Count <= 0))
            {
                cachedCoopModifiers = item.modifiers;
            }
            if (cachedCoopModifiers == null || cachedCoopModifiers.Count <= 0)
            {
                return;
            }
            if(GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER && item != null && item.PickedUp && item.Owner != null && item.Owner.characterIdentity == PlayableCharactersE.SPCultist)
            {
                var player = item.Owner;
                var companionCount = (player.companions?.Count).GetValueOrDefault() + (player.trailOrbitals?.Count).GetValueOrDefault() + (player.orbitals?.Count).GetValueOrDefault();
                if(companionCount > 0 && cachedCompanions <= 0)
                {
                    item.modifiers = new();
                    player.stats.RecalculateStats(player, false, false);
                }
                else if(companionCount <= 0 && cachedCompanions > 0)
                {
                    item.modifiers = cachedCoopModifiers;
                    player.stats.RecalculateStats(player, false, false);
                }
                cachedCompanions = companionCount;
            }
            else if(item != null)
            {
                if(item.modifiers != cachedCoopModifiers)
                {
                    item.modifiers = cachedCoopModifiers;
                    if(item.PickedUp && item.Owner != null)
                    {
                        item.Owner.stats.RecalculateStats(item.Owner, false, false);
                    }
                }
            }
        }

        public List<StatModifier> cachedCoopModifiers;
        private CoopPassiveItem item;
        private int cachedCompanions;

    }
}
