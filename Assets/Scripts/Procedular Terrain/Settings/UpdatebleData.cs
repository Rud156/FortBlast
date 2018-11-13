using UnityEngine;
using UnityEditor;

namespace FortBlast.ProceduralTerrain.Settings
{
    public class UpdatebleData : ScriptableObject
    {
        public event System.Action OnValuesUpdated;
        public bool autoUpdate;

#if UNITY_EDITOR

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        protected virtual void OnValidate()
        {
            if (autoUpdate)
                EditorApplication.update += NotifyOnValuesUpdated;
        }

        public void NotifyOnValuesUpdated()
        {
            EditorApplication.update -= NotifyOnValuesUpdated;
            OnValuesUpdated?.Invoke();
        }

#endif
    }
}
