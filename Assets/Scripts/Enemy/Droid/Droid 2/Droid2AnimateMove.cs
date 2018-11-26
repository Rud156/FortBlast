using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Droid2
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Droid2AnimateMove : MonoBehaviour
    {
        public Animator droidAnimator;

        private NavMeshAgent _droidAgent;
        private const string _animatorMovement = "Movement";

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float maxVelocity = _droidAgent.speed * _droidAgent.speed;
            float currentVelocity = _droidAgent.velocity.sqrMagnitude;

            float movementSpeed = ExtensionFunctions.Map(currentVelocity, 0, maxVelocity, 0, 1);
            droidAnimator.SetFloat(_animatorMovement, movementSpeed);
        }
    }
}