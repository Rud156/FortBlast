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

        private void Start() => _droidAgent = GetComponent<NavMeshAgent>();

        private void Update()
        {
            var droidSpeed = _droidAgent.speed;
            var maxVelocity = droidSpeed * droidSpeed;
            var currentVelocity = _droidAgent.velocity.sqrMagnitude;

            var rotationSpeed = ExtensionFunctions.Map(currentVelocity, 0, maxVelocity,
                0, rotationSpeedMaxVelocity);

            leftWheel.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            rightWheel.transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}