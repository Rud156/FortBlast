using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class BuildingAndTowerSpawner : MonoBehaviour
    {
        private int _currentCount;

        private int _terrainCount;
        private int _totalBuildingToBeCreated;
        private int _totalTerrainCount;

        [Header("Building Prefabs")] public GameObject buildingPrefab;
        public ClearingSettings clearingSettings;
        public float heightAboveBaseGround;

        [Header("Creation Stats")] public LevelSettings levelSettings;
        public GameObject towerPrefab;
        public int[] yRotation;

        private void Start()
        {
            _totalBuildingToBeCreated = levelSettings.maxTowers;
        }

        public void SetTotalTerrainCount(int totalTerrainCount)
        {
            _totalTerrainCount = totalTerrainCount;
        }

        public void AddTerrainData(Vector3[] meshVertices, Vector3 meshCenter, Transform parent)
        {
            var meshData = new TerrainMeshData
            {
                vertices = meshVertices,
                meshCenter = meshCenter,
                parent = parent
            };

            CheckAndCreateTowersAndBuildings(meshData);
        }

        private void CheckAndCreateTowersAndBuildings(TerrainMeshData meshData)
        {
            var selectionProbability = (float) _totalBuildingToBeCreated / (_totalTerrainCount - _currentCount);
            var randomValue = Random.value;

            if (selectionProbability >= randomValue)
            {
                var meshVertexIndex = Mathf.FloorToInt(Random.value * meshData.vertices.Length);
                var meshVertex = meshData.vertices[meshVertexIndex] +
                                 new Vector3(meshData.meshCenter.x, 0, meshData.meshCenter.y);
                meshVertex.y = heightAboveBaseGround;

                if (clearingSettings.useOnlyCenterTile && clearingSettings.createClearing)
                    if (meshData.meshCenter == Vector3.zero)
                        return;

                var randomNumber = Random.Range(0, 1000) % yRotation.Length;
                var buildingInstance = Instantiate(buildingPrefab, meshVertex,
                    buildingPrefab.transform.rotation);
                buildingInstance.transform.SetParent(meshData.parent);

                var towerPointsParent = buildingInstance.transform.GetChild(0);
                var position = towerPointsParent.GetChild(randomNumber).position;

                var towerInstance = Instantiate(
                    towerPrefab,
                    position,
                    Quaternion.Euler(0, yRotation[randomNumber], 0)
                );

                towerInstance.transform.SetParent(buildingInstance.transform);

                _totalBuildingToBeCreated -= 1;
            }

            _currentCount += 1;
        }

        private struct TerrainMeshData
        {
            public Vector3[] vertices;
            public Vector3 meshCenter;
            public Transform parent;
        }

        #region Singleton

        public static BuildingAndTowerSpawner instance;

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