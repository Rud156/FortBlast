﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class InventoryManager : MonoBehaviour
    {
        #region Singleton
        public static InventoryManager instance;

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


        public List<InventoryItem> inventoryItems;

        [Header("Inventory Display")]
        public RectTransform contentContainer;
        public GameObject itemDisplayPrefab;

        [Header("UI Display Image States")]
        public Sprite defaultBorder;
        public Sprite selectedBorder;
        public Sprite notAvailableBorder;

        private class InventoryDisplay
        {
            public InventoryItem inventoryItem;
            public Button itemButton;
            public Image itemBorder;
            public Text itemNameText;
            public RawImage itemImage;
        }

        private List<InventoryDisplay> _itemsDisplay;

        private InventoryDisplay _itemSelected;
        private InventoryDisplay ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                UpdateUIWithItemSelected();
                _itemSelected = value;
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _itemsDisplay = new List<InventoryDisplay>();

            CreateUIDisplayItems();
            ResourceManager.instance.resourcesChanged += UpdateUIWithResources;

            ItemSelected = null;
        }

        private void UpdateUIWithResources()
        {
            foreach (var item in _itemsDisplay)
            {
                if (ResourceManager.instance.HasResource(item.inventoryItem.displayName))
                {
                    item.itemButton.interactable = true;
                    item.itemBorder.sprite = defaultBorder;
                }
                else
                {
                    item.itemButton.interactable = false;
                    item.itemBorder.sprite = notAvailableBorder;
                }
            }
        }

        private void UpdateUIWithItemSelected()
        {
            UpdateUIWithResources();
            if (ItemSelected == null)
                return;

            foreach (var item in _itemsDisplay)
            {
                if (item == ItemSelected)
                {
                    item.itemBorder.sprite = selectedBorder;
                    // TODO: Display and update the right part of the UI with data
                    break;
                }
            }
        }

        private void SelectItem(InventoryDisplay item) => ItemSelected = item;

        private void CreateUIDisplayItems()
        {
            foreach (var item in inventoryItems)
            {
                GameObject itemDisplayInstance = Instantiate(itemDisplayPrefab, transform.position,
                    Quaternion.identity);
                itemDisplayInstance.transform.SetParent(contentContainer);

                InventoryDisplay inventoryDisplay = new InventoryDisplay();
                inventoryDisplay.inventoryItem = item;

                inventoryDisplay.itemButton = itemDisplayInstance
                    .transform.GetChild(0).GetComponent<Button>();
                inventoryDisplay.itemBorder = itemDisplayInstance
                    .transform.GetChild(0).GetComponent<Image>();

                inventoryDisplay.itemImage = itemDisplayInstance
                    .transform.GetChild(1).GetComponent<RawImage>();
                inventoryDisplay.itemNameText = itemDisplayInstance.
                    transform.GetChild(2).GetComponent<Text>();


                inventoryDisplay.itemButton.onClick.AddListener(() =>
                    SelectItem(inventoryDisplay));

                _itemsDisplay.Add(inventoryDisplay);
            }
        }
    }
}