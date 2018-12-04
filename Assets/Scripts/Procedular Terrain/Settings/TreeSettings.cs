using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TreeSettings", menuName = "Terrain/TreeSettings")]
    public class TreeSettings : ScriptableObject
    {
        [Header("Min Max Spawn Max LOD")]
        public int minTreesInMaxChunkSize;
        public int maxTreesInMaxChunkSize;

        [Header("Create Clearing")]
        public bool createClearing = true;
        public bool useOnlyCenterTile = true;
        public Vector2 clearingBottomLeft;
        public Vector2 clearingTopRight;
    }
}
