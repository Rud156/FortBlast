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
        private readonly ClearingSettings _clearingSettings;
        private readonly LevelSettings _levelSettings;
        private readonly Vector3 _meshCenter;
        private readonly Transform _parent;

        public TerrainInteractiblesCreator(Vector2 meshCenter, Transform parent,
            LevelSettings levelSettings, ClearingSettings clearingSettings)
        {
            _meshCenter = new Vector3(meshCenter.x, 0, meshCenter.y);
            _parent = parent;
            _levelSettings = levelSettings;
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
                        case TerrainInteractibles.Collectibles:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _levelSettings.maxCollectibles);
                        case TerrainInteractibles.Droids:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _levelSettings.maxDroids);
                        default:
                            return FixedRandomPointsSpawner.GeneratePoints(meshVertices,
                                _levelSettings.maxCollectibles);
                    }
                },
                points =>
                    OnInteractiblesPointsReceived(points, interactiblesType)
            );
        }

        private void OnInteractiblesPointsReceived(object interactiblesPoints,
            TerrainInteractibles interactiblesType)
        {
            var rawPoints = (Vector3[]) interactiblesPoints;

            for (var i = 0; i < rawPoints.Length; i++)
                rawPoints[i] += _meshCenter;

            switch (interactiblesType)
            {
                case TerrainInteractibles.Collectibles:
                    if (_clearingSettings.createClearing)
                        ClearCollectiblePointsAndSpawn(rawPoints);
                    else
                        CollectiblesSpawner.instance.SpawnCollectibles(rawPoints, _parent);
                    break;
                case TerrainInteractibles.Droids:
                    DroidSpawner.instance.SpawnDroids(rawPoints, _parent);
                    break;
            }
        }

        private void ClearCollectiblePointsAndSpawn(Vector3[] rawPoints)
        {
            var points = new List<Vector3>();
            for (var i = 0; i < rawPoints.Length; i++)
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