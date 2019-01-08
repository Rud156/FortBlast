using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Enums;
using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Resources
{
    public class InventoryManager : MonoBehaviour
    {
        private const float FiftyPercentAlpha = 0.196f;

        public List<InventoryItem> inventoryItems;

        [Header("UI Display Image States")] public Sprite defaultBorder;
        public Sprite selectedBorder;
        public Sprite notAvailableBorder;

        [Header("Inventory Display")] public GameObject inventory;
        public GameObject itemDisplayPrefab;
        public ScrollRect scrollRect;
        public RectTransform contentContainer;

        [Header("Item Use Interaction")] public Button itemConfirmButton;
        public Text itemConfirmButtonText;

        [Header("Item Details")] public GameObject itemDetail;
        public Text itemDetailDescription;
        public RawImage itemDetailImage;
        public Text itemDetailName;
        public Text itemDetailType;

        private List<InventoryDisplay> _itemsDisplay;
        private InventoryDisplay _itemSelected;
        private HealthSetter _playerHealth;


        private InventoryDisplay ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                _itemSelected = value;
                UpdateUIWithItemSelected();
            }
        }

        private void Start()
        {
            _itemsDisplay = new List<InventoryDisplay>();

            CreateUIDisplayItems();
            ResourceManager.instance.resourcesChanged += UpdateUIWithResources;

            ItemSelected = null;

            itemConfirmButton.onClick.AddListener(SpawnItemOnButtonPress);

            _playerHealth = GameObject.FindGameObjectWithTag(TagManager.Player)?.GetComponent<HealthSetter>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(Controls.InventoryKey))
                OpenInventory();
        }


        private void SpawnItemOnButtonPress()
        {
            if (ItemSelected == null)
                return;

            if (ResourceManager.instance.HasResource(ItemSelected.inventoryItem.displayName))
            {
                if (ItemSelected.inventoryItem.type == ItemType.Spawnable)
                    GameManager.instance.InventoryItemSelected(ItemSelected.inventoryItem);
                else if (ItemSelected.inventoryItem.type == ItemType.Consumable)
                    _playerHealth.AddHealth(ItemSelected.inventoryItem.healthAmount);

                ResourceManager.instance.UseResource(ItemSelected.inventoryItem.displayName);

                ClearItemSelection();
                CloseInventory();
            }
        }

        private void UpdateUIWithResources()
        {
            foreach (var item in _itemsDisplay)
                if (ResourceManager.instance.HasResource(item.inventoryItem.displayName))
                {
                    item.itemCountText.text =
                        $"X {ResourceManager.instance.CountResource(item.inventoryItem.displayName)}";
                    item.itemBorder.sprite = defaultBorder;
                    item.itemBorder.color = Color.white;
                }
                else
                {
                    item.itemCountText.text = "X 0";
                    item.itemBorder.sprite = notAvailableBorder;
                    item.itemBorder.color = new Color(1, 1, 1, FiftyPercentAlpha);
                }
        }

        private void UpdateUIWithItemSelected()
        {
            UpdateUIWithResources();

            if (ItemSelected == null)
                return;

            foreach (var item in _itemsDisplay)
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
                    {
                        itemConfirmButton.interactable = true;
                    }
                    else
                    {
                        itemConfirmButton.gameObject.SetActive(false);
                        itemConfirmButton.interactable = false;
                    }

                    break;
                }
        }

        private void SelectItem(InventoryDisplay item)
        {
            ItemSelected = item;
        }

        private void CreateUIDisplayItems()
        {
            foreach (var item in inventoryItems)
            {
                var itemDisplayInstance = Instantiate(itemDisplayPrefab, contentContainer.position,
                    Quaternion.identity);
                var itemDisplayTransform = itemDisplayInstance.GetComponent<RectTransform>();
                itemDisplayTransform.SetParent(contentContainer, false);

                var inventoryDisplay = new InventoryDisplay
                {
                    inventoryItem = item,
                    itemButton = itemDisplayInstance
                        .transform.GetChild(0).GetComponent<Button>(),
                    itemBorder = itemDisplayInstance
                        .transform.GetChild(0).GetComponent<Image>(),
                    itemImage = itemDisplayInstance
                        .transform.GetChild(1).GetComponent<RawImage>()
                };


                inventoryDisplay.itemImage.texture = item.image;
                inventoryDisplay.itemNameText = itemDisplayInstance.transform.GetChild(2).GetComponent<Text>();
                inventoryDisplay.itemNameText.text = item.displayName;

                inventoryDisplay.itemCountText = itemDisplayInstance
                    .transform.GetChild(3).GetComponent<Text>();


                inventoryDisplay.itemButton.onClick.AddListener(() =>
                    SelectItem(inventoryDisplay));

                _itemsDisplay.Add(inventoryDisplay);
            }
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

        public void ClearItemSelection()
        {
            ItemSelected = null;
        }

        #endregion InventoryActions

        private class InventoryDisplay
        {
            public InventoryItem inventoryItem;
            public Image itemBorder;
            public Button itemButton;
            public Text itemCountText;
            public RawImage itemImage;
            public Text itemNameText;
        }

        #region Singleton

        public static InventoryManager instance;

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