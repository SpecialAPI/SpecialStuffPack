using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Enemies
{
    public class AdvancedEnemyDatabaseEntry : EnemyDatabaseEntry
    {
        public override AssetBundle assetBundle => AssetBundleManager.specialeverything;
    }
}
