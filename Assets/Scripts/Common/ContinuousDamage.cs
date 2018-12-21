using UnityEngine;

namespace FortBlast.Common
{
    public class ContinuousDamage : MonoBehaviour
    {
        private HealthSetter _healthSetter;
        public float damagePerSecond;

        private void Update()
        {
            if (_healthSetter == null)
                return;

            _healthSetter.ReduceHealth(damagePerSecond * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            var healthSetter = other.GetComponent<HealthSetter>();
            if (healthSetter != null)
                _healthSetter = healthSetter;
        }

        private void OnTriggerExit(Collider other)
        {
            var healthSetter = other.GetComponent<HealthSetter>();
            if (healthSetter != null)
                _healthSetter = null;
        }
    }
}