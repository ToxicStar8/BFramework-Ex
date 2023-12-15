/*********************************************
 * BFramework
 * Chatgpt即时问答工具
 * 创建时间：2023/08/16 17:00:23
 *********************************************/
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// Chatgpt即时问答工具
    /// </summary>
    public class ChatgptWindow : EditorWindow
    {
        private static ChatgptWindow _chatgptWindow;

        [MenuItem("BFramework/Chatgpt Tools",false,1000)]
        private static void OpenWindow()
        {
            var win = CreateWindow<ChatgptWindow>("Chatgpt即时问答工具");
            win.position = new Rect(100, 100, 600, 450);
            win.OnInit();
            _chatgptWindow?.Close();
            _chatgptWindow = win;
        }

        public void OnInit()
        {
            _openaiSaveKeyPath = Application.persistentDataPath + "/openaikey.txt";
            _version = "gpt-3.5-turbo-1106";

            if (File.Exists(_openaiSaveKeyPath))
            {
                _openaiKey = File.ReadAllText(_openaiSaveKeyPath);
            }
        }

        private string _question = "";
        private string _answer = "";
        private string _openaiKey = "";
        private string _openaiSaveKeyPath = "";
        private string _version = "";
        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("当前版本：", GUILayout.Width(80f));
            _version = EditorGUILayout.TextField(_version, GUILayout.Width(200f));
            if (GUILayout.Button("切换gpt-3.5"))
            {
                _version = "gpt-3.5-turbo-1106";
            }
            if (GUILayout.Button("切换gpt-4"))
            {
                _version = "gpt-4-1106-preview";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("你的问题：", GUILayout.Width(80f));
            _question = EditorGUILayout.TextField(_question, GUILayout.Width(440f));
            if (GUILayout.Button("确定"))
            {
                HttpPost();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("回复：", GUILayout.Width(80f));
            _answer = EditorGUILayout.TextField(_answer, GUILayout.MinHeight(300f), GUILayout.MaxHeight(1000f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("OpenAI Key：", GUILayout.Width(80f));
            _openaiKey = EditorGUILayout.TextField(_openaiKey, GUILayout.Width(400f));
            if (GUILayout.Button("保存"))
            {
                if (File.Exists(_openaiSaveKeyPath))
                {
                    File.Delete(_openaiSaveKeyPath);
                }
                File.WriteAllText(_openaiSaveKeyPath, _openaiKey);
            }
            if (GUILayout.Button("打开"))
            {
                OpenFileTools.OpenFile(Application.persistentDataPath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }

        private UnityWebRequest _webRequest;
        private string _openaiUrl = "https://api.openai.com/v1/chat/completions";
        private void HttpPost()
        {
            _webRequest = new UnityWebRequest(_openaiUrl, "POST");
            _webRequest.downloadHandler = new DownloadHandlerBuffer();
            var json = "{\"model\":\""+ _version + "\",\"messages\": [{\"role\": \"user\", \"content\": \"" + _question + "\"}]}";
            _webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            _webRequest.SetRequestHeader("Content-Type", "application/json");
            _webRequest.SetRequestHeader("Authorization", "Bearer " + _openaiKey);

            Debug.Log(string.Format("<color=#FFCCFF>url={0}{1}</color>", _openaiUrl, json));
            //新建一个对象用来暂用协程，用完删除
            var go = new GameObject("chatgptWindow");
            var emtpyMono = go.AddComponent<EmtpyMono>();
            emtpyMono.StartCoroutine(SendRequest(emtpyMono));
        }

        private IEnumerator SendRequest(EmtpyMono emtpyMono)
        {
            _webRequest.timeout = 120;
            yield return _webRequest.SendWebRequest();
            if (_webRequest.result == UnityWebRequest.Result.ConnectionError || _webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                //更新错误
                _answer = _webRequest.error;
                //打印错误
                if (!string.IsNullOrWhiteSpace(_answer))
                {
                    Debug.Log(_answer);
                }
            }
            else
            {
                //更新回复
                var jsonData = JsonMapper.ToObject<ChatgptMessage>(_webRequest.downloadHandler.text);
                _answer = jsonData.choices[0].message.content;
                //打印数据
                if (!string.IsNullOrWhiteSpace(_answer))
                {
                    Debug.Log(string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":{1}}}</color>", _webRequest.responseCode, _answer));
                }
            }
            _webRequest = null;
            DestroyImmediate(emtpyMono.gameObject);
        }

        public class ChatgptMessage
        {
            public string id;           //唯一标识符
            public string @object;      //
            public long created;        //创建时间
            public string model;        //chatgpt模式
            public List<Choices> choices;           //
            public Tokens usage;        //字符统计
            public class Choices
            {
                public int index;                   //消息顺序
                public ChatMessage message;   //消息列表
                public string finish_reason;        //

                public class ChatMessage
                {
                    public string role;
                    public string content;
                }
            }

            public class Tokens
            {
                public int prompt_tokens;           //发送的Token数
                public int completion_tokens;       //回复的Token数
                public int total_tokens;            //合计Token数
            }
        }
    }

}