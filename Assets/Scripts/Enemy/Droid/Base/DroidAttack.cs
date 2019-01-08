using UnityEngine;

namespace FortBlast.Enemy.Droid.Base
{
    public abstract class DroidAttack : MonoBehaviour
    {
        [Header("Launch Stats")] public float launchSpeed;
        public float attackTime;
        public float playerBaseOffset;

        [Header("Effects")] public GameObject droidBullet;
        public GameObject launchEffect;

        [Header("Launch Points")] public Transform[] launchPoints;

        public abstract float Attack(Transform target, bool usePlayerOffset = false);

        public virtual void EndAttack()
        {
        }
    }
}