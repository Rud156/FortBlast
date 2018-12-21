using FortBlast.Extras;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Droid2
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Droid2AnimateMove : MonoBehaviour
    {
        private const string AnimatorMovement = "Movement";

        private NavMeshAgent _droidAgent;
        public Animator droidAnimator;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _droidAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            var maxVelocity = _droidAgent.speed * _droidAgent.speed;
            var currentVelocity = _droidAgent.velocity.sqrMagnitude;

            var movementSpeed = ExtensionFunctions.Map(currentVelocity, 0, maxVelocity, 0, 1);
            droidAnimator.SetFloat(AnimatorMovement, movementSpeed);
        }
    }
}