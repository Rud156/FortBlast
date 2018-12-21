using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace FortBlast.Spawner
{
    public static class FixedRandomPointsSpawner
    {
        public static Vector3[] GeneratePoints(Vector3[] worldVertices, int totalPoints)
        {
            var randomPoints = new List<Vector3>();
            var rand = new Random();

            for (var i = 0; i < worldVertices.Length; i++)
            {
                var selectionProbability = (float) totalPoints / (worldVertices.Length - i);
                var randomValue = (float) rand.NextDouble();

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