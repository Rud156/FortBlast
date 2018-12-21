using System;
using UnityEditor;
using UnityEngine;

namespace FortBlast.ProceduralTerrain.Settings
{
    public class UpdatebleData : ScriptableObject
    {
        public bool autoUpdate;
        public event Action OnValuesUpdated;

#if UNITY_EDITOR

        /// <summary>
        ///     Called when the script is loaded or a value is changed in the
        ///     inspector (Called in the editor only).
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