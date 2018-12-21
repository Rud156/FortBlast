using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Collider))]
    public class ReflectorTriggerEventCreator : MonoBehaviour
    {
        public delegate void OnBulletCollided(GameObject bullet);

        public OnBulletCollided onBulletCollided;

        /// <summary>
        ///     OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Bullet))
                onBulletCollided?.Invoke(other.gameObject);
        }
    }
}