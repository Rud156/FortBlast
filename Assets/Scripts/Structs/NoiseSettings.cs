using System;
using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Structs
{
    [Serializable]
    public class NoiseSettings
    {
        public float lacunarity = 2;
        public NormalizedMode normalizedMode;

        [Header("Map Noise Data")] public int octaves = 6;

        public Vector2 offset;

        [Range(0, 1)] public float persistance = 0.6f;

        public float scale = 50;

        [Header("Randomness")] public int seed;

        public void ValidateValues()
        {
            scale = Mathf.Max(0.01f, scale);
            octaves = Mathf.Max(1, octaves);
            lacunarity = Mathf.Max(1, lacunarity);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}