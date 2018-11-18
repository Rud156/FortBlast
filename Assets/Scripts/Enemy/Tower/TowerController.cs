using System.Collections;
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
        public float attackTime;
        public float attackDamage;
        public float playerBaseOffset;
        public float maxPlayerTargetRange;

        [Header("Prefabs And Movement Points")]
        public Transform towerTop;
        public Transform laserShootingPoint;
        public GameObject laser;

        private Transform _player;
        private Renderer _laserRenderer;
        private bool _attacking;
        private bool _laserCreated;

        private Transform _distactorHolder;

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
            _laserCreated = false;
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
                float normalizedAngle = CheckTargetInsideFOV(_player);
                if (normalizedAngle != -1)
                    CheckAndAttackPlayer(normalizedAngle);
                else
                {
                    Transform closestDistractor = GetClosestDistractor();
                    normalizedAngle = CheckTargetInsideFOV(closestDistractor, false);

                    if (normalizedAngle != -1)
                        CheckAndAttackDistractor(closestDistractor, normalizedAngle);
                    else
                        LazeAndLookAround();
                }
            }
            else if (_laserCreated)
                TileLaserTexture();
        }

        public void ActivateTower() => _deactivateTower = false;

        public void DeactivateTower() => _deactivateTower = true;

        private void LookAtTarget(Transform target)
        {
            if (target == null)
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

            if (IsAngleWithinToleranceLevel(normalizedAngle))
                _coroutine = StartCoroutine(AttackPlayer());
        }

        private void CheckAndAttackDistractor(Transform closestDistractor, float normalizedAngle)
        {
            if (_lazingAround)
            {
                StopCoroutine(_coroutine);
                _lazingAround = false;
            }

            LookAtTarget(closestDistractor);
            if (IsAngleWithinToleranceLevel(normalizedAngle))
                _coroutine = StartCoroutine(AttackDistractor(closestDistractor));
        }

        private void LazeAndLookAround()
        {
            if (!_lazingAround)
                _coroutine = StartCoroutine(LazilyLookAround());
            else
                towerTop.rotation = Quaternion.Slerp(towerTop.rotation, _lazeLookRotation,
                    rotationSpeed * Time.deltaTime);
        }

        private Transform GetClosestDistractor()
        {
            float minDistance = float.MaxValue;
            Transform targetObject = null;

            for (int i = 0; i < _distactorHolder.childCount; i++)
            {
                float distance = Vector3.Distance(_distactorHolder.GetChild(i).position,
                    towerTop.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetObject = _distactorHolder.GetChild(i);
                }
            }

            return targetObject;
        }

        private float CheckTargetInsideFOV(Transform target, bool checkPlayerInBuildingStatus = true)
        {
            if (target == null)
                return -1;

            float distanceToPlayer = Vector3.Distance(target.position, towerTop.position);
            if (distanceToPlayer > maxPlayerTargetRange)
                return -1;
            if (checkPlayerInBuildingStatus && GlobalData.playerInBuilding)
                return -1;

            Vector3 modifiedPlayerPosition = new Vector3(target.position.x, 0, target.position.z);
            Vector3 modifiedTowerTopPosition =
                new Vector3(towerTop.position.x, 0, towerTop.position.z);

            Vector3 lookDirection = modifiedPlayerPosition - modifiedTowerTopPosition;
            float angleToPlayer = Vector3.Angle(lookDirection, towerTop.forward);
            float normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            else
                return -1;
        }

        private bool IsAngleWithinToleranceLevel(float normalizedAngle)
        {
            if (normalizedAngle < 0)
                return false;

            if (normalizedAngle <= attackAngleTolerance)
                return true;

            return false;
        }

        private void TileLaserTexture()
        {
            float textureTilling = Mathf.Sin(Time.time) * 4f + 1;
            _laserRenderer.material.mainTextureScale = new Vector2(textureTilling, 1);
        }

        private IEnumerator AttackPlayer()
        {
            Quaternion rotation = Quaternion.LookRotation(_player.position - laserShootingPoint.position);
            GameObject laserInstance = Instantiate(laser, laserShootingPoint.position, rotation);

            LineRenderer lineRenderer = laserInstance.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(0, laserShootingPoint.position);
            lineRenderer.SetPosition(1, _player.position + Vector3.up * playerBaseOffset);

            _laserRenderer = lineRenderer.GetComponent<Renderer>();
            _attacking = true;
            _laserCreated = true;

            _player.GetComponent<PlayerShooterAbsorbDamage>().DamagePlayerAndDecreaseHealth(attackDamage);

            yield return new WaitForSeconds(attackTime);
            _laserCreated = false;
            Destroy(laserInstance);

            yield return new WaitForSeconds(waitTimeBetweenAttack - attackTime);
            _attacking = false;
        }

        private IEnumerator AttackDistractor(Transform closestDistractor)
        {
            if (closestDistractor == null)
                yield break;

            Quaternion rotation = Quaternion.LookRotation(closestDistractor.position -
                laserShootingPoint.position);
            GameObject laserInstance = Instantiate(laser, laserShootingPoint.position, rotation);

            LineRenderer lineRenderer = laserInstance.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(0, laserShootingPoint.position);
            lineRenderer.SetPosition(1, closestDistractor.position);

            _laserRenderer = lineRenderer.GetComponent<Renderer>();
            _attacking = true;
            _laserCreated = true;

            yield return new WaitForSeconds(attackTime);
            _laserCreated = false;
            Destroy(laserInstance);
            Destroy(closestDistractor?.gameObject);

            yield return new WaitForSeconds(waitTimeBetweenAttack - attackTime);
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
