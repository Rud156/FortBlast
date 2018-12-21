using FortBlast.ProceduralTerrain.DataHolders;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Generators
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, int levelOfDetail,
            MeshSettings meshSettings)
        {
            var skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var numVertsPerLine = meshSettings.numVerticesPerLine;
            var topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

            var meshData = new MeshData(numVertsPerLine, skipIncrement, meshSettings.useFlatShading);
            var vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
            var meshVertexIndex = 0;
            var outOfMeshVertexIndex = -1;

            for (var x = 0; x < numVertsPerLine; x++)
            for (var y = 0; y < numVertsPerLine; y++)
            {
                var isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 ||
                                        x == numVertsPerLine - 1;
                var skippedVertex = x > 2 && (x < numVertsPerLine - 3) & (y > 2) && y < numVertsPerLine - 3
                                    && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex -= 1;
                }
                else if (!skippedVertex)
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex += 1;
                }
            }

            for (var x = 0; x < numVertsPerLine; x++)
            for (var y = 0; y < numVertsPerLine; y++)
            {
                var skippedVertex = x > 2 && (x < numVertsPerLine - 3) & (y > 2) && y < numVertsPerLine - 3
                                    && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (skippedVertex)
                    continue;

                var isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 ||
                                        x == 0 || x == numVertsPerLine - 1;
                var isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 ||
                                        x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                var isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 &&
                                   !isOutOfMeshVertex && !isMeshEdgeVertex;
                var isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 ||
                                              x == 2 || x == numVertsPerLine - 3) &&
                                             !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                var percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                var height = heightMap[x, y];
                var vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y)
                                       * meshSettings.meshWorldSize;
                var vertexIndex = vertexIndicesMap[x, y];

                if (isEdgeConnectionVertex)
                {
                    var isVertical = x == 2 || x == numVertsPerLine - 3;
                    var distanceToMainVertexA = (isVertical ? y - 2 : x - 2) % skipIncrement;
                    var distanceToMainVertexB = skipIncrement - distanceToMainVertexA;
                    var distancePercentFromAToB = distanceToMainVertexA / (float) skipIncrement;

                    var heightMainVertexA = heightMap[isVertical ? x : x - distanceToMainVertexA,
                        isVertical ? y - distanceToMainVertexA : y];
                    var heightMainVertexB = heightMap[isVertical ? x : x + distanceToMainVertexB,
                        isVertical ? y + distanceToMainVertexB : y];

                    height = heightMainVertexA * (1 - distancePercentFromAToB) +
                             heightMainVertexB * distancePercentFromAToB;
                }

                meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y),
                    percent, vertexIndex);

                var createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 &&
                                     (!isEdgeConnectionVertex || x != 2 && y != 2);

                if (createTriangle)
                {
                    var currentIncrement = isMainVertex &&
                                           x != numVertsPerLine - 3 && y != numVertsPerLine - 3
                        ? skipIncrement
                        : 1;

                    var a = vertexIndicesMap[x, y];
                    var b = vertexIndicesMap[x + currentIncrement, y];
                    var c = vertexIndicesMap[x, y + currentIncrement];
                    var d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
            }

            meshData.FinalizeMeshData();

            return meshData;
        }
    }
}