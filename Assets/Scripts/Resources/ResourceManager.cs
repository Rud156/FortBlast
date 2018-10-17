using System.Collections.Generic;
using System.Text;
using FortBlast.Common;
using FortBlast.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        #region  Singleton
        public static ResourceManager instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(instance);
        }
        #endregion Singleton

        [Header("UI Display")]
        public Text contentDisplay;
        public Animator contentDisplayAnimator;

        private Dictionary<string, InventoryItemStats> items;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            items = new Dictionary<string, InventoryItemStats>();
            // TODO: Read initial resources from file
        }

        public void AddResources(List<InventoryItemStats> itemStats, bool displayOnUI = true)
        {
            StringBuilder sb = new StringBuilder("Found ");

            foreach (var item in itemStats)
            {
                if (items.ContainsKey(item.inventoryItem.displayName))
                {
                    InventoryItemStats inventoryItemStats = items[item.inventoryItem.displayName];
                    inventoryItemStats.itemCount += item.itemCount;
                    items[item.inventoryItem.displayName] = inventoryItemStats;
                }
                else
                {
                    InventoryItemStats inventoryItemStats = new InventoryItemStats();
                    inventoryItemStats.itemCount = item.itemCount;
                    inventoryItemStats.inventoryItem = item.inventoryItem;
                    items[item.inventoryItem.displayName] = inventoryItemStats;
                }

                sb.Append($"{item.itemCount} {item.inventoryItem.displayName}, ");
            }

            contentDisplay.text = sb.ToString();
            contentDisplayAnimator.SetTrigger(Controls.UIDisplayTextTrigger);

            // TODO: Write this content to the disk
        }

        public GameObject SpawnResource(string itemName)
        {
            if (!items.ContainsKey(itemName))
                return null;
            else
            {
                InventoryItemStats inventoryItemStats = items[itemName];
                inventoryItemStats.itemCount -= 1;

                if (inventoryItemStats.itemCount <= 0)
                    items.Remove(itemName);

                items[itemName] = inventoryItemStats;
                return inventoryItemStats.inventoryItem.prefab;
            }
        }

        public bool ResourceAvailable(string itemName)
        {
            if (items.ContainsKey(itemName))
                return true;
            else
                return false;
        }
    }
}
