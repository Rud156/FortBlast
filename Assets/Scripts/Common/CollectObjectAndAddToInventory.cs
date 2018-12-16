using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Resources;
using UnityEngine;

namespace FortBlast.Common
{
    public class CollectObjectAndAddToInventory : MonoBehaviour
    {
        public InventoryItem inventoryItem;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TagManager.Player))
                ResourceManager.instance.AddResource(inventoryItem);
        }
    }
}