using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Common
{
    public class DestroyOnTrigger : MonoBehaviour
    {
        public bool destoryOthersOnTrigger;

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (destoryOthersOnTrigger)
                Destroy(other.gameObject);
            else
                Destroy(gameObject);
        }
    }
}