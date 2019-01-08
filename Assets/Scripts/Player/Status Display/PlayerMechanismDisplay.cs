using FortBlast.Player.AffecterActions;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerMechanismDisplay : MonoBehaviour
    {
        [Header("Reflector Colors")] public Color minReflectorColor = Color.red;
        public Color halfReflectorColor = Color.yellow;
        public Color maxReflectorColor = Color.green;

        [Header("Teleporter Colors")] public Color minTeleporterColor = Color.white;
        public Color halfTeleporterColor = Color.magenta;
        public Color maxTeleporterColor = Color.cyan;

        [Header("Required Components")] public PlayerHandControls playerHandControls;
        public Image reflectionDial;
        public Image teleporterDial;

        private void Update()
        {
            DisplayReflectionCount();
            DisplayTeleporterCount();
        }

        private void DisplayReflectionCount()
        {
            var currentReflectionCount = playerHandControls.GetCurrentReflectorCount();
            float maxReflectionCount = playerHandControls.maxReflectionCount;
            var reflectionRatio = currentReflectionCount / maxReflectionCount;
            if (reflectionRatio <= 0.5f)
                reflectionDial.color = Color.Lerp(minReflectorColor, halfReflectorColor,
                    reflectionRatio * 2);
            else
                reflectionDial.color = Color.Lerp(halfReflectorColor, maxReflectorColor,
                    (reflectionRatio - 0.5f) * 2);

            reflectionDial.fillAmount = reflectionRatio;
        }

        private void DisplayTeleporterCount()
        {
            var currentTeleporterCount = playerHandControls.GetCurrentTeleporterCount();
            float maxTeleporterCount = playerHandControls.maxTeleporterCount;
            var teleporterRatio = currentTeleporterCount / maxTeleporterCount;
            if (teleporterRatio <= 0.5f)
                reflectionDial.color = Color.Lerp(minTeleporterColor, halfTeleporterColor,
                    teleporterRatio * 2);
            else
                teleporterDial.color = Color.Lerp(halfTeleporterColor, maxTeleporterColor,
                    (teleporterRatio - 0.5f) * 2);

            teleporterDial.fillAmount = teleporterRatio;
        }

        #region Singleton

        private static PlayerMechanismDisplay _instance;

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