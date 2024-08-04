using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class UndoGun : GunBehaviour
    {
        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (gun.IsReloading && undoProjs.Count > 0)
            {
                gun.StartCoroutine(HandleRewind());
            }
        }

        public IEnumerator HandleRewind()
        {
            var oldundo = undoProjs;
            undoProjs = new();
            var delay = gun.reloadTime / (oldundo.Count - 1);
            foreach(var p in oldundo)
            {
                if(gun == null || gun.CurrentOwner == null)
                {
                    yield break;
                }
                if(p != null && p.undoProj != null)
                {
                    var undo = OwnedShootProjectile(p.undoProj, p.start, (p.end - p.start).ToAngle(), gun.CurrentOwner);
                    undo.baseData.range = (p.end - p.start).magnitude;
                    if(gun.CurrentOwner is PlayerController c)
                    {
                        c.DoPostProcessProjectile(undo);
                    }
                    AkSoundEngine.PostEvent("TresUndosUndo", gun.gameObject);
                }
                yield return new WaitForSeconds(delay);
            }
            yield break;
        }

        [NonSerialized]
        public List<UndoPosition> undoProjs = new();

        public class UndoPosition
        {
            public UndoPosition(Vector2 end, Vector2 start, Projectile proj)
            {
                this.end = end;
                this.start = start;
                undoProj = proj;
            }

            public Vector2 start;
            public Vector2 end;
            public Projectile undoProj;
        }
    }
}
