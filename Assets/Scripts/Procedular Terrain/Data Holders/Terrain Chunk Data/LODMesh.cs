using FortBlast.Extras;
using FortBlast.ProceduralTerrain.DataHolders;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class LODMesh
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
}