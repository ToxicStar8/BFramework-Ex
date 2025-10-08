using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    [CustomEditor(typeof(ButtonEx), true)]
    public class ButtonExEditor : ButtonEditor
    {
        private SerializedProperty ClickScaleProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            ClickScaleProperty = serializedObject.FindProperty("ClickScale");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("点击缩放", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ClickScaleProperty, new GUIContent("缩放比例"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}