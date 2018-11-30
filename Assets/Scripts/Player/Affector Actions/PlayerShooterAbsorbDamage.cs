using FortBlast.Player.StatusSetters;
using UnityEngine;
using FortBlast.Player.Data;
using FortBlast.Extras;
using FortBlast.Common;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerShooterAbsorbDamage : MonoBehaviour
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
        public AbsorberTriggerEventCreator reflectorTrigger;
        public float teleportDistance;

        private Animator _playerAnimator;
        private bool _mechanismActive;
        private MechanismState _mechanismState;

        private bool _teleporterPrevState;

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
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                Debug.Break();

            DisplayAbsorberOnInput();
        }

        public void ActivateAbsorber() => _mechanismActive = true;
        public void DeActivateAbsorber()
        {
            _playerAnimator.SetBool(PlayerData.PlayerShooting, false);
            reflector.SetActive(false);
            teleporter.SetActive(false);

            _mechanismActive = false;
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
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity *= -1;
        }

        private void DisplayAbsorberOnInput()
        {
            if (!_mechanismActive)
                return;

            bool mouseLeftPressed = Input.GetMouseButton(0);
            bool mouseRightPressed = Input.GetMouseButton(1);
            bool mousePressed = mouseLeftPressed || mouseRightPressed;

            if (mouseLeftPressed)
                _mechanismState = MechanismState.Reflect;
            else if (mouseRightPressed)
                _mechanismState = MechanismState.Teleport;
            else if (!mousePressed)
                _mechanismState = MechanismState.ShutOff;

            CheckAndTeleportPlayer(mouseRightPressed);
            _teleporterPrevState = mouseRightPressed;

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed);
            reflector.SetActive(mouseLeftPressed);
            teleporter.SetActive(mouseRightPressed);
        }

        private void CheckAndTeleportPlayer(bool currentRightMousePressed)
        {
            if (currentRightMousePressed != _teleporterPrevState
                && _teleporterPrevState == true)
            {
                RaycastHit hit;
                Vector3 destination = transform.position + transform.forward * teleportDistance;

                if (Physics.Linecast(transform.position, destination, out hit))
                    destination = transform.position + transform.forward * (hit.distance - 1);

                transform.position = destination;
            }
        }
    }
}