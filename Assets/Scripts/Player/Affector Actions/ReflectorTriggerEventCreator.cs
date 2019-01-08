using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Player.AffecterActions
{
    [RequireComponent(typeof(Collider))]
    public class ReflectorTriggerEventCreator : MonoBehaviour
    {
        public delegate void OnBulletCollided(GameObject bullet);

        public OnBulletCollided onBulletCollided;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Bullet))
                onBulletCollided?.Invoke(other.gameObject);
        }
    }
}