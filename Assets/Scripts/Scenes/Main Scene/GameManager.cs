using FortBlast.Extras;
using FortBlast.Player.AffecterActions;
using FortBlast.Player.Movement;
using FortBlast.Resources;
using UnityEngine;

namespace FortBlast.Scenes.MainScene
{
    public class GameManager : MonoBehaviour
    {

        #region Singleton
        public static GameManager instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }
        #endregion Singleton

        public PlayerLookAtController playerLookAtController;
        public PlayerHandControls playerAbsorbDamageController;
        public PlayerSpawner playerSpawner;

        private bool _inventoryOpen;
        private bool _itemSpawned;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _inventoryOpen = false;
            _itemSpawned = false;

            LockCursor();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (!Input.GetKeyDown(Controls.CloseKey))
                return;

            if (_itemSpawned)
                SelectedItemDiscarded();

            else if (_inventoryOpen)
                CloseInventory();
        }

        #region Inventory

        public void InventoryOpened()
        {
            _inventoryOpen = true;
            playerLookAtController.DeActivateRotation();
            playerAbsorbDamageController.DeActivateMechanism();

            UnlockCursor();
        }

        public void CloseInventory()
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
    }
}
