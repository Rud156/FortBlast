using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FadeOutParticles : MonoBehaviour
    {
        private float _decreaseRate;
        private ParticleSystem.EmissionModule _emissionModule;
        private ParticleSystem _particles;
        public float timeToZero;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _particles = GetComponent<ParticleSystem>();
            _emissionModule = _particles.emission;
            _decreaseRate = _emissionModule.rateOverTime.constant / timeToZero;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            _emissionModule.rateOverTime = _emissionModule.rateOverTime.constant -
                                           _decreaseRate * Time.deltaTime;
        }
    }
}