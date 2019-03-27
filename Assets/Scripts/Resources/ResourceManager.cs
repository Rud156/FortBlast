using System.Collections.Generic;
using System.Text;
using FortBlast.Enums;
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

        private Dictionary<ItemID, InventoryItemStats> _items;

        private void Start()
        {
            _items = new Dictionary<ItemID, InventoryItemStats>();

            // TODO: Remove After Testing
            _items.Add(testItem.itemId, new InventoryItemStats
            {
                inventoryItem = testItem,
                itemCount = 20
            });
            // TODO: Read initial resources from file
            resourcesChanged?.Invoke();
        }

        public void AddResource(InventoryItem item, int count = 1)
        {
            if (_items.ContainsKey(item.itemId))
            {
                var inventoryItemStats = _items[item.itemId];
                inventoryItemStats.itemCount += count;
                _items[item.itemId] = inventoryItemStats;
            }
            else
            {
                var inventoryItemStats = new InventoryItemStats
                {
                    itemCount = count, inventoryItem = item
                };
                _items.Add(item.itemId, inventoryItemStats);
            }

            resourcesChanged?.Invoke();
        }

        public void AddResources(List<InventoryItemStats> itemStats, bool displayOnUi = true)
        {
            var sb = new StringBuilder("Found ");

            foreach (var item in itemStats)
            {
                if (_items.ContainsKey(item.inventoryItem.itemId))
                {
                    var inventoryItemStats = _items[item.inventoryItem.itemId];
                    inventoryItemStats.itemCount += item.itemCount;
                    _items[item.inventoryItem.itemId] = inventoryItemStats;
                }
                else
                {
                    var inventoryItemStats = new InventoryItemStats
                    {
                        itemCount = item.itemCount,
                        inventoryItem = item.inventoryItem
                    };
                    _items.Add(item.inventoryItem.itemId, inventoryItemStats);
                }

                sb.Append($"{item.itemCount} {item.inventoryItem.displayName}, ");
            }

            sb.Length -= 2;

            if (displayOnUi)
            {
                contentDisplay.text = sb.ToString();
                contentDisplayAnimator.SetTrigger(Controls.UIDisplayTextTrigger);
            }

            resourcesChanged?.Invoke();
        }

        public void UseResource(ItemID itemId)
        {
            if (!_items.ContainsKey(itemId))
                return;

            var inventoryItemStats = _items[itemId];
            inventoryItemStats.itemCount -= 1;

            if (inventoryItemStats.itemCount <= 0)
                _items.Remove(itemId);
            else
                _items[itemId] = inventoryItemStats;

            resourcesChanged?.Invoke();
        }

        public bool HasResource(ItemID itemId)
        {
            return _items.ContainsKey(itemId);
        }

        public int CountResource(ItemID itemId)
        {
            return _items.ContainsKey(itemId) ? _items[itemId].itemCount : 0;
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