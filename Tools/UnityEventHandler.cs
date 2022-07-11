using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    public class UnityEventHandler : MonoBehaviour
    {
        private void Awake()
        {
            OnAwake?.Invoke();
        }

        public Action OnAwake;

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        public Action OnFixedUpdate;

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        public Action OnLateUpdate;

        private void Start()
        {
            OnStart?.Invoke();
        }

        public Action OnStart;

        private void Update()
        {
            OnStart?.Invoke();
        }

        public Action OnUpdate;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }

        public Action OnDestroyed;

        private void OnDisable()
        {
            OnDisabled?.Invoke();
        }

        public Action OnDisabled;

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        public Action OnEnabled;
    }
}
