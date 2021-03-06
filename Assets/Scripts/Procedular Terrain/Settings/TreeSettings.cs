﻿using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TreeSettings", menuName = "Terrain/TreeSettings")]
    public class TreeSettings : ScriptableObject
    {
        [Header("Min Max Spawn Max LOD")] public int minTreesInMaxChunkSize;
        public int maxTreesInMaxChunkSize;

        [Header("General")] public bool useRandomTreeRotation;
    }
}