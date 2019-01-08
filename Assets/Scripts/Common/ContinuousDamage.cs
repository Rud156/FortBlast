using UnityEngine;

namespace FortBlast.Common
{
    public class ContinuousDamage : MonoBehaviour
    {
        public float damagePerSecond;
        
        private HealthSetter _healthSetter;

        private void Update()
        {
            if (!_healthSetter)
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