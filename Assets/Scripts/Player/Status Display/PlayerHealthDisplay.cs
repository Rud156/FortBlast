using FortBlast.Common;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [Header("UI Display")] public Slider healthSlider;
        public Image healthFiller;

        [Header("Controllers")] public Transform mainCameraHolder;
        public HealthSetter playerHealthSetter;

        [Header("Health Stats")] public Color minHealthColor = Color.red;
        public Color halfHealthColor = Color.yellow;
        public Color maxHealthColor = Color.green;

        private void Start() => playerHealthSetter.healthZero += PlayerDead;

        private void Update() => DisplayHealthToUI();

        private void DisplayHealthToUI()
        {
            var currentHealthAmount = playerHealthSetter.GetCurrentHealth();
            var maxHealthAmount = playerHealthSetter.maxHealthAmount;
            var healthRatio = currentHealthAmount / maxHealthAmount;
            healthFiller.color = healthRatio <= 0.5f
                ? Color.Lerp(minHealthColor, halfHealthColor, healthRatio * 2)
                : Color.Lerp(halfHealthColor, maxHealthColor, (healthRatio - 0.5f) * 2);

            healthSlider.value = healthRatio;
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