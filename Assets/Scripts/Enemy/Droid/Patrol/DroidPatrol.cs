using System;
using System.Collections;
using System.Collections.Generic;
using FortBlast.Enemy.Droid.Helpers;
using FortBlast.Extras;
using FortBlast.Scenes.MainScene;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Patrol
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(DroidAttack))]
    [RequireComponent(typeof(DroidLaze))]
    public class DroidPatrol : MonoBehaviour
    {
        [Header("Patrol Stats")]
        public float minimumDetectionDistance;
        public float lookRotationSpeed;

        [Header("Droid FOV")]
        public Transform lookingPoint;
        [Range(0, 360)]
        public float maxLookAngle;
        public float angleTolerance;

        [Header("Distances")]
        public float distanceToStopFromIntrestingTarget;
        public float distanceToStopFromPatrolPoint;

        [Header("Attack")]
        public float waitTimeBetweenAttacks = 5f;

        private NavMeshAgent _droidAgent;
        private DroidLaze _droidLaze;
        private DroidAttack _droidAttack;

        private Transform _currentTarget;
        private Vector3[] _terrainMeshVertices;

        private Transform _player;
        private bool _playerFound;
        private bool _attacking;

        private Transform _distactorHolder;
        private bool _distactorFound;

        private Coroutine _coroutine;
        private bool _lazingAround;
        // Used Only When The Player Is Found
        private float _currentNormalizedLookAngle;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
            _droidLaze = GetComponent<DroidLaze>();
            _droidAttack = GetComponent<DroidAttack>();

            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _distactorHolder = GameObject.FindGameObjectWithTag(TagManager.DistractorHolder)?.transform;

            _terrainMeshVertices = transform.parent.GetComponent<MeshFilter>().mesh.vertices;

            SetAgentRandomPatrolPoint();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            CheckAndSetNextTarget();
            CheckPatrolPointTargetReached();
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Terrain))
                return;

            _terrainMeshVertices = other.gameObject.GetComponent<MeshFilter>().mesh.vertices;
            transform.SetParent(other.transform);
        }

        private void CheckPatrolPointTargetReached()
        {
            if (!_droidAgent.isOnNavMesh)
                return;

            if (!_droidAgent.pathPending)
            {
                if (_droidAgent.remainingDistance <= _droidAgent.stoppingDistance)
                {
                    if (!_droidAgent.hasPath || _droidAgent.velocity.sqrMagnitude == 0f)
                    {
                        if (_playerFound && !_attacking)
                        {
                            if (DroidPatrolHelpers.
                                IsAngleWithinToleranceLevel(_currentNormalizedLookAngle, angleTolerance))
                            {
                                // Attack Player
                                _coroutine = StartCoroutine(AttackPlayer());
                            }
                        }
                        else if (_distactorFound && !_attacking)
                        {
                            if (DroidPatrolHelpers.
                                IsAngleWithinToleranceLevel(_currentNormalizedLookAngle, angleTolerance))
                            {
                                // Attack Player
                                _coroutine = StartCoroutine(AttackDistractor(_currentTarget));
                            }
                        }
                        else if (!_playerFound && !_distactorFound && !_lazingAround)
                        {
                            // Laze Then Move to Next Patrol Point
                            _coroutine = StartCoroutine(LazePatrolPoint());
                        }
                    }
                }
            }
        }

        private void CheckAndSetNextTarget()
        {
            float angleWRTTarget = DroidPatrolHelpers
                .CheckTargetInsideFOV(_player, minimumDetectionDistance, maxLookAngle, lookingPoint);
            _currentNormalizedLookAngle = angleWRTTarget;

            if (angleWRTTarget != -1)
            {
                _playerFound = true;
                CheckAndSetIntrestingTarget(_player);
            }
            else
            {
                Transform closestDistractor = DroidPatrolHelpers
                    .GetClosestDistractor(_distactorHolder, transform);

                angleWRTTarget = DroidPatrolHelpers
                    .CheckTargetInsideFOV(closestDistractor,
                    minimumDetectionDistance, maxLookAngle, lookingPoint, false);

                _currentNormalizedLookAngle = angleWRTTarget;

                if (angleWRTTarget != -1)
                {
                    _distactorFound = true;
                    CheckAndSetIntrestingTarget(closestDistractor);
                }
                else if (_playerFound || _distactorFound)
                {
                    _playerFound = false;
                    _distactorFound = false;
                    SetAgentRandomPatrolPoint();
                }
            }
        }

        private void LookTowardsTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = targetPosition - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation,
                    lookRotationSpeed * Time.deltaTime);
            }
        }

        private void SetAgentDestination(Vector3 position)
        {
            if (!_droidAgent.isOnNavMesh || _currentTarget == null)
                return;

            _droidAgent.SetDestination(position);
        }

        private void CheckAndSetIntrestingTarget(Transform target)
        {
            _currentTarget = target;
            _droidAgent.stoppingDistance = distanceToStopFromIntrestingTarget;

            SetAgentDestination(_currentTarget.position);
            LookTowardsTarget(_currentTarget.position);
            ResetAnimationOnFindingIntrestingTarget();
        }

        private void SetAgentRandomPatrolPoint()
        {
            if (_lazingAround)
                return;

            _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
            Vector3 randomPatrolPoint = DroidPatrolHelpers.GetNextTarget(_terrainMeshVertices);
            _currentTarget = transform.parent;

            SetAgentDestination(randomPatrolPoint);
            LookTowardsTarget(randomPatrolPoint);
        }

        private void ResetAnimationOnFindingIntrestingTarget()
        {
            if (!_lazingAround)
                return;

            _droidLaze.StopLazingAbout();
            StopCoroutine(_coroutine);
            _lazingAround = false;
        }

        private IEnumerator AttackPlayer()
        {
            _attacking = true;
            yield return StartCoroutine(_droidAttack.AttackPlayer(_player));
            yield return new WaitForSeconds(waitTimeBetweenAttacks);
            _attacking = false;
        }

        private IEnumerator AttackDistractor(Transform closestDistractor)
        {
            _attacking = true;
            yield return StartCoroutine(_droidAttack.AttackDistractor(closestDistractor));
            yield return new WaitForSeconds(waitTimeBetweenAttacks);
            _attacking = false;
        }

        private IEnumerator LazePatrolPoint()
        {
            _lazingAround = true;
            float lazeWaitTime = _droidLaze.LazeAroundSpot();
            yield return new WaitForSeconds(lazeWaitTime);
            _droidLaze.StopLazingAbout();

            _lazingAround = false;
            SetAgentRandomPatrolPoint();
        }
    }
}