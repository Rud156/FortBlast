using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Enums;
using FortBlast.Player.Affecter_Actions;
using FortBlast.Player.Movement;
using FortBlast.Scenes.Main_Scene;
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
        public GameObject inventory;
        public RectTransform contentContainer;
        public GameObject itemDisplayPrefab;
        public ScrollRect scrollRect;

        [Header("UI Display Image States")]
        public Sprite defaultBorder;
        public Sprite selectedBorder;
        public Sprite notAvailableBorder;

        [Header("Item Details")]
        public GameObject itemDetail;
        public Text itemDetailName;
        public RawImage itemDetailImage;
        public Text itemDetailDescription;
        public Text itemDetailType;

        [Header("Item Use Interaction")]
        public Button itemConfirmButton;
        public Text itemConfirmButtonText;

        private class InventoryDisplay
        {
            public InventoryItem inventoryItem;
            public Button itemButton;
            public Image itemBorder;
            public Text itemNameText;
            public RawImage itemImage;
            public Text itemCountText;
        }

        private List<InventoryDisplay> _itemsDisplay;

        private InventoryDisplay _itemSelected;
        private InventoryDisplay ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                _itemSelected = value;
                UpdateUIWithItemSelected();
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

            itemConfirmButton.onClick.AddListener(SpawnItemOnButtonPress);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(Controls.InventoryKey))
                OpenInventory();
        }

        #region InventoryActions

        public void OpenInventory()
        {
            inventory.SetActive(true);
            scrollRect.verticalNormalizedPosition = 1;

            GameManager.instance.InventoryOpened();
        }

        public void CloseInventory()
        {
            inventory.SetActive(false);
            itemDetail.SetActive(false);

            ClearItemSelection();
        }

        public void ClearItemSelection() => ItemSelected = null;

        #endregion InventoryActions

        private void SpawnItemOnButtonPress()
        {
            if (ItemSelected == null)
                return;

            if (ResourceManager.instance.HasResource(ItemSelected.inventoryItem.displayName))
            {
                GameManager.instance.InventoryItemSelected(ItemSelected.inventoryItem);
                ResourceManager.instance.UseResource(ItemSelected.inventoryItem.displayName);
                ClearItemSelection();
                CloseInventory();
            }
        }

        private void UpdateUIWithResources()
        {
            foreach (var item in _itemsDisplay)
            {
                if (ResourceManager.instance.HasResource(item.inventoryItem.displayName))
                {
                    item.itemCountText.text =
                        $"X {ResourceManager.instance.CountResource(item.inventoryItem.displayName)}";
                    item.itemBorder.sprite = defaultBorder;
                }
                else
                {
                    item.itemCountText.text = "X 0";
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

                    itemDetailName.text = ItemSelected.inventoryItem.displayName;
                    itemDetailDescription.text = ItemSelected.inventoryItem.description;
                    itemDetailImage.texture = ItemSelected.inventoryItem.image;

                    switch (ItemSelected.inventoryItem.type)
                    {
                        case ItemType.Consumable:
                            itemDetailType.text = "Can be <b>consumed</b>";
                            itemConfirmButton.gameObject.SetActive(true);
                            itemConfirmButtonText.text = "Consume";
                            break;

                        case ItemType.Spawnable:
                            itemDetailType.text = "Can be used for <b>distraction</b>";
                            itemConfirmButton.gameObject.SetActive(true);
                            itemConfirmButtonText.text = "Use";
                            break;

                        case ItemType.UpgradeHelper:
                            itemDetailType.text = "Can be used for <b>upgrades</b>";
                            itemConfirmButton.gameObject.SetActive(false);
                            break;
                    }

                    itemDetail.SetActive(true);

                    if (ResourceManager.instance.HasResource(ItemSelected.inventoryItem.displayName))
                        itemConfirmButton.interactable = true;
                    else
                    {
                        itemConfirmButton.gameObject.SetActive(false);
                        itemConfirmButton.interactable = false;
                    }

                    break;
                }
            }
        }

        private void SelectItem(InventoryDisplay item) => ItemSelected = item;

        private void CreateUIDisplayItems()
        {
            foreach (var item in inventoryItems)
            {
                GameObject itemDisplayInstance = Instantiate(itemDisplayPrefab, contentContainer.position,
                    Quaternion.identity);
                RectTransform itemDisplayTransform = itemDisplayInstance.GetComponent<RectTransform>();
                itemDisplayTransform.SetParent(contentContainer, false);

                InventoryDisplay inventoryDisplay = new InventoryDisplay();
                inventoryDisplay.inventoryItem = item;

                inventoryDisplay.itemButton = itemDisplayInstance
                    .transform.GetChild(0).GetComponent<Button>();
                inventoryDisplay.itemBorder = itemDisplayInstance
                    .transform.GetChild(0).GetComponent<Image>();

                inventoryDisplay.itemImage = itemDisplayInstance
                    .transform.GetChild(1).GetComponent<RawImage>();
                inventoryDisplay.itemImage.texture = item.image;
                inventoryDisplay.itemNameText = itemDisplayInstance.
                    transform.GetChild(2).GetComponent<Text>();
                inventoryDisplay.itemNameText.text = item.displayName;

                inventoryDisplay.itemCountText = itemDisplayInstance
                    .transform.GetChild(3).GetComponent<Text>();


                inventoryDisplay.itemButton.onClick.AddListener(() =>
                    SelectItem(inventoryDisplay));

                _itemsDisplay.Add(inventoryDisplay);
            }
        }
    }
}
