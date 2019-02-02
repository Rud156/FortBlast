using System;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    [Serializable]
    public struct UIItem
    {
        public InventoryItem item;
        public Text displayText;
    }

    public class AdditionalItemsUIDisplay : MonoBehaviour
    {
        public UIItem[] items;

        private void Start()
        {
            ResourceManager.instance.resourcesChanged += UpdateUIWithResources;
        }

        private void UpdateUIWithResources()
        {
            foreach (var inventoryItem in items)
                inventoryItem.displayText.text =
                    ResourceManager.instance.CountResource(inventoryItem.item.itemId).ToString();
        }

        #region Singleton

        private static AdditionalItemsUIDisplay _instance;

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
    }
}