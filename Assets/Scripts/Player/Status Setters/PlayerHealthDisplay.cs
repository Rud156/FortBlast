using FortBlast.Common;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusSetters
{
    [RequireComponent(typeof(HealthSetter))]
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [Header("Camera Holder")]
        public Transform mainCameraHolder;

        [Header("UI Display")]
        public Slider healthSlider;
        public Image healthFiller;

        [Header("Health Stats")]
        public Color minHealthColor = Color.red;
        public Color halfHealthColor = Color.yellow;
        public Color maxHealthColor = Color.green;

        private HealthSetter _playerHealthSetter;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerHealthSetter = GetComponent<HealthSetter>();
            _playerHealthSetter.healthZero += PlayerDead;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => DisplayHealthToUI();

        private void DisplayHealthToUI()
        {

            float currentHealthAmount = _playerHealthSetter.GetCurrentHealth();
            float maxHealthAmount = _playerHealthSetter.maxHealthAmount;
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
