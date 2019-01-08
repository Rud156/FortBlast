using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "MeshSettings", menuName = "Terrain/MeshSettings")]
    public class MeshSettings : UpdatebleData
    {
        public const int numSupportedLODs = 5;
        public const int numSupportedChunkSizes = 9;
        public const int numSupportedFlatshadedChunkSizes = 3;
        public static readonly int[] supportedChunkSizes = {48, 72, 96, 120, 144, 168, 192, 216, 240};

        [Header("Chunk Data")] [Range(0, numSupportedChunkSizes - 1)]
        public int chunkSizeIndex;

        [Range(0, numSupportedFlatshadedChunkSizes - 1)]
        public int flatshadedChunkSizeIndex;

        public float meshScale = 2;
        public bool useFlatShading;

        // Number of vertices per line for mesh rendered at LOD = 0
        // Includes to 2 extra vertices that are excluded from final mesh but used for calculating normals
        public int numVerticesPerLine =>
            supportedChunkSizes[useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;

        public float meshWorldSize => (numVerticesPerLine - 3) * meshScale;
    }
}