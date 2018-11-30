using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "TextureData", menuName = "Terrain/Texture")]
    public class TextureData : UpdatebleData
    {
        public Layer[] layers;

        private float _savedMinHeight;
        private float _savedMaxHeight;

        private const int TextureSize = 512;
        private const TextureFormat _textureFormat = TextureFormat.RGB565;

        public void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", layers.Length);
            material.SetColorArray("baseColors", layers.Select(_ => _.tint).ToArray());
            material.SetFloatArray("baseStartHeights", layers.Select(_ => _.startHeight).ToArray());
            material.SetFloatArray("baseBlends", layers.Select(_ => _.blendStrength).ToArray());
            material.SetFloatArray("baseColorStrengths", layers.Select(_ => _.tintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", layers.Select(_ => _.textureScale).ToArray());

            Texture2DArray textureArray = GenerateTextureArray(layers.Select(_ => _.texture).ToArray());
            material.SetTexture("baseTextures", textureArray);

            UpdateMeshHeights(material, _savedMinHeight, _savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            _savedMinHeight = minHeight;
            _savedMaxHeight = maxHeight;

            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray textureArray = new Texture2DArray(TextureSize, TextureSize,
                textures.Length, _textureFormat, true);

            for (int i = 0; i < textures.Length; i++)
                textureArray.SetPixels(textures[i].GetPixels(), i);

            textureArray.Apply();
            return textureArray;
        }

        [System.Serializable]
        public class Layer
        {
            public Texture2D texture;
            public Color tint;
            [Range(0, 1)]
            public float tintStrength;
            [Range(0, 1)]
            public float startHeight;
            [Range(0, 1)]
            public float blendStrength;
            public float textureScale;

        }
    }
}
