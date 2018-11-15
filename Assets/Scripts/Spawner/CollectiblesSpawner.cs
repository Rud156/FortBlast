using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class CollectiblesSpawner : MonoBehaviour
    {
        #region Singleton

        public static CollectiblesSpawner instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public List<GameObject> collectiblePrefabs;

        public GameObject[] SpawnCollectibles(Vector3[] points, Transform parent)
        {
            GameObject[] gameObjectInstances = new GameObject[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                int randomIndex = Random.Range(0, 1000) % collectiblePrefabs.Count;
                gameObjectInstances[i] = Instantiate(
                    collectiblePrefabs[randomIndex],
                    points[i],
                    collectiblePrefabs[randomIndex].transform.rotation
                );
                gameObjectInstances[i].transform.SetParent(parent);
            }

            return gameObjectInstances;
        }
    }
}
