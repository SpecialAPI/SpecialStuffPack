﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class SetProjectileDestroyMode : BraveBehaviour
    {
        public void Start()
        {
            projectile.DestroyMode = destroyMode;
        }

        public Projectile.ProjectileDestroyMode destroyMode;
    }
}
