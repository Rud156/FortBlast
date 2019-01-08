using FortBlast.ProceduralTerrain.DataHolders;
using FortBlast.ProceduralTerrain.Generators;
using FortBlast.ProceduralTerrain.Settings;
using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.ProceduralTerrainCreators
{
    public class MapPreview : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap,
            Mesh,
            FalloffMap
        }

        [Header("Debug")] public bool autoUpdate;
        public DrawMode drawMode;

        [Header("Map Data")] [Range(0, MeshSettings.numSupportedLODs - 1)]
        public int editorPreviewLOD;

        public HeightMapSettings heightMapSettings;
        public MeshSettings meshSettings;
        public TextureData textureData;

        [Header("Mesh Components")] public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public Material terrainMaterial;

        public void DrawMapInEditor()
        {
            textureData.ApplyToMaterial(terrainMaterial);
            textureData.UpdateMeshHeights(terrainMaterial,
                heightMapSettings.minHeight, heightMapSettings.maxHeight);

            var heightMap = HeightMapGenerator.GenerateHeightMap(
                meshSettings.numVerticesPerLine,
                meshSettings.numVerticesPerLine,
                heightMapSettings,
                Vector2.zero
            );
            if (drawMode == DrawMode.NoiseMap)
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            else if (drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerateTerrainMesh(
                        heightMap.values,
                        editorPreviewLOD,
                        meshSettings
                    )
                );
            }
            else
            {
                var values = FalloffGenerator.GenerateFalloffMap(meshSettings.numVerticesPerLine);
                DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(values, 0, 1)));
            }
        }

        private void OnValidate()
        {
            if (meshSettings != null)
            {
                meshSettings.OnValuesUpdated -= OnValuesUpdated;
                meshSettings.OnValuesUpdated += OnValuesUpdated;
            }

            if (heightMapSettings != null)
            {
                heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }

            if (textureData != null)
            {
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }

        private void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            textureRenderer.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }

        private void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();

            textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
                DrawMapInEditor();
        }

        private void OnTextureValuesUpdated()
        {
            textureData.ApplyToMaterial(terrainMaterial);
        }

        #region Singleton

        private static MapPreview _instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}