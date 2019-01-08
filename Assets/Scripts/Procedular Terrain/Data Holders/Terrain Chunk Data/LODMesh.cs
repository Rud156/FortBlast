using System;
using FortBlast.Extras;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class LODMesh
    {
        public bool hasMesh;
        public bool hasRequestedMesh;
        public Mesh mesh;
        public Vector3[] meshVertices;

        private readonly int _lod;

        public LODMesh(int lod)
        {
            _lod = lod;
        }

        public event Action UpdateCallback;

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
            var meshData = (MeshData) meshDataObject;

            mesh = meshData.CreateMesh();
            hasMesh = true;
            meshVertices = meshData.GetVertices();

            UpdateCallback?.Invoke();
        }
    }
}