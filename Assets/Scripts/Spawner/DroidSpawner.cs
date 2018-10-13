using System.Collections.Generic;
using FortBlast.Enemy.Droid.Patrol;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class DroidSpawner : MonoBehaviour
    {
        public List<Transform> patrolRoutes;
        public GameObject droidPrefab;
        public int maxDroidsToSpawn;
        public Transform droidParent;

        [Header("Debug")]
        public bool spawnOnStart;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            if (spawnOnStart)
                StartSpawnDroids();
        }

        public void StartSpawnDroids()
        {
            int totalDroidsToSpawn = Mathf.Min(maxDroidsToSpawn, patrolRoutes.Count);

            for (int i = 0; i < patrolRoutes.Count; i++)
            {
                float selectionProbability = (float)totalDroidsToSpawn / (patrolRoutes.Count - i);
                float randomValue = Random.Range(0f, 1f);

                if (selectionProbability >= randomValue)
                {
                    GameObject droidInstance = Instantiate(droidPrefab, patrolRoutes[i].position,
                        droidPrefab.transform.rotation);
                    droidInstance.transform.SetParent(droidParent);
                    droidInstance.GetComponent<DroidPatrol>().patrolPointGroup = patrolRoutes[i];
                    totalDroidsToSpawn -= 1;
                }
            }
        }
    }
}
