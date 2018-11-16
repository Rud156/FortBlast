using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class NavMeshBaker : MonoBehaviour
    {

        #region Singleton

        public static NavMeshBaker instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private NavMeshSurface _navMeshSurface;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _navMeshSurface = GetComponent<NavMeshSurface>();

        public void ReBuildNavMesh() =>
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);

        public void BuildInitialNavMesh() =>
            _navMeshSurface.BuildNavMesh();
    }
}
