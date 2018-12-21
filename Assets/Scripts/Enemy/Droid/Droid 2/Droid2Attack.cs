using System.Collections;
using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid2
{
    public class Droid2Attack : DroidAttack
    {
        private const string AttackAnimationParam = "Attacking";

        private Transform _target;
        private bool _usePlayerOffset;
        public Animator droidAnimator;

        [Header("Attack Animations")] public float timeDiffSameAttack;

        public float totalAttackTimes;

        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            _target = target;
            _usePlayerOffset = usePlayerOffset;
            droidAnimator.SetBool(AttackAnimationParam, true);

            return attackTime;
        }

        public override void EndAttack()
        {
            droidAnimator.SetBool(AttackAnimationParam, false);
        }

        public void AttackTarget()
        {
            for (var i = 0; i < totalAttackTimes; i++)
                StartCoroutine(AttackDelayedStart(i * timeDiffSameAttack));
        }

        private IEnumerator AttackDelayedStart(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            for (var i = 0; i < launchPoints.Length; i++)
            {
                var position = _usePlayerOffset ? _target.position + Vector3.up * playerBaseOffset : _target.position;

                var lookRotation = Quaternion.LookRotation(position - launchPoints[i].position);
                launchPoints[i].transform.rotation = lookRotation;

                Instantiate(launchEffect, launchPoints[i].position, lookRotation);

                var bulletInstance = Instantiate(droidBullet,
                    launchPoints[i].transform.position, Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = launchPoints[i].transform.forward *
                                                                    launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }
        }
    }
}