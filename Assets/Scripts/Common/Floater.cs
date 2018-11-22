using System.Collections;
using System.Collections.Generic;
using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Common
{
    public class Floater : MonoBehaviour
    {
        public float degreesPerSecond = 15.0f;
        public float amplitude = 0.5f;
        public float frequency = 1f;
        public Direction direction;

        // Position Storage Variables
        private Vector3 _posOffset = new Vector3();
        private Vector3 _tempPos = new Vector3();

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _posOffset = transform.localPosition;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
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
