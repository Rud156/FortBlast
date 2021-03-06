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
    public class DroidPatrol : MonoBehaviour
    {
        [Header("Distances")] public float distanceToStopFromInterestingTarget;
        public float distanceToStopFromPatrolPoint;
        public float minimumDetectionDistance;

        [Header("Droid FOV")] public Transform lookingPoint;
        public float lookRotationSpeed;
        [Range(0, 360)] public float maxLookAngle;
        public float angleTolerance;
        
        private Coroutine _coroutine;
        
        private Transform _currentTarget;
        private Transform _distractorHolder;

        private NavMeshAgent _droidAgent;
        private DroidAttack _droidAttack;
        private DroidAttackTarget _droidAttackTarget;
        private DroidLaze _droidLaze;
        private HealthSetter _droidHealthSetter;
        
        private bool _lazingAround;
        private float _currentNormalizedLookAngle;
        private bool _attacking;
        
        private Transform _player;
        private Vector3 _meshCenter;
        private Vector3[] _terrainMeshVertices;

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
            _droidAttackTarget = DroidAttackTarget.None;

            SetAgentRandomPatrolPoint();
        }

        private void Update()
        {
            CheckAndSetNextTarget();
            CheckPatrolPointTargetReached();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Terrain))
                return;

            _terrainMeshVertices = other.gameObject.GetComponent<MeshFilter>().mesh.vertices;
            transform.SetParent(other.transform);
        }

        private void NotifyKilled()
        {
            MissionManager.instance.AddEnemyKilled();
            _droidHealthSetter.healthZero -= NotifyKilled;
        }

        private void CheckPatrolPointTargetReached()
        {
            if (!_droidAgent.isOnNavMesh)
                return;

            switch (_droidAttackTarget)
            {
                case DroidAttackTarget.Player:
                    if (!_attacking &&
                        DroidPatrolHelpers.IsAngleWithinToleranceLevel(_currentNormalizedLookAngle, angleTolerance))
                        _coroutine = StartCoroutine(AttackTarget(_player, true));

                    break;

                case DroidAttackTarget.Distractor:
                    if (!_attacking &&
                        DroidPatrolHelpers.IsAngleWithinToleranceLevel(_currentNormalizedLookAngle, angleTolerance))
                        _coroutine = StartCoroutine(AttackTarget(_currentTarget));

                    break;
            }

            if (!_droidAgent.pathPending && !_lazingAround && _droidAttackTarget == DroidAttackTarget.None)
                if (_droidAgent.remainingDistance <= _droidAgent.stoppingDistance)
                    if (!_droidAgent.hasPath || _droidAgent.velocity.sqrMagnitude == 0f)
                        _coroutine = StartCoroutine(LazePatrolPoint());
        }

        private void CheckAndSetNextTarget()
        {
            var angleWRTTarget = DroidPatrolHelpers
                .CheckTargetInsideFOV(_player, minimumDetectionDistance, maxLookAngle, lookingPoint);
            _currentNormalizedLookAngle = angleWRTTarget;

            if (angleWRTTarget != -1)
            {
                _droidAttackTarget = DroidAttackTarget.Player;
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
                    _droidAttackTarget = DroidAttackTarget.Distractor;
                    CheckAndSetInterestingTarget(closestDistractor);
                }
                else if (_droidAttackTarget != DroidAttackTarget.None)
                {
                    _droidAttackTarget = DroidAttackTarget.None;
                    SetAgentRandomPatrolPoint();
                }
            }
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

        private void SetAgentDestination(Vector3 position)
        {
            if (!_droidAgent.isOnNavMesh || !_currentTarget)
                return;

            _droidAgent.SetDestination(position);
        }

        private void CheckAndSetInterestingTarget(Transform target)
        {
            _currentTarget = target;
            _droidAgent.stoppingDistance = distanceToStopFromInterestingTarget;
            var position = _currentTarget.position;

            SetAgentDestination(position);
            LookTowardsTarget(position);
            ResetAnimationOnFindingInterestingTarget();
        }

        private void SetAgentRandomPatrolPoint()
        {
            if (_lazingAround)
                return;

            _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
            var randomPatrolPoint = DroidPatrolHelpers.GetNextTarget(_terrainMeshVertices) +
                                    _meshCenter;
            _currentTarget = transform.parent;

            SetAgentDestination(randomPatrolPoint);
            LookTowardsTarget(randomPatrolPoint);
        }

        private void ResetAnimationOnFindingInterestingTarget()
        {
            if (!_lazingAround)
                return;

            StopCoroutine(_coroutine);
            _lazingAround = false;
        }

        private IEnumerator AttackTarget(Transform targetPosition, bool usePlayerOffset = false)
        {
            _attacking = true;
            var attackTime = _droidAttack.Attack(targetPosition, usePlayerOffset);
            yield return new WaitForSeconds(attackTime);

            _attacking = false;
            _droidAttack.EndAttack();
        }

        private IEnumerator LazePatrolPoint()
        {
            _lazingAround = true;
            var lazeWaitTime = _droidLaze.LazeAroundSpot();
            yield return new WaitForSeconds(lazeWaitTime);

            _lazingAround = false;
            SetAgentRandomPatrolPoint();
        }

        private enum DroidAttackTarget
        {
            Player,
            Distractor,
            None
        }
    }
}