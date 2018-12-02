using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FadeOutParticles : MonoBehaviour
    {
        public float timeToZero;

        private float _decreaseRate;
        private ParticleSystem _particles;
        private ParticleSystem.EmissionModule _emissionModule;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _particles = GetComponent<ParticleSystem>();
            _emissionModule = _particles.emission;
            _decreaseRate = _emissionModule.rateOverTime.constant / timeToZero;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            _emissionModule.rateOverTime = _emissionModule.rateOverTime.constant -
                _decreaseRate * Time.deltaTime;
        }

    }
}