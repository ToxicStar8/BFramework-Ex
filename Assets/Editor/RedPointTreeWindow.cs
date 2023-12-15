/*********************************************
 * BFramework
 * 红点树预览工具
 * 创建时间：2023/12/15 10:43:23
 *********************************************/
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TreeEditor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Framework
{
    /// <summary>
    /// 红点树预览窗口
    /// </summary>
    public class RedPointTreeWindow : EditorWindow
    {
        private static RedPointTreeWindow _redPointWindow;

        [MenuItem("BFramework/RedPoint Show", false, 100)]
        private static void OpenWindow()
        {
            var win = CreateWindow<RedPointTreeWindow>("红点树预览工具");
            win.position = new Rect(100, 100, 600, 450);
            win.OnInit();
            _redPointWindow?.Close();
            _redPointWindow = win;
        }

        private void OnInit()
        {

        }

        private void OnGUI()
        {
            if (!Application.isPlaying || GameGod.Instance == null || GameGod.Instance.RedPointManager == null)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("仅在运行下显示池对象概况");
                return;
            }

            DrawNode(GameGod.Instance.RedPointManager.RootNode, 0);

            //实时重绘
            Repaint();
        }

        private void DrawNode(TrieTreeNode node, int depth)
        {
            EditorGUI.indentLevel = depth;
            EditorGUILayout.BeginHorizontal();
            node.IsExpanded = EditorGUILayout.Foldout(node.IsExpanded, node.Key);
            EditorGUILayout.EndHorizontal();

            if (node.IsExpanded)
            {
                foreach (var child in node.ChildrenDic)
                {
                    DrawNode(child.Value, depth + 1);
                }
            }
        }
    }
}