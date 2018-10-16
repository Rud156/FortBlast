using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Resources
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        public enum ItemType
        {
            Spawnable,
            UpgradeHelper,
            Consumable
        };

        [Header("Basic Data")]
        public string displayName;
        [TextArea]
        public string description;
        public Texture2D image;

        [Header("Item Details")]
        public ItemType type;
        public GameObject prefab;

        [Header("Item Specifics")]
        public float healthAmount;
        public GameObject glassBreakingAudioPrefab;
    }
}
