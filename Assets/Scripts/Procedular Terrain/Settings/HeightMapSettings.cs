using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "HeightMapSettings", menuName = "Terrain/HeightMapSettings")]
    public class HeightMapSettings : UpdatebleData
    {
        public AnimationCurve heightCurve;

        [Header("Mesh Data")] public float heightMultiplier;

        public NoiseSettings noiseSettings;

        [Header("Color Data")] public bool useFalloff;

        public float minHeight => heightMultiplier * heightCurve.Evaluate(0);

        public float maxHeight => heightMultiplier * heightCurve.Evaluate(1);

#if UNITY_EDITOR

        /// <summary>
        ///     Called when the script is loaded or a value is changed in the
        ///     inspector (Called in the editor only).
        /// </summary>
        protected override void OnValidate()
        {
            noiseSettings.ValidateValues();
            base.OnValidate();
        }

#endif
    }
}