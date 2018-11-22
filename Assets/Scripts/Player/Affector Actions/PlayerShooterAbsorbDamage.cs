using FortBlast.Player.StatusSetters;
using UnityEngine;
using FortBlast.Player.Data;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Animator))]
    public class PlayerShooterAbsorbDamage : MonoBehaviour
    {
        [Header("Absorber System")]
        public GameObject absorber;

        private Animator _playerAnimator;
        private bool _absorberActive;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerAnimator = GetComponent<Animator>();

            _absorberActive = true;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => DisplayAbsorberOnInput();

        public void ReflectBullet()
        {

        }

        public void AbsorbBullet()
        {

        }

        public void ActivateAbsorber() => _absorberActive = true;
        public void DeActivateAbsorber() => _absorberActive = false;

        private void DisplayAbsorberOnInput()
        {
            if (!_absorberActive)
                return;

            bool mouseLeftPressed = Input.GetMouseButton(0);
            bool mouseRightPressed = Input.GetMouseButton(1);
            bool mousePressed = mouseLeftPressed || mouseRightPressed;

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed);
            absorber.SetActive(mouseLeftPressed);
        }
    }
}