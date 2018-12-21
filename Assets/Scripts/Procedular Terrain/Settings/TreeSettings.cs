using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TreeSettings", menuName = "Terrain/TreeSettings")]
    public class TreeSettings : ScriptableObject
    {
        public int maxTreesInMaxChunkSize;

        [Header("Min Max Spawn Max LOD")] public int minTreesInMaxChunkSize;
    }
}