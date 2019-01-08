using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FadeOutParticles : MonoBehaviour
    {
        public float timeToZero;
        
        private float _decreaseRate;
        private ParticleSystem.EmissionModule _emissionModule;
        private ParticleSystem _particles;

        private void Start()
        {
            _particles = GetComponent<ParticleSystem>();
            _emissionModule = _particles.emission;
            _decreaseRate = _emissionModule.rateOverTime.constant / timeToZero;
        }

        private void Update()
        {
            _emissionModule.rateOverTime = _emissionModule.rateOverTime.constant -
                                           _decreaseRate * Time.deltaTime;
        }
    }
}