using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class WoodenDice : PassiveItem
    {
        public static void Init()
        {
            string name = "Wooden Dice";
            string shortdesc = "Totally Rigged";
            string longdesc = "Randomizes bullet damage. Damage randomization is rigged, either in the owner's favor or against it. Dropping the item or going to a new floor will reroll if the randomization is rigged" +
                " in the owner's favor or against it, as well as how much it affects the roll outcomes. Both of these values are always hidden, however the multiplier from the damage roll will be shown when an enemy is hit with " +
                "a bullet.\n\nA strange wooden dice that is probably cursed.";
            var item = EasyItemInit<WoodenDice>("WoodenDice", "unlucky_dice_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.RerollChancePerRiggedness = 0.25f;
            item.BaseMultiplier = 0.5f;
            item.MultiplierPerRoll = 0.2f;
            item.colorGradient = new();
            item.colorGradient.SetKeys(new GradientColorKey[]
            {
                new()
                {
                    color = Color.red,
                    time = 0.5f,
                },
                new()
                {
                    color = Color.green,
                    time = 1.5f
                }
            }, new GradientAlphaKey[]
            {
                new()
                {
                    alpha = 1f,
                    time = 0f
                }
            });
            item.snakeEyesProjectile = ChamberGunOublietteObject.GetProjectile();
            item.midnightProjectile = FinishedGunObject.DefaultModule.finalProjectile;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += RerollDamage;
            player.OnNewFloorLoaded += FloorRiggednessReroll;
        }

        public void FloorRiggednessReroll(PlayerController player)
        {
            Riggedness = Random.Range(1, 7);
        }

        public void RerollDamage(Projectile proj, float f)
        {
            RerollProjectileDamage(proj);
            if(Owner.PlayerHasActiveSynergy("Gambling Addiction"))
            {
                RerollProjectileDamage(proj);
            }
        }

        public void RerollProjectileDamage(Projectile proj)
        {
            var roll = Roll;
            var mult = BaseMultiplier + (roll - 1) * MultiplierPerRoll;
            proj.baseData.damage *= mult;
            var d = proj.AddComponent<ShowMultiplierOnHit>();
            d.mult = mult;
            d.colorGradient = colorGradient;
            if (LastRoll == 1 && roll == 1 && Owner.PlayerHasActiveSynergy("Snake Eyes"))
            {
                var pos = Owner.specRigidbody.UnitCenter.ToVector3ZisY(0f);
                DoCustomTextPopup(pos, pos.y - Owner.specRigidbody.UnitBottomCenter.y, "SNAKE EYES!", Color.red);
                var snakeeyes = OwnedShootProjectile(snakeEyesProjectile, Owner.CurrentGun != null ? Owner.CurrentGun.barrelOffset.position.XY() : Owner.CenterPosition, Owner.GetAimDirection(), Owner);
                var homing = snakeeyes.GetOrAddComponent<HomingModifier>();
                homing.HomingRadius = 10f;
                homing.AngularVelocity = 420f;
                var pierce = snakeeyes.GetOrAddComponent<PierceProjModifier>();
                pierce.penetration = 1;
                pierce.preventPenetrationOfActors = false;
                pierce.penetratesBreakables = true;
                snakeeyes.CanTransmogrify = true;
                snakeeyes.ChanceToTransmogrify = 100f;
                snakeeyes.TransmogrifyTargetGuids = new string[] { HarmlessSnakeGuid };
            }
            else if(LastRoll == 6 && roll == 6 && Owner.PlayerHasActiveSynergy("Midnight"))
            {
                var pos = Owner.specRigidbody.UnitCenter.ToVector3ZisY(0f);
                DoCustomTextPopup(pos, pos.y - Owner.specRigidbody.UnitBottomCenter.y, "MIDNIGHT!", Color.green);
                OwnedShootProjectile(midnightProjectile, Owner.CurrentGun != null ? Owner.CurrentGun.barrelOffset.position.XY() : Owner.CenterPosition, Owner.GetAimDirection(), Owner);
            }
            LastRoll = roll;
        }

        public int Roll
        {
            get
            {
                var value = Random.Range(1, 7);
                if((Riggedness <= 3 && value >= 4) || (Riggedness >= 4 && value <= 3))
                {
                    if(Random.value < RerollChancePerRiggedness * (Mathf.Abs(Riggedness - 3.5f) + 0.5f))
                    {
                        return Random.Range(1, 7);
                    }
                }
                return value;
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= RerollDamage;
            player.OnNewFloorLoaded -= FloorRiggednessReroll;
            base.DisableEffect(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            var result = base.Drop(player);
            result.OnGrounded += RerollRiggedness;
            AkSoundEngine.PostEvent("DiceThrow", result.gameObject);
            return result;
        }

        public void RerollRiggedness(DebrisObject obj)
        {
            Riggedness = Random.Range(1, 7);
            obj.OnGrounded -= RerollRiggedness;
            AkSoundEngine.PostEvent("DiceLand", obj.gameObject);
        }

        public new void Start()
        {
            base.Start();
            Riggedness = Random.Range(1, 7);
        }

        public int Riggedness;
        public float BaseMultiplier;
        public float MultiplierPerRoll;
        public float RerollChancePerRiggedness;
        [NonSerialized]
        public int LastRoll = -1;
        public Projectile snakeEyesProjectile;
        public Projectile midnightProjectile;
        public Gradient colorGradient;
    }
}
