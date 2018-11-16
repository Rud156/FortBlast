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

        public GameObject buildingPrefab;
        public GameObject towerPrefab;
        public int[] yRotation;

        public void CreateTowersAndBuildings(Vector3 meshVertex, Transform parent)
        {
            int randomNumber = Random.Range(0, 1000) % yRotation.Length;
            GameObject buildingInstance = Instantiate(buildingPrefab, meshVertex,
                buildingPrefab.transform.rotation);
            buildingInstance.transform.SetParent(parent);

            Transform towerPointsParent = buildingInstance.transform.GetChild(0);
            Vector3 position = towerPointsParent.GetChild(randomNumber).position;

            GameObject towerInstance = Instantiate(
                towerPrefab,
                position,
                Quaternion.Euler(0, yRotation[randomNumber], 0)
            );

			towerInstance.transform.SetParent(buildingInstance.transform);
        }
    }
}
