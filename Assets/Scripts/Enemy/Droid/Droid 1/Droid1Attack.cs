using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid1
{
    public class Droid1Attack : DroidAttack
    {
        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            for (var i = 0; i < launchPoints.Length; i++)
            {
                var targetPosition = target.position;
                var position = usePlayerOffset
                    ? targetPosition +
                      Vector3.up * playerBaseOffset
                    : targetPosition;

                var lookRotation = Quaternion.LookRotation(position -
                                                           launchPoints[i].position);
                launchPoints[i].transform.rotation = lookRotation;

                Instantiate(launchEffect, launchPoints[i].position, lookRotation);

                var bulletInstance = Instantiate(droidBullet,
                    launchPoints[i].position, Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = launchPoints[i].forward *
                                                                    launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }

            return attackTime;
        }
    }
}