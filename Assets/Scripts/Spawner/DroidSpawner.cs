﻿using System.Collections.Generic;
using FortBlast.Enemy.Droid.Patrol;
using UnityEngine;

namespace FortBlast.Spawner
{
    public class DroidSpawner : MonoBehaviour
    {
        #region Singleton

        public static DroidSpawner instance;

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

        public GameObject[] droidPrefabs;

        public GameObject[] SpawnDroids(Vector3[] meshVertices, Transform parent)
        {
            GameObject[] droids = new GameObject[meshVertices.Length];

            for (int i = 0; i < meshVertices.Length; i++)
            {
                GameObject droidPrefab;
                int randomValue = Random.Range(0, 100);

                if (randomValue < 10)
                    droidPrefab = droidPrefabs[1];
                else
                    droidPrefab = droidPrefabs[0];

                droids[i] = Instantiate(droidPrefab, meshVertices[i],
                    droidPrefab.transform.rotation);
                droids[i].transform.SetParent(parent);
            }

            return droids;
        }
    }
}
