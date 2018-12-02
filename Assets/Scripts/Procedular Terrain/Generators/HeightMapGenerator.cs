using System.Collections;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class HeightMapGenerator
    {
        private static float[,] _falloffMap;

        public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings,
            Vector2 sampleCenter)
        {
            float[,] noiseMap = NoiseGenerator
                .GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);
            float[,] values = noiseMap;
            AnimationCurve heightCurveThreadSafe = new AnimationCurve(settings.heightCurve.keys);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            if (settings.useFalloff && _falloffMap == null)
                _falloffMap = FalloffGenerator.GenerateFalloffMap(width);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    values[i, j] *= heightCurveThreadSafe
                        .Evaluate(values[i, j] - (settings.useFalloff ? _falloffMap[i, j] : 0))
                        * settings.heightMultiplier;

                    if (values[i, j] > maxValue)
                        maxValue = values[i, j];

                    if (values[i, j] < minValue)
                        minValue = values[i, j];
                }
            }

            return new HeightMap(values, minValue, maxValue);
        }
    }
}
