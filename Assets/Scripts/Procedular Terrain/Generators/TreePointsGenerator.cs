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
            int treePointsCount = Mathf.FloorToInt
                (random.Next(treeSettings.minTreesInMaxChunkSize, treeSettings.maxTreesInMaxChunkSize) /
                (float)MeshSettings.supportedChunkSizes[MeshSettings.supportedChunkSizes.Length - 1] *
                chunkSize);

            Vector3[] selectedPoints = new Vector3[treePointsCount];
            int counter = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                bool isPointSelected = random.NextDouble() < treeSettings.treePointSelectionProbability;
                if (!isPointSelected)
                    continue;

                if (counter >= treePointsCount)
                    break;

                selectedPoints[counter] = vertices[i];
                counter += 1;
            }

            return selectedPoints;
        }
    }
}
