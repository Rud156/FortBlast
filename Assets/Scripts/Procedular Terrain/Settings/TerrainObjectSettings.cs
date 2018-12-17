using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(menuName = "Terrain/TerrainObjectSettings", fileName = "TerrainObjectSettings")]
    public class TerrainObjectSettings : ScriptableObject
    {
        [Header("Items Per Terrain Chunk")] public int maxDroids;
        public int maxTowers;
        public int maxCollectibles;

        [Header("Terrain Size Based on LOD")] public LODInfo[] detailLevels;
    }
}