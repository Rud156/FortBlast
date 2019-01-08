using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.UI
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        public delegate void FadeInComplete();

        public FadeInComplete fadeInComplete;

        public delegate void FadeOutComplete();

        public FadeOutComplete fadeOutComplete;

        public delegate void FadeStart();

        public FadeStart fadeStart;


        [Header("Fade Rate")] public float fadeInRate;
        public float fadeOutRate;

        private Image _fadeImage;

        private float _currentAlpha;
        private bool _activateFadeIn;
        private bool _activateFadeOut;

        private void Start()
        {
            _fadeImage = GetComponent<Image>();
            _currentAlpha = ExtensionFunctions.Map(_fadeImage.color.a, 0, 1, 0, 255);
        }

        private void Update()
        {
            if (_activateFadeIn)
                FadeIn();
            else if (_activateFadeOut)
                FadeOut();
        }

        public void StartFadeIn()
        {
            fadeStart?.Invoke();
            _activateFadeIn = true;
            _activateFadeOut = false;
        }

        private void FadeIn()
        {
            _currentAlpha -= fadeInRate * Time.deltaTime;

            var fadeImageColor = _fadeImage.color;
            _fadeImage.color =
                ExtensionFunctions.ConvertAndClampColor(fadeImageColor.r, fadeImageColor.g, fadeImageColor.b,
                    _currentAlpha);

            if (!(_currentAlpha <= 0))
                return;

            fadeInComplete?.Invoke();
            _activateFadeIn = false;
            _fadeImage.gameObject.SetActive(false);
        }

        public void StartFadeOut()
        {
            _fadeImage.gameObject.SetActive(true);

            fadeStart?.Invoke();
            _activateFadeOut = true;
            _activateFadeIn = false;
        }

        private void FadeOut()
        {
            _currentAlpha += fadeOutRate * Time.deltaTime;

            var fadeImageColor = _fadeImage.color;
            _fadeImage.color =
                ExtensionFunctions.ConvertAndClampColor(fadeImageColor.r, fadeImageColor.g, fadeImageColor.b,
                    _currentAlpha);

            if (!(_currentAlpha >= 255))
                return;

            fadeOutComplete?.Invoke();
            _activateFadeOut = false;
        }

        #region Singleton

        public static Fader instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}