using System.Collections;
using FortBlast.Enemy.Tower.Helpers;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Tower
{
    public class TowerController : MonoBehaviour
    {
        private bool _attacking;
        private Coroutine _coroutine;

        private bool _deactivateTower;
        private Transform _distactorHolder;

        private Quaternion _lazeLookRotation;
        private bool _lazingAround;

        private Transform _player;

        [Header("Attack")] public float attackAngleTolerance;

        public GameObject bombLaunchEffect;
        public GameObject bombPrefab;
        public float launchSpeed;

        [Header("Movement")] [Range(0, 360)] public int maxLookAngle;

        public float maxTargetRange;
        public float rotationSpeed;
        public Transform shootingPoint;

        [Header("Prefabs And Movement Points")]
        public Transform towerTop;

        public float waitTimeBetweenAttack;
        public float waitTimeBetweenLaze;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _distactorHolder = GameObject.FindGameObjectWithTag(TagManager.DistractorHolder)?.transform;

            _attacking = false;
            _lazingAround = false;
            _deactivateTower = false;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (_deactivateTower)
                return;

            if (!_attacking)
            {
                var normalizedAngle = TowerControllerHelpers
                    .CheckTargetInsideFOV(_player,
                        towerTop, maxTargetRange, maxLookAngle);

                if (normalizedAngle != -1)
                {
                    CheckAndAttackPlayer(normalizedAngle);
                }
                else
                {
                    var closestDistractor = TowerControllerHelpers.GetClosestDistractor(
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

        public void ActivateTower()
        {
            _deactivateTower = false;
        }

        public void DeactivateTower()
        {
            _deactivateTower = true;
        }

        private void LookAtTarget(Transform target)
        {
            if (!target)
                return;

            var lookDirection = target.position - towerTop.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(lookDirection);
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
            var lookRotation = Quaternion.LookRotation(target.position - shootingPoint.position);
            shootingPoint.transform.rotation = lookRotation;

            var bombLaunchEffectInstance =
                Instantiate(bombLaunchEffect, shootingPoint.position, Quaternion.identity);
            bombLaunchEffectInstance.transform.SetParent(shootingPoint.transform);

            var bombInstance = Instantiate(bombPrefab, shootingPoint.position, Quaternion.identity);
            bombInstance.GetComponent<Rigidbody>().velocity = shootingPoint.transform.forward * launchSpeed;

            _attacking = true;

            yield return new WaitForSeconds(waitTimeBetweenAttack);
            _attacking = false;
        }

        private IEnumerator LazilyLookAround()
        {
            var randomAngle = Random.Range(0, 360);
            _lazeLookRotation = Quaternion.Euler(0, randomAngle, 0);
            _lazingAround = true;

            yield return new WaitForSeconds(waitTimeBetweenLaze);
            _lazingAround = false;
        }
    }
}