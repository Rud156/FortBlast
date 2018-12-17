using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Common
{
    public class ContinuousDamage : MonoBehaviour
    {
        public float damagePerSecond;

        private HealthSetter _healthSetter;

        private void Update()
        {
            if(_healthSetter == null)
                return;
            
            _healthSetter.ReduceHealth(damagePerSecond * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            HealthSetter healthSetter = other.GetComponent<HealthSetter>();
            if (healthSetter != null)
                _healthSetter = healthSetter;
        }

        private void OnTriggerExit(Collider other)
        {
            HealthSetter healthSetter = other.GetComponent<HealthSetter>();
            if (healthSetter != null)
                _healthSetter = null;
        }
    }
}