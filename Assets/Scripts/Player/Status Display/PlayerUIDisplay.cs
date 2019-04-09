using System.Collections.Generic;
using FortBlast.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.StatusDisplay
{
    public class PlayerUIDisplay : MonoBehaviour
    {
        #region Animations

        private const string StatusDisplayOpenParam = "StatusOpen";

        private static readonly int StatusDisplayOpenAnimation = Animator.StringToHash(StatusDisplayOpenParam);

        #endregion

        #region Delegates

        public delegate void StatusDisplayOpened();

        public StatusDisplayOpened statusDisplayOpened;

        #endregion

        [Header("Display")] public TextMeshProUGUI generalDisplayText;
        public Image borderImage;
        public List<GameObject> listDisplayObjects;
        public TextMeshProUGUI headerText;

        [Header("General")] public Animator statusDisplayAnimator;
        public GameObject generalDisplayTextGameObject;
        public GameObject listDisplayGameObject;

        private bool _statusDisplayOpen;

        private void Start()
        {
            listDisplayGameObject.SetActive(false);
            generalDisplayTextGameObject.SetActive(false);
        }

        public void DisplayGeneralText(string textToDisplay, Color textColor)
        {
            OpenStatusDisplay();
            generalDisplayTextGameObject.SetActive(true);

            float textAlpha = generalDisplayText.color.a;
            generalDisplayText.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);

            generalDisplayText.text = textToDisplay;
            borderImage.color = textColor;

            listDisplayGameObject.SetActive(false);
        }

        public void DisplayItemList(List<InventoryItemStats> inventoryDisplayItems, string headerTextContent,
            Color color, Color headerColor)
        {
            OpenStatusDisplay();
            listDisplayGameObject.SetActive(true);

            foreach (var listDisplayObject in listDisplayObjects)
                listDisplayObject.SetActive(false);

            borderImage.color = color;
            headerText.color = headerColor;

            headerText.text = headerTextContent;

            for (int i = 0; i < inventoryDisplayItems.Count; i++)
            {
                var inventoryDisplayItem = inventoryDisplayItems[i];
                GameObject listItemInstance = listDisplayObjects[i];
                listItemInstance.SetActive(true);

                Transform listItemTransform = listItemInstance.transform;
                listItemTransform.GetChild(0).GetComponent<Image>().sprite = inventoryDisplayItem.inventoryItem.image;
                listItemTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    inventoryDisplayItem.inventoryItem.displayName;
                listItemTransform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color;
                listItemTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"x {inventoryDisplayItem.itemCount}";
                listItemTransform.GetChild(2).GetComponent<TextMeshProUGUI>().color = color;
            }

            generalDisplayTextGameObject.SetActive(false);
        }

        private void OpenStatusDisplay()
        {
            if (_statusDisplayOpen)
                return;

            _statusDisplayOpen = true;
            statusDisplayOpened?.Invoke();
            statusDisplayAnimator.SetBool(StatusDisplayOpenAnimation, true);
        }

        public void CloseStatusDisplay()
        {
            _statusDisplayOpen = false;
            statusDisplayAnimator.SetBool(StatusDisplayOpenAnimation, false);

            listDisplayGameObject.SetActive(false);
            generalDisplayTextGameObject.SetActive(false);
        }

        #region Singleton

        public static PlayerUIDisplay instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion
    }
}
