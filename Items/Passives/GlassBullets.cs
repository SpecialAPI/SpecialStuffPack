using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class GlassBullets : PassiveItem
    {
        public static void Init()
        {
            var name = "Glass Bullets";
            var shortdesc = "Fragile Damage";
            var longdesc = "Incrases damage. Breaks when the owner takes damage, dealing an extra half heart of damage. Repairs itself upon entering a new floor.";
            var item = EasyItemInit<GlassBullets>("glassbullets", "glass_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.damageMult = 1.5f;
            item.notBrokenSpriteId = item.sprite.spriteId;
            item.brokenSpriteId = AddSpriteToCollection("glass_bullets_broken_001", SpriteBuilder.itemCollection);
            item.shatterBlood = TeleporterPrototypeObject.TelefragVFXPrefab;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += Repair;
            player.PostProcessProjectile += DamageUp;
            player.OnReceivedDamage += Break;
            player.healthHaver.ModifyDamage += ModifyDamage;
        }

        public void Repair(PlayerController p)
        {
            if (broken)
            {
                broken = false;
                sprite.SetSprite(notBrokenSpriteId);
                if (Minimap.HasInstance && Minimap.Instance.UIMinimap != null)
                {
                    if (Minimap.Instance.UIMinimap.dockItems != null)
                    {
                        var thisSprite = Minimap.Instance.UIMinimap.dockItems.Find(x => x.Second == this);
                        if (thisSprite != null)
                        {
                            thisSprite.First.SetSprite(notBrokenSpriteId);
                        }
                    }
                    if (Minimap.Instance.UIMinimap.secondaryDockItems != null)
                    {
                        var thisSprite = Minimap.Instance.UIMinimap.secondaryDockItems.Find(x => x.Second == this);
                        if (thisSprite != null)
                        {
                            thisSprite.First.SetSprite(notBrokenSpriteId);
                        }
                    }
                }
            }
        }

        public void DamageUp(Projectile proj, float f)
        {
            if (!broken)
            {
                proj.baseData.damage *= damageMult;
            }
        }

        public void Break(PlayerController p)
        {
            if (broken)
            {
                return;
            }
            broken = true;
            sprite.SetSprite(brokenSpriteId);
            if(Minimap.HasInstance && Minimap.Instance.UIMinimap != null)
            {
                if(Minimap.Instance.UIMinimap.dockItems != null)
                {
                    var thisSprite = Minimap.Instance.UIMinimap.dockItems.Find(x => x.Second == this);
                    if (thisSprite != null)
                    {
                        thisSprite.First.SetSprite(brokenSpriteId);
                    }
                }
                if (Minimap.Instance.UIMinimap.secondaryDockItems != null)
                {
                    var thisSprite = Minimap.Instance.UIMinimap.secondaryDockItems.Find(x => x.Second == this);
                    if (thisSprite != null)
                    {
                        thisSprite.First.SetSprite(brokenSpriteId);
                    }
                }
            }
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultBlobulonGoop).TimedAddGoopCircle(Owner.sprite.WorldBottomCenter, 1f, 0.5f, false);
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", p.gameObject);
        }

        public void ModifyDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            if(!broken && args.ModifiedDamage > 0f)
            {
                args.ModifiedDamage += 0.5f;
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player != null)
            {
                player.OnNewFloorLoaded -= Repair;
                player.PostProcessProjectile -= DamageUp;
                player.OnReceivedDamage -= Break;
                player.healthHaver.ModifyDamage -= ModifyDamage;
            }
        }

        public bool broken;
        public int brokenSpriteId;
        public int notBrokenSpriteId;
        public float damageMult;
        public GameObject shatterBlood;
    }
}
