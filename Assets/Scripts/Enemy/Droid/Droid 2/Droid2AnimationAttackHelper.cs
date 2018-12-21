using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid2
{
    public class Droid2AnimationAttackHelper : MonoBehaviour
    {
        private Droid2Attack _attack;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _attack = GetComponentInParent<Droid2Attack>();
        }

        public void AttackTarget()
        {
            _attack.AttackTarget();
        }
    }
}