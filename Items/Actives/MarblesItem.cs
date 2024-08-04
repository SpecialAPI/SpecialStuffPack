using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class MarblesItem : PlayerItem
    {
        public static void Init()
        {
            string name = "Marbles";
            string shortdesc = "Powerful";
            string longdesc = "Just ten marbles. You can launch them at your foes.";
            MarblesItem item = EasyItemInit<MarblesItem>("items/marbles", "sprites/marbles_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.numberOfUses = 10;
            item.consumable = true;
            item.SetCooldownType(CooldownType.Timed, 0.5f);
            item.Projectiles = new List<Projectile>();
            foreach(ProjectileModule module in GetItemById<Gun>(480).RawSourceVolley.projectiles) 
            { 
                if(module != null && module.chargeProjectiles != null && module.chargeProjectiles.Count > 0)
                {
                    item.Projectiles.AddRange(module.chargeProjectiles.ConvertAll((ProjectileModule.ChargeProjectile proj) => proj.Projectile));
                }
            }
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            BraveInput input = BraveInput.GetInstanceForPlayer(user.PlayerIDX);
            float aim = 0f;
            if (input != null && input.ActiveActions != null)
            {
                if (input.IsKeyboardAndMouse())
                {
                    aim = (user.unadjustedAimPoint.XY() - user.CenterPosition).ToAngle();
                }
                else
                {
                    aim = input.ActiveActions.Aim.Angle;
                }
            }
            GameObject go = SpawnManager.SpawnProjectile(BraveUtility.RandomElement(Projectiles).gameObject, user.CenterPosition, Quaternion.Euler(0f, 0f, aim));
            if(go != null)
            {
                Projectile proj = go.GetComponent<Projectile>();
                if(proj != null)
                {
                    proj.Owner = user;
                    proj.Shooter = user.specRigidbody;
                }
            }
        }

        public List<Projectile> Projectiles;
    }
}
