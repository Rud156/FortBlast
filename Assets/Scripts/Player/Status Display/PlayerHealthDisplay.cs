using FortBlast.Common;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        #region Singleton

        private static PlayerHealthDisplay _instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        [Header("Controllers")]
        public Transform mainCameraHolder;
        public HealthSetter playerHealthSetter;

        [Header("UI Display")]
        public Slider healthSlider;
        public Image healthFiller;

        [Header("Health Stats")]
        public Color minHealthColor = Color.red;
        public Color halfHealthColor = Color.yellow;
        public Color maxHealthColor = Color.green;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => playerHealthSetter.healthZero += PlayerDead;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => DisplayHealthToUI();

        private void DisplayHealthToUI()
        {

            float currentHealthAmount = playerHealthSetter.GetCurrentHealth();
            float maxHealthAmount = playerHealthSetter.maxHealthAmount;
            float healthRatio = currentHealthAmount / maxHealthAmount;
            if (healthRatio <= 0.5f)
                healthFiller.color = Color.Lerp(minHealthColor, halfHealthColor, healthRatio * 2);
            else
                healthFiller.color = Color.Lerp(halfHealthColor, maxHealthColor, (healthRatio - 0.5f) * 2);

            healthSlider.value = healthRatio;
        }

        private void PlayerDead() => mainCameraHolder.SetParent(null);
    }
}
