using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [Header("General Information")] public List<FlickerInformation> lightFlicker;
        [Header("Debug")] public bool flickerOnStart;

        private Coroutine _flickerCoroutine;
        private Light _affectorLight;
        private List<FlickerInformation> _shuffledFlickerCopy;

        private void Start()
        {
            _affectorLight = GetComponent<Light>();

            if (flickerOnStart)
                StartFlickering();
        }

        public void EnableFlickering() => StartFlickering();

        public void DisableFlickering() => StopFlickering();

        private void StartFlickering() => _flickerCoroutine = StartCoroutine(FlickerLight());

        private void StopFlickering() => StopCoroutine(_flickerCoroutine);

        private IEnumerator FlickerLight()
        {
            while (true)
            {
                _shuffledFlickerCopy = ExtensionFunctions.Shuffle(lightFlicker);
                foreach (FlickerInformation flickerInformation in _shuffledFlickerCopy)
                {
                    _affectorLight.intensity = flickerInformation.onIntensity;
                    yield return new WaitForSeconds(flickerInformation.onTime);

                    _affectorLight.intensity = flickerInformation.offIntensity;
                    yield return new WaitForSeconds(flickerInformation.offIntensity);
                }
            }
        }
    }
}