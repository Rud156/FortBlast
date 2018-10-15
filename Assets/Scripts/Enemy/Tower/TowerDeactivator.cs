using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Enemy.Tower
{
    public class TowerDeactivator : MonoBehaviour
    {
        [Header("UI Display")]
        public GameObject uiPrompt;
        public Slider timerSlider;
        public GameObject towerSwitchLight;

        [Header("Activation Stats")]
        public TowerController towerController;
        public float maxInteractionTime;

        private float _currentInteractionTime;
        private bool _playerNearby;
        private bool _inPlayerFOV;
        private bool _towerDeactivated;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerNearby = false;
            _inPlayerFOV = false;
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
        /// OnBecameVisible is called when the renderer became visible by any camera.
        /// </summary>
        void OnBecameVisible() => _inPlayerFOV = true;

        /// <summary>
        /// OnBecameInvisible is called when the renderer is no longer visible by any camera.
        /// </summary>
        void OnBecameInvisible() => _inPlayerFOV = false;

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
            if (Input.GetKey(KeyCode.E) && _playerNearby && _inPlayerFOV)
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