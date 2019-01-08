using UnityEngine;

namespace FortBlast.Common
{
    public class DestroyOnTrigger : MonoBehaviour
    {
        public bool destoryOthersOnTrigger;

        private void OnTriggerEnter(Collider other) => Destroy(destoryOthersOnTrigger ? other.gameObject : gameObject);
    }
}