using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    [System.Serializable]
    public struct UIItem
    {
        public InventoryItem item;
        public Text displayText;
    }

    public class AdditionalItemsUIDisplay : MonoBehaviour
    {
        #region Singleton

        private static AdditionalItemsUIDisplay _instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public UIItem[] items;

        private void Start() => ResourceManager.instance.resourcesChanged += UpdateUIWithResources;

        private void UpdateUIWithResources()
        {
            foreach (var inventoryItem in items)
            {
                inventoryItem.displayText.text =
                    ResourceManager.instance.CountResource(inventoryItem.item.displayName).ToString();
            }
        }
    }
}