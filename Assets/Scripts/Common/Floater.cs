using FortBlast.Enums;
using UnityEngine;

namespace FortBlast.Common
{
    public class Floater : MonoBehaviour
    {
        public float amplitude = 0.5f;
        public float degreesPerSecond = 15.0f;
        public Direction direction;
        public float frequency = 1f;
        
        // Position Storage Variables
        private Vector3 _posOffset;
        private Vector3 _tempPos;

        private void Start() => _posOffset = transform.localPosition;

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