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
        private enum AbsorberState
        {
            Reflect,
            Absorb,
            ShutOff
        }

        [Header("Absorber System")]
        public GameObject absorber;
        public AbsorberTriggerEventCreator absorberTrigger;

        private Animator _playerAnimator;
        private bool _absorberMechanismActive;
        private AbsorberState _absorberState;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerAnimator = GetComponent<Animator>();
            _absorberMechanismActive = true;

            absorberTrigger.onBulletCollided += OnBulletCollided;
            _absorberState = AbsorberState.ShutOff;
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

        public void ActivateAbsorber() => _absorberMechanismActive = true;
        public void DeActivateAbsorber() => _absorberMechanismActive = false;

        private void OnBulletCollided(GameObject bullet)
        {
            switch (_absorberState)
            {
                case AbsorberState.Reflect:
                    ReflectBullet(bullet);
                    break;

                case AbsorberState.Absorb:
                    AbsorbBullet(bullet);
                    break;
            }
        }

        private void ReflectBullet(GameObject bullet)
        {
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity *= -1;
        }

        private void AbsorbBullet(GameObject bullet)
        {
            Debug.Log("Absorbed Bullet");
        }

        private void DisplayAbsorberOnInput()
        {
            if (!_absorberMechanismActive)
                return;

            bool mouseLeftPressed = Input.GetMouseButton(0);
            bool mouseRightPressed = Input.GetMouseButton(1);
            bool mousePressed = mouseLeftPressed || mouseRightPressed;

            if (mouseLeftPressed)
                _absorberState = AbsorberState.Reflect;
            else if (mouseRightPressed)
                _absorberState = AbsorberState.Absorb;
            else if (!mousePressed)
                _absorberState = AbsorberState.ShutOff;

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed);
            absorber.SetActive(mousePressed); // TODO: Change Based on Left or Right Click
        }
    }
}