using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        public float flickerRate;
        public bool useRandomFlickerRate = false;

        private Light _light;
        private float _currentValue;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            if (useRandomFlickerRate)
                flickerRate = Random.Range(0.0f, 10f);

            _light = GetComponent<Light>();
            _currentValue = flickerRate;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => MakeLightFlicker();

        private void MakeLightFlicker()
        {
            _currentValue -= Time.deltaTime;
            if (_currentValue <= 0)
            {
                _light.enabled = !_light.enabled;
                _currentValue = flickerRate;
            }
        }
    }
}