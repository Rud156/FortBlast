using FortBlast.Common;
using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Resources
{
    public class InventoryHotKeysManager : MonoBehaviour
    {
        public InventoryItem bandage;
        public InventoryItem bottle;
        public InventoryItem chocolate;
        
        private HealthSetter _playerHealth;

        private void Start() =>
            _playerHealth = GameObject.FindGameObjectWithTag(TagManager.Player)?.GetComponent<HealthSetter>();

        private void Update()
        {
            if (Input.GetKeyDown(Controls.HotKey_1))
                SpawnBottleOnKeyPress();
            if (Input.GetKeyDown(Controls.HotKey_2))
                UseBandageOnKeyPress();
            if (Input.GetKeyDown(Controls.HotKey_3))
                EatChocolateOnKeyPress();
        }

        #region Singleton

        private static InventoryHotKeysManager _instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        #region HotKeyActions

        private void SpawnBottleOnKeyPress()
        {
            if (!ResourceManager.instance.HasResource(bottle.displayName))
                return;

            GameManager.instance.InventoryItemSelected(bottle);
            ResourceManager.instance.UseResource(bottle.displayName);

            InventoryManager.instance.CloseInventory();
            InventoryManager.instance.ClearItemSelection();
        }

        private void EatChocolateOnKeyPress()
        {
            if (!ResourceManager.instance.HasResource(chocolate.displayName))
                return;

            _playerHealth.AddHealth(chocolate.healthAmount);
            ResourceManager.instance.UseResource(chocolate.displayName);

            InventoryManager.instance.CloseInventory();
            InventoryManager.instance.ClearItemSelection();
        }

        private void UseBandageOnKeyPress()
        {
            if (!ResourceManager.instance.HasResource(bandage.displayName))
                return;

            _playerHealth.AddHealth(bandage.healthAmount);
            ResourceManager.instance.UseResource(bandage.displayName);

            InventoryManager.instance.CloseInventory();
            InventoryManager.instance.ClearItemSelection();
        }

        #endregion HotKeyActions
    }
}