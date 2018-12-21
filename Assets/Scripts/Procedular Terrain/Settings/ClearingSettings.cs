using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "ClearingSettings", menuName = "Terrain/ClearingSettings")]
    public class ClearingSettings : ScriptableObject
    {
        public Vector2 clearingBottomLeft;
        public Vector2 clearingTopRight;
        [Header("Create Clearing")] public bool createClearing = true;
        public bool useOnlyCenterTile = true;
    }
}