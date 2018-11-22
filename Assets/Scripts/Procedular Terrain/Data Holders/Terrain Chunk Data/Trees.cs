using FortBlast.Extras;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class Trees
    {
        public GameObject[] trees;
        public Vector3[] treePoints;

        public bool hasRequestedTreePoints;
        public bool hasReceivedTreePoints;
        public bool hasPlacedTrees;

        private Vector3 _meshCenter;
        private TreeSettings _treeSettings;

        public Trees(Vector2 meshCenter, TreeSettings treeSettings)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _treeSettings = treeSettings;

            trees = new GameObject[0];
            treePoints = new Vector3[0];
        }

        public void RequestTreePoints(Vector3[] meshVertices, int chunkSizeIndex)
        {
            hasRequestedTreePoints = true;
            ThreadedDataRequester.RequestData(
                () =>
                    TreePointsGenerator.SelectTreePoints(meshVertices, chunkSizeIndex, _treeSettings),
                OnTreePointsReceived
            );
        }

        public void PlaceTreesOnPoints()
        {
            hasPlacedTrees = true;
            float maxValue = float.MinValue;
            for (int i = 0; i < treePoints.Length; i++)
                if (treePoints[i].y > maxValue)
                    maxValue = treePoints[i].y;

            for (int i = 0; i < treePoints.Length; i++)
            {
                float normalizedPoint = ExtensionFunctions.Map(treePoints[i].y, 0, maxValue, 0, 1);
                trees[i] = TreesManager.instance.RequestTree(normalizedPoint);

                if (trees[i] != null)
                {
                    trees[i].transform.position = treePoints[i] + _meshCenter;
                    trees[i].SetActive(true);
                }
            }
        }

        public void ClearTrees()
        {
            for (int i = 0; i < trees.Length; i++)
            {
                trees[i]?.SetActive(false);
                trees[i] = null;
            }

            hasPlacedTrees = false;
        }

        private void OnTreePointsReceived(object treePointsObject)
        {
            hasReceivedTreePoints = true;
            treePoints = (Vector3[])treePointsObject;
            trees = new GameObject[treePoints.Length];

            PlaceTreesOnPoints();
        }
    }
}