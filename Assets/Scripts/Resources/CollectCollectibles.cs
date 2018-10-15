using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class CollectCollectibles : MonoBehaviour
    {
        [System.Serializable]
        public struct Collectibles
        {
            public InventoryItem item;
            public int itemCount;
        }

        public List<Collectibles> collectibles;
        public Slider uiSlider;
        public GameObject uiDisplay;
        public float maxInteractionTime;

        private float _currentInteractionTime;
        private bool _collectibleCollected;
        private bool _isPlayerNearby;
        private bool _isPlayerLooking;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _currentInteractionTime = 0;
            _collectibleCollected = false;
            _isPlayerNearby = false;
            _isPlayerLooking = false;
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = true;
        }

        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = false;
        }

        /// <summary>
        /// OnBecameVisible is called when the renderer became visible by any camera.
        /// </summary>
        void OnBecameVisible() => _isPlayerLooking = true;

        /// <summary>
        /// OnBecameInvisible is called when the renderer is no longer visible by any camera.
        /// </summary>
        void OnBecameInvisible() => _isPlayerLooking = false;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_collectibleCollected)
                return;

            if (_isPlayerNearby && _isPlayerLooking)
                CheckInteractionTime();
            else
            {
                _currentInteractionTime = 0;
                uiDisplay.SetActive(false);
            }

            float interactionRatio = _currentInteractionTime / maxInteractionTime;
            uiSlider.value = interactionRatio;
        }

        private void CheckInteractionTime()
        {
            if (Input.GetKey(Controls.InteractionCode) && _isPlayerNearby && _isPlayerLooking)
                _currentInteractionTime += Time.deltaTime;

            if (_currentInteractionTime >= maxInteractionTime)
            {
                _collectibleCollected = true;
                uiDisplay.SetActive(false);
                // Put the items in the inventory
            }
        }
    }
}
