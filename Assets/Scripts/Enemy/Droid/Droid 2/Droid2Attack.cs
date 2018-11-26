using System.Collections;
using System.Collections.Generic;
using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid2
{
    public class Droid2Attack : DroidAttack
    {
        [Header("Attack Animations")]
        public float timeDiffSameAttack;
        public float totalAttackTimes;
        public Animator droidAnimator;

        private Transform _target;
        private bool _usePlayerOffset;

        private const string attackAnimationParam = "Attacking";

        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            _target = target;
            _usePlayerOffset = usePlayerOffset;
            droidAnimator.SetBool(attackAnimationParam, true);

            return base.attackTime;
        }

        public override void EndAttack() => droidAnimator.SetBool(attackAnimationParam, false);

        public void AttackTarget()
        {
            for (int i = 0; i < totalAttackTimes; i++)
                StartCoroutine(AttackDelayedStart(i * timeDiffSameAttack));
        }

        private IEnumerator AttackDelayedStart(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            for (int i = 0; i < base.launchPoints.Length; i++)
            {
                Vector3 position = _usePlayerOffset ? _target.position + Vector3.up * base.playerBaseOffset :
                    _target.position;

                Quaternion lookRotation = Quaternion.LookRotation(position - base.launchPoints[i].position);
                base.launchPoints[i].transform.rotation = lookRotation;

                GameObject bulletInstance = Instantiate(base.droidBullet, base.launchPoints[i].transform.position,
                        Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = base.launchPoints[i].transform.forward *
                    base.launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }
        }
    }
}