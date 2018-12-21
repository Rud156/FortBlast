using FortBlast.Player.Data;
using UnityEngine;

namespace FortBlast.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovementController : MonoBehaviour
    {
        private Animator _playerAnimator;

        private Rigidbody _playerRB;
        private float _prevMoveZ;
        public float animatorLerpRate;

        [Header("Movement")] public float movementSpeed;

        public float runningSpeed;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _playerRB = GetComponent<Rigidbody>();
            _playerAnimator = GetComponent<Animator>();
            _prevMoveZ = 0;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            var moveX = Input.GetAxis(PlayerData.HorizontalAxis);
            var moveZ = Input.GetAxis(PlayerData.VerticalAxis);
            var runKeyPressed = Input.GetKey(KeyCode.LeftShift);


            SetAnimator(moveX, moveZ, runKeyPressed);
            MovePlayer(moveX, moveZ, runKeyPressed);
        }

        private void SetAnimator(float moveX, float moveZ, bool runKeyPressed)
        {
            _playerAnimator.SetFloat(PlayerData.PlayerHorizontal, moveX);

            var potentialMoveZ = moveZ;
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
            var xVelocity = Vector3.zero;
            var zVelocity = Vector3.zero;

            if (moveZ != 0)
                zVelocity = transform.forward * moveZ;

            if (moveX != 0)
                xVelocity = transform.right * moveX;

            var playerSpeed = runKeyPressed && moveZ > 0 ? runningSpeed : movementSpeed;
            var combinedVelocity = (zVelocity + xVelocity) * playerSpeed * Time.deltaTime;
            _playerRB.velocity = new Vector3(combinedVelocity.x, _playerRB.velocity.y, combinedVelocity.z);
        }
    }
}