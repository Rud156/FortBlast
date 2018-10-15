using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class CheckpointSpawner : MonoBehaviour
    {
        public List<Transform> checkPoints;

        [Header("Debug")]
        public bool spawnOnStart;

        private Transform _currentCheckPoint;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _currentCheckPoint = null;

            if (spawnOnStart)
                SpawnNextCheckPoint();
        }

        public void SpawnNextCheckPoint()
        {
            int randomValue;
            Transform prevCheckPoint = _currentCheckPoint;

            while (prevCheckPoint != _currentCheckPoint)
            {
                randomValue = Random.Range(0, 1000);
                _currentCheckPoint = checkPoints[randomValue % checkPoints.Count];
            }
        }
    }
}
