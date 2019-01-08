using EZCameraShake;
using FortBlast.Player.Data;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerHandControls : MonoBehaviour
    {
        [Header("Reflection Controls")] public int maxReflectionCount;
        public ReflectorTriggerEventCreator reflectorTrigger;
        public float reflectionGenerationRate;

        [Header("Teleporter Controls")] public int maxTeleporterCount;
        public GameObject teleporter;
        public float teleportDistance;
        public float teleporterGenerationRate;
        public GameObject teleporterLandEffect;

        [Header("Camera Shaker")] public CameraShakerStats reflectCameraShaker;
        public CameraShakerStats teleportCameraShaker;

        [Header("Mechanism System")] public GameObject reflector;
        public Transform lookPoint;

        private float _currentReflectionCount;
        private float _currentTeleporterCount;
        private bool _teleporterPrevState;

        private bool _mechanismActive;
        private MechanismState _mechanismState;
        private Animator _playerAnimator;

        private void Start()
        {
            _playerAnimator = GetComponent<Animator>();
            _mechanismActive = true;

            reflectorTrigger.onBulletCollided += OnBulletCollided;
            _mechanismState = MechanismState.ShutOff;

            _teleporterPrevState = false;
            _currentTeleporterCount = maxTeleporterCount;

            _currentReflectionCount = maxReflectionCount;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) // TODO: Remove this later on...
                Debug.Break();

            DisplayAbsorberOnInput();
            UpdateTeleporterAndReflectionHealth();
        }

        private void OnBulletCollided(GameObject bullet)
        {
            switch (_mechanismState)
            {
                case MechanismState.Reflect:
                    ReflectBullet(bullet);
                    break;
            }
        }

        private void ReflectBullet(GameObject bullet)
        {
            bullet.layer = 9; // Put it in the Droid Layer to enable collision
            var bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity *= -1;

            CameraShaker.Instance.ShakeOnce(
                reflectCameraShaker.magnitude,
                reflectCameraShaker.roughness,
                reflectCameraShaker.fadeInTime,
                reflectCameraShaker.fadeOutTime
            );
            _currentReflectionCount -= 1;
        }

        private void DisplayAbsorberOnInput()
        {
            if (!_mechanismActive)
                return;

            var reflectorActive = Input.GetMouseButton(0) && _currentReflectionCount > 1;
            var teleporterActive = Input.GetMouseButton(1) && _currentTeleporterCount > 1;
            var mechanismActive = reflectorActive || teleporterActive;

            if (reflectorActive)
                _mechanismState = MechanismState.Reflect;
            else if (teleporterActive)
                _mechanismState = MechanismState.Teleport;
            else
                _mechanismState = MechanismState.ShutOff;

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mechanismActive);
            reflector.SetActive(reflectorActive);
            teleporter.SetActive(teleporterActive);

            CheckAndTeleportPlayer(teleporterActive);
        }

        private void UpdateTeleporterAndReflectionHealth()
        {
            if (_currentTeleporterCount < maxTeleporterCount)
                _currentTeleporterCount += teleporterGenerationRate * Time.deltaTime;

            if (_currentReflectionCount < maxReflectionCount)
                _currentReflectionCount += reflectionGenerationRate * Time.deltaTime;
        }

        private void CheckAndTeleportPlayer(bool currentTeleporterState)
        {
            if (currentTeleporterState != _teleporterPrevState
                && _teleporterPrevState)
            {
                RaycastHit hit;
                var destination = lookPoint.position + lookPoint.forward * teleportDistance;

                if (Physics.Linecast(lookPoint.position, destination, out hit))
                    destination = lookPoint.position + lookPoint.forward * (hit.distance - 1);

                if (Physics.Raycast(destination, Vector3.down, out hit))
                    destination = new Vector3(destination.x, hit.point.y, destination.z);

                transform.position = destination;
                Instantiate(teleporterLandEffect, transform.position, Quaternion.identity);

                CameraShaker.Instance.ShakeOnce(
                    teleportCameraShaker.magnitude,
                    teleportCameraShaker.roughness,
                    teleportCameraShaker.fadeInTime,
                    teleportCameraShaker.fadeOutTime
                );
                _currentTeleporterCount -= 1;
            }

            _teleporterPrevState = currentTeleporterState;
        }

        private enum MechanismState
        {
            Reflect,
            Teleport,
            ShutOff
        }

        #region  MechanismController

        public void ActivateMechanism()
        {
            _mechanismActive = true;
        }

        public void DeActivateMechanism()
        {
            _playerAnimator.SetBool(PlayerData.PlayerShooting, false);
            reflector.SetActive(false);
            teleporter.SetActive(false);

            _mechanismActive = false;
        }

        #endregion MechanismController

        #region MechanismControlVariables

        public float GetCurrentTeleporterCount()
        {
            return _currentTeleporterCount;
        }

        public float GetCurrentReflectorCount()
        {
            return _currentReflectionCount;
        }

        #endregion MechanismControlVariables
    }
}