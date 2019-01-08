using System.Collections;
using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid2
{
    public class Droid2Attack : DroidAttack
    {
        private static readonly int Attacking = Animator.StringToHash(AttackAnimationParam);
        private const string AttackAnimationParam = "Attacking";
        
        [Header("Attack Animations")] public float timeDiffSameAttack;
        public Animator droidAnimator;
        public float totalAttackTimes;
        
        private Transform _target;
        private bool _usePlayerOffset;

        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            _target = target;
            _usePlayerOffset = usePlayerOffset;
            droidAnimator.SetBool(Attacking, true);

            return attackTime;
        }

        public override void EndAttack() => droidAnimator.SetBool(Attacking, false);

        public void AttackTarget()
        {
            for (var i = 0; i < totalAttackTimes; i++)
                StartCoroutine(AttackDelayedStart(i * timeDiffSameAttack));
        }

        private IEnumerator AttackDelayedStart(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            foreach (var launchPoint in launchPoints)
            {
                var position = _usePlayerOffset ? _target.position + Vector3.up * playerBaseOffset : _target.position;

                var lookRotation = Quaternion.LookRotation(position - launchPoint.position);
                launchPoint.transform.rotation = lookRotation;

                Instantiate(launchEffect, launchPoint.position, lookRotation);

                var bulletInstance = Instantiate(droidBullet,
                    launchPoint.transform.position, Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = launchPoint.transform.forward *
                                                                    launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }
        }
    }
}