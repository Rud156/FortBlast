using FortBlast.BaseClasses;
using FortBlast.Extras;
using FortBlast.Player.Data;
using FortBlast.Resources;
using UnityEngine;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerSpawner : MonoBehaviour
    {
        public float itemLaunchVelocity;
        public GameObject launchEffect;
        public Transform spawnPoint;

        private InventoryItem _item;
        private Collider _itemCollider;

        private Transform _itemHolder;
        private GameObject _itemInstance;
        private Rigidbody _itemRB;
        private Animator _playerAnimator;

        private void Start()
        {
            _playerAnimator = GetComponent<Animator>();
            _itemInstance = null;

            _itemHolder = GameObject.FindGameObjectWithTag(TagManager.DistractorHolder)?.transform;
        }

        private void Update()
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
            _itemCollider = _itemInstance.GetComponent<Collider>();

            _itemRB.isKinematic = true;
            _itemCollider.enabled = false;

            _playerAnimator.SetBool(PlayerData.PlayerSpawning, true);
            _item = item;
        }

        private void SpawnItemWorld()
        {
            if (_itemInstance == null)
                return;

            GameManager.instance.InventoryItemUsed();

            _itemRB.isKinematic = false;
            _itemRB.velocity = itemLaunchVelocity * transform.forward;

            _itemCollider.enabled = true;

            Instantiate(launchEffect, spawnPoint.position, Quaternion.identity);

            _playerAnimator.SetBool(PlayerData.PlayerSpawning, false);

            _itemInstance.transform.SetParent(_itemHolder);
            _itemInstance = null;
        }
    }
}