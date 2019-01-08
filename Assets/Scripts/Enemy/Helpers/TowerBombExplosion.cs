using FortBlast.Common;
using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(LightFlickerFastFaster))]
    public class TowerBombExplosion : MonoBehaviour
    {
        public GameObject bombExplosion;
        public float explosionRadius;
        
        private DamageAmountSetter _damageAmountSetter;
        private LightFlickerFastFaster _lightFlickerFastFaster;

        private void Start()
        {
            _lightFlickerFastFaster = GetComponent<LightFlickerFastFaster>();
            _lightFlickerFastFaster.flickerComplete += ExplodeBomb;

            _damageAmountSetter = GetComponent<DamageAmountSetter>();
        }

        private void ExplodeBomb()
        {
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (var collider in colliders)
            {
                var healthSetter = collider.GetComponent<HealthSetter>();
                if (healthSetter != null)
                    healthSetter.ReduceHealth(_damageAmountSetter.damageAmount);
            }

            Instantiate(bombExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}