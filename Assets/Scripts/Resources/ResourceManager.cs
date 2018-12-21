using System.Collections.Generic;
using System.Text;
using FortBlast.Extras;
using FortBlast.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        public delegate void ResourcesChanged();

        [Header("UI Display")] public Text contentDisplay;
        public Animator contentDisplayAnimator;

        private Dictionary<string, InventoryItemStats> items;

        public ResourcesChanged resourcesChanged;

        [Header("Test Item")] public InventoryItem testItem; // TODO: Remove this later on...

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            items = new Dictionary<string, InventoryItemStats>();
            items.Add(testItem.displayName, new InventoryItemStats
            {
                inventoryItem = testItem,
                itemCount = 20
            });
            // TODO: Read initial resources from file
            resourcesChanged?.Invoke();
        }

        public void AddResource(InventoryItem item, int count = 1)
        {
            if (items.ContainsKey(item.displayName))
            {
                var inventoryItemStats = items[item.displayName];
                inventoryItemStats.itemCount += count;
                items[item.displayName] = inventoryItemStats;
            }
            else
            {
                var inventoryItemStats = new InventoryItemStats
                {
                    itemCount = count, inventoryItem = item
                };
                items.Add(item.displayName, inventoryItemStats);
            }

            resourcesChanged?.Invoke();
        }

        public void AddResources(List<InventoryItemStats> itemStats, bool displayOnUI = true)
        {
            var sb = new StringBuilder("Found ");

            foreach (var item in itemStats)
            {
                if (items.ContainsKey(item.inventoryItem.displayName))
                {
                    var inventoryItemStats = items[item.inventoryItem.displayName];
                    inventoryItemStats.itemCount += item.itemCount;
                    items[item.inventoryItem.displayName] = inventoryItemStats;
                }
                else
                {
                    var inventoryItemStats = new InventoryItemStats();
                    inventoryItemStats.itemCount = item.itemCount;
                    inventoryItemStats.inventoryItem = item.inventoryItem;
                    items.Add(item.inventoryItem.displayName, inventoryItemStats);
                }

                sb.Append($"{item.itemCount} {item.inventoryItem.displayName}, ");
            }

            sb.Length -= 2;
            contentDisplay.text = sb.ToString();
            contentDisplayAnimator.SetTrigger(Controls.UIDisplayTextTrigger);

            resourcesChanged?.Invoke();
        }

        public void UseResource(string itemName)
        {
            if (!items.ContainsKey(itemName))
                return;

            var inventoryItemStats = items[itemName];
            inventoryItemStats.itemCount -= 1;

            if (inventoryItemStats.itemCount <= 0)
                items.Remove(itemName);
            else
                items[itemName] = inventoryItemStats;

            resourcesChanged?.Invoke();
        }

        public bool HasResource(string itemName)
        {
            return items.ContainsKey(itemName);
        }

        public int CountResource(string itemName)
        {
            return items.ContainsKey(itemName) ? items[itemName].itemCount : 0;
        }

        #region  Singleton

        public static ResourceManager instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(instance);
        }

        #endregion Singleton
    }
}