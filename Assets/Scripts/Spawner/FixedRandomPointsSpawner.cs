using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace FortBlast.Spawner
{
    public static class FixedRandomPointsSpawner
    {
        public static Vector3[] GeneratePoints(Vector3[] worldVertices, float totalPoints)
        {
            var randomPoints = new List<Vector3>();
            var random = new Random();

            if (totalPoints == 1)
            {
                Vector3 randomPoint = GetSingleRandomPoint(worldVertices);
                randomPoints.Add(randomPoint);
                return randomPoints.ToArray();
            }

            if (totalPoints < 1)
            {
                var selectPoint = random.NextDouble() <= totalPoints;
                if (selectPoint)
                    randomPoints.Add(GetSingleRandomPoint(worldVertices));

                return randomPoints.ToArray();
            }

            for (var i = 0; i < worldVertices.Length; i++)
            {
                var selectionProbability = (float) totalPoints / (worldVertices.Length - i);
                var randomValue = (float) random.NextDouble();

                if (selectionProbability >= randomValue)
                {
                    randomPoints.Add(worldVertices[i]);
                    totalPoints -= 1;
                }
            }

            return randomPoints.ToArray();
        }

        private static Vector3 GetSingleRandomPoint(Vector3[] worldVertices)
        {
            var random = new Random();

            var randomIndex = Mathf.FloorToInt((float)random.NextDouble() * worldVertices.Length);
            return worldVertices[randomIndex];
        }
    }
}