using UnityEngine;

namespace FortBlast.Common
{
    public class ObjectFollowPlayer : MonoBehaviour
    {
        [Header("Base Stats")] public float followSpeed = 10f;
        public Vector3 objectOffset;

        [Header(("Rotation"))] public bool useRotation;
        public float lookSpeed = 10f;

        [Header("Player")] public Transform player;

        private Vector3 _lastPlayerPosition;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _lastPlayerPosition = player.position;

        void LateUpdate()
        {
            if (useRotation)
                LookAtTarget();

            MoveToTarget();
            UpdateLastPlayerPosition();
        }

        private void LookAtTarget()
        {
            Vector3 targetPosition = player ? player.position : _lastPlayerPosition;

            Vector3 lookDirection = targetPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);
        }

        private void MoveToTarget()
        {
            // Player is NULL only when destroyed
            Vector3 targetVector = _lastPlayerPosition;
            Vector3 forwardVector = Vector3.forward;
            Vector3 rightVector = Vector3.right;
            Vector3 upVector = Vector3.up;

            if (player)
            {
                targetVector = player.position;
                forwardVector = player.forward;
                rightVector = player.right;
                upVector = player.up;
            }

            Vector3 targetPosition = targetVector +
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