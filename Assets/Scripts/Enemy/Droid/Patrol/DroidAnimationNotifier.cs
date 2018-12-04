using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAnimationNotifier : MonoBehaviour
    {
        private DroidDefense _droidDefense;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _droidDefense = GetComponentInParent<DroidDefense>();

        // Hit Animation Complete
        public void HitAnimationComplete() => _droidDefense.HitAnimationComplete();
    }
}