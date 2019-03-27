using FortBlast.BaseClasses;
using FortBlast.ProceduralTerrain.ProceduralTerrainCreators;
using FortBlast.UI;

namespace FortBlast.Scenes.MainScene
{
    public class MainSceneGameManager : GameManager
    {
        protected override void Start()
        {
            base.Start();
            TerrainGenerator.instance.terrainGenerationComplete += StartFadingIn;
        }

        #region PlayerBase

        private void StartFadingIn()
        {
            Fader.instance.StartFadeIn();
            TerrainGenerator.instance.terrainGenerationComplete -= StartFadingIn;
        }

        #endregion PlayerBase

        #region Singleton

        public static MainSceneGameManager instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
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