using System.Collections;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class BuildingAndTowerSpawner : MonoBehaviour
    {
        private struct TerrainMeshData
        {
            public Vector3[] vertices;
            public Vector3 meshCenter;
            public Transform parent;
        }

        #region Singleton

        public static BuildingAndTowerSpawner instance;

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

        [Header("Building Prefabs")] public GameObject buildingPrefab;
        public GameObject towerPrefab;
        public int[] yRotation;

        [Header("Creation Stats")] public LevelSettings levelSettings;
        public ClearingSettings clearingSettings;
        public float heightAboveBaseGround;

        private int _terrainCount;
        private int _totalTerrainCount;
        private int _currentCount;
        private int _totalBuildingToBeCreated;

        private void Start() => _totalBuildingToBeCreated = levelSettings.maxTowers;

        public void SetTotalTerrainCount(int totalTerrainCount) => _totalTerrainCount = totalTerrainCount;

        public void AddTerrainData(Vector3[] meshVertices, Vector3 meshCenter, Transform parent)
        {
            TerrainMeshData meshData = new TerrainMeshData
            {
                vertices = meshVertices,
                meshCenter = meshCenter,
                parent = parent
            };

            CheckAndCreateTowersAndBuildings(meshData);
        }

        private void CheckAndCreateTowersAndBuildings(TerrainMeshData meshData)
        {
            float selectionProbability = (float) _totalBuildingToBeCreated / (_totalTerrainCount - _currentCount);
            float randomValue = Random.value;

            if (selectionProbability >= randomValue)
            {
                int meshVertexIndex = Mathf.FloorToInt(Random.value * meshData.vertices.Length);
                Vector3 meshVertex = meshData.vertices[meshVertexIndex] +
                                     new Vector3(meshData.meshCenter.x, 0, meshData.meshCenter.y);
                meshVertex.y = heightAboveBaseGround;

                if (clearingSettings.useOnlyCenterTile && clearingSettings.createClearing)
                    if (meshData.meshCenter == Vector3.zero)
                        return;

                int randomNumber = Random.Range(0, 1000) % yRotation.Length;
                GameObject buildingInstance = Instantiate(buildingPrefab, meshVertex,
                    buildingPrefab.transform.rotation);
                buildingInstance.transform.SetParent(meshData.parent);

                Transform towerPointsParent = buildingInstance.transform.GetChild(0);
                Vector3 position = towerPointsParent.GetChild(randomNumber).position;

                GameObject towerInstance = Instantiate(
                    towerPrefab,
                    position,
                    Quaternion.Euler(0, yRotation[randomNumber], 0)
                );

                towerInstance.transform.SetParent(buildingInstance.transform);

                _totalBuildingToBeCreated -= 1;
            }

            _currentCount += 1;
        }
    }
}