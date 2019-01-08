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

        public ResourcesChanged resourcesChanged;

        [Header("UI Display")] public Text contentDisplay;
        public Animator contentDisplayAnimator;

        [Header("Test Item")] public InventoryItem testItem; // TODO: Remove this later on...

        private Dictionary<string, InventoryItemStats> _items;

        private void Start()
        {
            _items = new Dictionary<string, InventoryItemStats>();
            _items.Add(testItem.displayName, new InventoryItemStats
            {
                inventoryItem = testItem,
                itemCount = 20
            });
            // TODO: Read initial resources from file
            resourcesChanged?.Invoke();
        }

        public void AddResource(InventoryItem item, int count = 1)
        {
            if (_items.ContainsKey(item.displayName))
            {
                var inventoryItemStats = _items[item.displayName];
                inventoryItemStats.itemCount += count;
                _items[item.displayName] = inventoryItemStats;
            }
            else
            {
                var inventoryItemStats = new InventoryItemStats
                {
                    itemCount = count, inventoryItem = item
                };
                _items.Add(item.displayName, inventoryItemStats);
            }

            resourcesChanged?.Invoke();
        }

        public void AddResources(List<InventoryItemStats> itemStats, bool displayOnUI = true)
        {
            var sb = new StringBuilder("Found ");

            foreach (var item in itemStats)
            {
                if (_items.ContainsKey(item.inventoryItem.displayName))
                {
                    var inventoryItemStats = _items[item.inventoryItem.displayName];
                    inventoryItemStats.itemCount += item.itemCount;
                    _items[item.inventoryItem.displayName] = inventoryItemStats;
                }
                else
                {
                    var inventoryItemStats = new InventoryItemStats();
                    inventoryItemStats.itemCount = item.itemCount;
                    inventoryItemStats.inventoryItem = item.inventoryItem;
                    _items.Add(item.inventoryItem.displayName, inventoryItemStats);
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
            if (!_items.ContainsKey(itemName))
                return;

            var inventoryItemStats = _items[itemName];
            inventoryItemStats.itemCount -= 1;

            if (inventoryItemStats.itemCount <= 0)
                _items.Remove(itemName);
            else
                _items[itemName] = inventoryItemStats;

            resourcesChanged?.Invoke();
        }

        public bool HasResource(string itemName)
        {
            return _items.ContainsKey(itemName);
        }

        public int CountResource(string itemName)
        {
            return _items.ContainsKey(itemName) ? _items[itemName].itemCount : 0;
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