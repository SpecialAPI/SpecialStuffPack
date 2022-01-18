using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class WizardKnightController : BraveBehaviour
    {
        public void Update()
        {
            if(!m_hasSpawnedEnemies && gameObject.activeSelf && aiActor.ParentRoom != null)
            {
                foreach(string s in EnemiesToSpawnOnSpawn)
                {
                    AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(s), aiActor.ParentRoom.GetRandomAvailableCellDumb(), aiActor.ParentRoom, false, AIActor.AwakenAnimationType.Default, true);
                }
                m_hasSpawnedEnemies = true;
            }
        }

        private bool m_hasSpawnedEnemies;
        public List<string> EnemiesToSpawnOnSpawn;
        public State CurrentState;

        public enum State
        {
            None,
            SwordCharge,
            SwordLaunch,
            SwordSpin,
            SwordSpinSlash,
            SwordSwipeBegin,
            SwordSwipe,
            SwordSwipeReturn
        }
    }
}
