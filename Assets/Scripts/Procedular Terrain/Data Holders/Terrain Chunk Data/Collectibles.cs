using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Spawner;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class Collectibles
    {
        public bool hasRequestedCollectiblePoints;

        private Transform _parent;
        private Vector3 _meshCenter;

        public Collectibles(Vector2 meshCenter, Transform parent)
        {
            _meshCenter = meshCenter;
            _parent = parent;
        }

        public void RequestCollectiblePoints(Vector3[] meshVertices)
        {
            hasRequestedCollectiblePoints = true;

            ThreadedDataRequester.RequestData(
                () =>
                    FixedRandomPointsSpawner.GeneratePoints(meshVertices, 10),
                OnCollectiblePointsReceived
            );
        }

        private void OnCollectiblePointsReceived(object collectiblePoints)
        {
            Vector3[] rawPoints = (Vector3[])collectiblePoints;

            for (int i = 0; i < rawPoints.Length; i++)
                rawPoints[i] += _meshCenter;

            CollectiblesSpawner.instance.SpawnCollectibles(rawPoints, _parent);
        }
    }
}
