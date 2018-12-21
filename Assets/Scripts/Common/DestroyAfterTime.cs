using UnityEngine;

namespace FortBlast.Common
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destoryAfterTime = 5;

        // Use this for initialization
        private void Start()
        {
            Destroy(gameObject, destoryAfterTime);
        }
    }
}