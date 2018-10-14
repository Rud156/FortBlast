using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Assets.Scripts.Common
{
    public class ObjectFollowPlayer : MonoBehaviour
    {
        public Vector3 offset;

        private Transform _player;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _player = GameObject.FindGameObjectWithTag(TagManager.Player)?.transform;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (_player == null)
                return;

            Vector3 followPosition = _player.position
                + Vector3.forward * offset.z
                + Vector3.right * offset.x
                + Vector3.up * offset.y;

            transform.position = followPosition;
        }
    }
}