using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Resources
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        [TextArea] public string description;

        [Header("Basic Data")] public string displayName;

        public GameObject glassBreakingAudioPrefab;

        [Header("Item Specifics")] public float healthAmount;

        public Texture2D image;
        public GameObject prefab;

        [Header("Item Details")] public ItemType type;
    }
}