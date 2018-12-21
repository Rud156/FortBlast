using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Camera
{
    public class MoveCameraWithMouse : MonoBehaviour
    {
        private const string MouseY = "Mouse Y";

        private float _pitch;
        public float maxCameraAngle = 340;
        public float minCameraAngle = 30;
        public float verticalSpeed;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _pitch = transform.localRotation.eulerAngles.x;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            SetAndLimitPitch();
        }

        /// <summary>
        ///     LateUpdate is called every frame, if the Behaviour is enabled.
        ///     It is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }

        private void SetAndLimitPitch()
        {
            var mouseY = Input.GetAxis(MouseY);
            _pitch += -mouseY * verticalSpeed * Time.deltaTime;

            if (_pitch < 0 || _pitch > 360)
                _pitch = ExtensionFunctions.To360Angle(_pitch);

            if (_pitch > minCameraAngle && _pitch < maxCameraAngle)
            {
                var diffToMinAngle = Mathf.Abs(minCameraAngle - _pitch);
                var diffToMaxAngle = Mathf.Abs(maxCameraAngle - _pitch);

                if (diffToMinAngle < diffToMaxAngle)
                    _pitch = minCameraAngle;
                else
                    _pitch = maxCameraAngle;
            }
        }
    }
}