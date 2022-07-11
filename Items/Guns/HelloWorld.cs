using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class HelloWorld
    {
        public static void Init()
        {
            var gun = GunBuilder.EasyGunInit("helloworld", "Hello World Gun", "A coder's first gun", "what", "helloworld_idle_001", "helloworld_idle_001", "gunsprites/helloworldgun/", 100, 1f, Vector3.zero, Empty, 
                "", PickupObject.ItemQuality.EXCLUDED, GunClass.NONE, SpecialStuffModule.globalPrefix, out var finish);
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    GetItemById<Gun>(38).DefaultModule.projectiles[0]
                },
                cooldownTime = 0.25f
            });
            finish();
        }
    }
}
