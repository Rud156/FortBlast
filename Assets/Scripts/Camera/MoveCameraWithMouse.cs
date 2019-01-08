using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.CustomCamera
{
    public class MoveCameraWithMouse : MonoBehaviour
    {
        private const string MouseY = "Mouse Y";

        [Header(("Movement Stats"))] public float maxCameraAngle = 340;
        public float minCameraAngle = 30;
        public float verticalSpeed;

        [Header(("Camera"))] public Transform playerCamera;

        private float _pitch;

        private void Start() => _pitch = playerCamera.localRotation.eulerAngles.x;

        private void Update() => SetAndLimitPitch();

        private void LateUpdate() => playerCamera.localRotation = Quaternion.Euler(_pitch, 0, 0);

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