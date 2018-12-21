using FortBlast.Enums;
using FortBlast.Structs;
using UnityEngine;
using Random = System.Random;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings,
            Vector2 sampleCenter)
        {
            var noiseMap = new float[mapWidth, mapHeight];

            var rnd = new Random(settings.seed);
            var octaveOffset = new Vector2[settings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (var i = 0; i < settings.octaves; i++)
            {
                var offsetX = rnd.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
                var offsetY = rnd.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;

                octaveOffset[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;

            for (var x = 0; x < mapWidth; x++)
            for (var y = 0; y < mapHeight; y++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < settings.octaves; i++)
                {
                    var sampleX = (x - halfWidth + octaveOffset[i].x) / settings.scale * frequency;
                    var sampleY = (y - halfHeight + octaveOffset[i].y) / settings.scale * frequency;

                    // * 2 - 1 Added to make the noise values go between -1 to 1
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;

                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

                if (settings.normalizedMode == NormalizedMode.Global)
                {
                    var normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }

            // Use this in case only 1 chunk is to be generated.
            // Then the full range of the noise map is used
            if (settings.normalizedMode == NormalizedMode.Local)
                for (var x = 0; x < mapWidth; x++)
                for (var y = 0; y < mapHeight; y++)
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight,
                        maxLocalNoiseHeight, noiseMap[x, y]);

            return noiseMap;
        }
    }
}