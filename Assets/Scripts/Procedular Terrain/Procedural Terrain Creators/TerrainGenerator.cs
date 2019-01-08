using System.Collections;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.DataHolders;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Spawner;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    public class TerrainGenerator : MonoBehaviour
    {
        public delegate void TerrainGenerationInitialComplete();

        public TerrainGenerationInitialComplete terrainGenerationComplete;

        private const float ViewerMoveThresholdForChunkUpdate = 25f;

        private const float SqrViewerMoveThresholdForChunkUpdate =
            ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        [Header("Map Values")] public int colliderLODIndex;
        public bool fixedTerrainSize;
        public Material mapMaterial;

        [Header("Settings")] public MeshSettings meshSettings;
        public TextureData textureData;
        public TreeSettings treeSettings;
        public HeightMapSettings heightMapSettings;
        public LevelSettings levelSettings;
        public ClearingSettings clearingSettings;

        [Header("Extra Terrain Params")] public Transform viewer;
        public bool enemiesOnCenterTile;

        private int _chunksVisibleInViewDistance;
        private float _meshWorldSize;

        private Vector2 _prevViewerPosition;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDict;
        private bool _updatingChunks;

        private Vector2 _viewerPosition;
        private List<TerrainChunk> _visibleTerrainChunks;

        private void Start()
        {
            _prevViewerPosition = new Vector2(int.MaxValue, int.MaxValue);

            textureData.ApplyToMaterial(mapMaterial);
            textureData.UpdateMeshHeights(mapMaterial,
                heightMapSettings.minHeight, heightMapSettings.maxHeight);

            _terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
            _visibleTerrainChunks = new List<TerrainChunk>();

            var maxViewDistance = levelSettings.detailLevels[levelSettings.detailLevels.Length - 1]
                .visibleDistanceThreshold;

            _meshWorldSize = meshSettings.meshWorldSize;
            _chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / _meshWorldSize);
            _updatingChunks = false;

            BuildingAndTowerSpawner.instance.SetTotalTerrainCount(
                (_chunksVisibleInViewDistance * 2 + 1) * (_chunksVisibleInViewDistance * 2 + 1)
            );

            StartCoroutine(UpdateVisibleChunks(true, true));
        }

        private void Update()
        {
            if (!viewer)
                return;

            _viewerPosition =
                new Vector2(viewer.position.x, viewer.position.z);
            if (_viewerPosition != _prevViewerPosition)
                foreach (var chunk in _visibleTerrainChunks)
                    chunk.UpdateCollisionMesh();


            if ((_prevViewerPosition - _viewerPosition).sqrMagnitude >
                SqrViewerMoveThresholdForChunkUpdate && !_updatingChunks)
            {
                StartCoroutine(UpdateVisibleChunks(!fixedTerrainSize, false));
                _prevViewerPosition = _viewerPosition;
            }
        }

        private IEnumerator UpdateVisibleChunks(bool createNewChunks, bool cleanBuildNavMesh)
        {
            _updatingChunks = true;
            var alreadyUpdatedChunkCoords = new HashSet<Vector2>();

            for (var i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
                yield return null;
            }

            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            for (var xOffset = -_chunksVisibleInViewDistance;
                xOffset <= _chunksVisibleInViewDistance;
                xOffset++)
            for (var yOffset = -_chunksVisibleInViewDistance;
                yOffset <= _chunksVisibleInViewDistance;
                yOffset++)
            {
                var viewChunkCoord =
                    new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewChunkCoord))
                {
                    if (_terrainChunkDict.ContainsKey(viewChunkCoord))
                    {
                        _terrainChunkDict[viewChunkCoord].UpdateTerrainChunk();
                    }
                    else if (createNewChunks)
                    {
                        var createEnemies = !(!enemiesOnCenterTile && viewChunkCoord == Vector2.zero);

                        var newChunk = new TerrainChunk(viewChunkCoord,
                            heightMapSettings, meshSettings, treeSettings, levelSettings, clearingSettings,
                            levelSettings.detailLevels, colliderLODIndex, transform, viewer, mapMaterial,
                            createEnemies);
                        _terrainChunkDict.Add(viewChunkCoord, newChunk);
                        newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }

                yield return null;
            }

            if (cleanBuildNavMesh)
            {
                terrainGenerationComplete?.Invoke();
                NavMeshBaker.instance.BuildInitialNavMesh();
            }
            else
            {
                NavMeshBaker.instance.ReBuildNavMesh();
            }

            _updatingChunks = false;
        }

        private void OnTerrainChunkVisibilityChanged(TerrainChunk terrainChunk, bool isVisible)
        {
            if (isVisible)
                _visibleTerrainChunks.Add(terrainChunk);
            else
                _visibleTerrainChunks.Remove(terrainChunk);
        }

        #region Singleton

        public static TerrainGenerator instance;

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