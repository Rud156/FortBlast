using System.Collections;
using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Player.Data;
using FortBlast.Resources;
using FortBlast.Scenes.Main_Scene;
using UnityEngine;

namespace FortBlast.Player.Affecter_Actions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerSpawner : MonoBehaviour
    {
        public Transform spawnPoint;
        public float itemLaunchVelocity;
        public GameObject launchEffect;

        private Animator _playerAnimator;

        private InventoryItem _item;
        private GameObject _itemInstance;
        private Rigidbody _itemRB;
        private BoxCollider _itemCollider;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerAnimator = GetComponent<Animator>();
            _itemInstance = null;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                SpawnItemWorld();
        }

        public void ClearItemIfNotSpawned()
        {
            if (_itemInstance == null)
                return;

            _playerAnimator.SetBool(PlayerData.PlayerSpawning, false);
            ResourceManager.instance.AddResource(_item);
            Destroy(_itemInstance);
        }

        public void SpawnItemDisplay(InventoryItem item)
        {
            _itemInstance = Instantiate(item.prefab, spawnPoint.position, Quaternion.identity);
            _itemInstance.transform.SetParent(spawnPoint);

            _itemRB = _itemInstance.GetComponent<Rigidbody>();
            _itemCollider = _itemInstance.GetComponent<BoxCollider>();

            _itemRB.isKinematic = true;
            _itemCollider.enabled = false;

            _playerAnimator.SetBool(PlayerData.PlayerSpawning, true);
            _item = item;
        }

        public void SpawnItemWorld()
        {
            if (_itemInstance == null)
                return;

            GameManager.instance.InventoryItemUsed();

            _itemRB.isKinematic = false;
            _itemRB.velocity = itemLaunchVelocity * transform.forward;

            _itemCollider.enabled = true;

            Instantiate(launchEffect, spawnPoint.position, Quaternion.identity);

            _playerAnimator.SetBool(PlayerData.PlayerSpawning, false);

            _itemInstance.transform.SetParent(null);
            _itemInstance = null;
        }
    }
}
