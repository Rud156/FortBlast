using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Camera
{
    public class MoveCameraWithMouse : MonoBehaviour
    {
        public float minCameraAngle = 30;
        public float maxCameraAngle = 340;
        public float verticalSpeed;

        private float _pitch;
        private const string MouseY = "Mouse Y";

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _pitch = transform.localRotation.eulerAngles.x;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => SetAndLimitPitch();

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate() => transform.localRotation = Quaternion.Euler(_pitch, 0, 0);

        private void SetAndLimitPitch()
        {
            float mouseY = Input.GetAxis(MouseY);
            _pitch += -mouseY * verticalSpeed * Time.deltaTime;

            if (_pitch < 0 || _pitch > 360)
                _pitch = ExtensionFunctions.To360Angle(_pitch);

            if (_pitch > minCameraAngle && _pitch < maxCameraAngle)
            {
                float diffToMinAngle = Mathf.Abs(minCameraAngle - _pitch);
                float diffToMaxAngle = Mathf.Abs(maxCameraAngle - _pitch);

                if (diffToMinAngle < diffToMaxAngle)
                    _pitch = minCameraAngle;
                else
                    _pitch = maxCameraAngle;
            }
        }
    }
}