using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Helpers
{
    public static class DroidPatrolHelpers
    {
        public static Vector3 GetNextTarget(Vector3[] meshVertices)
        {
            return meshVertices[Random.Range(0, meshVertices.Length)];
        }

        public static bool IsAngleWithinToleranceLevel(float normalizedAngle, float angleTolerance)
        {
            if (normalizedAngle < 0)
                return false;

            if (normalizedAngle <= angleTolerance)
                return true;

            return false;
        }

        public static Transform GetClosestDistractor(Transform distactorHolder, Transform currentPosition)
        {
            var minDistance = float.MaxValue;
            Transform targetObject = null;

            for (var i = 0; i < distactorHolder.childCount; i++)
            {
                var distance = Vector3.Distance(distactorHolder.GetChild(i).position,
                    currentPosition.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetObject = distactorHolder.GetChild(i);
                }
            }

            return targetObject;
        }

        public static float CheckTargetInsideFOV(Transform target,
            float minimumDetectionDistance, float maxLookAngle,
            Transform lookingPoint, bool checkPlayerInBuildingStatus = true)
        {
            if (target == null)
                return -1;

            var distanceToPlayer = Vector3.Distance(target.position, lookingPoint.position);
            if (distanceToPlayer > minimumDetectionDistance)
                return -1;
            if (checkPlayerInBuildingStatus && GlobalData.playerInBuilding)
                return -1;

            var modifiedPlayerPosition = new Vector3(target.position.x, 0, target.position.z);
            var modifiedLookingPosition =
                new Vector3(lookingPoint.position.x, 0, lookingPoint.position.z);

            var lookDirection = modifiedPlayerPosition - modifiedLookingPosition;
            var angleToPlayer = Vector3.Angle(lookDirection, lookingPoint.forward);
            var normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            return -1;
        }
    }
}