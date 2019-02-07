using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Structs;
using FortBlast.UI;
using UnityEngine;

namespace FortBlast.Resources
{
    public class CollectCollectibles : MonoBehaviour
    {
        [Header("Collectible Stats")] public GameObject collectionCompletedExplosion;
        public float maxInteractionTime;
        public List<InventoryItemStats> collectibles;

        private bool _collectibleCollected;
        private float _currentInteractionTime;

        private bool _isPlayerLooking;
        private bool _isPlayerNearby;


        private void Start()
        {
            _currentInteractionTime = 0;
            _collectibleCollected = false;
            _isPlayerNearby = false;
            _isPlayerLooking = false;
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
                UniSlider.instance.DiscardSlider(gameObject);
            }

            if (_currentInteractionTime > 0)
            {
                var interactionRatio = _currentInteractionTime / maxInteractionTime;
                UniSlider.instance.UpdateSliderValue(interactionRatio, gameObject);
            }
        }

        private void CheckInteractionTime()
        {
            if (Input.GetKey(Controls.InteractionKey))
            {
                _currentInteractionTime += Time.deltaTime;
                UniSlider.instance.InitSlider(gameObject);
            }
            else
            {
                _currentInteractionTime = 0;
                UniSlider.instance.DiscardSlider(gameObject);
            }

            if (_currentInteractionTime >= maxInteractionTime)
            {
                _collectibleCollected = true;
                UniSlider.instance.DiscardSlider(gameObject);

                var collectionItems = new List<InventoryItemStats>();
                for (var i = 0; i < collectibles.Count; i++)
                {
                    var inventoryItemStats = collectibles[i];
                    var randomValue = Random.Range(0, 1000) % inventoryItemStats.itemCount;

                    if (inventoryItemStats.itemCount <= 1)
                        randomValue = inventoryItemStats.itemCount;
                    if (randomValue <= 0)
                        randomValue = 1;

                    var newCollectionItem = new InventoryItemStats
                    {
                        itemCount = randomValue,
                        inventoryItem = inventoryItemStats.inventoryItem
                    };

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