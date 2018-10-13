using FortBlast.Player.Data;
using UnityEngine;

namespace FortBlast.Player.Movement
{
    public class PlayerLookAtController : MonoBehaviour
    {
        [Header("Rotation")]
        public float rotationSpeed;

        [Header("Debug")]
        public bool lockCursor;

        private float _yaw;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _yaw = transform.rotation.eulerAngles.y;

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => RotatePlayerOnMouse();

        private void RotatePlayerOnMouse()
        {
            float mouseX = Input.GetAxis(PlayerData.MouseX);
            _yaw += mouseX * rotationSpeed * Time.deltaTime;
            transform.eulerAngles = Vector3.up * _yaw;
        }
    }
}