using System.Collections;
using System.Collections.Generic;
using FortBlast.Enums;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings,
            Vector2 sampleCenter)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random rnd = new System.Random(settings.seed);
            Vector2[] octaveOffset = new Vector2[settings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (int i = 0; i < settings.octaves; i++)
            {
                float offsetX = rnd.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
                float offsetY = rnd.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;

                octaveOffset[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < settings.octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffset[i].x) / settings.scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffset[i].y) / settings.scale * frequency;

                        // * 2 - 1 Added to make the noise values go between -1 to 1
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
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
                        float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            // Use this in case only 1 chunk is to be generated.
            // Then the full range of the noise map is used
            if (settings.normalizedMode == NormalizedMode.Local)
            {
                for (int x = 0; x < mapWidth; x++)
                    for (int y = 0; y < mapHeight; y++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight,
                            maxLocalNoiseHeight, noiseMap[x, y]);
                    }
            }

            return noiseMap;
        }
    }
}