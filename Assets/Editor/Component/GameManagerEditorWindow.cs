/*********************************************
 * BFramework
 * GameGod EditorWindow扩展
 * 创建时间：2023/04/25 11:52:36
 *********************************************/
using MainPackage;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// GameGod EditorWindow扩展
    /// </summary>
    public class GameManagerEditorWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        [MenuItem("BFramework/GameManager监视窗口")]
        public static void OpenWindow()
        {
            var window = GetWindow<GameManagerEditorWindow>("GameManager监视窗口");
            window.Show();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying || GameGod.Instance == null)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("仅在运行下显示Manager数据概况");
                return;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            //========== 类对象池 ==========
            if (GameGod.Instance.PoolManager != null)
            {
                GUI.contentColor = Color.cyan;
                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("类对象池", GUILayout.Width(200));
                GUILayout.Label("创建数量", GUILayout.Width(100));
                GUILayout.Label("池中数量", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUI.contentColor = Color.white;
                foreach (var item in GameGod.Instance.PoolManager.InspectorDic)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                    GUILayout.Label(item.Key, GUILayout.Width(200));
                    GUILayout.Label(item.Value[0].ToString(), GUILayout.Width(100));
                    GUILayout.Label(item.Value[1].ToString(), GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }

                //========== 游戏对象池 ==========
                GUILayout.Space(5);
                GUI.contentColor = Color.cyan;
                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("游戏对象池", GUILayout.Width(200));
                GUILayout.Label("创建数量", GUILayout.Width(100));
                GUILayout.Label("池中数量", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUI.contentColor = Color.white;
                foreach (var item in GameGod.Instance.PoolManager.GameObjectPoolDic)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                    GUILayout.Label(item.Key, GUILayout.Width(200));
                    GUILayout.Label(item.Value.ObjLinkedList.Count.ToString(), GUILayout.Width(100));
                    GUILayout.Label(item.Value.ObjQueue.Count.ToString(), GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }
            }

            //========== 计时器 ==========
            if (GameGod.Instance.TimerManager != null)
            {
                GUILayout.Space(10);
                GUI.contentColor = Color.cyan;
                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("使用中的计时器名", GUILayout.Width(400));
                GUILayout.EndHorizontal();

                GUI.contentColor = Color.white;
                foreach (var item in GameGod.Instance.TimerManager.TimerInfoDic)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                    GUILayout.Label(item.Key, GUILayout.Width(400));
                    GUILayout.EndHorizontal();
                }
            }

            //========== 音频 ==========
            if (GameGod.Instance.AudioManager != null)
            {
                GUILayout.Space(10);
                GUI.contentColor = Color.cyan;
                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("音频管理器", GUILayout.Width(200));
                GUILayout.Label("音量", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUI.contentColor = Color.white;
                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("背景音乐音量", GUILayout.Width(200));
                GUILayout.Label(GameGod.Instance.AudioManager.GetVolumeBackground().ToString("F2"), GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box", GUILayout.Width(400));
                GUILayout.Label("音效音量", GUILayout.Width(200));
                GUILayout.Label(GameGod.Instance.AudioManager.GetVolumeSound().ToString("F2"), GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            //实时重绘
            Repaint();
        }
    }
}
