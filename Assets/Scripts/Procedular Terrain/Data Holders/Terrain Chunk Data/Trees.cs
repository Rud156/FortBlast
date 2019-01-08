using FortBlast.Extras;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.ProceduralTerrain.Settings;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class Trees
    {
        private readonly ClearingSettings _clearingSettings;

        private readonly Vector3 _meshCenter;
        private readonly TreeSettings _treeSettings;
        public bool hasPlacedTrees;
        public bool hasReceivedTreePoints;

        public bool hasRequestedTreePoints;
        private Vector3[] treePoints;
        private GameObject[] trees;

        public Trees(Vector2 meshCenter, TreeSettings treeSettings, ClearingSettings clearingSettings)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _treeSettings = treeSettings;
            _clearingSettings = clearingSettings;

            trees = new GameObject[0];
            treePoints = new Vector3[0];
        }

        public void RequestTreePoints(Vector3[] meshVertices, int chunkSizeIndex)
        {
            hasRequestedTreePoints = true;
            ThreadedDataRequester.RequestData(
                () =>
                    TreePointsGenerator.SelectTreePoints(meshVertices, chunkSizeIndex,
                        _meshCenter, _treeSettings, _clearingSettings),
                OnTreePointsReceived
            );
        }

        public void PlaceTreesOnPoints()
        {
            hasPlacedTrees = true;
            var maxValue = float.MinValue;
            for (var i = 0; i < treePoints.Length; i++)
                if (treePoints[i].y > maxValue)
                    maxValue = treePoints[i].y;

            for (var i = 0; i < treePoints.Length; i++)
            {
                if (treePoints[i] == Vector3.zero)
                    Debug.Log("Tree At Zero");

                var normalizedPoint = ExtensionFunctions.Map(treePoints[i].y, 0, maxValue,
                    0, 1);
                trees[i] = TreesManager.instance.RequestTree(normalizedPoint);

                if (trees[i] != null)
                {
                    trees[i].transform.position = treePoints[i] + _meshCenter;
                    if (_treeSettings.useRandomTreeRotation)
                        trees[i].transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    trees[i].SetActive(true);
                }
            }
        }

        public void ClearTrees()
        {
            for (var i = 0; i < trees.Length; i++)
            {
                trees[i]?.SetActive(false);
                trees[i] = null;
            }

            hasPlacedTrees = false;
        }

        private void OnTreePointsReceived(object treePointsObject)
        {
            hasReceivedTreePoints = true;
            treePoints = (Vector3[]) treePointsObject;
            trees = new GameObject[treePoints.Length];

            PlaceTreesOnPoints();
        }
    }
}