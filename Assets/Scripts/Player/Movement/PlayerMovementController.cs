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
        public float runningSpeed;
        public float animatorLerpRate;

        private Rigidbody _playerRB;
        private Animator _playerAnimator;
        private float _prevMoveZ;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerRB = GetComponent<Rigidbody>();
            _playerAnimator = GetComponent<Animator>();
            _prevMoveZ = 0;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float moveX = Input.GetAxis(PlayerData.HorizontalAxis);
            float moveZ = Input.GetAxis(PlayerData.VerticalAxis);
            bool runKeyPressed = Input.GetKey(KeyCode.LeftShift);


            SetAnimator(moveX, moveZ, runKeyPressed);
            MovePlayer(moveX, moveZ, runKeyPressed);
        }

        private void SetAnimator(float moveX, float moveZ, bool runKeyPressed)
        {
            _playerAnimator.SetFloat(PlayerData.PlayerHorizontal, moveX);

            float potentialMoveZ = moveZ;
            if (moveZ > 0)
            {
                if (runKeyPressed)
                    potentialMoveZ = Mathf.Lerp(_prevMoveZ, Mathf.Clamp01(potentialMoveZ),
                        animatorLerpRate * Time.deltaTime);
                else
                    potentialMoveZ = Mathf.Lerp(_prevMoveZ, Mathf.Clamp(potentialMoveZ, 0, 0.5f),
                        animatorLerpRate * Time.deltaTime);
            }

            _playerAnimator.SetFloat(PlayerData.PlayerVertical, potentialMoveZ);
            _prevMoveZ = potentialMoveZ;
        }

        private void MovePlayer(float moveX, float moveZ, bool runKeyPressed)
        {
            Vector3 xVelocity = Vector3.zero;
            Vector3 zVelocity = Vector3.zero;

            if (moveZ != 0)
                zVelocity = transform.forward * moveZ;

            if (moveX != 0)
                xVelocity = transform.right * moveX;

            float playerSpeed = runKeyPressed && moveZ > 0 ? runningSpeed : movementSpeed;
            Vector3 combinedVelocity = (zVelocity + xVelocity) * playerSpeed * Time.deltaTime;
            _playerRB.velocity = new Vector3(combinedVelocity.x, _playerRB.velocity.y, combinedVelocity.z);
        }
    }
}