using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class UndoProjectile : BraveBehaviour
    {
        public void Start()
        {
            startPosition = transform.position;
            if(projectile != null)
            {
                projectile.OnDestruction += RecordDestruction;
            }
        }

        public void RecordDestruction(Projectile proj)
        {
            if(proj.PossibleSourceGun != null)
            {
                proj.PossibleSourceGun.GetOrAddComponent<UndoGun>().undoProjs.Add(new(startPosition, transform.position, projToFireOnUndo));
            }
        }

        public Vector2 startPosition;
        public Projectile projToFireOnUndo;
    }
}
