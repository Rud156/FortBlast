using FortBlast.Extras;
using FortBlast.Player.AffecterActions;
using FortBlast.Player.Movement;
using FortBlast.Resources;
using JetBrains.Annotations;
using UnityEngine;

namespace FortBlast.BaseClasses
{
    public abstract class GameManager : MonoBehaviour
    {
        [Header("Player")] public PlayerHandControls playerAbsorbDamageController;
        public PlayerLookAtController playerLookAtController;
        public PlayerSpawner playerSpawner;

        private bool _inventoryOpen;
        private bool _itemSpawned;

        protected virtual void Start()
        {
            _inventoryOpen = false;
            _itemSpawned = false;

            LockCursor();
            InventoryManager.instance.inventoryOpened += InventoryOpened;
            InventoryManager.instance.inventoryItemSelected += InventoryItemSelected;
        }

        protected void Update()
        {
            if (!Input.GetKeyDown(Controls.CloseKey))
                return;

            if (_itemSpawned)
                SelectedItemDiscarded();

            else if (_inventoryOpen)
                CloseInventory();
        }

        protected void OnDestroy()
        {
            InventoryManager.instance.inventoryOpened -= InventoryOpened;
            InventoryManager.instance.inventoryItemSelected -= InventoryItemSelected;
        }

        #region Inventory

        private void InventoryOpened()
        {
            _inventoryOpen = true;
            playerLookAtController.DeActivateRotation();
            playerAbsorbDamageController.DeActivateMechanism();

            UnlockCursor();
        }

        private void CloseInventory()
        {
            _inventoryOpen = false;
            playerLookAtController.ActivateRotation();
            playerAbsorbDamageController.ActivateMechanism();

            InventoryManager.instance.CloseInventory();
            LockCursor();
        }

        #endregion Inventory

        #region InventoryItem

        public void InventoryItemSelected(InventoryItem item)
        {
            _inventoryOpen = false;
            _itemSpawned = true;

            playerSpawner.SpawnItemDisplay(item);
            playerLookAtController.ActivateRotation();
        }

        public void InventoryItemUsed()
        {
            _itemSpawned = false;
            playerAbsorbDamageController.ActivateMechanism();
        }

        public void SelectedItemDiscarded()
        {
            _itemSpawned = false;
            playerSpawner.ClearItemIfNotSpawned();
            playerAbsorbDamageController.ActivateMechanism();
        }

        #endregion InventoryItem

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        #region Singleton

        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion
    }
}