using System;
using System.Collections;
using FortBlast.Common;
using FortBlast.Enemy.Droid.Base;
using FortBlast.Enemy.Droid.Helpers;
using FortBlast.Extras;
using FortBlast.Missions;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Patrol
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(DroidAttack))]
    [RequireComponent(typeof(DroidLaze))]
    public class DroidFSMPatrol : MonoBehaviour
    {
        [Header("Distances")] public float distanceToStopFromInterestingTarget;
        public float distanceToStopFromPatrolPoint;
        public float minimumDetectionDistance;

        [Header("Droid FOV")] public Transform lookingPoint;
        public float lookRotationSpeed;
        [Range(0, 360)] public float maxLookAngle;
        public float angleTolerance;

        private Coroutine _runningCoroutine;

        private Transform _currentTarget;
        private Transform _distractorHolder;

        private NavMeshAgent _droidAgent;
        private DroidAttack _droidAttack;
        private DroidLaze _droidLaze;
        private HealthSetter _droidHealthSetter;

        private float _currentNormalizedLookAngle;

        private Transform _player;
        private Vector3 _meshCenter;
        private Vector3[] _terrainMeshVertices;

        private enum DroidAttackTarget
        {
            None,
            Player,
            Distractor
        }

        private enum DroidState
        {
            Idle,
            IdlePlaying,

            TargetLockOn,

            Attacking,
            AttackPlaying
        }

        private DroidAttackTarget _droidAttackTarget;
        private DroidState _droidState;

        private float _currentIdlingTime;

        private void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
            _droidLaze = GetComponent<DroidLaze>();
            _droidAttack = GetComponent<DroidAttack>();
            _droidHealthSetter = GetComponent<HealthSetter>();
            _droidHealthSetter.healthZero += NotifyKilled;

            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _distractorHolder = GameObject.FindGameObjectWithTag(TagManager.DistractorHolder)?.transform;

            _meshCenter = transform.position;
            _terrainMeshVertices = transform.parent.GetComponent<MeshFilter>().mesh.vertices;

            UpdateDroidState(DroidState.Idle);
            UpdateDroidAttackTarget(DroidAttackTarget.Player);
        }

        private void Update()
        {
            switch (_droidState)
            {
                case DroidState.IdlePlaying:
                case DroidState.Idle:
                    CheckAndIdleDroidForTime();
                    break;

                case DroidState.TargetLockOn:
                    MoveTowardsTarget();
                    break;

                case DroidState.AttackPlaying:
                case DroidState.Attacking:
                    AttackTarget();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Lazing Around

        private void CheckAndIdleDroidForTime()
        {
            if (_droidState == DroidState.Idle)
                _runningCoroutine = StartCoroutine(LazePatrolPoint());


            SetNextTarget();
        }

        private void SetNextTarget()
        {
            var angleWRTTarget = DroidPatrolHelpers
                .CheckTargetInsideFOV(_player, minimumDetectionDistance, maxLookAngle, lookingPoint);
            _currentNormalizedLookAngle = angleWRTTarget;

            if (angleWRTTarget != -1)
            {
                UpdateDroidAttackTarget(DroidAttackTarget.Player);
                CheckAndSetInterestingTarget(_player);
                MissionManager.instance.IncrementSpottedTimes();
            }
            else
            {
                var closestDistractor = DroidPatrolHelpers
                    .GetClosestDistractor(_distractorHolder, transform);

                angleWRTTarget = DroidPatrolHelpers
                    .CheckTargetInsideFOV(closestDistractor,
                        minimumDetectionDistance, maxLookAngle, lookingPoint, false);

                _currentNormalizedLookAngle = angleWRTTarget;

                if (angleWRTTarget != -1)
                {
                    UpdateDroidAttackTarget(DroidAttackTarget.Distractor);
                    CheckAndSetInterestingTarget(closestDistractor);
                }
                else if (_droidAttackTarget != DroidAttackTarget.None)
                {
                    UpdateDroidAttackTarget(DroidAttackTarget.None);
                    SetRandomPointAsTarget();
                }
                else
                {
                    if (!_droidAgent.pathPending &&
                        _droidAgent.remainingDistance <= _droidAgent.stoppingDistance &&
                        (!_droidAgent.hasPath || _droidAgent.velocity.sqrMagnitude == 0f))
                    {
                        switch (_droidAttackTarget)
                        {
                            case DroidAttackTarget.None:
                                UpdateDroidState(DroidState.Idle);
                                break;

                            case DroidAttackTarget.Player:
                            case DroidAttackTarget.Distractor:
                                UpdateDroidState(DroidState.Attacking);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        private void CheckAndSetInterestingTarget(Transform target)
        {
            _currentTarget = target;
            _droidAgent.stoppingDistance = distanceToStopFromInterestingTarget;
            var position = _currentTarget.position;

            SetAgentDestination(position);
            LookTowardsTarget(position);
            ResetAnimationOnFindingInterestingTarget();

            UpdateDroidState(DroidState.TargetLockOn);
        }

        private void SetRandomPointAsTarget()
        {
            _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
            var randomPatrolPoint = DroidPatrolHelpers.GetNextTarget(_terrainMeshVertices) +
                                    _meshCenter;
            _currentTarget = transform.parent;

            SetAgentDestination(randomPatrolPoint);
            LookTowardsTarget(randomPatrolPoint);

            UpdateDroidState(DroidState.TargetLockOn);
        }

        private void ResetAnimationOnFindingInterestingTarget() =>
            StopCoroutine(_runningCoroutine);

        private IEnumerator LazePatrolPoint()
        {
            UpdateDroidState(DroidState.IdlePlaying);

            var lazeWaitTime = _droidLaze.LazeAroundSpot();
            yield return new WaitForSeconds(lazeWaitTime);

            UpdateDroidState(DroidState.Idle);
        }

        #endregion Lazing Around

        #region Targetting Target

        private void MoveTowardsTarget() => SetNextTarget();

        private void SetAgentDestination(Vector3 targetPosition)
        {
            if (!_droidAgent.isOnNavMesh || !_currentTarget)
                return;

            _droidAgent.SetDestination(targetPosition);
        }

        private void LookTowardsTarget(Vector3 targetPosition)
        {
            var lookDirection = targetPosition - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation,
                    lookRotationSpeed * Time.deltaTime);
            }
        }

        #endregion Targetting Target

        #region Attack Target

        private void AttackTarget()
        {
            if (_droidState == DroidState.AttackPlaying)
                return;

            switch (_droidAttackTarget)
            {
                case DroidAttackTarget.None:
                    // No Target Just Patrolling
                    break;

                case DroidAttackTarget.Player:
                    _runningCoroutine = StartCoroutine(AttackTarget(_player, true));
                    UpdateDroidState(DroidState.AttackPlaying);
                    break;

                case DroidAttackTarget.Distractor:
                    _runningCoroutine = StartCoroutine(AttackTarget(_currentTarget));
                    UpdateDroidState(DroidState.AttackPlaying);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator AttackTarget(Transform targetPosition, bool usePlayerOffset = false)
        {
            var attackTime = _droidAttack.Attack(targetPosition, usePlayerOffset);
            yield return new WaitForSeconds(attackTime);

            _droidAttack.EndAttack();
        }

        #endregion Attack Target

        #region Helpers

        private void UpdateDroidState(DroidState droidState) => _droidState = droidState;

        private void UpdateDroidAttackTarget(DroidAttackTarget droidAttackTarget) =>
            _droidAttackTarget = droidAttackTarget;

        private void NotifyKilled()
        {
            MissionManager.instance.AddEnemyKilled();
            _droidHealthSetter.healthZero -= NotifyKilled;
        }

        #endregion Helpers
    }
}