using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidAnimationNotifier : MonoBehaviour
    {
        public DroidDefense droidDefense;

        // Hit Animation Complete
        public void HitAnimationComplete() => droidDefense.HitAnimationComplete();
    }
}