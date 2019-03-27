using FortBlast.Resources;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace FortBlast.Scenes.BaseScene
{
    public class InventoryBulletinDisplay : MonoBehaviour
    {
        [Header("Display")]
        public TextMeshProUGUI bulletinText;

        private void Start() => ResourceManager.instance.resourcesChanged += UpdateBulletinDisplay;

        private void OnDestroy() => ResourceManager.instance.resourcesChanged -= UpdateBulletinDisplay;

        private void UpdateBulletinDisplay()
        {
            List<InventoryItem> inventoryItems = InventoryManager.instance.GetInventoryItems();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<align=center>Upcoming Requirements</align>\n\n");

            for (var i = 0; i < inventoryItems.Count; i++)
            {
                var inventoryItem = inventoryItems[i];
                stringBuilder.Append($"{i + 1}. {inventoryItem.name} - X {ResourceManager.instance.CountResource(inventoryItem.itemId)}\n");
            }

            bulletinText.text = stringBuilder.ToString();
        }
    }
}
