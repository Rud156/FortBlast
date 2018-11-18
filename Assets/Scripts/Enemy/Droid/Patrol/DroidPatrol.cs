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
        public float distanceToStopFromPlayer;
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
        private bool _attackingPlayer;

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
            _playerFound = true;

            _terrainMeshVertices = transform.parent.GetComponent<MeshFilter>().mesh.vertices;
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
                        if (_playerFound && !_attackingPlayer)
                        {
                            if (IsAngleWithinToleranceLevel(_currentNormalizedLookAngle))
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

        private void CheckAndSetNextTarget()
        {
            float angleWRTPlayer = IsPlayerInsideFOV();
            _currentNormalizedLookAngle = angleWRTPlayer;
            if (angleWRTPlayer != -1)
            {
                _currentTarget = _player;
                _playerFound = true;
                _droidAgent.stoppingDistance = distanceToStopFromPlayer;

                SetAgentDestination(_currentTarget.position);
                LookTowardsTarget(_currentTarget.position);
                ResetAnimationOnFindingPlayer();
            }
            // For Finding the Next Patrol Point Right After Player Left Range
            else if (_playerFound)
                SetAgentRandomPatrolPoint();
        }

        private float IsPlayerInsideFOV()
        {
            if (_player == null)
                return -1;

            float distanceToPlayer = Vector3.Distance(_player.position, lookingPoint.position);
            if (distanceToPlayer > minimumDetectionDistance || GlobalData.playerInBuilding)
                return -1;

            Vector3 modifiedPlayerPosition = new Vector3(_player.position.x, 0, _player.position.z);
            Vector3 modifiedLookingPosition =
                new Vector3(lookingPoint.position.x, 0, lookingPoint.position.z);

            Vector3 lookDirection = modifiedPlayerPosition - modifiedLookingPosition;
            float angleToPlayer = Vector3.Angle(lookDirection, lookingPoint.forward);
            float normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            else
                return -1;
        }

        private void SetAgentRandomPatrolPoint()
        {
            _playerFound = false;
            _droidAgent.stoppingDistance = distanceToStopFromPatrolPoint;
            Vector3 randomPatrolPoint = DroidPatrolHelpers.GetNextTarget(_terrainMeshVertices);
            _currentTarget = transform.parent;

            SetAgentDestination(randomPatrolPoint);
            LookTowardsTarget(randomPatrolPoint);
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

        private bool IsAngleWithinToleranceLevel(float normalizedAngle)
        {
            if (normalizedAngle < 0)
                return false;

            if (normalizedAngle <= angleTolerance)
                return true;

            return false;
        }

        private IEnumerator AttackPlayer()
        {
            _attackingPlayer = true;
            yield return StartCoroutine(_droidAttack.AttackPlayer(_player));
            yield return new WaitForSeconds(waitTimeBetweenAttacks);
            _attackingPlayer = false;
        }

        private IEnumerator LazePatrolPoint()
        {
            _lazingAround = true;
            float lazeWaitTime = _droidLaze.LazeAroundSpot();
            yield return new WaitForSeconds(lazeWaitTime);
            _droidLaze.StopLazingAbout();

            SetAgentRandomPatrolPoint();
            _lazingAround = false;
        }
    }
}