using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Resources
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        [Header("Basic Data")] public ItemID itemId;
        public string displayName;
        [TextArea] public string description;
        public ItemType type;

        [Header("Item Specifics")] public float healthAmount;
        public GameObject glassBreakingAudioPrefab;

        [Header("Item Details")]
        public Texture2D image;
        public GameObject prefab;
    }
}