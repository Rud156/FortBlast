using UnityEngine;
using UnityEngine.AI;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    public class NavMeshBaker : MonoBehaviour
    {
        #region Singleton

        public static NavMeshBaker instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public NavMeshSurface botSurface;
        public NavMeshSurface creatureSurface;

        public void ReBuildNavMesh()
        {
            botSurface.UpdateNavMesh(botSurface.navMeshData);
            creatureSurface.UpdateNavMesh(creatureSurface.navMeshData);
        }

        public void BuildInitialNavMesh()
        {
            botSurface.BuildNavMesh();
            creatureSurface.BuildNavMesh();
        }
    }
}