using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        private const int PostProcessLayer = 8;
        private const int PostProcessOverridePriority = 100;

        [Header("UI Display")] public Slider healthSlider;
        public Image healthFiller;

        [Header("Controllers")] public Transform mainCameraHolder;
        public HealthSetter playerHealthSetter;

        [Header("Health Stats")] public Color minHealthColor = Color.red;
        public Color halfHealthColor = Color.yellow;
        public Color maxHealthColor = Color.green;

        private PostProcessVolume _postProcessVolume;
        private Vignette _healthVignette;
        private bool _postProcessOverrideApplied;

        private void Start()
        {
            playerHealthSetter.healthZero += PlayerDead;
            playerHealthSetter.healthChanged += DisplayHealthToUI;

            DisplayHealthToUI();
            CreateOverrideVignetteEffect();
        }

        private void DisplayHealthToUI()
        {
            var currentHealthAmount = playerHealthSetter.GetCurrentHealth();
            var maxHealthAmount = playerHealthSetter.maxHealthAmount;
            var healthRatio = currentHealthAmount / maxHealthAmount;

            if (healthRatio <= 0.5f)
            {
                if (!_postProcessOverrideApplied)
                {
                    CreateOverrideVignetteEffect();
                    _postProcessVolume = PostProcessManager.instance.QuickVolume(
                        PostProcessLayer,
                        PostProcessOverridePriority,
                        _healthVignette
                    );
                    _postProcessOverrideApplied = true;
                }

                _healthVignette.color.value = Color.Lerp(
                    Color.black,
                    minHealthColor,
                    ExtensionFunctions.Map(healthRatio, 0, 0.5f, 1, 0.25f)
                );
            }
            else if (healthRatio > 0.5f && _postProcessOverrideApplied)
            {
                RuntimeUtilities.DestroyVolume(_postProcessVolume, true);
                _postProcessOverrideApplied = false;
            }

            healthFiller.color = healthRatio <= 0.5f
                ? Color.Lerp(minHealthColor, halfHealthColor, healthRatio * 2)
                : Color.Lerp(halfHealthColor, maxHealthColor, (healthRatio - 0.5f) * 2);

            healthSlider.value = healthRatio;
        }

        private void CreateOverrideVignetteEffect()
        {
            _healthVignette = ScriptableObject.CreateInstance<Vignette>();
            _healthVignette.enabled.Override(true);
            _healthVignette.color.Override(minHealthColor);
        }

        private void PlayerDead() => mainCameraHolder.SetParent(null);

        #region Singleton

        private static PlayerHealthDisplay _instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}