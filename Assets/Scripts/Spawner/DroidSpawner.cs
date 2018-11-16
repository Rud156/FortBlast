using System.Collections.Generic;
using FortBlast.Enemy.Droid.Patrol;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class DroidSpawner : MonoBehaviour
    {
        public GameObject droidPrefab;
        public int maxDroidsToSpawn;
        public Transform droidParent;

        public void StartSpawnDroids()
        {
            int totalDroidsToSpawn = maxDroidsToSpawn;

            for (int i = 0; i < maxDroidsToSpawn; i++)
            {
                float selectionProbability = (float)totalDroidsToSpawn / (totalDroidsToSpawn - i);
                float randomValue = Random.Range(0f, 1f);

                if (selectionProbability >= randomValue)
                {
                    // TODO: Implement Droid Spawning Logic Here...
                    totalDroidsToSpawn -= 1;
                }
            }
        }
    }
}
