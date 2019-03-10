using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FortBlast.CustomDebug
{
    [RequireComponent(typeof(Button))]
    public class DebugSceneSwitcher : MonoBehaviour
    {
        public int sceneIndex;

        private Button _sceneSwitcherButton;

        private void Start()
        {
            _sceneSwitcherButton = GetComponent<Button>();
            _sceneSwitcherButton.onClick.AddListener(HandleSceneSwitch);
        }

        private void HandleSceneSwitch()
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}