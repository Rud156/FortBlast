using System.Collections.Generic;
using FortBlast.Enums;
using FortBlast.Extras;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Spawner;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.DataHolders.TerrainChunkData
{
    public class TerrainInteractiblesCreator
    {
        private Transform _parent;
        private Vector3 _meshCenter;
        private TerrainObjectSettings _terrainObjectSettings;
        private ClearingSettings _clearingSettings;

        public TerrainInteractiblesCreator(Vector2 meshCenter, Transform parent,
            TerrainObjectSettings terrainObjectSettings, ClearingSettings clearingSettings)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _parent = parent;
            _terrainObjectSettings = terrainObjectSettings;
            _clearingSettings = clearingSettings;
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
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _terrainObjectSettings.maxCollectibles);
                        case TerrainInteractibles.droids:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _terrainObjectSettings.maxDroids);
                        case TerrainInteractibles.towers:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _terrainObjectSettings.maxTowers);
                        default:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _terrainObjectSettings.maxCollectibles);
                    }
                },
                (points) =>
                    OnInteractiblesPointsReceived(points, interactiblesType)
            );
        }

        private void OnInteractiblesPointsReceived(object interactiblesPoints,
            TerrainInteractibles interactiblesType)
        {
            Vector3[] rawPoints = (Vector3[]) interactiblesPoints;

            for (int i = 0; i < rawPoints.Length; i++)
                rawPoints[i] += _meshCenter;

            switch (interactiblesType)
            {
                case TerrainInteractibles.collectibles:
                    if (_clearingSettings.createClearing)
                        ClearCollectiblePointsAndSpawn(rawPoints);
                    else
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

        private void ClearCollectiblePointsAndSpawn(Vector3[] rawPoints)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < rawPoints.Length; i++)
            {
                if (rawPoints[i].x > _clearingSettings.clearingBottomLeft.x &&
                    rawPoints[i].z < _clearingSettings.clearingTopRight.x &&
                    rawPoints[i].z > _clearingSettings.clearingBottomLeft.y &&
                    rawPoints[i].z < _clearingSettings.clearingTopRight.y)
                    continue;

                points.Add(rawPoints[i]);
            }

            CollectiblesSpawner.instance.SpawnCollectibles(points.ToArray(), _parent);
        }
    }
}