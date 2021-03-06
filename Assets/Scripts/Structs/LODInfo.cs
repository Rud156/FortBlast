using System;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

[Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;

    public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold => visibleDistanceThreshold * visibleDistanceThreshold;
}