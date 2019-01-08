using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid1
{
    public class Droid1Attack : DroidAttack
    {
        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            foreach (var launchPoint in launchPoints)
            {
                var targetPosition = target.position;
                var position = usePlayerOffset
                    ? targetPosition +
                      Vector3.up * playerBaseOffset
                    : targetPosition;

                var lookRotation = Quaternion.LookRotation(position -
                                                           launchPoint.position);
                launchPoint.transform.rotation = lookRotation;

                Instantiate(launchEffect, launchPoint.position, lookRotation);

                var bulletInstance = Instantiate(droidBullet,
                    launchPoint.position, Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = launchPoint.forward *
                                                                    launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }

            return attackTime;
        }
    }
}