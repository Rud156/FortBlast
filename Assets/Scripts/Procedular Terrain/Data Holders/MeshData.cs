using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders
{
    public class MeshData
    {
        private Vector3[] _bakedNormals;
        private int _outOfMeshTriangleIndex;
        private readonly int[] _outOfMeshTriangles;

        private readonly Vector3[] _outOfMeshVertices;

        private int _triangleIndex;
        private readonly int[] _triangles;

        private readonly bool _useFlatShading;
        private Vector2[] _uvs;
        private Vector3[] _vertices;

        public MeshData(int numVertsPerLine, int skipIncrement, bool useFlatShading)
        {
            var numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
            var numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
            var numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
            var numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

            _vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
            _uvs = new Vector2[_vertices.Length];

            var numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
            var numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
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
            {
                _outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            }
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
            var mesh = new Mesh {vertices = _vertices, triangles = _triangles, uv = _uvs};

            if (_useFlatShading)
                mesh.RecalculateNormals();
            else
                mesh.normals = _bakedNormals;
            return mesh;
        }

        private void BakeNormals()
        {
            _bakedNormals = CalculateNormals();
        }

        private Vector3[] CalculateNormals()
        {
            var vertexNormals = new Vector3[_vertices.Length];

            var triangleCount = _triangles.Length / 3;
            for (var i = 0; i < triangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = _triangles[normalTriangleIndex];
                var vertexIndexB = _triangles[normalTriangleIndex + 1];
                var vertexIndexC = _triangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            var borderTriangleCount = _outOfMeshTriangles.Length / 3;
            for (var i = 0; i < borderTriangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = _outOfMeshTriangles[normalTriangleIndex];
                var vertexIndexB = _outOfMeshTriangles[normalTriangleIndex + 1];
                var vertexIndexC = _outOfMeshTriangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0)
                    vertexNormals[vertexIndexA] += triangleNormal;
                if (vertexIndexB >= 0)
                    vertexNormals[vertexIndexB] += triangleNormal;
                if (vertexIndexC >= 0)
                    vertexNormals[vertexIndexC] += triangleNormal;
            }

            for (var i = 0; i < vertexNormals.Length; i++)
                vertexNormals[i].Normalize();

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            var pointA = indexA < 0 ? _outOfMeshVertices[-indexA - 1] : _vertices[indexA];
            var pointB = indexB < 0 ? _outOfMeshVertices[-indexB - 1] : _vertices[indexB];
            var pointC = indexC < 0 ? _outOfMeshVertices[-indexC - 1] : _vertices[indexC];

            var sideAB = pointB - pointA;
            var sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        private void FlatShading()
        {
            var flatShadedVertices = new Vector3[_triangles.Length];
            var flatShadedUVs = new Vector2[_triangles.Length];

            for (var i = 0; i < flatShadedVertices.Length; i++)
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