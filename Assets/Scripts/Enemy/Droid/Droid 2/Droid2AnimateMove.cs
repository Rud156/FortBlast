using FortBlast.Extras;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.Enemy.Droid.Droid2
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Droid2AnimateMove : MonoBehaviour
    {
        private const string AnimatorMovement = "Movement";
        
        public Animator droidAnimator;
        
        private NavMeshAgent _droidAgent;

        private void Start() => _droidAgent = GetComponent<NavMeshAgent>();

        private void Update()
        {
            var maxVelocity = _droidAgent.speed * _droidAgent.speed;
            var currentVelocity = _droidAgent.velocity.sqrMagnitude;

            var movementSpeed = ExtensionFunctions.Map(currentVelocity, 0, maxVelocity, 
                0, 1);
            droidAnimator.SetFloat(AnimatorMovement, movementSpeed);
        }
    }
}