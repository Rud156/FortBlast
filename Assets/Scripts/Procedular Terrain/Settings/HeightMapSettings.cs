using FortBlast.Structs;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    [CreateAssetMenu(fileName = "HeightMapSettings", menuName = "Terrain/HeightMapSettings")]
    public class HeightMapSettings : UpdatebleData
    {
        public NoiseSettings noiseSettings;

        [Header("Mesh Data")]
        public float heightMultiplier;
        public AnimationCurve heightCurve;

        [Header("Color Data")]
        public bool useFalloff;

        public float minHeight
        {
            get
            {
                return heightMultiplier * heightCurve.Evaluate(0);
            }
        }
        public float maxHeight
        {
            get
            {
                return heightMultiplier * heightCurve.Evaluate(1);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        protected override void OnValidate()
        {
            noiseSettings.ValidateValues();
            base.OnValidate();
        }

#endif
    }
}