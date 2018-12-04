using FortBlast.Extras;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FortBlast.UI
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
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

        public delegate void FadeStart();
        public delegate void FadeInComplete();
        public delegate void FadeOutComplete();

        public FadeStart fadeStart;
        public FadeInComplete fadeInComplete;
        public FadeOutComplete fadeOutComplete;

        [Header("Fade Rate")] public float fadeInRate;
        public float fadeOutRate;

        private Image fadeImage;
        private bool activateFadeIn;
        private bool activateFadeOut;
        private float currentAlpha;

        private void Start()
        {
            fadeImage = GetComponent<Image>();
            currentAlpha = ExtensionFunctions.Map(fadeImage.color.a, 0, 1, 0, 255);
        }

        private void Update()
        {
            if (activateFadeIn)
                FadeIn();
            else if (activateFadeOut)
                FadeOut();
        }

        public void StartFadeIn()
        {
            fadeStart?.Invoke();
            activateFadeIn = true;
            activateFadeOut = false;
        }

        private void FadeIn()
        {
            currentAlpha -= fadeInRate * Time.deltaTime;

            Color fadeImageColor = fadeImage.color;
            fadeImage.color =
                ExtensionFunctions.
                    ConvertAndClampColor(fadeImageColor.r, fadeImageColor.g, fadeImageColor.b,
                    currentAlpha);

            if (!(currentAlpha <= 0))
                return;

            fadeInComplete?.Invoke();
            activateFadeIn = false;
            fadeImage.gameObject.SetActive(false);
        }

        public void StartFadeOut()
        {
            fadeImage.gameObject.SetActive(true);

            fadeStart?.Invoke();
            activateFadeOut = true;
            activateFadeIn = false;
        }

        private void FadeOut()
        {
            currentAlpha += fadeOutRate * Time.deltaTime;

            Color fadeImageColor = fadeImage.color;
            fadeImage.color =
                ExtensionFunctions.
                    ConvertAndClampColor(fadeImageColor.r, fadeImageColor.g, fadeImageColor.b,
                    currentAlpha);

            if (!(currentAlpha >= 255))
                return;

            fadeOutComplete?.Invoke();
            activateFadeOut = false;
        }
    }
}