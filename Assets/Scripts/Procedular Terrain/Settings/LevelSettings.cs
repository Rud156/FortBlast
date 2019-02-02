using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(menuName = "Terrain/LevelSettings", fileName = "LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        [Header("Level Collectibles Info")] public int totalMachineParts;

        [Header("Terrain Size Based on LOD")] public LODInfo[] detailLevels;

        [Header("Items Per Terrain Chunk")] public int maxDroids;
        public int maxTowers;
        public int maxCollectibles;
    }
}