using System;
using FortBlast.Enums;
using FortBlast.Extras;
using FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Spawner;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders
{
    public class TerrainChunk
    {
        private const float ColliderGenerationDistantThreshold = 5;
        private Bounds _bounds;

        private readonly Trees _chunkTrees;
        private bool _collectiblesRequested;
        private readonly int _colliderLODIndex;

        private readonly LODInfo[] _detailLevels;
        private bool _droidsRequested;
        private bool _hasSetCollider;

        private HeightMap _heightMap;

        private bool _heightMapReceived;

        private readonly HeightMapSettings _heightMapSettings;
        private readonly LODMesh[] _lodMeshes;
        private readonly float _maxViewDistance;
        private readonly MeshCollider _meshCollider;
        private bool _meshDatSentForTower;
        private readonly MeshFilter _meshFilter;

        private readonly GameObject _meshObject;

        private readonly MeshRenderer _meshRenderer;
        private readonly MeshSettings _meshSettings;
        private int _prevLODIndex;
        private readonly Vector2 _sampleCenter;

        private readonly TerrainInteractiblesCreator _terrainInteractibles;

        private readonly Transform _viewer;
        public Vector2 coord;

        public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings,
            MeshSettings meshSettings, TreeSettings treeSettings,
            LevelSettings levelSettings, ClearingSettings clearingSettings, LODInfo[] detailLevels,
            int colliderLODIndex, Transform parent, Transform viewer, Material material,
            bool createEnemies)
        {
            this.coord = coord;

            _droidsRequested = !createEnemies;

            _detailLevels = detailLevels;
            _prevLODIndex = -1;
            _colliderLODIndex = colliderLODIndex;
            _heightMapSettings = heightMapSettings;
            _meshSettings = meshSettings;
            _viewer = viewer;

            _sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
            var position = coord * meshSettings.meshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

            _meshObject = new GameObject("Terrain Chunk");
            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.SetParent(parent);
            _meshObject.layer = 11;
            _meshObject.tag = TagManager.Terrain;

            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = material;
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();

            _chunkTrees = new Trees(position, treeSettings, clearingSettings);
            _terrainInteractibles = new TerrainInteractiblesCreator(position, _meshObject.transform,
                levelSettings, clearingSettings);

            SetVisible(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (var i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].lod);
                _lodMeshes[i].updateCallback += UpdateTerrainChunk;
                if (i == _colliderLODIndex)
                    _lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }

            _maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        }

        private Vector2 viewerPosition => new Vector2(_viewer.position.x, _viewer.position.z);

        public event Action<TerrainChunk, bool> onVisibilityChanged;

        public void Load()
        {
            ThreadedDataRequester.RequestData(
                () =>
                    HeightMapGenerator.GenerateHeightMap(
                        _meshSettings.numVerticesPerLine,
                        _meshSettings.numVerticesPerLine,
                        _heightMapSettings,
                        _sampleCenter
                    ),
                OnHeightMapReceived
            );
        }

        public void UpdateTerrainChunk()
        {
            if (!_heightMapReceived)
                return;

            var viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            var wasVisible = IsVisible();
            var visible = viewerDistanceFromNearestEdge <= _maxViewDistance;

            if (visible)
            {
                var lodIndex = 0;
                for (var i = 0; i < _detailLevels.Length - 1; i++)
                    if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceThreshold)
                        lodIndex = i + 1;
                    else
                        break;

                if (lodIndex != _prevLODIndex)
                {
                    var lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        _prevLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;

                        if (!_droidsRequested)
                            CreateInitialDroids(lodMesh.meshVertices);
                        if (!_meshDatSentForTower)
                        {
                            BuildingAndTowerSpawner.instance.AddTerrainData(lodMesh.meshVertices, _sampleCenter,
                                _meshObject.transform);
                            _meshDatSentForTower = true;
                        }
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(_heightMap, _meshSettings);
                    }

                    if (lodIndex == 0 && lodMesh.hasMesh)
                        LOD0ValidStateAvailable(lodMesh);
                    else if (lodIndex != 0)
                        _chunkTrees.ClearTrees();
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                onVisibilityChanged?.Invoke(this, visible);
            }
        }

        public void UpdateCollisionMesh()
        {
            if (_hasSetCollider)
                return;

            var sqrDistanceFromViewerToEdge = _bounds.SqrDistance(viewerPosition);

            if (sqrDistanceFromViewerToEdge < _detailLevels[_colliderLODIndex].sqrVisibleDistanceThreshold)
                if (!_lodMeshes[_colliderLODIndex].hasRequestedMesh)
                    _lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);

            if (sqrDistanceFromViewerToEdge <
                ColliderGenerationDistantThreshold * ColliderGenerationDistantThreshold)
                if (_lodMeshes[_colliderLODIndex].hasMesh)
                {
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].mesh;
                    _hasSetCollider = true;
                }
        }

        private void SetVisible(bool visible)
        {
            if (!visible)
                _chunkTrees.ClearTrees();

            _meshObject.SetActive(visible);
        }

        private bool IsVisible()
        {
            return _meshObject.activeInHierarchy;
        }

        private void LOD0ValidStateAvailable(LODMesh lodMesh)
        {
            RequestAndPlaceCollectibles(lodMesh);
            RequestAndPlaceTrees(lodMesh);
        }

        private void CreateInitialDroids(Vector3[] meshVertices)
        {
            _terrainInteractibles.RequestInteractiblesPoints(meshVertices, TerrainInteractibles.droids);
            _droidsRequested = true;
        }

        private void RequestAndPlaceTrees(LODMesh lodMesh)
        {
            if (!_chunkTrees.hasRequestedTreePoints)
                _chunkTrees.RequestTreePoints(
                    lodMesh.meshVertices,
                    _meshSettings.chunkSizeIndex
                );
            else if (!_chunkTrees.hasPlacedTrees && _chunkTrees.hasReceivedTreePoints)
                _chunkTrees.PlaceTreesOnPoints();
        }

        private void RequestAndPlaceCollectibles(LODMesh lodMesh)
        {
            if (!_collectiblesRequested)
                _terrainInteractibles.RequestInteractiblesPoints(lodMesh.meshVertices,
                    TerrainInteractibles.collectibles);
            _collectiblesRequested = true;
        }

        private void OnHeightMapReceived(object heightMapObject)
        {
            _heightMap = (HeightMap) heightMapObject;
            _heightMapReceived = true;

            UpdateTerrainChunk();
        }
    }
}