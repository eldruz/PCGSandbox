using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LSystems
{
    [CustomEditor(typeof(LSystemMesh))]
    public class LSystemMeshEditor : Editor
    {
        private LSystemMesh meshCreator;

        private void OnEnable()
        {
            meshCreator = target as LSystemMesh;
            Undo.undoRedoPerformed += RefreshMesh;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= RefreshMesh;
        }

        private void RefreshMesh()
        {
            if (Application.isPlaying)
            {
                meshCreator.Refresh();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                RefreshMesh();
            }
        }
    }
}


