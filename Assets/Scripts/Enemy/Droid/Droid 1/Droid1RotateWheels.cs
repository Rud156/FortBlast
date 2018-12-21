using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Droid1
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Droid1RotateWheels : MonoBehaviour
    {
        public Transform leftWheel;
        public Transform rightWheel;
        public float rotationSpeedMaxVelocity;

        private NavMeshAgent _droidAgent;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _droidAgent = GetComponent<NavMeshAgent>();

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float droidSpeed = _droidAgent.speed;
            float maxVelocity = droidSpeed * droidSpeed;
            float currentVelocity = _droidAgent.velocity.sqrMagnitude;

            float rotationSpeed = ExtensionFunctions.Map(currentVelocity, 0, maxVelocity,
                0, rotationSpeedMaxVelocity);

            leftWheel.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            rightWheel.transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}