using FortBlast.Extras;
using FortBlast.Player.AffecterActions;
using FortBlast.Player.Movement;
using FortBlast.Player.StatusDisplay;
using FortBlast.Resources;
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
        private bool _statusDisplayOpen;

        protected virtual void Start()
        {
            _inventoryOpen = false;
            _itemSpawned = false;
            _statusDisplayOpen = false;

            LockCursor();

            InventoryManager.instance.inventoryOpened += InventoryOpened;
            InventoryManager.instance.inventoryItemSelected += InventoryItemSelected;

            PlayerUIDisplay.instance.statusDisplayOpened += StatusDisplayOpened;
        }

        protected void Update()
        {
            if (!Input.GetKeyDown(Controls.CloseKey))
                return;

            if (_itemSpawned)
                SelectedItemDiscarded();

            else if (_inventoryOpen)
                CloseInventory();

            else if (_statusDisplayOpen)
                CloseStatusDisplay();
        }

        protected void OnDestroy()
        {
            InventoryManager.instance.inventoryOpened -= InventoryOpened;
            InventoryManager.instance.inventoryItemSelected -= InventoryItemSelected;
            PlayerUIDisplay.instance.statusDisplayOpened -= StatusDisplayOpened;
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

        #region Status Display

        private void StatusDisplayOpened() => _statusDisplayOpen = true;

        private void CloseStatusDisplay()
        {
            _statusDisplayOpen = false;
            PlayerUIDisplay.instance.CloseStatusDisplay();
        }

        #endregion

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