using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FortBlast.Scenes.General
{   
    public class LevelSceneManager : MonoBehaviour
    {
        private AsyncOperation _asyncOperation;
        
        public void AsyncLoadScene(int sceneIndex = 1) => StartCoroutine(AsyncLoadSceneEnumerator(sceneIndex));

        public void ActivateBackgroundScene()
        {
            if(_asyncOperation == null)
                return;

            _asyncOperation.allowSceneActivation = true;
        }

        private IEnumerator AsyncLoadSceneEnumerator(int sceneIndex)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            asyncOperation.allowSceneActivation = false;
            _asyncOperation = asyncOperation;
            
            while (!asyncOperation.isDone)
                yield return null;
        }
        
        #region Singleton

        public static LevelSceneManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            
            if(instance != this)
                Destroy(gameObject);
        }

        #endregion
    }
}