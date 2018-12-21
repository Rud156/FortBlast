using System;

namespace FortBlast.Structs
{
    [Serializable]
    public struct CameraShakerStats
    {
        public float magnitude;
        public float roughness;
        public float fadeOutTime;
        public float fadeInTime;
    }
}