using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.ProceduralTerrain.Settings;
using Unity.Jobs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders
{
    public class TerrainChunk
    {
        public event System.Action<TerrainChunk, bool> onVisibilityChanged;
        public Vector2 coord;

        private const float _colliderGenerationDistantThreshold = 5;

        private GameObject _meshObject;
        private Vector2 _sampleCenter;
        private Bounds _bounds;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;
        private int _colliderLODIndex;

        private HeightMap _heightMap;

        private bool _heightMapRequested;
        private bool _heightMapReceived;
        private int _prevLODIndex;
        private bool _hasSetCollider;
        private float _maxViewDistance;

        private Trees _chunkTrees;

        private HeightMapSettings _heightMapSettings;
        private MeshSettings _meshSettings;

        private Transform _viewer;

        private Vector2 viewerPosition
        {
            get
            {
                return new Vector2(_viewer.position.x, _viewer.position.z);
            }
        }

        public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings,
            MeshSettings meshSettings, TreeSettings treeSettings, LODInfo[] detailLevels,
            int colliderLODIndex, Transform parent, Transform viewer, Material material)
        {
            this.coord = coord;

            _detailLevels = detailLevels;
            _prevLODIndex = -1;
            _colliderLODIndex = colliderLODIndex;
            _heightMapSettings = heightMapSettings;
            _meshSettings = meshSettings;
            _viewer = viewer;

            _sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
            Vector2 position = coord * meshSettings.meshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

            _meshObject = new GameObject("Terrain Chunk");
            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.SetParent(parent);

            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = material;
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();

            _chunkTrees = new Trees(position, treeSettings);

            // Dividing by 10 as plane is 10 units by default
            // _meshObject.transform.localScale = Vector3.one * size / 10f;
            SetVisible(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].lod);
                _lodMeshes[i].updateCallback += UpdateTerrainChunk;
                if (i == _colliderLODIndex)
                    _lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }

            _maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        }

        public void Load()
        {
            _heightMapRequested = true;
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
            {
                if (!_heightMapRequested)
                    Load();
                return;
            }

            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            bool wasVisible = IsVisible();
            bool visible = viewerDistanceFromNearestEdge <= _maxViewDistance;

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < _detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }

                if (lodIndex != _prevLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        _prevLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                        lodMesh.RequestMesh(_heightMap, _meshSettings);

                    if (lodIndex == 0 && lodMesh.hasMesh)
                    {
                        if (!_chunkTrees.hasRequestedTreePoints)
                            _chunkTrees.RequestTreePoints(
                                lodMesh.meshVertices,
                                _meshSettings.chunkSizeIndex
                            );
                        else if (!_chunkTrees.hasPlacedTrees && _chunkTrees.hasReceivedTreePoints)
                            _chunkTrees.PlaceTreesOnPoints();
                    }
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

            float sqrDistanceFromViewerToEdge = _bounds.SqrDistance(viewerPosition);

            if (sqrDistanceFromViewerToEdge < _detailLevels[_colliderLODIndex].sqrVisibleDistanceThreshold)
            {
                if (!_lodMeshes[_colliderLODIndex].hasRequestedMesh)
                    _lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);
            }

            if (sqrDistanceFromViewerToEdge <
                _colliderGenerationDistantThreshold * _colliderGenerationDistantThreshold)
            {
                if (_lodMeshes[_colliderLODIndex].hasMesh)
                {
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].mesh;
                    _hasSetCollider = true;
                }
            }
        }

        public void SetVisible(bool visible)
        {
            if (!visible)
                _chunkTrees.ClearTrees();

            _meshObject.SetActive(visible);
        }

        public bool IsVisible() => _meshObject.activeInHierarchy;

        private void OnHeightMapReceived(object heightMapObject)
        {
            _heightMap = (HeightMap)heightMapObject;
            _heightMapReceived = true;

            UpdateTerrainChunk();
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;

        public Vector3[] meshVertices;

        public event System.Action updateCallback;

        private int _lod;

        public LODMesh(int lod)
        {
            _lod = lod;
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(
                () =>
                    MeshGenerator.GenerateTerrainMesh(heightMap.values, _lod, meshSettings),
                OnMeshDataReceived
            );
        }

        private void OnMeshDataReceived(object meshDataObject)
        {
            MeshData meshData = (MeshData)meshDataObject;

            mesh = meshData.CreateMesh();
            hasMesh = true;
            meshVertices = meshData.GetVertices();

            updateCallback?.Invoke();
        }
    }

    class Trees
    {
        public GameObject[] trees;
        public Vector3[] treePoints;

        public bool hasRequestedTreePoints;
        public bool hasReceivedTreePoints;
        public bool hasPlacedTrees;

        private Vector3 _meshCenter;
        private TreeSettings _treeSettings;

        public Trees(Vector2 meshCenter, TreeSettings treeSettings)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _treeSettings = treeSettings;

            trees = new GameObject[0];
            treePoints = new Vector3[0];
        }

        public void RequestTreePoints(Vector3[] meshVertices, int chunkSizeIndex)
        {
            hasRequestedTreePoints = true;
            ThreadedDataRequester.RequestData(
                () =>
                    TreePointsGenerator.SelectTreePoints(meshVertices, chunkSizeIndex, _treeSettings),
                OnTreePointsReceived
            );
        }

        public void PlaceTreesOnPoints()
        {
            hasPlacedTrees = true;
            float maxValue = float.MinValue;
            for (int i = 0; i < treePoints.Length; i++)
                if (treePoints[i].y > maxValue)
                    maxValue = treePoints[i].y;

            for (int i = 0; i < treePoints.Length; i++)
            {
                float normalizedPoint = ExtensionFunctions.Map(treePoints[i].y, 0, maxValue, 0, 1);
                trees[i] = TreesManager.instance.RequestTree(normalizedPoint);

                if (trees[i] != null)
                {
                    trees[i].transform.position = treePoints[i] + _meshCenter;
                    trees[i].SetActive(true);
                }
            }
        }

        public void ClearTrees()
        {
            for (int i = 0; i < trees.Length; i++)
                trees[i]?.SetActive(false);

            hasPlacedTrees = false;
        }

        private void OnTreePointsReceived(object treePointsObject)
        {
            hasReceivedTreePoints = true;
            treePoints = (Vector3[])treePointsObject;
            trees = new GameObject[treePoints.Length];

            PlaceTreesOnPoints();
        }
    }
}