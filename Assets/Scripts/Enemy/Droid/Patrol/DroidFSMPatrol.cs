using System;
using FortBlast.Common;
using FortBlast.Enemy.Droid.Base;
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
            Patrolling,
            Attacking,
            TargetLockOn,
        }

        private DroidAttackTarget _droidAttackTarget;
        private DroidState _droidState;

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
        }

        private void Update()
        {
            switch (_droidState)
            {
                case DroidState.Idle:
                    break;

                case DroidState.Patrolling:
                    break;

                case DroidState.Attacking:
                    break;

                case DroidState.TargetLockOn:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NotifyKilled()
        {
            MissionManager.instance.AddEnemyKilled();
            _droidHealthSetter.healthZero -= NotifyKilled;
        }
    }
}