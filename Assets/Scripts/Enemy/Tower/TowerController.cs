using System.Collections;
using System.Collections.Generic;
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
        public float attackAngleDifferenceTolerance;
        public float waitTimeBetweenAttack;
        public float attackDamage;

        [Header("Attack Stuff")]
        public Transform towerTop;
        public Transform laserShootingPoint;
        public GameObject laser;

        private Transform _player;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {

        }

        private bool CheckPlayerInsideFOV()
        {
            if (_player == null)
                return false;

            Vector3 modifiedPlayerPosition = new Vector3(_player.position.x, 0, _player.position.z);
            Vector3 modifiedTowerTopPosition =
                new Vector3(towerTop.position.x, 0, towerTop.position.z);

            Vector3 lookDirection = modifiedPlayerPosition - modifiedTowerTopPosition;
            float angleToPlayer = Vector3.Angle(lookDirection, towerTop.forward);
            float normalizedAngle = ExtensionFunctions.To360Angle(angleToPlayer);

            if (normalizedAngle <= maxLookAngle)
                return true;
            else
                return false;
        }

        private void LookAtPlayer()
        {
            if (_player == null)
                return;

			
        }
    }
}
