using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    [RequireComponent(typeof(Collider))]
    public class DroidDefense : MonoBehaviour
    {
        private const string AnimatorHitParam = "Hit";
        private static readonly int Hit = Animator.StringToHash(AnimatorHitParam);

        [Header("Animation")] public Animator droidAnimator;
        [Range(0, 1)] public float selectionProbability;
        public GameObject shieldEffect;

        private bool _initiateShieldActivation;
        private bool _shieldActivated;
        private GameObject _shieldSystem;

        private void Update()
        {
            if (!_shieldSystem)
                _shieldActivated = false;

            if (Input.GetKeyDown(KeyCode.F)) // TODO: Remove this later
                Debug.Break();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Bullet))
                return;

            var probability = Random.value;
            if (probability <= selectionProbability && !_shieldActivated)
            {
                _initiateShieldActivation = true;
                droidAnimator.SetTrigger(Hit);
            }

            if (_shieldActivated)
                Destroy(other.gameObject);
        }

        public void HitAnimationComplete()
        {
            if (!_initiateShieldActivation || _shieldActivated)
                return;

            _shieldActivated = true;

            _shieldSystem = Instantiate(shieldEffect, transform.position, Quaternion.identity);
            _shieldSystem.transform.SetParent(transform);
            _shieldSystem.transform.localScale = Vector3.one;

            _initiateShieldActivation = false;
        }
    }
}