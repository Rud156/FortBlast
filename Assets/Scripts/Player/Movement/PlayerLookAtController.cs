using FortBlast.Player.Data;
using UnityEngine;

namespace FortBlast.Player.Movement
{
    public class PlayerLookAtController : MonoBehaviour
    {
        [Header("Rotation")] public float rotationSpeed;
        
        private bool _rotationActive;
        private float _yaw;

        private void Start()
        {
            _yaw = transform.rotation.eulerAngles.y;
            _rotationActive = true;
        }

        private void Update() => RotatePlayerOnMouse();

        public void ActivateRotation() => _rotationActive = true;

        public void DeActivateRotation() => _rotationActive = false;

        private void RotatePlayerOnMouse()
        {
            var mouseX = _rotationActive ? Input.GetAxis(PlayerData.MouseX) : 0;
            _yaw += mouseX * rotationSpeed * Time.deltaTime;
            transform.eulerAngles = Vector3.up * _yaw;
        }
    }
}