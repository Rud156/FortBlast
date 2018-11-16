using System.Collections;
using System.Collections.Generic;
using FortBlast.Enums;
using FortBlast.Extras;
using FortBlast.Spawner;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class TerrainInteractiblesCreator
    {
        private Transform _parent;
        private Vector3 _meshCenter;

        public TerrainInteractiblesCreator(Vector2 meshCenter, Transform parent)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _parent = parent;
        }

        public void RequestInteractiblesPoints(Vector3[] meshVertices,
            TerrainInteractibles interactiblesType)
        {
            ThreadedDataRequester.RequestData(
                () =>
                {
                    switch (interactiblesType)
                    {
                        case TerrainInteractibles.collectibles:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices, 10);
                        case TerrainInteractibles.droids:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices, 3);
                        case TerrainInteractibles.towers:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices, 1);
                        default:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices, 10);
                    }
                },
                (points) =>
                    OnInteractiblesPointsReceived(points, interactiblesType)
            );
        }

        private void OnInteractiblesPointsReceived(object interactiblesPoints,
            TerrainInteractibles interactiblesType)
        {
            Vector3[] rawPoints = (Vector3[])interactiblesPoints;

            for (int i = 0; i < rawPoints.Length; i++)
                rawPoints[i] += _meshCenter;

            switch (interactiblesType)
            {
                case TerrainInteractibles.collectibles:
                    CollectiblesSpawner.instance.SpawnCollectibles(rawPoints, _parent);
                    break;
                case TerrainInteractibles.droids:
                    DroidSpawner.instance.SpawnDroids(rawPoints, _parent);
                    break;
                case TerrainInteractibles.towers:
                    BuildingAndTowerSpawner.instance.CreateTowersAndBuildings(rawPoints[0], _parent);
                    break;
            }
        }
    }
}
