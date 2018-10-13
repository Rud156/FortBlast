using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DroidPatrol : MonoBehaviour
    {
        [Header("Patrol Stats")]
        public List<Transform> patrolPoints;
        public float minimumDetectionDistance;
        public float lookRotationSpeed;

        [Header("Distances")]
        public float distanceToStopFromPlayer;
        public float distanceToStopFromPatrolPoint;


        private NavMeshAgent _droidAgent;

        private Transform _currentTarget;
        private int _currentPatrolPointIndex;

        private Transform _player;
        private bool _playerFound;
        private bool _coroutineRunning;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _playerFound = true;
            _coroutineRunning = false;
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
            if (!_droidAgent.pathPending && !_coroutineRunning)
            {
                if (_droidAgent.remainingDistance <= _droidAgent.stoppingDistance)
                {
                    if (!_droidAgent.hasPath || _droidAgent.velocity.sqrMagnitude == 0f)
                    {
                        if (_playerFound)
                        {
                            // Attack Player
                            StartCoroutine(AttackPlayer());
                        }
                        else
                        {
                            // Laze Then Move to Next Patrol Point
                            StartCoroutine(LazePatrolPoint());
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

        #region FindNextTarget

        private void CheckAndSetNextTarget()
        {
            bool isPlayerNearby = CheckPlayerInRange();
            if (isPlayerNearby)
            {
                _currentTarget = _player;
                _playerFound = true;
                _droidAgent.stoppingDistance = distanceToStopFromPlayer;
            }
            else
            {
                if (_playerFound)
                {
                    _currentPatrolPointIndex = GetClosestPatrolPoint();
                    _playerFound = false;
                    _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
                }

                _currentTarget = _currentPatrolPointIndex == -1 ?
                     null : patrolPoints[_currentPatrolPointIndex];
            }
        }

        private bool CheckPlayerInRange()
        {
            if (_player == null)
                return false;

            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            if (distanceToPlayer <= minimumDetectionDistance)
                return true;

            return false;
        }

        private int GetClosestPatrolPoint()
        {
            if (_currentTarget == _player)
                return -1;

            float minDistance = float.MaxValue;
            int currentPatrolPointIndex = -1;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                float currentPointDistance = Vector3.Distance(patrolPoints[i].position, transform.position);
                if (currentPointDistance < minDistance)
                {
                    minDistance = currentPointDistance;
                    currentPatrolPointIndex = i;
                }
            }

            return currentPatrolPointIndex;
        }

        private int GetNextSequentialPatrolPoint(int currentPatrolPointIndex)
        {
            currentPatrolPointIndex += 1;
            if (currentPatrolPointIndex >= patrolPoints.Count)
                currentPatrolPointIndex = 0;

            return currentPatrolPointIndex;
        }

        #endregion FindNextTarget

        IEnumerator AttackPlayer()
        {
            _coroutineRunning = true;
            yield return new WaitForSeconds(5);
            _coroutineRunning = false;
        }

        private IEnumerator LazePatrolPoint()
        {
            _coroutineRunning = true;
            yield return new WaitForSeconds(5);

            _currentPatrolPointIndex = GetNextSequentialPatrolPoint(_currentPatrolPointIndex);
            _coroutineRunning = false;
        }
    }
}