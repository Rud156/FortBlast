using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class BuildingAndTowerSpawner : MonoBehaviour
    {
        #region Singleton

        public static BuildingAndTowerSpawner instance;

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

        public void CreateTowersAndBuildings(Vector3 meshVertex, Transform parent)
        {
			
        }
    }
}
