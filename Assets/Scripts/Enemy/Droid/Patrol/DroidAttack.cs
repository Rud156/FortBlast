using System.Collections;
using FortBlast.Player.Affecter_Actions;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAttack : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject droidLaser;

        [Header("Launch Points")]
        public Transform launchPoint_1;
        public Transform launchPoint_2;

        [Header("Launch Stats")]
        public float playerBaseOffset;
        public float attackDamage;

        private Renderer _laserRenderer_1;
        private Renderer _laserRenderer_2;
        private bool _laserCreated;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _laserCreated = false;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_laserCreated)
                TileLaserTexture();
        }

        private void TileLaserTexture()
        {
            float textureTilling = Mathf.Cos(Time.time) * 4f + 1;

            _laserRenderer_1.material.mainTextureScale = new Vector2(textureTilling, 1);
            _laserRenderer_2.material.mainTextureScale = new Vector2(textureTilling, 1);
        }

        public IEnumerator AttackPlayer(Transform player)
        {
            Quaternion rotation_1 = Quaternion.LookRotation(player.position - launchPoint_1.position);
            Quaternion rotation_2 = Quaternion.LookRotation(player.position - launchPoint_2.position);

            GameObject droidLaserInstance_1 = Instantiate(droidLaser, launchPoint_1.position, rotation_1);
            GameObject droidLaserInstance_2 = Instantiate(droidLaser, launchPoint_1.position, rotation_2);

            LineRenderer line_1 = droidLaserInstance_1.GetComponentInChildren<LineRenderer>();
            LineRenderer line_2 = droidLaserInstance_2.GetComponentInChildren<LineRenderer>();



            line_1.SetPosition(0, launchPoint_1.position);
            line_1.SetPosition(1, player.position + Vector3.up * playerBaseOffset);
            line_2.SetPosition(0, launchPoint_2.position);
            line_2.SetPosition(1, player.position + Vector3.up * playerBaseOffset);

            _laserCreated = true;
            _laserRenderer_1 = line_1.GetComponent<Renderer>();
            _laserRenderer_2 = line_2.GetComponent<Renderer>();

            player.GetComponent<PlayerShooterAbsorbDamage>().DamagePlayerAndDecreaseHealth(attackDamage * 2);

            yield return new WaitForSeconds(0.5f);

            _laserCreated = false;
            Destroy(droidLaserInstance_1);
            Destroy(droidLaserInstance_2);
        }
    }
}