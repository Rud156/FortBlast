using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;
using Random = System.Random;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class TreePointsGenerator
    {
        public static Vector3[] SelectTreePoints(Vector3[] vertices, int chunkSizeIndex,
            Vector3 meshCenter, TreeSettings treeSettings, ClearingSettings clearingSettings)
        {
            var random = new Random();
            var chunkSize = MeshSettings.supportedChunkSizes[chunkSizeIndex];
            var totalTreePoints = Mathf.FloorToInt
            (random.Next(treeSettings.minTreesInMaxChunkSize, treeSettings.maxTreesInMaxChunkSize) /
             (float) MeshSettings.supportedChunkSizes[MeshSettings.supportedChunkSizes.Length - 1] *
             chunkSize);

            var selectedPoints = new Vector3[totalTreePoints];
            var index = 0;
            var tileCenter = clearingSettings.useOnlyCenterTile ? meshCenter : Vector3.zero;

            for (var i = 0; i < vertices.Length; i++)
            {
                if (clearingSettings.createClearing)
                {
                    var modifiedVertices = vertices[i] + tileCenter;

                    if (modifiedVertices.x > clearingSettings.clearingBottomLeft.x &&
                        modifiedVertices.z < clearingSettings.clearingTopRight.x &&
                        modifiedVertices.z > clearingSettings.clearingBottomLeft.y &&
                        modifiedVertices.z < clearingSettings.clearingTopRight.y)
                        continue;
                }

                var selectionProbability = (float) totalTreePoints / (vertices.Length - i);
                var randomValue = (float) random.NextDouble();

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