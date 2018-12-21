using UnityEngine;

namespace FortBlast.Enemy.Droid.Base
{
    public abstract class DroidAttack : MonoBehaviour
    {
        [Header("Launch Stats")] public float attackTime;

        [Header("Effects")] public GameObject droidBullet;

        public GameObject launchEffect;

        [Header("Launch Points")] public Transform[] launchPoints;

        public float launchSpeed;
        public float playerBaseOffset;

        public abstract float Attack(Transform target, bool usePlayerOffset = false);

        public virtual void EndAttack()
        {
        }
    }
}