using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Enemy.Tower.Helpers
{
    public static class TowerControllerHelpers
    {
        public static Transform GetClosestDistractor(Transform distactorHolder, Transform towerTop)
        {
            var minDistance = float.MaxValue;
            Transform targetObject = null;

            for (var i = 0; i < distactorHolder.childCount; i++)
            {
                var distance = Vector3.Distance(distactorHolder.GetChild(i).position,
                    towerTop.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetObject = distactorHolder.GetChild(i);
                }
            }

            return targetObject;
        }

        public static float CheckTargetInsideFOV(Transform target, Transform towerTop,
            float maxPlayerTargetRange, float maxLookAngle, bool checkPlayerInBuildingStatus = true)
        {
            if (target == null)
                return -1;

            var distanceToPlayer = Vector3.Distance(target.position, towerTop.position);
            if (distanceToPlayer > maxPlayerTargetRange)
                return -1;
            if (checkPlayerInBuildingStatus && GlobalData.playerInBuilding)
                return -1;

            var modifiedPlayerPosition = new Vector3(target.position.x, 0, target.position.z);
            var modifiedTowerTopPosition =
                new Vector3(towerTop.position.x, 0, towerTop.position.z);

            var lookDirection = modifiedPlayerPosition - modifiedTowerTopPosition;
            var angleToPlayer = Vector3.Angle(lookDirection, towerTop.forward);
            var normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            return -1;
        }

        public static bool IsAngleWithinToleranceLevel(float normalizedAngle, float attackAngleTolerance)
        {
            if (normalizedAngle < 0)
                return false;

            if (normalizedAngle <= attackAngleTolerance)
                return true;

            return false;
        }
    }
}