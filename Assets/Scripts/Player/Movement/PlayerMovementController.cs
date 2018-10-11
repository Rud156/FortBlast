using FortBlast.Player.Data;
using UnityEngine;

namespace FortBlast.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement")]
        public float movementSpeed;

        private Rigidbody _playerRB;
        private Animator _playerAnimator;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerRB = GetComponent<Rigidbody>();
            _playerAnimator = GetComponent<Animator>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float moveX = Input.GetAxis(PlayerData.HorizontalAxis);
            float moveZ = Input.GetAxis(PlayerData.VerticalAxis);


            SetAnimator(moveX, moveZ);
            MovePlayer(moveX, moveZ);
        }

        private void SetAnimator(float moveX, float moveZ)
        {
            _playerAnimator.SetFloat(PlayerData.PlayerHorizontal, moveX);
            _playerAnimator.SetFloat(PlayerData.PlayerVertical, moveZ);
        }

        private void MovePlayer(float moveX, float moveZ)
        {
            Vector3 xVelocity = Vector3.zero;
            Vector3 zVelocity = Vector3.zero;

            if (moveZ != 0)
                zVelocity = transform.forward * moveZ;

            if (moveX != 0)
                xVelocity = transform.right * moveX;

            Vector3 combinedVelocity = (zVelocity + xVelocity) * movementSpeed * Time.deltaTime;
            _playerRB.velocity = new Vector3(combinedVelocity.x, _playerRB.velocity.y, combinedVelocity.z);
        }
    }
}