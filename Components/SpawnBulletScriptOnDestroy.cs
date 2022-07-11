using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class SpawnBulletScriptOnDestroy : BraveBehaviour
    {
        public override void OnDestroy()
        {
            SpawnManager.SpawnBulletScript(owner, script, GetComponentInChildren<tk2dBaseSprite>().WorldCenter, null, false, null);
            base.OnDestroy();
        }

        public BulletScriptSelector script;
        public GameActor owner;
    }
}
