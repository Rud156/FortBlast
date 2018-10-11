using FortBlast.Player.Data;
using UnityEngine;

namespace FortBlast.Player.Shooter
{
    [RequireComponent(typeof(Animator))]
    public class PlayerShooterController : MonoBehaviour
    {
        public GameObject absorbObject;

        private Animator _playerAnimator;
        private bool activateAbsorbObject;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _playerAnimator = GetComponent<Animator>();

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            bool mousePressed = Input.GetMouseButton(0);

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed);
            absorbObject.SetActive(mousePressed);
        }
    }
}