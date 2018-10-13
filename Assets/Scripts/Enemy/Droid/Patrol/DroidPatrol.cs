using System.Collections;
using System.Collections.Generic;
using FortBlast.Enemy.Droid.Helpers;
using FortBlast.Extras;
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
        public Transform patrolPointGroup;
        public float minimumDetectionDistance;
        public float lookRotationSpeed;

        [Header("Distances")]
        public float distanceToStopFromPlayer;
        public float distanceToStopFromPatrolPoint;


        private NavMeshAgent _droidAgent;
        private DroidLaze _droidLaze;
        private DroidAttack _droidAttack;

        private List<Transform> _patrolPoints;
        private Transform _currentTarget;
        private int _currentPatrolPointIndex;

        private Transform _player;
        private bool _playerFound;
        private bool _attackingPlayer;

        private Coroutine _coroutine;
        private bool _lazingAround;

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
            _playerFound = true;

            _patrolPoints = new List<Transform>();
            for (int i = 0; i < patrolPointGroup.childCount; i++)
                _patrolPoints.Add(patrolPointGroup.GetChild(i));

        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            CheckAndSetNextTarget();
            LookTowardsTarget();

            SetAgentDestination();
            CheckPatrolPointTargetReached();
        }

        private void CheckPatrolPointTargetReached()
        {
            if (!_droidAgent.pathPending)
            {
                if (_droidAgent.remainingDistance <= _droidAgent.stoppingDistance)
                {
                    if (!_droidAgent.hasPath || _droidAgent.velocity.sqrMagnitude == 0f)
                    {
                        if (_playerFound && !_attackingPlayer)
                        {
                            // Attack Player
                            _coroutine = StartCoroutine(AttackPlayer());
                        }
                        else if (!_playerFound && !_lazingAround)
                        {
                            // Laze Then Move to Next Patrol Point
                            _coroutine = StartCoroutine(LazePatrolPoint());
                        }
                    }
                }
            }
        }

        private void LookTowardsTarget()
        {
            if (_currentTarget == null)
                return;

            Vector3 lookDirection = _currentTarget.position - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation,
                    lookRotationSpeed * Time.deltaTime);
            }
        }

        private void SetAgentDestination()
        {
            if (!_droidAgent.isOnNavMesh || _currentTarget == null)
                return;

            _droidAgent.SetDestination(_currentTarget.position);
        }

        private void CheckAndSetNextTarget()
        {
            bool isPlayerNearby = DroidPatrolHelpers
                .CheckPlayerInRange(_player, transform, minimumDetectionDistance);
            if (isPlayerNearby)
            {
                _currentTarget = _player;
                _playerFound = true;
                _droidAgent.stoppingDistance = distanceToStopFromPlayer;
                ResetAnimationOnFindingPlayer();
            }
            else
            {
                if (_playerFound)
                {
                    _currentPatrolPointIndex = DroidPatrolHelpers
                        .GetClosestPatrolPoint(_currentTarget, _player, transform, _patrolPoints);
                    _playerFound = false;
                    _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
                }

                _currentTarget = _currentPatrolPointIndex == -1 ?
                     null : _patrolPoints[_currentPatrolPointIndex];
            }
        }

        private void ResetAnimationOnFindingPlayer()
        {
            if (_lazingAround)
            {
                _droidLaze.StopLazingAbout();
                StopCoroutine(_coroutine);
                _lazingAround = false;
            }
        }

        IEnumerator AttackPlayer()
        {
            _attackingPlayer = true;
            yield return StartCoroutine(_droidAttack.AttackPlayer(_player));
            yield return new WaitForSeconds(5);
            _attackingPlayer = false;
        }

        private IEnumerator LazePatrolPoint()
        {
            _lazingAround = true;
            float lazeWaitTime = _droidLaze.LazeAroundSpot();
            yield return new WaitForSeconds(lazeWaitTime);
            _droidLaze.StopLazingAbout();

            _currentPatrolPointIndex = DroidPatrolHelpers
                .GetNextSequentialPatrolPoint(_currentPatrolPointIndex, _patrolPoints.Count);
            _lazingAround = false;
        }
    }
}