using System;
using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Structs
{
    [Serializable]
    public class NoiseSettings
    {
        [Header("Map Noise Data")] public NormalizedMode normalizedMode;
        public Vector2 offset;
        public float scale = 50;


        [Header("Randomness")] public int seed;
        public float lacunarity = 2;
        [Range(0, 1)] public float persistance = 0.6f;
        public int octaves = 6;

        public void ValidateValues()
        {
            scale = Mathf.Max(0.01f, scale);
            octaves = Mathf.Max(1, octaves);
            lacunarity = Mathf.Max(1, lacunarity);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}