using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(menuName = "Terrain/LevelSettings", fileName = "LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        [Header("Terrain Size Based on LOD")] public LODInfo[] detailLevels;
        public int maxCollectibles;
        [Header("Items Per Terrain Chunk")] public int maxDroids;
        public int maxTowers;
    }
}