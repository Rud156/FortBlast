using System.Collections;
using TMPro;
using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextFlicker : MonoBehaviour
    {
        [Header("Effect")] [Range(0, 1)] public float minAlphaIntensity;
        [Range(0, 1)] public float maxAlphaIntensity;
        public float timeBetweenGlitch;

        [Header("Debug")] public bool glitchOnStart;

        private TMP_Text _text;
        private bool _isActive;
        private Coroutine _glitchCoroutine;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();

            if (glitchOnStart)
                StartGlitching();
        }

        public void StartGlitching() => _glitchCoroutine = StartCoroutine(PlayGlitching());

        public void StopGlitching() => StopCoroutine(_glitchCoroutine);

        private IEnumerator PlayGlitching()
        {
            while (true)
            {
                Color textColor = _text.color;
                float randomIntensity = Random.Range(minAlphaIntensity, maxAlphaIntensity);

                _text.color = new Color(textColor.r, textColor.g, textColor.b, randomIntensity);
                yield return new WaitForSeconds(timeBetweenGlitch);
            }
        }
    }
}