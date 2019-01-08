using UnityEngine;

namespace FortBlast.Common
{
    public class HideOnPlay : MonoBehaviour
    {
        private void Start() => gameObject.SetActive(false);
    }
}