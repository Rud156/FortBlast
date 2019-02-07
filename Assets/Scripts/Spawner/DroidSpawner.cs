using UnityEngine;

namespace FortBlast.Spawner
{
    public class DroidSpawner : MonoBehaviour
    {
        public GameObject[] droidPrefabs = new GameObject[2];
        [Range(0.1f, 0.9f)] public float secondDroidSpawnProbability;

        public void SpawnDroids(Vector3[] meshVertices, Transform parent)
        {
            var droids = new GameObject[meshVertices.Length];

            for (var i = 0; i < meshVertices.Length; i++)
            {
                var randomValue = Random.value;

                var droidPrefab = randomValue <= secondDroidSpawnProbability ?
                    droidPrefabs[1] : droidPrefabs[0];

                droids[i] = Instantiate(droidPrefab, meshVertices[i],
                    droidPrefab.transform.rotation);
                droids[i].transform.SetParent(parent);
            }
        }

        #region Singleton

        public static DroidSpawner instance;

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