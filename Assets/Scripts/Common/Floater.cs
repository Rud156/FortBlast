using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Common
{
    public class Floater : MonoBehaviour
    {
        // Position Storage Variables
        private Vector3 _posOffset;
        private Vector3 _tempPos;
        public float amplitude = 0.5f;
        public float degreesPerSecond = 15.0f;
        public Direction direction;
        public float frequency = 1f;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _posOffset = transform.localPosition;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Float up/down with a Sin()
            _tempPos = _posOffset;
            switch (direction)
            {
                case Direction.xAxis:
                    _tempPos.x += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
                    break;

                case Direction.yAxis:
                    _tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
                    break;

                case Direction.zAxis:
                    _tempPos.z += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
                    break;
            }


            transform.localPosition = _tempPos;
        }
    }
}