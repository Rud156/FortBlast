using System.Collections;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.Spawner
{
    public static class FixedRandomPointsSpawner
    {
        public static Vector3[] GeneratePoints(Vector3[] worldVertices, int totalPoints)
        {
            List<Vector3> randomPoints = new List<Vector3>();
            System.Random rand = new System.Random();

            for (int i = 0; i < worldVertices.Length; i++)
            {
                float selectionProbability = (float) totalPoints / (worldVertices.Length - i);
                float randomValue = (float) rand.NextDouble();

                if (selectionProbability >= randomValue)
                {
                    randomPoints.Add(worldVertices[i]);
                    totalPoints -= 1;
                }
            }

            return randomPoints.ToArray();
        }
    }
}