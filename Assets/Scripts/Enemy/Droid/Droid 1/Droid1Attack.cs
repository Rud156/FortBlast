using System.Collections;
using System.Collections.Generic;
using FortBlast.Enemy.Droid.Base;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Droid1
{
    public class Droid1Attack : DroidAttack
    {
        public override float Attack(Transform target, bool usePlayerOffset = false)
        {
            for (int i = 0; i < base.launchPoints.Length; i++)
            {
                var targetPosition = target.position;
                Vector3 position = usePlayerOffset ? targetPosition +
                    Vector3.up * base.playerBaseOffset : targetPosition;

                Quaternion lookRotation = Quaternion.LookRotation(position -
                    base.launchPoints[i].position);
                base.launchPoints[i].transform.rotation = lookRotation;

                Instantiate(base.launchEffect, base.launchPoints[i].position, lookRotation);

                GameObject bulletInstance = Instantiate(base.droidBullet,
                    base.launchPoints[i].position, Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = base.launchPoints[i].forward *
                    base.launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }

            return base.attackTime;
        }
    }
}