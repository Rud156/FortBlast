using UnityEngine;

namespace FortBlast.Common
{
    public class ObjectFollowPlayer : MonoBehaviour
    {
        private Vector3 _lastPlayerPosition;
        [Header("Base Stats")] public float followSpeed = 10f;
        public float lookSpeed = 10f;
        public Vector3 objectOffset;

        [Header("Player")] public Transform player;

        [Header("Rotation")] public bool useRotation;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _lastPlayerPosition = player.position;
        }

        private void LateUpdate()
        {
            if (useRotation)
                LookAtTarget();

            MoveToTarget();
            UpdateLastPlayerPosition();
        }

        private void LookAtTarget()
        {
            var targetPosition = player ? player.position : _lastPlayerPosition;

            var lookDirection = targetPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);
        }

        private void MoveToTarget()
        {
            // Player is NULL only when destroyed
            var targetVector = _lastPlayerPosition;
            var forwardVector = Vector3.forward;
            var rightVector = Vector3.right;
            var upVector = Vector3.up;

            if (player)
            {
                targetVector = player.position;
                forwardVector = player.forward;
                rightVector = player.right;
                upVector = player.up;
            }

            var targetPosition = targetVector +
                                 forwardVector * objectOffset.z +
                                 rightVector * objectOffset.x +
                                 upVector * objectOffset.y;

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        private void UpdateLastPlayerPosition()
        {
            if (player)
                _lastPlayerPosition = player.position;
        }
    }
}