using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Enemy.Tower
{
    public class TowerDeactivator : MonoBehaviour
    {
        private float _currentInteractionTime;
        private bool _playerNearby;
        private bool _towerDeactivated;
        public float maxInteractionTime;
        [Header("UI Display")] public Slider timerSlider;

        [Header("Activation Stats")] public TowerController towerController;
        public GameObject towerSwitchLight;

        /// <summary>
        ///     OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _playerNearby = true;
        }

        /// <summary>
        ///     OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _playerNearby = false;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (_towerDeactivated)
                return;

            CheckPlayerInteraction();
            UpdateSliderAndDeactivateTower();

            CheckAndDeactivateTower();
        }

        private void CheckPlayerInteraction()
        {
            if (Input.GetKey(Controls.InteractionKey) && _playerNearby)
                _currentInteractionTime += Time.deltaTime;
            else
                _currentInteractionTime = 0;
        }

        private void UpdateSliderAndDeactivateTower()
        {
            timerSlider.value = _currentInteractionTime / maxInteractionTime;
        }

        private void CheckAndDeactivateTower()
        {
            if (_currentInteractionTime >= maxInteractionTime)
            {
                _towerDeactivated = true;
                towerSwitchLight.SetActive(false);

                towerController.DeactivateTower();
            }
        }
    }
}