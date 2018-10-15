using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class CollectiblesSpawner : MonoBehaviour
    {
        public List<Transform> collectiblePoints;
        public List<GameObject> collectiblePrefabs;
        public Transform collectiblesParent;
        public int maxCollectiblesToSpawn;

        [Header("Debug")]
        public bool spawnOnStart;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            if (spawnOnStart)
                StartSpawnCollectibles();
        }

        public void StartSpawnCollectibles()
        {
            int totalCollectiblesToSpawn = Mathf.Min(maxCollectiblesToSpawn, collectiblePoints.Count);

            for (int i = 0; i < collectiblePoints.Count; i++)
            {
                float selectionProbability =
                    (float)totalCollectiblesToSpawn / (collectiblePoints.Count - i);
                float randomValue = Random.Range(0f, 1f);

                if (selectionProbability >= randomValue)
                {
                    int randomRangeValue = Random.Range(0, 1000);
                    int randomNumber = randomRangeValue % collectiblePrefabs.Count;

                    GameObject droidInstance = Instantiate(collectiblePrefabs[randomNumber],
                        collectiblePoints[i].position,
                        collectiblePrefabs[randomNumber].transform.rotation);
                    droidInstance.transform.SetParent(collectiblesParent);
                    totalCollectiblesToSpawn -= 1;
                }
            }
        }
    }
}
