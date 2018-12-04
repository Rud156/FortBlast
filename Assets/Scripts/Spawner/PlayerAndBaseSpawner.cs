using System.Collections;
using System.Collections.Generic;
using FortBlast.UI;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class PlayerAndBaseSpawner : MonoBehaviour
    {
        public GameObject playerBase;
        public Transform player;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            player.GetComponent<Rigidbody>().isKinematic = true;
            Fader.instance.fadeStart += FadeStart;
        }

        private void FadeStart()
        {
            GameObject playerBaseInstance = Instantiate(playerBase, Vector3.zero, Quaternion.identity);
            Vector3 playerSpawnPoint = playerBaseInstance.transform.GetChild(0).position;

            player.transform.position = playerSpawnPoint;
            player.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}