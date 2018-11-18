using System.Collections.Generic;
using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Helpers
{
    public static class DroidPatrolHelpers
    {
        public static Vector3 GetNextTarget(Vector3[] meshVertices) =>
            meshVertices[Random.Range(0, meshVertices.Length)];

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
            float minDistance = float.MaxValue;
            Transform targetObject = null;

            for (int i = 0; i < distactorHolder.childCount; i++)
            {
                float distance = Vector3.Distance(distactorHolder.GetChild(i).position,
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

            float distanceToPlayer = Vector3.Distance(target.position, lookingPoint.position);
            if (distanceToPlayer > minimumDetectionDistance)
                return -1;
            if (checkPlayerInBuildingStatus && GlobalData.playerInBuilding)
                return -1;

            Vector3 modifiedPlayerPosition = new Vector3(target.position.x, 0, target.position.z);
            Vector3 modifiedLookingPosition =
                new Vector3(lookingPoint.position.x, 0, lookingPoint.position.z);

            Vector3 lookDirection = modifiedPlayerPosition - modifiedLookingPosition;
            float angleToPlayer = Vector3.Angle(lookDirection, lookingPoint.forward);
            float normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            else
                return -1;
        }
    }
}