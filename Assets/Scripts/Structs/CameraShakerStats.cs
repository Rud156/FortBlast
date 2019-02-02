using UnityEngine;

namespace FortBlast.Structs
{
    [CreateAssetMenu(fileName = "CameraShakerStats", menuName = "Camera/Stats/Shaker")]
    public class CameraShakerStats : ScriptableObject
    {
        public float magnitude;
        public float roughness;
        public float fadeOutTime;
        public float fadeInTime;
    }
}