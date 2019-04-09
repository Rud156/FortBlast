using FortBlast.Extras;
using FortBlast.Resources;
using FortBlast.Structs;
using FortBlast.UI;
using System.Collections.Generic;
using FortBlast.Player.StatusDisplay;
using UnityEngine;

namespace FortBlast.Buildings.BaseScene
{
    [RequireComponent(typeof(BoxCollider))]
    public class EquipmentFixer : MonoBehaviour
    {
        private const float DelayedFixingTime = 0.5f;

        [Header("Display")]
        public GameObject repairEffect;
        public Vector3 effectOffset;
        public Vector3 effectScale;

        [Header("Fixing Requirements")]
        public List<InventoryItemStats> inventoryItems;

        [Header("Object Affected")]
        public float totalFixingTime;
        public EquipmentToBeAffected[] equipments;

        private float _currentFixingTime;
        private bool _equipmentFixed;

        private bool _playerIsNearby;
        private bool _isPlayerLooking;

        private bool _resourcesAvailable;

        private void Start() => _currentFixingTime = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _playerIsNearby = true;
            if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerLooking = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
            {
                _playerIsNearby = false;
                UniSlider.instance.DiscardSlider(gameObject);
            }
            if (other.CompareTag(TagManager.VisibleCollider))
            {
                _isPlayerLooking = false;
                UniSlider.instance.DiscardSlider(gameObject);
            }
        }

        private void Update()
        {
            if (_equipmentFixed)
                return;

            if (_playerIsNearby && _isPlayerLooking)
                CheckPlayerInteraction();
            else
                _currentFixingTime = 0;
        }

        private void CheckPlayerInteraction()
        {
            if (Input.GetKeyDown(Controls.InteractionKey))
            {
                CheckForResources();
                PlayerUIDisplay.instance.DisplayItemList(
                    inventoryItems, "Not Enough Resources Available",
                    Color.white, Color.red);
                UniSlider.instance.InitSlider(gameObject);
            }
            else if (Input.GetKeyUp(Controls.InteractionKey))
                UniSlider.instance.DiscardSlider(gameObject);
            else if (Input.GetKey(Controls.InteractionKey))
            {
                if (!_resourcesAvailable)
                {
                    _currentFixingTime = 0;
                    return;
                }

                _currentFixingTime += Time.deltaTime;
            }

            if (_currentFixingTime > 0)
            {
                float interactionTimeRatio = _currentFixingTime / totalFixingTime;
                UniSlider.instance.UpdateSliderValue(interactionTimeRatio, gameObject);
            }

            if (_currentFixingTime >= totalFixingTime)
            {
                Vector3 effectPosition = equipments[0].affectedObject.transform.position + effectOffset;
                GameObject repairEffectInstance = Instantiate(repairEffect, effectPosition, Quaternion.identity);
                repairEffectInstance.transform.localScale = effectScale;

                _equipmentFixed = true;

                UniSlider.instance.DiscardSlider(gameObject);
                Invoke(nameof(FixObjects), DelayedFixingTime);
            }
        }

        private void FixObjects()
        {
            foreach (EquipmentToBeAffected equipment in equipments)
            {
                GameObject affectedObject = equipment.affectedObject;
                affectedObject.transform.position = equipment.originalPosition;
                affectedObject.transform.rotation = Quaternion.Euler(equipment.originalRotation);
            }
        }

        private void CheckForResources()
        {
            bool allResourcesAvailable = true;
            foreach (InventoryItemStats inventoryItemStat in inventoryItems)
            {
                int itemCount = ResourceManager.instance.CountResource(inventoryItemStat.inventoryItem.itemId);

                if (itemCount < inventoryItemStat.itemCount)
                {
                    allResourcesAvailable = false;
                    break;
                }
            }

            _resourcesAvailable = allResourcesAvailable;
        }


        [System.Serializable]
        public struct EquipmentToBeAffected
        {
            public GameObject affectedObject;
            public Vector3 originalPosition;
            public Vector3 originalRotation;
        }
    }
}