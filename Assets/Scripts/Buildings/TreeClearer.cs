using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Buildings
{
    [RequireComponent(typeof(Rigidbody))]
    public class TreeClearer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Tree))
                other.gameObject.SetActive(false);
        }
    }
}