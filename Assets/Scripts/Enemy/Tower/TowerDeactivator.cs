using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Enemy.Tower
{
    public class TowerDeactivator : MonoBehaviour
    {
        [Header("UI Display")] public GameObject uiPrompt;
        public Slider timerSlider;
        public GameObject towerSwitchLight;

        [Header("Activation Stats")] public TowerController towerController;
        public float maxInteractionTime;

        private float _currentInteractionTime;
        private bool _playerNearby;
        private bool _towerDeactivated;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerNearby = false;
            _towerDeactivated = false;
            _currentInteractionTime = 0;
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _playerNearby = true;
        }

        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _playerNearby = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_towerDeactivated)
                return;

            DisplayPrompt();

            CheckPlayerInteraction();
            UpdateSliderAndDeactivateTower();
        }

        private void DisplayPrompt()
        {
            if (_playerNearby)
                uiPrompt.SetActive(true);
            else
                uiPrompt.SetActive(false);
        }

        private void CheckPlayerInteraction()
        {
            if (Input.GetKey(Controls.InteractionKey) && _playerNearby)
            {
                _currentInteractionTime += Time.deltaTime;
                timerSlider.gameObject.SetActive(true);
            }
            else
            {
                _currentInteractionTime = 0;
                timerSlider.gameObject.SetActive(false);
            }
        }

        private void UpdateSliderAndDeactivateTower()
        {
            timerSlider.value = _currentInteractionTime / maxInteractionTime;
            if (_currentInteractionTime >= maxInteractionTime)
            {
                _towerDeactivated = true;

                timerSlider.gameObject.SetActive(false);
                uiPrompt.SetActive(false);
                towerSwitchLight.SetActive(false);

                towerController.DeactivateTower();
            }
        }
    }
}