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
    }
}