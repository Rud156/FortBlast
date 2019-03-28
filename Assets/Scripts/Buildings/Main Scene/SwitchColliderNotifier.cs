using UnityEngine;

namespace FortBlast.Buildings.MainScene
{
    [RequireComponent(typeof(Collider))]
    public class SwitchColliderNotifier : MonoBehaviour
    {
        public delegate void TriggerEnter(Collider other);

        public delegate void TriggerExit(Collider other);

        public TriggerEnter triggerEnter;
        public TriggerExit triggerExit;

        private void OnTriggerEnter(Collider other) => triggerEnter?.Invoke(other);

        private void OnTriggerExit(Collider other) => triggerExit?.Invoke(other);
    }
}