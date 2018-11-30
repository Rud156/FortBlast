using System.Collections;
using System.Collections.Generic;
using FortBlast.Player.AffecterActions;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Base
{
    public abstract class DroidAttack : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject droidBullet;
        public GameObject launchEffect;

        [Header("Launch Points")]
        public Transform[] launchPoints;

        [Header("Launch Stats")]
        public float attackTime;
        public float playerBaseOffset;
        public float launchSpeed;

        public abstract float Attack(Transform target, bool usePlayerOffset = false);

        public virtual void EndAttack() { }
    }
}