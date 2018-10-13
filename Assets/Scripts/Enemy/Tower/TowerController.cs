using System.Collections;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Tower
{
    public class TowerController : MonoBehaviour
    {
        [Header("Movement")]
        [Range(0, 360)]
        public int maxLookAngle;
        public float rotationSpeed;

        [Header("Attack")]
        public float attackAngleTolerance;
        public float waitTimeBetweenAttack;
        public float attackDamage;
        public float playerBaseOffset;

        [Header("Attack Stuff")]
        public Transform towerTop;
        public Transform laserShootingPoint;
        public GameObject laser;

        private Transform _player;
        private Renderer _laserRenderer;
        private bool _attackingPlayer;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;
            _attackingPlayer = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {

        }

        private float CheckPlayerInsideFOV()
        {
            if (_player == null)
                return -1;

            Vector3 modifiedPlayerPosition = new Vector3(_player.position.x, 0, _player.position.z);
            Vector3 modifiedTowerTopPosition =
                new Vector3(towerTop.position.x, 0, towerTop.position.z);

            Vector3 lookDirection = modifiedPlayerPosition - modifiedTowerTopPosition;
            float angleToPlayer = Vector3.Angle(lookDirection, towerTop.forward);
            float normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return normalizedAngle;
            else
                return -1;
        }

        private bool IsAngleWithinToleranceLevel(float normalizedAngle)
        {
            if (normalizedAngle < 0)
                return false;

            if (normalizedAngle <= attackAngleTolerance)
                return true;

            return false;
        }

        private void LookAtPlayer()
        {
            if (_player == null)
                return;

            Vector3 lookDirection = _player.position - towerTop.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                towerTop.rotation = Quaternion.Slerp(towerTop.rotation, lookRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private void TileLaserTexture()
        {
            float textureTilling = Mathf.Sin(Time.time) * 4f + 1;
            _laserRenderer.material.mainTextureScale = new Vector2(textureTilling, 1);
        }

        private IEnumerator AttackPlayer()
        {
            Quaternion rotation = Quaternion.LookRotation(_player.position - laserShootingPoint.position);
            GameObject laserInstance = Instantiate(laser, laserShootingPoint.position, rotation);

            LineRenderer lineRenderer = laserInstance.GetComponentInChildren<LineRenderer>();
            lineRenderer.SetPosition(0, laserShootingPoint.position);
            lineRenderer.SetPosition(1, _player.position + Vector3.up * playerBaseOffset);

            _laserRenderer = lineRenderer.GetComponent<Renderer>();
            _attackingPlayer = true;

            yield return new WaitForSeconds(waitTimeBetweenAttack);
            _attackingPlayer = false;
        }
    }
}
