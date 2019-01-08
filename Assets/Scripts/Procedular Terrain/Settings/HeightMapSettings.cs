using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "HeightMapSettings", menuName = "Terrain/HeightMapSettings")]
    public class HeightMapSettings : UpdatebleData
    {
        [Header("Mesh Data")] public float heightMultiplier;
        public AnimationCurve heightCurve;

        [Header("Color Data")] public bool useFalloff;
        public NoiseSettings noiseSettings;

        public float minHeight => heightMultiplier * heightCurve.Evaluate(0);

        public float maxHeight => heightMultiplier * heightCurve.Evaluate(1);

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            noiseSettings.ValidateValues();
            base.OnValidate();
        }

#endif
    }
}