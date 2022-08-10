using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class EnderGun : GunBehaviour
    {
        public static void Init()
        {
            string name = "Ender Gun";
            string shortdesc = "End the lives of your enemies!";
            string longdesc = "Imitates the owner's starter gun, but bigger.";
            var gun = EasyGunInit("guns/ender", name, shortdesc, longdesc, "ender_idle_001", "ender_idle_001", "gunsprites/endergun/", 100, 1f, new(16, 16), Empty, "", PickupObject.ItemQuality.S, GunClass.NONE, 
                out var finish);
            gun.LocalInfiniteAmmo = true;
            var g = gun.AddComponent<EnderGun>();
            g.sizeUp = 3f;
            g.dmgUp = 5f;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    GetProjectile(89)
                }
            });
            finish();
        }

        public override void Update()
        {
            base.Update();
            if(gun == null)
            {
                return;
            }
            var play = PlayerOwner;
            if(play != null && !transformed)
            {
                var id = GetStarterGun(play);
                if(id >= 0)
                {
                    var g = GetItemById<Gun>(id);
                    gun.TransformToTargetGun(g);
                    transform.Find("SecondaryHand")?.gameObject?.SetActive(g.Handedness == GunHandedness.TwoHanded);
                    gun.gunHandedness = GunHandedness.AutoDetect;
                    gun.TrickGunAlternatesHandedness = true;
                    var isTrick = gun.IsTrickGun;
                    gun.IsTrickGun = true;
                    string.IsNullOrEmpty(gun.Handedness.ToString());
                    gun.IsTrickGun = isTrick;
                    gun.m_cachedGunHandedness = null;
                    gun.barrelOffset.position *= sizeUp;
                    gun.m_originalBarrelOffsetPosition *= sizeUp;
                    sprite.scale *= sizeUp;
                    transformed = true;
                }
            }
            else if(play == null && transformed)
            {
                gun.TransformToTargetGun(GetItemById<Gun>(gun.PickupObjectId));
                transform.Find("SecondaryHand")?.gameObject?.SetActive(true);
                gun.barrelOffset.position /= sizeUp;
                gun.m_originalBarrelOffsetPosition /= sizeUp;
                sprite.scale /= sizeUp;
                transformed = false;
            }
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.baseData.damage *= dmgUp;
            projectile.AdditionalScaleMultiplier *= sizeUp;
        }

        public int GetStarterGun(PlayerController player)
        {
            if (player?.inventory?.AllGuns == null)
            {
                return -1;
            }
            var id1 = -1;
            var id2 = -1;
            var id3 = -1;
            foreach(var g in player.inventory.AllGuns)
            {
                var id = g.PickupObjectId;
                if(id == 417) //no big swords for you
                {
                    continue;
                }
                var isInStartingList = (player.startingGunIds?.Contains(g.PickupObjectId)).GetValueOrDefault();
                var isStarterForAchievement = g.StarterGunForAchievement;
                if(id1 < 0)
                {
                    if (isInStartingList && isStarterForAchievement)
                    {
                        id1 = id;
                    }
                    else if(id2 < 0)
                    {
                        if (isInStartingList)
                        {
                            id2 = id;
                        }
                        else if(isStarterForAchievement && id3 < 0)
                        {
                            id3 = id;
                        }
                    }
                }
                
            }
            return id1 >= 0 ? id1 : id2 >= 0 ? id2 : id3 >= 0 ? id3 : 86;
        }

        private bool transformed;
        public float sizeUp;
        public float dmgUp;
    }
}
