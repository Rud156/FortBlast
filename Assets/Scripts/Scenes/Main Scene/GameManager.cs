using FortBlast.Extras;
using FortBlast.Player.AffecterActions;
using FortBlast.Player.Movement;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.Resources;
using FortBlast.UI;
using UnityEngine;

namespace FortBlast.Scenes.MainScene
{
    public class GameManager : MonoBehaviour
    {
        public PlayerHandControls playerAbsorbDamageController;
        public PlayerLookAtController playerLookAtController;
        public PlayerSpawner playerSpawner;
        
        private bool _inventoryOpen;
        private bool _itemSpawned;

        private void Start()
        {
            _inventoryOpen = false;
            _itemSpawned = false;

            LockCursor();
            TerrainGenerator.instance.terrainGenerationComplete += StartFadingIn;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(Controls.CloseKey))
                return;

            if (_itemSpawned)
                SelectedItemDiscarded();

            else if (_inventoryOpen)
                CloseInventory();
        }

        #region PlayerBase

        private void StartFadingIn()
        {
            Fader.instance.StartFadeIn();
        }

        #endregion PlayerBase
        
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

        #region Singleton

        public static GameManager instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}