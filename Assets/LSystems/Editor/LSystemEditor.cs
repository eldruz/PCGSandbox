using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace LSystems
{
    [CustomPropertyDrawer(typeof(Production))]
    public class ProductionProperty : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, GUIContent.none);
            contentPosition.width *= 0.5f;
            EditorGUI.indentLevel = 0;

            EditorGUIUtility.labelWidth = 32f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("predecessor"), new GUIContent("Pred"));

            contentPosition.x += contentPosition.width;
            EditorGUIUtility.labelWidth = 32f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("successor"), new GUIContent("Succ"));

            EditorGUI.EndProperty();
        }
    }

    [CustomEditor(typeof(LSystemConfig))]
    public class LSystemEditor : Editor
    {
        private ReorderableList deterministicList;
        private ReorderableList stochasticList;

        public void OnEnable()
        {
            deterministicList = new ReorderableList(serializedObject, serializedObject.FindProperty("deterministicProductions"), true, true, true, true);
            deterministicList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = deterministicList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("predecessor"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, 160, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("successor"), GUIContent.none);
            };

            deterministicList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Deterministic Productions");
            };

            deterministicList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("successor").stringValue = string.Empty;
            };

            stochasticList = new ReorderableList(serializedObject, serializedObject.FindProperty("stochasticProductions"), true, true, true, true);
            stochasticList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = stochasticList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("predecessor"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, 160, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("successor"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("probability"), GUIContent.none);
            };

            stochasticList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Sochastic Productions");
            };

            stochasticList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("successor").stringValue = string.Empty;
                element.FindPropertyRelative("probability").floatValue = 0f;
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            deterministicList.DoLayoutList();
            stochasticList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

    }
}
