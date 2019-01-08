using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid2
{
    public class Droid2AnimationAttackHelper : MonoBehaviour
    {
        private Droid2Attack _attack;

        private void Start() => _attack = GetComponentInParent<Droid2Attack>();

        public void AttackTarget() => _attack.AttackTarget();
    }
}