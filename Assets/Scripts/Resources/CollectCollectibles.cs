using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Extras;
using FortBlast.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class CollectCollectibles : MonoBehaviour
    {
        public List<InventoryItemStats> collectibles;
        public GameObject collectionCompletedExplosion;
        public float maxInteractionTime;

        [Header("UI Display")]
        public Slider uiSlider;
        public GameObject collectibleUiDisplay;

        private float _currentInteractionTime;
        private bool _collectibleCollected;
        private bool _isPlayerNearby;
        private bool _isPlayerLooking;

        private Transform _player;

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

            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = true;
            if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerLooking = true;
        }

        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = false;
            else if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerLooking = false;
        }

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
                collectibleUiDisplay.SetActive(false);
            }

            float interactionRatio = _currentInteractionTime / maxInteractionTime;
            uiSlider.value = interactionRatio;

            RotateCanvasTowardsPlayer();
        }

        private void RotateCanvasTowardsPlayer()
        {
            if (_player == null)
                return;

            Vector3 lookDirection = _player.position - collectibleUiDisplay.transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(-lookDirection);
                collectibleUiDisplay.transform.rotation =
                    Quaternion.Slerp(collectibleUiDisplay.transform.rotation, rotation,
                        5 * Time.deltaTime);
            }
        }

        private void CheckInteractionTime()
        {
            if (Input.GetKey(Controls.InteractionKey))
            {
                _currentInteractionTime += Time.deltaTime;
                collectibleUiDisplay.SetActive(true);
            }
            else
            {
                _currentInteractionTime = 0;
                collectibleUiDisplay.SetActive(false);
            }

            if (_currentInteractionTime >= maxInteractionTime)
            {
                _collectibleCollected = true;
                collectibleUiDisplay.SetActive(false);

                List<InventoryItemStats> collectionItems = new List<InventoryItemStats>();
                for (int i = 0; i < collectibles.Count; i++)
                {
                    InventoryItemStats inventoryItemStats = collectibles[i];
                    int randomValue = Random.Range(0, 1000) % inventoryItemStats.itemCount;

                    if (inventoryItemStats.itemCount <= 1)
                        randomValue = inventoryItemStats.itemCount;
                    if (randomValue <= 0)
                        randomValue = 1;

                    InventoryItemStats newCollectionItem = new InventoryItemStats();
                    newCollectionItem.itemCount = randomValue;
                    newCollectionItem.inventoryItem = inventoryItemStats.inventoryItem;

                    collectionItems.Add(newCollectionItem);

                }

                ResourceManager.instance.AddResources(collectionItems);
                DestoryCollectible();
            }
        }

        private void DestoryCollectible()
        {
            Instantiate(collectionCompletedExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
