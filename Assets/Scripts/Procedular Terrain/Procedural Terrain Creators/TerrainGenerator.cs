using System.Collections;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.DataHolders;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    public class TerrainGenerator : MonoBehaviour
    {
        #region Singleton

        private static TerrainGenerator _instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton


        [Header("Settings")]
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;

        [Header("Map Values")]
        public Transform viewer;
        public Material mapMaterial;
        public LODInfo[] detailLevels;
        public int colliderLODIndex;

        [Header("Terrain Size")]
        public bool fixedTerrainSize;

        private const float _viewerMoveThresholdForChunkUpdate = 25f;
        private const float _sqrViewerMoveThresholdForChunkUpdate =
            _viewerMoveThresholdForChunkUpdate * _viewerMoveThresholdForChunkUpdate;

        private Vector2 _viewerPosition;
        private List<TerrainChunk> _visibleTerrainChunks;

        private float _meshWorldSize;
        private int _chunksVisibleInViewDistance;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDict;

        private Vector2 _prevViewerPosition;
        private bool _updatingChunks;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _prevViewerPosition = new Vector2(int.MaxValue, int.MaxValue);

            textureData.ApplyToMaterial(mapMaterial);
            textureData.UpdateMeshHeights(mapMaterial,
                heightMapSettings.minHeight, heightMapSettings.maxHeight);

            _terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
            _visibleTerrainChunks = new List<TerrainChunk>();

            float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;

            _meshWorldSize = meshSettings.meshWorldSize;
            _chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / _meshWorldSize);
            _updatingChunks = false;

            StartCoroutine(UpdateVisibleChunks(true));
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            _viewerPosition =
                new Vector2(viewer.position.x, viewer.position.z);
            if (_viewerPosition != _prevViewerPosition)
                foreach (TerrainChunk chunk in _visibleTerrainChunks)
                    chunk.UpdateCollisionMesh();


            if ((_prevViewerPosition - _viewerPosition).sqrMagnitude > _sqrViewerMoveThresholdForChunkUpdate
                && !_updatingChunks)
            {
                StartCoroutine(UpdateVisibleChunks(!fixedTerrainSize));
                _prevViewerPosition = _viewerPosition;
            }
        }

        private IEnumerator UpdateVisibleChunks(bool createNewChunks)
        {
            _updatingChunks = true;
            HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

            for (int i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
                yield return null;
            }

            int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance;
                    xOffset++)
            {
                for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance;
                    yOffset++)
                {
                    Vector2 viewChunkCoord =
                        new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (!alreadyUpdatedChunkCoords.Contains(viewChunkCoord))
                    {
                        if (_terrainChunkDict.ContainsKey(viewChunkCoord))
                        {
                            _terrainChunkDict[viewChunkCoord].UpdateTerrainChunk();
                        }
                        else if (createNewChunks)
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewChunkCoord,
                                heightMapSettings, meshSettings,
                                detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                            _terrainChunkDict.Add(viewChunkCoord, newChunk);
                            newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }
                    yield return null;
                }
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
    }
}