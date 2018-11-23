using System.Collections;
using System.Collections.Generic;
using FortBlast.Player.AffecterActions;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAttack : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject droidBullet;
        public GameObject bulletLaunchEffect;

        [Header("Launch Points")]
        public Transform[] launchPoints;

        [Header("Launch Stats")]
        public float playerBaseOffset;
        public float launchSpeed;

        public IEnumerator Attack(Transform target, bool usePlayerOffset = false)
        {
            if (target == null)
                yield break;

            for (int i = 0; i < launchPoints.Length; i++)
            {
                Vector3 position = usePlayerOffset ? target.position + Vector3.up * playerBaseOffset :
                    target.position;

                Quaternion lookRotation = Quaternion.LookRotation(position - launchPoints[i].position);
                launchPoints[i].transform.rotation = lookRotation;

                GameObject bulletLaunchEffectInstance =
                    Instantiate(bulletLaunchEffect, launchPoints[i].transform.position,
                    Quaternion.identity);
                bulletLaunchEffectInstance.transform.SetParent(launchPoints[i].transform);

                GameObject bulletInstance = Instantiate(droidBullet, launchPoints[i].transform.position,
                    Quaternion.identity);
                bulletInstance.transform.rotation = lookRotation;
                bulletInstance.GetComponent<Rigidbody>().velocity = launchPoints[i].transform.forward *
                    launchSpeed;
                bulletInstance.layer = 10; // Put it in the Initial Bullet Layer
            }


            yield return new WaitForSeconds(0.5f);
        }
    }
}