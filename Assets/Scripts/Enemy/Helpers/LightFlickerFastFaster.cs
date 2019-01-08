using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(Renderer))]
    public class LightFlickerFastFaster : MonoBehaviour
    {
        public delegate void FlickerComplete();

        public FlickerComplete flickerComplete;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Header("Color Data")] [Range(1, 4)] public float emissionStrength;
        public Color emissionColor;

        [Header("Flicker Data")] public float startFlickerRate;
        [Range(4, 10)] public int maxFlickerTimes;

        private bool _completed;
        private float _currentFlickerRateValue;
        private float _currentFlickerTimeValue;
        private bool _lightOn;

        private Renderer _renderer;

        private void Start()
        {
            _currentFlickerRateValue = startFlickerRate;
            _currentFlickerTimeValue = maxFlickerTimes * 2;

            _completed = false;
            _lightOn = false;

            _renderer = GetComponent<Renderer>();
        }

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

        private void TurnLightOn() =>
            _renderer.material.SetColor(EmissionColor, emissionColor * emissionStrength);

        private void TurnLightOff() => _renderer.material.SetColor(EmissionColor, emissionColor);
    }
}