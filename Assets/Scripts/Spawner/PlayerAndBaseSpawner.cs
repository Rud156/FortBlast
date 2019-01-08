using FortBlast.UI;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class PlayerAndBaseSpawner : MonoBehaviour
    {
        public Transform player;
        public GameObject playerBase;

        private void Start()
        {
            player.GetComponent<Rigidbody>().isKinematic = true;
            Fader.instance.fadeStart += FadeStart;
        }

        private void FadeStart()
        {
            var playerBaseInstance = Instantiate(playerBase, Vector3.zero, Quaternion.identity);
            var playerSpawnPoint = playerBaseInstance.transform.GetChild(0).position;

            player.transform.position = playerSpawnPoint;
            player.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}