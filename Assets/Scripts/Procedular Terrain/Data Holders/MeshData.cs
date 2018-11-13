using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders
{
    public class MeshData
    {
        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;
        private Vector3[] _bakedNormals;

        private Vector3[] _outOfMeshVertices;
        private int[] _outOfMeshTriangles;

        private int _triangleIndex;
        private int _outOfMeshTriangleIndex;

        private bool _useFlatShading;

        public MeshData(int numVertsPerLine, int skipIncrement, bool useFlatShading)
        {
            int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
            int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
            int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
            int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

            _vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
            _uvs = new Vector2[_vertices.Length];

            int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
            int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
            _triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

            _outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
            _outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];

            _useFlatShading = useFlatShading;
            _triangleIndex = 0;
            _outOfMeshTriangleIndex = 0;
        }

        public Vector3[] GetVertices() => _vertices;

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if (vertexIndex < 0)
                _outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            else
            {
                _vertices[vertexIndex] = vertexPosition;
                _uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                _outOfMeshTriangles[_outOfMeshTriangleIndex] = a;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 1] = b;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 2] = c;

                _outOfMeshTriangleIndex += 3;
            }
            else
            {
                _triangles[_triangleIndex] = a;
                _triangles[_triangleIndex + 1] = b;
                _triangles[_triangleIndex + 2] = c;

                _triangleIndex += 3;
            }
        }

        public void FinalizeMeshData()
        {
            if (_useFlatShading)
                FlatShading();
            else
                BakeNormals();
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = _vertices;
            mesh.triangles = _triangles;
            mesh.uv = _uvs;
            if (_useFlatShading)
                mesh.RecalculateNormals();
            else
                mesh.normals = _bakedNormals;
            return mesh;
        }

        private void BakeNormals() =>
            _bakedNormals = CalculateNormals();

        private Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[_vertices.Length];

            int triangleCount = _triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = _triangles[normalTriangleIndex];
                int vertexIndexB = _triangles[normalTriangleIndex + 1];
                int vertexIndexC = _triangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            int borderTriangleCount = _outOfMeshTriangles.Length / 3;
            for (int i = 0; i < borderTriangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = _outOfMeshTriangles[normalTriangleIndex];
                int vertexIndexB = _outOfMeshTriangles[normalTriangleIndex + 1];
                int vertexIndexC = _outOfMeshTriangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0)
                    vertexNormals[vertexIndexA] += triangleNormal;
                if (vertexIndexB >= 0)
                    vertexNormals[vertexIndexB] += triangleNormal;
                if (vertexIndexC >= 0)
                    vertexNormals[vertexIndexC] += triangleNormal;
            }

            for (int i = 0; i < vertexNormals.Length; i++)
                vertexNormals[i].Normalize();

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = indexA < 0 ? _outOfMeshVertices[-indexA - 1] : _vertices[indexA];
            Vector3 pointB = indexB < 0 ? _outOfMeshVertices[-indexB - 1] : _vertices[indexB];
            Vector3 pointC = indexC < 0 ? _outOfMeshVertices[-indexC - 1] : _vertices[indexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        private void FlatShading()
        {
            Vector3[] flatShadedVertices = new Vector3[_triangles.Length];
            Vector2[] flatShadedUVs = new Vector2[_triangles.Length];

            for (int i = 0; i < flatShadedVertices.Length; i++)
            {
                flatShadedVertices[i] = _vertices[_triangles[i]];
                flatShadedUVs[i] = _uvs[_triangles[i]];
                _triangles[i] = i;
            }

            _vertices = flatShadedVertices;
            _uvs = flatShadedUVs;
        }
    }
}