using System.Collections;
using System.Collections.Generic;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    [RequireComponent(typeof(Collider))]
    public class DroidDefense : MonoBehaviour
    {
        public GameObject shieldEffect;
        [Range(0, 1)]
        public float selectionProbability;

        private bool _shieldActivated;
        private GameObject _shieldSystem;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_shieldSystem == null)
                _shieldActivated = false;

            if (Input.GetKeyDown(KeyCode.F))
                Debug.Break();
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagManager.Bullet))
                return;

            float probability = Random.value;
            if (probability <= selectionProbability && !_shieldActivated)
            {
                _shieldActivated = true;

                _shieldSystem = Instantiate(shieldEffect, transform.position, Quaternion.identity);
                _shieldSystem.transform.SetParent(transform);
                _shieldSystem.transform.localScale = Vector3.one;
            }

            if (_shieldActivated)
                Destroy(other.gameObject);
        }
    }
}