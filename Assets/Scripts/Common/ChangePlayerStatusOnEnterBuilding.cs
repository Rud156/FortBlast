using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(Collider))]
    public class ChangePlayerStatusOnEnterBuilding : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Player))
                return;

            GlobalData.playerInBuilding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(TagManager.Player))
                return;

            GlobalData.playerInBuilding = false;
        }
    }
}