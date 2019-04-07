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
        private const string StatusDisplayCloseParam = "StatusClose";

        private static readonly int StatusDisplayOpenAnimation = Animator.StringToHash(StatusDisplayOpenParam);
        private static readonly int StatusDisplayCloseAnimation = Animator.StringToHash(StatusDisplayCloseParam);

        #endregion

        #region Delegates

        public delegate void StatusDisplayOpened();

        public StatusDisplayOpened statusDisplayOpened;

        #endregion

        [Header("Prefabs")] public GameObject listItemPrefab;

        [Header("Display")] public Text generalDisplayText;
        public RectTransform listHolder;
        public List<GameObject> listDisplayObjects;

        [Header("General")] public Animator statusDisplayAnimator;

        public void DisplayGeneralText(string textToDisplay, Color color)
        {
            float textAlpha = generalDisplayText.color.a;
            generalDisplayText.color = new Color(color.r, color.g, color.b, textAlpha);

            generalDisplayText.text = textToDisplay;
        }

        public void DisplayItemList(List<InventoryItemStats> inventoryDisplayItems)
        {
            foreach (var listDisplayObject in listDisplayObjects)
            {
                listDisplayObject.SetActive(false);
            }

            for (int i = 0; i < inventoryDisplayItems.Count; i++)
            {
                var inventoryDisplayItem = inventoryDisplayItems[i];
                GameObject listItemInstance = listDisplayObjects[i];

                Transform listItemTransform = listItemInstance.transform;
                listItemTransform.GetChild(0).GetComponent<Image>().sprite = inventoryDisplayItem.inventoryItem.image;
                listItemTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    inventoryDisplayItem.inventoryItem.displayName;
                listItemTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"x {inventoryDisplayItem.itemCount}";
            }
        }

        public void OpenStatusDisplay()
        {
            statusDisplayOpened?.Invoke();
            statusDisplayAnimator.SetBool(StatusDisplayOpenAnimation, true);
        }

        public void CloseStatusDisplay() =>
            statusDisplayAnimator.SetBool(StatusDisplayCloseAnimation, false);

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
