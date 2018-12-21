using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class CollectiblesSpawner : MonoBehaviour
    {
        public List<GameObject> collectiblePrefabs;

        public void SpawnCollectibles(Vector3[] points, Transform parent)
        {
            var gameObjectInstances = new GameObject[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                var randomIndex = Random.Range(0, 1000) % collectiblePrefabs.Count;
                gameObjectInstances[i] = Instantiate(
                    collectiblePrefabs[randomIndex],
                    points[i],
                    collectiblePrefabs[randomIndex].transform.rotation
                );
                gameObjectInstances[i].transform.SetParent(parent);
            }
        }

        #region Singleton

        public static CollectiblesSpawner instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}