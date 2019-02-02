using FortBlast.Enums;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Missions
{
    public class MissionManager : MonoBehaviour
    {
        [Header("UI Display")] public Text totalDisplay;
        public Text timerTextDisplay;

        [Header("Level End Indicator")] public GameObject machinePartsBorder;

        [Header("Level End UI")] public GameObject levelEndUi;
        public Text totalSpottedText;
        public Text totalGameTimeText;
        public Text totalMachinePartsCollected;
        public Button continueButton;

        [Header("Level Data")] public LevelSettings levelSettings;

        private int _totalMachinePartToBeCollected;
        private int _totalSpottedTimes;
        private bool _gameEndIndicated;

        private float _currentLevelTime;

        private void Start()
        {
            TerrainGenerator.instance.terrainGenerationComplete += Init;
            continueButton.onClick.AddListener(HandleContinueButtonClicked);
        }

        private void Update()
        {
            UpdateTimerDisplay();
            CheckForGameEnd();
        }

        public void IncrementSpottedTimes() => _totalSpottedTimes += 1;

        public void DisplayGameEndUI()
        {
            // TODO: Plug Back In When Loading Scene Exists
            // LevelSceneManager.instance.AsyncLoadScene();
            levelEndUi.SetActive(true);

            totalSpottedText.text = $"{_totalSpottedTimes}";

            var machinePartsCollected = ResourceManager.instance.CountResource(ItemID.MachinePart);
            var machinePartsDisplayText = $"{machinePartsCollected} / {_totalMachinePartToBeCollected}";
            totalMachinePartsCollected.text = machinePartsDisplayText;

            var totalLevelTime = Mathf.FloorToInt(_currentLevelTime);
            totalGameTimeText.text = $"{totalLevelTime}";
        }

        private void Init()
        {
            var totalMachineParts = levelSettings.totalMachineParts;
            _currentLevelTime = 0;

            _totalMachinePartToBeCollected = totalMachineParts;
            totalDisplay.text = $"/ {_totalMachinePartToBeCollected}";

            TerrainGenerator.instance.terrainGenerationComplete -= Init;
        }

        private void UpdateTimerDisplay()
        {
            _currentLevelTime += Time.deltaTime;
            timerTextDisplay.text = $"{_currentLevelTime}";
        }

        private void CheckForGameEnd()
        {
            if (_gameEndIndicated)
                return;

            var collectedMachineParts = ResourceManager.instance.CountResource(ItemID.MachinePart);
            if (collectedMachineParts < _totalMachinePartToBeCollected)
                return;

            _gameEndIndicated = true;
            machinePartsBorder.SetActive(true);
        }

        private void HandleContinueButtonClicked()
        {
            // TODO: Use Async Scene Loading to Switch to Next Scene
            // LevelSceneManager.instance.ActivateBackgroundScene();
        }

        #region Singleton

        public static MissionManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}