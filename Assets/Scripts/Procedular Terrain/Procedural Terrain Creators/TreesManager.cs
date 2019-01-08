using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    public class TreesManager : MonoBehaviour
    {
        public List<GameObject> treeBases;
        public List<Tree> trees;

        private void GenerateTrees()
        {
            for (var i = 0; i < trees.Count; i++)
            for (var j = 0; j < trees[i].maxInstanceCount; j++)
            {
                var treeInstance =
                    Instantiate(trees[i].prefab, Vector3.zero, Quaternion.identity);
                treeInstance.transform.SetParent(transform);

                var treeBaseInstance =
                    Instantiate(
                        treeBases[Random.Range(0, 1000) % treeBases.Count],
                        Vector3.zero,
                        Quaternion.identity
                    );
                treeBaseInstance.transform.SetParent(treeInstance.transform);
                treeInstance.SetActive(false);

                trees[i].instantiatedTrees.Add(treeInstance);
            }
        }

        public GameObject RequestTree(float heightValue)
        {
            for (var i = 0; i < trees.Count; i++)
                if (trees[i].minHeightRange <= heightValue && trees[i].maxHeightRange >= heightValue)
                {
                    for (var j = 0; j < trees[i].maxInstanceCount; j++)
                        if (!trees[i].instantiatedTrees[j].activeInHierarchy)
                            return trees[i].instantiatedTrees[j];

                    return null;
                }

            return null;
        }

        [Serializable]
        public struct Tree
        {
            [Header("Prefab Stats")] public GameObject prefab;
            public int maxInstanceCount;

            [Header("Placement Range")] [Range(0, 1)]
            public float minHeightRange;

            [Range(0, 1)] public float maxHeightRange;

            [HideInInspector] public List<GameObject> instantiatedTrees;
        }

        #region Singleton

        public static TreesManager instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);

            GenerateTrees();
        }

        #endregion Singleton
    }
}