using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(Collider))]
    public class ChangePlayerStatusOnEnterBuilding : MonoBehaviour
    {
        /// <summary>
        ///     OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Player))
                return;

            GlobalData.playerInBuilding = true;
        }

        /// <summary>
        ///     OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(TagManager.Player))
                return;

            GlobalData.playerInBuilding = false;
        }
    }
}