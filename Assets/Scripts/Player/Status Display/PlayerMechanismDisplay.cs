using System.Collections;
using System.Collections.Generic;
using FortBlast.Player.AffecterActions;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerMechanismDisplay : MonoBehaviour
    {
        #region Singleton

        private static PlayerMechanismDisplay _instance;

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

        [Header("Required Components")]
        public PlayerHandControls playerHandControls;
        public Image reflectionDial;
        public Image teleporterDial;

        [Header("Reflector Colors")]
        public Color minReflectorColor = Color.red;
        public Color halfReflectorColor = Color.yellow;
        public Color maxReflectorColor = Color.green;

        [Header("Teleporter Colors")]
        public Color minTeleporterColor = Color.white;
        public Color halfTeleporterColor = Color.magenta;
        public Color maxTeleporterColor = Color.cyan;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            DisplayReflectionCount();
            DisplayTeleporterCount();
        }

        private void DisplayReflectionCount()
        {
            float currentReflectionCount = playerHandControls.GetCurrentReflectorCount();
            float maxReflectionCount = playerHandControls.maxReflectionCount;
            float reflectionRatio = currentReflectionCount / maxReflectionCount;
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
            float currentTeleporterCount = playerHandControls.GetCurrentTeleporterCount();
            float maxTeleporterCount = playerHandControls.maxTeleporterCount;
            float teleporterRatio = currentTeleporterCount / maxTeleporterCount;
            if (teleporterRatio <= 0.5f)
                reflectionDial.color = Color.Lerp(minTeleporterColor, halfTeleporterColor,
                    teleporterRatio * 2);
            else
                teleporterDial.color = Color.Lerp(halfTeleporterColor, maxTeleporterColor,
                    (teleporterRatio - 0.5f) * 2);

            teleporterDial.fillAmount = teleporterRatio;
        }
    }
}