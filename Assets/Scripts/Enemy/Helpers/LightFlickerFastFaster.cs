using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(Renderer))]
    public class LightFlickerFastFaster : MonoBehaviour
    {
        public delegate void FlickerComplete();

        private bool _completed;

        private float _currentFlickerRateValue;
        private float _currentFlickerTimeValue;
        private bool _lightOn;


        private Renderer _renderer;
        public Color emissionColor;

        [Header("Color Data")] [Range(1, 4)] public float emissionStrength;

        public FlickerComplete flickerComplete;

        [Range(4, 10)] public int maxFlickerTimes;

        [Header("Flicker Data")] public float startFlickerRate;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _currentFlickerRateValue = startFlickerRate;
            _currentFlickerTimeValue = maxFlickerTimes * 2;

            _completed = false;
            _lightOn = false;

            _renderer = GetComponent<Renderer>();
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            MakeLightFlicker();
        }

        private void MakeLightFlicker()
        {
            if (_completed)
                return;

            _currentFlickerRateValue -= Time.deltaTime;

            if (_currentFlickerRateValue <= 0)
            {
                if (_lightOn)
                    TurnLightOn();
                else
                    TurnLightOff();

                if (_currentFlickerTimeValue <= 0)
                {
                    _completed = true;
                    flickerComplete?.Invoke();
                    return;
                }

                if (_currentFlickerTimeValue <= 8)
                {
                    _currentFlickerRateValue = 0.1f;
                }
                else if (_currentFlickerTimeValue <= 10)
                {
                    _currentFlickerRateValue = startFlickerRate / 2f;
                }
                else
                {
                    _currentFlickerRateValue = startFlickerRate;
                }

                _currentFlickerTimeValue -= 1;
                _lightOn = !_lightOn;
            }
        }

        private void TurnLightOn()
        {
            _renderer.material.SetColor("_EmissionColor", emissionColor * emissionStrength);
        }

        private void TurnLightOff()
        {
            _renderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}