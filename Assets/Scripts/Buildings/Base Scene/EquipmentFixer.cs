﻿using FortBlast.Extras;
using FortBlast.UI;
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

        [Header("Object Affected")]
        public float totalFixingTime;
        public EquipmentToBeAffected[] equipments;

        private float _currentFixingTime;
        private bool _equipmentFixed;
        private bool _playerIsNearby;
        private bool _isPlayerLooking;

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
                UniSlider.instance.InitSlider(gameObject);
            else if (Input.GetKeyUp(Controls.InteractionKey))
                UniSlider.instance.DiscardSlider(gameObject);
            else if (Input.GetKey(Controls.InteractionKey))
                _currentFixingTime += Time.deltaTime;

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


        [System.Serializable]
        public struct EquipmentToBeAffected
        {
            public GameObject affectedObject;
            public Vector3 originalPosition;
            public Vector3 originalRotation;
        }
    }
}