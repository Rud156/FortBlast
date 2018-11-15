using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TreeSettings", menuName = "Terrain/TreeSettings")]
    public class TreeSettings : ScriptableObject
    {
        [Range(0, 0.01f)]
        public float treePointSelectionProbability;

        [Header("Min Max Spawn Max LOD")]
        public int minTreesInMaxChunkSize;
        public int maxTreesInMaxChunkSize;
    }
}
