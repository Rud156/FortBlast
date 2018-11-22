using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Common
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destoryAfterTime = 5;

        // Use this for initialization
        void Start() => Destroy(gameObject, destoryAfterTime);
    }
}