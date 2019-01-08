using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class BuildingAndTowerSpawner : MonoBehaviour
    {
        [Header("Building Prefabs")] public GameObject buildingPrefab;
        public ClearingSettings clearingSettings;
        public float heightAboveBaseGround;
        public float safeBufferDistance;

        [Header("Creation Stats")] public LevelSettings levelSettings;
        public GameObject towerPrefab;
        public int[] yRotation;

        private int _currentCount;
        private int _terrainCount;
        private int _totalBuildingToBeCreated;
        private int _totalTerrainCount;

        private void Start() => _totalBuildingToBeCreated = levelSettings.maxTowers;

        public void SetTotalTerrainCount(int totalTerrainCount) => _totalTerrainCount = totalTerrainCount;

        public void AddTerrainData(Vector3[] meshVertices, Vector3 meshCenter, Transform parent, Bounds meshBounds)
        {
            var meshData = new TerrainMeshData
            {
                vertices = meshVertices,
                meshCenter = meshCenter,
                parent = parent,
                meshBounds = meshBounds
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
                CheckAndPlaceTower(randomNumber, meshVertex, meshData);

                _totalBuildingToBeCreated -= 1;
            }

            _currentCount += 1;
        }

        private void CheckAndPlaceTower(int randomCannonPoint, Vector3 meshVertex, TerrainMeshData meshData)
        {
            var buildingInstance = Instantiate(buildingPrefab, meshVertex,
                buildingPrefab.transform.rotation);
            buildingInstance.transform.SetParent(meshData.parent);

            Vector3 meshMinBounds = meshData.meshBounds.min;
            Vector3 meshMaxBounds = meshData.meshBounds.max;

            float terrainLeftX = meshMinBounds.x;
            float terrainRightX = meshMaxBounds.x;
            float terrainBottomZ = meshMinBounds.y;
            float terrainTopZ = meshMaxBounds.y;

            Collider buildingCollider = buildingInstance.GetComponent<Collider>();
            Bounds buildingBounds = buildingCollider.bounds;
            Vector3 minBounds = buildingBounds.min;
            Vector3 maxBounds = buildingBounds.max;

            float buildingLeftX = minBounds.x;
            float buildingRightX = maxBounds.x;
            float buildingBottomZ = minBounds.z;
            float buildingTopZ = maxBounds.z;

            Vector3 currentBuildingPosition = meshVertex;

            if (buildingLeftX < terrainLeftX)
                currentBuildingPosition.x += ((terrainLeftX - buildingLeftX) + safeBufferDistance);
            else if (buildingRightX > terrainRightX)
                currentBuildingPosition.x -= ((terrainRightX - buildingRightX) - safeBufferDistance);

            if (buildingBottomZ < terrainBottomZ)
                currentBuildingPosition.z += ((terrainBottomZ - buildingBottomZ) + safeBufferDistance);
            else if (buildingTopZ > terrainTopZ)
                currentBuildingPosition.z -= ((terrainTopZ - buildingTopZ) - safeBufferDistance);

            buildingInstance.transform.position = currentBuildingPosition;

            var towerPointsParent = buildingInstance.transform.GetChild(0);
            var position = towerPointsParent.GetChild(randomCannonPoint).position;

            var towerInstance = Instantiate(
                towerPrefab,
                position,
                Quaternion.Euler(0, yRotation[randomCannonPoint], 0)
            );

            towerInstance.transform.SetParent(buildingInstance.transform);
        }

        private struct TerrainMeshData
        {
            public Vector3[] vertices;
            public Vector3 meshCenter;
            public Transform parent;
            public Bounds meshBounds;
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