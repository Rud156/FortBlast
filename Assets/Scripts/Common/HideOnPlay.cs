using UnityEngine;

namespace FortBlast.Common
{
    public class HideOnPlay : MonoBehaviour
    {
        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}