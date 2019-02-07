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

            if (totalPoints == 1)
            {
                Vector3 randomPoint = GetSingleRandomPoint(worldVertices);
                randomPoints.Add(randomPoint);
                return randomPoints.ToArray();
            }
            else if (totalPoints < 1)
            {
                bool selectPoint = UnityEngine.Random.value <= totalPoints;
                if (selectPoint)
                    randomPoints.Add(GetSingleRandomPoint(worldVertices));

                return randomPoints.ToArray();
            }

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

        private static Vector3 GetSingleRandomPoint(Vector3[] worldVertices)
        {
            var randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * worldVertices.Length);
            return worldVertices[randomIndex];
        }
    }
}