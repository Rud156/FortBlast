using System.Collections;
using FortBlast.Enemy.Tower.Helpers;
using FortBlast.Extras;
using FortBlast.Player.AffecterActions;
using FortBlast.Scenes.MainScene;
using UnityEngine;

namespace FortBlast.Enemy.Tower
{
    public class TowerController : MonoBehaviour
    {
        [Header("Movement")]
        [Range(0, 360)]
        public int maxLookAngle;
        public float rotationSpeed;
        public float waitTimeBetweenLaze;

        [Header("Attack")]
        public float attackAngleTolerance;
        public float waitTimeBetweenAttack;
        public float maxTargetRange;
        public float launchSpeed;

        [Header("Prefabs And Movement Points")]
        public Transform towerTop;
        public Transform shootingPoint;
        public GameObject bombPrefab;
        public GameObject bombLaunchEffect;

        private Transform _player;
        private Transform _distactorHolder;
        private bool _attacking;

        private Quaternion _lazeLookRotation;
        private bool _lazingAround;
        private Coroutine _coroutine;

        private bool _deactivateTower;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _distactorHolder = GameObject.FindGameObjectWithTag(TagManager.DistractorHolder)?.transform;

            _attacking = false;
            _lazingAround = false;
            _deactivateTower = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_deactivateTower)
                return;

            if (!_attacking)
            {
                float normalizedAngle = TowerControllerHelpers
                    .CheckTargetInsideFOV(_player,
                    towerTop, maxTargetRange, maxLookAngle);

                if (normalizedAngle != -1)
                    CheckAndAttackPlayer(normalizedAngle);
                else
                {
                    Transform closestDistractor = TowerControllerHelpers.GetClosestDistractor(
                            _distactorHolder, towerTop
                        );
                    normalizedAngle = TowerControllerHelpers
                        .CheckTargetInsideFOV(closestDistractor,
                        towerTop, maxTargetRange, maxLookAngle, false);

                    if (normalizedAngle != -1)
                        CheckAndAttackDistractor(closestDistractor, normalizedAngle);
                    else
                        LazeAndLookAround();
                }
            }
        }

        public void ActivateTower() => _deactivateTower = false;

        public void DeactivateTower() => _deactivateTower = true;

        private void LookAtTarget(Transform target)
        {
            if (!target)
                return;

            Vector3 lookDirection = target.position - towerTop.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                towerTop.rotation = Quaternion.Slerp(towerTop.rotation, lookRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private void CheckAndAttackPlayer(float normalizedAngle)
        {
            if (_lazingAround)
            {
                StopCoroutine(_coroutine);
                _lazingAround = false;
            }

            LookAtTarget(_player);

            if (TowerControllerHelpers.IsAngleWithinToleranceLevel(normalizedAngle, attackAngleTolerance))
                _coroutine = StartCoroutine(AttackTarget(_player));
        }

        private void CheckAndAttackDistractor(Transform closestDistractor, float normalizedAngle)
        {
            if (_lazingAround)
            {
                StopCoroutine(_coroutine);
                _lazingAround = false;
            }

            LookAtTarget(closestDistractor);
            if (TowerControllerHelpers.IsAngleWithinToleranceLevel(normalizedAngle, attackAngleTolerance))
                _coroutine = StartCoroutine(AttackTarget(closestDistractor));
        }

        private void LazeAndLookAround()
        {
            if (!_lazingAround)
                _coroutine = StartCoroutine(LazilyLookAround());
            else
                towerTop.rotation = Quaternion.Slerp(towerTop.rotation, _lazeLookRotation,
                    rotationSpeed * Time.deltaTime);
        }

        private IEnumerator AttackTarget(Transform target)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.position - shootingPoint.position);
            shootingPoint.transform.rotation = lookRotation;

            GameObject bombLaunchEffectInstance =
                Instantiate(bombLaunchEffect, shootingPoint.position, Quaternion.identity);
            bombLaunchEffectInstance.transform.SetParent(shootingPoint.transform);

            GameObject bombInstance = Instantiate(bombPrefab, shootingPoint.position, Quaternion.identity);
            bombInstance.GetComponent<Rigidbody>().velocity = shootingPoint.transform.forward * launchSpeed;

            _attacking = true;

            yield return new WaitForSeconds(waitTimeBetweenAttack);
            _attacking = false;
        }

        private IEnumerator LazilyLookAround()
        {
            int randomAngle = Random.Range(0, 360);
            _lazeLookRotation = Quaternion.Euler(0, randomAngle, 0);
            _lazingAround = true;

            yield return new WaitForSeconds(waitTimeBetweenLaze);
            _lazingAround = false;
        }
    }
}
