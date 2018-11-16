using System.Collections;
using System.Collections.Generic;
using FortBlast.Player.AffecterActions;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAttack : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject droidLaser;

        [Header("Launch Points")]
        public Transform[] launchPoints;

        [Header("Launch Stats")]
        public float playerBaseOffset;
        public float attackDamage;

        private Renderer[] _laserRenderers;
        private GameObject[] _droidLaserInstance;
        private bool _laserCreated;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _laserCreated = false;
            _laserRenderers = new Renderer[launchPoints.Length];
            _droidLaserInstance = new GameObject[launchPoints.Length];
        }

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

            foreach (var laserRenderer in _laserRenderers)
                laserRenderer.material.mainTextureScale = new Vector2(textureTilling, 1);
        }

        public IEnumerator AttackPlayer(Transform player)
        {
            for (int i = 0; i < launchPoints.Length; i++)
            {
                Quaternion rotation = Quaternion.LookRotation(player.position - launchPoints[i].position);
                _droidLaserInstance[i] = Instantiate(droidLaser, launchPoints[i].position, rotation);

                LineRenderer line = _droidLaserInstance[i].GetComponentInChildren<LineRenderer>();
                line.SetPosition(0, launchPoints[i].position);
                line.SetPosition(1, player.position + Vector3.up * playerBaseOffset);
                _laserRenderers[i] = line.GetComponent<Renderer>();
            }

            _laserCreated = true;
            player.GetComponent<PlayerShooterAbsorbDamage>().DamagePlayerAndDecreaseHealth(attackDamage * 2);

            yield return new WaitForSeconds(0.5f);

            _laserCreated = false;
            for (int i = 0; i < _laserRenderers.Length; i++)
                Destroy(_droidLaserInstance[i]);
        }
    }
}