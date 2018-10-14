using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Enemy.Tower
{
    public class TowerDeactivator : MonoBehaviour
    {
        public GameObject uiPrompt;
        public Slider timerSlider;
        public float maxDistanceFromPLayer;
        public float maxInteractionTime;
        public TowerController towerController;

        private Transform _player;
        private float _currentInteractionTime;

        private bool _playerNearby;
        private bool _towerDeactivated;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _playerNearby = false;
            _towerDeactivated = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_towerDeactivated)
                return;

            CheckIfPlayerInRangeAndDisplayPrompt();
            DisplayPrompt();

            CheckPlayerInteraction();
            UpdateSliderAndDeactivateTower();
        }

        private void CheckIfPlayerInRangeAndDisplayPrompt()
        {
            if (_player == null)
                return;

            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            if (distanceToPlayer <= maxDistanceFromPLayer && !_towerDeactivated)
                _playerNearby = true;
            else
                _playerNearby = false;
        }

        private void DisplayPrompt()
        {
            if (_playerNearby && _currentInteractionTime == 0)
                uiPrompt.SetActive(true);
            else
                uiPrompt.SetActive(false);
        }

        private void CheckPlayerInteraction()
        {
            if (Input.GetKey(KeyCode.E) && _playerNearby)
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
                towerController.DeactivateTower();
            }
        }
    }
}