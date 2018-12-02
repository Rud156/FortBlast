using UnityEngine;
using FortBlast.Player.Data;
using FortBlast.Extras;
using FortBlast.Common;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerHandControls : MonoBehaviour
    {
        private enum MechanismState
        {
            Reflect,
            Teleport,
            ShutOff
        }

        [Header("Mechanism System")]
        public GameObject reflector;
        public GameObject teleporter;
        public Transform lookPoint;
        public ReflectorTriggerEventCreator reflectorTrigger;
        public float teleportDistance;

        [Header("Reflection Controls")]
        public int maxReflectionCount;
        public float reflectionGenerationRate;

        [Header("Teleporter Controls")]
        public int maxTeleporterCount;
        public float teleporterGenerationRate;


        private Animator _playerAnimator;
        private bool _mechanismActive;
        private MechanismState _mechanismState;

        private bool _teleporterPrevState;
        private float _currentTeleporterCount;

        private float _currentReflectionCount;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerAnimator = GetComponent<Animator>();
            _mechanismActive = true;

            reflectorTrigger.onBulletCollided += OnBulletCollided;
            _mechanismState = MechanismState.ShutOff;

            _teleporterPrevState = false;
            _currentTeleporterCount = maxTeleporterCount;

            _currentReflectionCount = maxReflectionCount;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) // TODO: Remove this later on...
                Debug.Break();

            DisplayAbsorberOnInput();
            UpdateTeleporterAndReflectionHealth();
        }

        #region  MechanismController

        public void ActivateMechanism() => _mechanismActive = true;
        public void DeActivateMechanism()
        {
            _playerAnimator.SetBool(PlayerData.PlayerShooting, false);
            reflector.SetActive(false);
            teleporter.SetActive(false);

            _mechanismActive = false;
        }

        #endregion MechanismController

        #region MechanismControlVariables

        public float GetCurrentTeleporterCount() => _currentTeleporterCount;

        public float GetCurrentReflectorCount() => _currentReflectionCount;

        #endregion MechanismControlVariables

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
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity *= -1;

            _currentReflectionCount -= 1;
        }

        private void DisplayAbsorberOnInput()
        {
            if (!_mechanismActive)
                return;

            bool reflectorActive = Input.GetMouseButton(0) && _currentReflectionCount > 1;
            bool teleporterActive = Input.GetMouseButton(1) && _currentTeleporterCount > 1;
            bool mechanismActive = reflectorActive || teleporterActive;

            if (reflectorActive)
                _mechanismState = MechanismState.Reflect;
            else if (teleporterActive)
                _mechanismState = MechanismState.Teleport;
            else if (!mechanismActive)
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
                && _teleporterPrevState == true)
            {
                RaycastHit hit;
                Vector3 destination = lookPoint.position + lookPoint.forward * teleportDistance;

                if (Physics.Linecast(lookPoint.position, destination, out hit))
                    destination = lookPoint.position + lookPoint.forward * (hit.distance - 1);

                if (Physics.Raycast(destination, Vector3.down, out hit))
                    destination = new Vector3(destination.x, hit.point.y, destination.z);

                transform.position = destination;
                _currentTeleporterCount -= 1;
            }

            _teleporterPrevState = currentTeleporterState;
        }
    }
}