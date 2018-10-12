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


        private NavMeshAgent _droidAgent;

        private Transform _currentTarget;
        private int _currentPatrolPointIndex;

        private Transform _player;
        private bool _resetPatrol;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag(TagManager.Player).transform;
            _resetPatrol = true;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            CheckAndSetNextTarget();
            LookTowardsTarget();

            SetAgentDestination();
        }

        private void LookTowardsTarget()
        {
            if (_currentTarget == null)
                return;

            Vector3 lookDirection = _currentTarget.position - transform.position;
            lookDirection.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation,
                lookRotationSpeed * Time.deltaTime);
        }

        private void SetAgentDestination()
        {
            if (!_droidAgent.isOnNavMesh || _currentTarget == null)
                return;

            _droidAgent.SetDestination(_currentTarget.position);
        }

        private void CheckAndSetNextTarget()
        {
            bool isPlayerNearby = CheckPlayerInRange();
            if (isPlayerNearby)
                _currentTarget = _player;
            else
            {
                if (_resetPatrol)
                {
                    _currentPatrolPointIndex = GetClosestPatrolPoint();
                    _resetPatrol = false;
                }
                else
                    _currentPatrolPointIndex = GetNextSequentialPatrolPoint(_currentPatrolPointIndex);

                _currentTarget = _currentPatrolPointIndex == -1 ?
                    patrolPoints[_currentPatrolPointIndex] : null;
            }
        }

        private bool CheckPlayerInRange()
        {
            if (_player == null)
                return false;

            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            if (distanceToPlayer <= minimumDetectionDistance)
            {
                _resetPatrol = true;
                return true;
            }

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
    }
}