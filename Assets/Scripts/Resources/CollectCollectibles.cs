using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class CollectCollectibles : MonoBehaviour
    {
        [Header("UI Display")] public Slider uiSlider;
        public GameObject collectibleUiDisplay;
        
        [Header("Collectible Stats")]
        public GameObject collectionCompletedExplosion;
        public float maxInteractionTime;
        public List<InventoryItemStats> collectibles;

        private bool _collectibleCollected;
        private float _currentInteractionTime;
        
        private bool _isPlayerLooking;
        private bool _isPlayerNearby;
        private Transform _player;


        private void Start()
        {
            _currentInteractionTime = 0;
            _collectibleCollected = false;
            _isPlayerNearby = false;
            _isPlayerLooking = false;

            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = true;
            if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerLooking = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = false;
            else if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerLooking = false;
        }

        private void Update()
        {
            if (_collectibleCollected)
                return;

            if (_isPlayerNearby && _isPlayerLooking)
            {
                CheckInteractionTime();
            }
            else
            {
                _currentInteractionTime = 0;
                collectibleUiDisplay.SetActive(false);
            }

            var interactionRatio = _currentInteractionTime / maxInteractionTime;
            uiSlider.value = interactionRatio;

            RotateCanvasTowardsPlayer();
        }

        private void RotateCanvasTowardsPlayer()
        {
            if (!_player)
                return;

            var lookDirection = _player.position - collectibleUiDisplay.transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(-lookDirection);
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

                var collectionItems = new List<InventoryItemStats>();
                for (var i = 0; i < collectibles.Count; i++)
                {
                    var inventoryItemStats = collectibles[i];
                    var randomValue = Random.Range(0, 1000) % inventoryItemStats.itemCount;

                    if (inventoryItemStats.itemCount <= 1)
                        randomValue = inventoryItemStats.itemCount;
                    if (randomValue <= 0)
                        randomValue = 1;

                    var newCollectionItem = new InventoryItemStats();
                    newCollectionItem.itemCount = randomValue;
                    newCollectionItem.inventoryItem = inventoryItemStats.inventoryItem;

                    collectionItems.Add(newCollectionItem);
                }

                ResourceManager.instance.AddResources(collectionItems);
                DestroyCollectible();
            }
        }

        private void DestroyCollectible()
        {
            Instantiate(collectionCompletedExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}