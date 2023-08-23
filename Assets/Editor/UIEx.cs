/*********************************************
 * BFramework
 * UI扩展
 * 创建时间：2022/12/25 20:40:23
 *********************************************/
using System.IO;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// UI扩展组件
    /// </summary>
    public class UIEx : Editor
    {
        #region Create UI Component
        [MenuItem("GameObject/UI/ButtonEx", false, 0)]
        public static void CreateButtonEx()
        {
            var trans = Selection.activeTransform;
            EditorApplication.ExecuteMenuItem("GameObject/UI/Legacy/Button");
            var btn = Selection.activeTransform.GetComponent<Button>();
            var go = btn.gameObject;
            DestroyImmediate(btn);
            btn = go.AddComponent<ButtonEx>();

            var img = Selection.activeTransform.GetComponent<Image>();
            go.transform.SetParent(trans);
            DestroyImmediate(img);
            img = go.AddComponent<ImageEx>();
            btn.targetGraphic = img;

            var txt = ReplaceTextEx(go.GetComponentInChildren<Text>());
            txt.text = "Button";
            txt.alignment = TextAnchor.MiddleCenter;
        }

        [MenuItem("GameObject/UI/ImageEx", false, 1)]
        public static void CreateImageEx()
        {
            var trans = Selection.activeTransform;
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            var img = Selection.activeTransform.GetComponent<Image>();
            var go = img.gameObject;
            DestroyImmediate(img);
            img = go.AddComponent<ImageEx>();
            img.raycastTarget = false;
            go.transform.SetParent(trans);
        }

        [MenuItem("GameObject/UI/TextEx", false, 2)]
        public static void CreateTextEx()
        {
            var trans = Selection.activeTransform;
            EditorApplication.ExecuteMenuItem("GameObject/UI/Legacy/Text");
            var txt = ReplaceTextEx(Selection.activeTransform.GetComponent<Text>());
            txt.name = "Txt_";
            var go = txt.gameObject;
            go.transform.SetParent(trans);
        }
        #endregion

        #region Replace Text
        /// <summary>
        /// Text替换成TextEx
        /// </summary>
        private static Text ReplaceTextEx(Text txt)
        {
            var go = txt.gameObject;
            DestroyImmediate(txt);
            txt = go.AddComponent<TextEx>();
            var fontDirPath = Application.dataPath + "/GameData/Art/Font/";
            var fileArr = new DirectoryInfo(fontDirPath).GetFiles("*", SearchOption.AllDirectories);
            if (fileArr.Length != 0)
            {
                var fontPath = fileArr[0].FullName;
                fontPath = "Assets" + fontPath.Split("Assets")[1];
                txt.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            }
            txt.supportRichText = false;
            txt.raycastTarget = false;
            txt.text = "TextEx...";
            txt.color = "#323232".ToColor32();
            return txt;
        }

        [MenuItem("CONTEXT/Text/替换为TextEx")]
        public static void TextReplaceTextEx()
        {
            ReplaceTextEx(Selection.activeTransform.GetComponent<Text>());
        }

        [MenuItem("CONTEXT/TextEx/替换为ImageEx")]
        public static void TextExReplaceImageEx()
        {
            var txt = Selection.activeTransform.GetComponent<TextEx>();
            var go = txt.gameObject;
            DestroyImmediate(txt);
            var img = go.AddComponent<ImageEx>();
            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.targetGraphic = img;
            }
            img.raycastTarget = btn != null;
        }
        #endregion

        #region Replace Image
        [MenuItem("CONTEXT/Image/替换为ImageEx")]
        public static void ImageReplaceImageEx()
        {
            var img = Selection.activeTransform.GetComponent<Image>();
            var go = img.gameObject;
            DestroyImmediate(img);
            img = go.AddComponent<ImageEx>();
        }

        [MenuItem("CONTEXT/Image/替换为RawImage")]
        public static void ImageReplaceRawImage()
        {
            var img = Selection.activeTransform.GetComponent<Image>();
            var go = img.gameObject;
            DestroyImmediate(img);
            var rawImg = go.AddComponent<RawImage>();
            rawImg.raycastTarget = false;
        }

        [MenuItem("CONTEXT/Image/替换为TextEx")]
        public static void ImageReplaceTextEx()
        {
            var img = Selection.activeTransform.GetComponent<Image>();
            var go = img.gameObject;
            DestroyImmediate(img);
            var txt = go.AddComponent<TextEx>();
            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.targetGraphic = txt;
            }
            txt.supportRichText = false;
            txt.raycastTarget = btn != null;
            txt.font = null;
        }
        #endregion

        [CustomEditor(typeof(ImageEx), true)]
        public class ImageExEditor : ImageEditor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                ImageEx img = (ImageEx)target;
                SerializedProperty sp = serializedObject.FindProperty("Offset");
                EditorGUILayout.PropertyField(sp, new GUIContent("Offset 倾斜偏移"));
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
