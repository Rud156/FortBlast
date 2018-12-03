using System;
using System.Collections.Generic;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class TreePointsGenerator
    {
        public static Vector3[] SelectTreePoints(Vector3[] vertices, int chunkSizeIndex,
            TreeSettings treeSettings)
        {
            System.Random random = new System.Random();
            int chunkSize = MeshSettings.supportedChunkSizes[chunkSizeIndex];
            int totalTreePoints = Mathf.FloorToInt
                (random.Next(treeSettings.minTreesInMaxChunkSize, treeSettings.maxTreesInMaxChunkSize) /
                (float)MeshSettings.supportedChunkSizes[MeshSettings.supportedChunkSizes.Length - 1] *
                chunkSize);

            Vector3[] selectedPoints = new Vector3[totalTreePoints];
            int index = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                if (treeSettings.createClearing &&
                    vertices[i].x > treeSettings.clearingBottomLeft.x &&
                    vertices[i].z < treeSettings.clearingTopRight.x &&
                    vertices[i].z > treeSettings.clearingBottomLeft.y &&
                    vertices[i].z < treeSettings.clearingTopRight.y)
                    continue;

                float selectionProbability = (float)totalTreePoints / (vertices.Length - i);
                float randomValue = (float)random.NextDouble();

                if (selectionProbability >= randomValue)
                {
                    selectedPoints[index] = vertices[i];
                    index += 1;
                    totalTreePoints -= 1;
                }
            }

            return selectedPoints;
        }
    }
}
