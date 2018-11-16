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

        public static Vector3 GetNextTarget(Vector3[] meshVertices) =>
            meshVertices[Random.Range(0, meshVertices.Length)];
    }
}