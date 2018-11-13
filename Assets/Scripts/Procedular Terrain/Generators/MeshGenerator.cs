using System.Collections;
using System.Collections.Generic;
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

            int skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            int numVertsPerLine = meshSettings.numVerticesPerLine;
            Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

            MeshData meshData = new MeshData(numVertsPerLine, skipIncrement, meshSettings.useFlatShading);
            int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
            int meshVertexIndex = 0;
            int outOfMeshVertexIndex = -1;

            for (int x = 0; x < numVertsPerLine; x++)
            {
                for (int y = 0; y < numVertsPerLine; y++)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 ||
                        x == numVertsPerLine - 1;
                    bool skippedVertex = x > 2 && x < numVertsPerLine - 3 & y > 2 && y < numVertsPerLine - 3
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
            }

            for (int x = 0; x < numVertsPerLine; x++)
            {
                for (int y = 0; y < numVertsPerLine; y++)
                {
                    bool skippedVertex = x > 2 && x < numVertsPerLine - 3 & y > 2 && y < numVertsPerLine - 3
                       && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                    if (skippedVertex)
                        continue;

                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 ||
                        x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 ||
                         x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 &&
                        !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 ||
                        x == 2 || x == numVertsPerLine - 3) &&
                        !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    float height = heightMap[x, y];
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y)
                        * meshSettings.meshWorldSize;
                    int vertexIndex = vertexIndicesMap[x, y];

                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int distanceToMainVertexA = (isVertical ? y - 2 : x - 2) % skipIncrement;
                        int distanceToMainVertexB = skipIncrement - distanceToMainVertexA;
                        float distancePercentFromAToB = distanceToMainVertexA / (float)skipIncrement;

                        float heightMainVertexA = heightMap[isVertical ? x : x - distanceToMainVertexA,
                            isVertical ? y - distanceToMainVertexA : y];
                        float heightMainVertexB = heightMap[isVertical ? x : x + distanceToMainVertexB,
                            isVertical ? y + distanceToMainVertexB : y];

                        height = heightMainVertexA * (1 - distancePercentFromAToB) +
                            heightMainVertexB * (distancePercentFromAToB);
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y),
                        percent, vertexIndex);

                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 &&
                        (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex &&
                            x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }

            meshData.FinalizeMeshData();

            return meshData;
        }
    }
}