using FortBlast.Enums;
using FortBlast.ProceduralTerrain.Generators;
using UnityEngine;

namespace FortBlast.Structs
{
    [System.Serializable]
    public class NoiseSettings
    {
        public NormalizedMode normalizedMode;
        public float scale = 50;

        [Header("Map Noise Data")]
        public int octaves = 6;
        [Range(0, 1)]
        public float persistance = 0.6f;
        public float lacunarity = 2;

        [Header("Randomness")]
        public int seed;
        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(0.01f, scale);
            octaves = Mathf.Max(1, octaves);
            lacunarity = Mathf.Max(1, lacunarity);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}