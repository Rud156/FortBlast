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

        [Header("Lights and Particles")]
        public Light absorberLight;
        public ParticleSystem absorberParticleSystem;

        [Header("Light Colors")]
        public Color reflectorLightColor;
        public Color absorberLightColor;

        [Header("Reflector Colors")]
        public Color minReflectorColor;
        public Color maxReflectorColor;

        [Header("Absorber Color")]
        public Color minAbsorberColor;
        public Color maxAbsorberColor;

        private Animator _playerAnimator;
        private bool _absorberMechanismActive;
        private AbsorberState _absorberState;

        private ParticleSystem.MainModule _absorberMain;
        private Renderer _absorberRenderer;

        private const string TintColor = "_TintColor";
        private const float FiftyPrecentAlpha = 0.19607843137254902f;

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

            _absorberMain = absorberParticleSystem.main;
            _absorberRenderer = absorber.GetComponent<Renderer>();
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
            bullet.layer = 9; // Put it in the Droid Layer to enable collision
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity *= -1;
        }

        private void AbsorbBullet(GameObject bullet)
        {
            Destroy(bullet);
            // TODO: Store bullet somewhere. So that it can be shot later
        }

        private void DisplayAbsorberOnInput()
        {
            if (!_absorberMechanismActive)
                return;

            bool mouseLeftPressed = Input.GetMouseButton(0);
            bool mouseRightPressed = Input.GetMouseButton(1);
            bool mousePressed = mouseLeftPressed || mouseRightPressed;

            if (mouseLeftPressed)
            {
                _absorberState = AbsorberState.Reflect;

                absorberLight.color = reflectorLightColor;
                _absorberMain.startColor = new ParticleSystem.MinMaxGradient(
                    minReflectorColor,
                    maxReflectorColor
                );
                _absorberRenderer.material.SetColor(TintColor,
                    new Color(reflectorLightColor.r, reflectorLightColor.g, reflectorLightColor.b,
                        FiftyPrecentAlpha)
                );
            }
            else if (mouseRightPressed)
            {
                _absorberState = AbsorberState.Absorb;

                absorberLight.color = absorberLightColor;
                _absorberMain.startColor = new ParticleSystem.MinMaxGradient(
                    minAbsorberColor,
                    maxAbsorberColor
                );
                _absorberRenderer.material.SetColor(TintColor,
                    new Color(absorberLightColor.r, absorberLightColor.g, absorberLightColor.b,
                        FiftyPrecentAlpha)
                );
            }
            else if (!mousePressed)
                _absorberState = AbsorberState.ShutOff;

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed);
            absorber.SetActive(mousePressed);
        }
    }
}