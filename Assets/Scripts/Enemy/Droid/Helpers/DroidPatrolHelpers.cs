using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Helpers
{
    public static class DroidPatrolHelpers
    {
        public static bool CheckPlayerInRange(Transform player, Transform currentPosition,
            float minimumDetectionDistance)
        {
            if (player == null)
                return false;

            float distanceToPlayer = Vector3.Distance(player.position, currentPosition.position);
            if (distanceToPlayer <= minimumDetectionDistance)
                return true;

            return false;
        }

        public static int GetClosestPatrolPoint(Transform currentTarget, Transform player,
            Transform currentPosition, List<Transform> patrolPoints)
        {
            if (currentTarget == player)
                return -1;

            float minDistance = float.MaxValue;
            int currentPatrolPointIndex = -1;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                float currentPointDistance = Vector3.Distance(patrolPoints[i].position, currentPosition.position);
                if (currentPointDistance < minDistance)
                {
                    minDistance = currentPointDistance;
                    currentPatrolPointIndex = i;
                }
            }

            return currentPatrolPointIndex;
        }

        public static int GetNextSequentialPatrolPoint(int currentPatrolPointIndex, int patrolPointsCount)
        {
            currentPatrolPointIndex += 1;
            if (currentPatrolPointIndex >= patrolPointsCount)
                currentPatrolPointIndex = 0;

            return currentPatrolPointIndex;
        }
    }
}