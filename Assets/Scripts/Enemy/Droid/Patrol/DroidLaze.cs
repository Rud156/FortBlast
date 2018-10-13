using UnityEngine;

namespace FortBlast.Enemy.Droid.Patrol
{
    public class DroidLaze : MonoBehaviour
    {
        public float waitForAnimationTime = 10f;
        public Animator droidAnimator;

        private const string LazeTrigger = "DroidLaze";

        public float LazeAroundSpot()
        {
            droidAnimator.SetBool(LazeTrigger, true);
            return waitForAnimationTime;
        }

        public void StopLazingAbout() => droidAnimator.SetBool(LazeTrigger, false);
    }
}