using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAttack : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject droidLaser;
        public GameObject laserPoint;

        [Header("Launch Points")]
        public Transform launchPoint_1;
        public Transform launchPoint_2;

        [Header("Launch Stats")]
        public float launchSpeed;

        public void AttackPlayer(Transform player)
        {
            GameObject laserPoint_1 = Instantiate(laserPoint, launchPoint_1.position, Quaternion.identity);
            GameObject laserPoint_2 = Instantiate(laserPoint, launchPoint_2.position, Quaternion.identity);
            laserPoint_1.transform.SetParent(launchPoint_1);
            laserPoint_2.transform.SetParent(launchPoint_2);

            Vector3 lookDirection_1 = player.position - launchPoint_1.position;
            Vector3 lookDirection_2 = player.position - launchPoint_2.position;

            Quaternion rotation_1 = Quaternion.LookRotation(lookDirection_1);
            Quaternion rotation_2 = Quaternion.LookRotation(lookDirection_2);

            GameObject droidLaser_1 = Instantiate(droidLaser, launchPoint_1.position, rotation_1);
            GameObject droidLaser_2 = Instantiate(droidLaser, launchPoint_2.position, rotation_2);

            Rigidbody laserRb_1 = droidLaser_1.GetComponent<Rigidbody>();
            laserRb_1.velocity = droidLaser_1.transform.forward * launchSpeed;
            Rigidbody laserRb_2 = droidLaser_2.GetComponent<Rigidbody>();
            laserRb_2.velocity = droidLaser_1.transform.forward * launchSpeed;
        }
    }
}