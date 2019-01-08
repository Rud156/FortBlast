using System;
using System.Linq;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TextureData", menuName = "Terrain/Texture")]
    public class TextureData : UpdatebleData
    {
        public Layer[] layers;

        private const int TextureSize = 512;
        private const TextureFormat _textureFormat = TextureFormat.RGB565;
        private float _savedMaxHeight;
        private float _savedMinHeight;

        private static readonly int BaseBlends = Shader.PropertyToID("baseBlends");
        private static readonly int BaseStartHeights = Shader.PropertyToID("baseStartHeights");
        private static readonly int BaseColors = Shader.PropertyToID("baseColors");
        private static readonly int LayerCount = Shader.PropertyToID("layerCount");
        private static readonly int BaseColorStrengths = Shader.PropertyToID("baseColorStrengths");
        private static readonly int BaseTextureScales = Shader.PropertyToID("baseTextureScales");
        private static readonly int BaseTextures = Shader.PropertyToID("baseTextures");
        private static readonly int MinHeight = Shader.PropertyToID("minHeight");
        private static readonly int MaxHeight = Shader.PropertyToID("maxHeight");

        public void ApplyToMaterial(Material material)
        {
            material.SetInt(LayerCount, layers.Length);
            material.SetColorArray(BaseColors, layers.Select(_ => _.tint).ToArray());
            material.SetFloatArray(BaseStartHeights, layers.Select(_ => _.startHeight).ToArray());
            material.SetFloatArray(BaseBlends, layers.Select(_ => _.blendStrength).ToArray());
            material.SetFloatArray(BaseColorStrengths, layers.Select(_ => _.tintStrength).ToArray());
            material.SetFloatArray(BaseTextureScales, layers.Select(_ => _.textureScale).ToArray());

            var textureArray = GenerateTextureArray(layers.Select(_ => _.texture).ToArray());
            material.SetTexture(BaseTextures, textureArray);

            UpdateMeshHeights(material, _savedMinHeight, _savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            _savedMinHeight = minHeight;
            _savedMaxHeight = maxHeight;

            material.SetFloat(MinHeight, minHeight);
            material.SetFloat(MaxHeight, maxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            var textureArray = new Texture2DArray(TextureSize, TextureSize,
                textures.Length, _textureFormat, true);

            for (var i = 0; i < textures.Length; i++)
                textureArray.SetPixels(textures[i].GetPixels(), i);

            textureArray.Apply();
            return textureArray;
        }

        [Serializable]
        public class Layer
        {
            [Range(0, 1)] public float blendStrength;

            [Range(0, 1)] public float startHeight;

            public Texture2D texture;
            public float textureScale;
            public Color tint;

            [Range(0, 1)] public float tintStrength;
        }
    }
}