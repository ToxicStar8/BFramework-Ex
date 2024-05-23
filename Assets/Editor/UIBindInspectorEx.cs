///*********************************************
// * BFramework
// * UIBind检查器扩展
// * 创建时间：2024/05/23 22:10:36
// *********************************************/
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using UnityEditor;
//using UnityEngine;

//namespace Framework
//{
//    /// <summary>
//    /// UIBind检查器扩展
//    /// </summary>
//    [CustomEditor(typeof(UIBindBase))]
//    public class UIBindInspectorEx : Editor
//    {
//        /// <summary>
//        /// 暂用绑定列表
//        /// </summary>
//        private static List<UIBindData> BindDatas = new List<UIBindData>();

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            GUILayout.Space(10);
//            if (GUILayout.Button("生成代码"))
//            {
//                GenerateUI();
//            }
//        }

//        [MenuItem("GameObject/生成UI代码", false, 10000)]
//        public static void GenerateUI()
//        {
//            var go = Selection.activeGameObject;
//            if (go == null || !(go.name.StartsWith("UI") || go.name.EndsWith("Unit")))
//            {
//                Debug.LogError("未选中UI/Unit！");
//                return;
//            }

//            //寻找到当前obj的路径
//            var directoryInfo = new DirectoryInfo(Application.dataPath);
//            string prefabName = go.name + ".prefab";
//            string path = string.Empty;
//            foreach (var item in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
//            {
//                if (item.Name == prefabName)
//                {
//                    //预制体路径
//                    path = item.FullName;
//                    if (go.name.StartsWith("UI"))
//                    {
//                        //替换成脚本路径
//                        path = path.Replace("GameData\\Prefabs", "GameData\\Scripts").Substring(0, path.Length - prefabName.Length);
//                    }
//                    else
//                    {
//                        path = path.Replace("GameData\\Prefabs", "GameData\\Scripts").Substring(0, path.Length - prefabName.Length - 6);
//                    }
//                    break;
//                }
//            }

//            if (string.IsNullOrEmpty(path))
//            {
//                Debug.LogError("UI内的Unit请直接使用UI生成代码");
//                return;
//            }

//            //创建目录
//            if (!Directory.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            //寻找全部绑定的组件
//            FindAllComponent(go);

//            //生成代码
//            if (go.name.StartsWith("UI"))
//            {
//                CreateUIScript(go, path);
//                CreateUIBindScript(go, path);
//                BindUIComponent(go);
//            }
//            else if (go.name.StartsWith("Unit"))
//            {
//                //var parentName = new DirectoryInfo(path).Name;
//                //CreateUnitScript(parentName, go, path);
//                //CreateUnitDesignScript(parentName, go, path);
//            }

//            Debug.Log(go.name + "生成完毕！");
//            //回收资源
//            System.GC.Collect();
//            //刷新编辑器
//            AssetDatabase.Refresh();
//        }

//        /// <summary>
//        /// 创建UI主要逻辑代码
//        /// </summary>
//        private static void CreateUIScript(GameObject go, string path)
//        {
//            string temp = @"/*********************************************
// * 
// * 脚本名：#UIName.cs
// * 创建时间：#Time
// *********************************************/
//using Framework;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace GameData
//{
//    public class #UIName : GameUIBase
//    {
//        private #UIName_Bind Bind;

//        public override void OnInit()
//        {
//            Bind = gameObject.GetComponent<#UIName_Bind>();
//#ButtonBindFunc
//        }

//        public override void OnShow(params object[] args)
//        {

//        }
//#ButtonOnClick
//        protected override void OnBeforeDestroy()
//        {

//        }
//    }
//}
//";
//            if (!File.Exists(path + go.name + ".cs"))
//            {
//                //游戏对象列表
//                var childList = go.GetComponentsInChildren<RectTransform>().ToList();
//                //按钮绑定方法文本
//                string btnBindFunc = string.Empty;
//                string btnOnClickStr = string.Empty;
//                for (int i = 0; i < childList.Count; i++)
//                {
//                    var child = childList[i];
//                    //如果有Unit 就把Unit下面的绑定组件全部置空
//                    if (child.name.StartsWith("Unit"))
//                    {
//                        var unitChildArr = child.GetComponentsInChildren<RectTransform>();
//                        foreach (var unitChild in unitChildArr)
//                        {
//                            //跳过自己
//                            if (child.name == unitChild.name)
//                            {
//                                continue;
//                            }
//                            childList.Remove(unitChild);
//                        }
//                    }
//                    //如果是按钮 就直接追加方法
//                    if (child.name.StartsWith("Btn_"))
//                    {
//                        string funcName = "OnClick_" + child.name;
//                        //组件监听
//                        btnBindFunc += "\r\n            Bind." + child.name + ".AddListener(" + funcName + ");";
//                        //方法
//                        btnOnClickStr += "\r\n        private void " + funcName + "()\r\n        {\r\n            Log(\"点击了" + child.name + "\");\r\n        }\r\n";
//                        continue;
//                    }

//                }
//                //替换文本
//                temp = temp.Replace("#UIName", go.name);
//                temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
//                temp = temp.Replace("#ButtonBindFunc", btnBindFunc);
//                temp = temp.Replace("#ButtonOnClick", btnOnClickStr);
//                //导出文件
//                File.WriteAllText(path + go.name + ".cs", temp, Encoding.UTF8);
//            }
//        }

//        /// <summary>
//        /// 创建UI绑定关系的Mono代码，每次覆盖
//        /// </summary>
//        private static void CreateUIBindScript(GameObject go, string path)
//        {
//            string temp = @"/*********************************************
// * 自动生成代码，禁止手动修改文件
// * 脚本名：#UIName_Bind.cs
// * 修改时间：#Time
// *********************************************/

//using Framework;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace GameData
//{
//    public class #UIName_Bind : MonoBehaviour
//    {#Component
//    }
//}
//";
//            //添加绑定文本
//            StringBuilder newComponentStr = new StringBuilder();
//            for (int i = 0; i < BindDatas.Count; i++)
//            {
//                var bindComponent = BindDatas[i];
//                newComponentStr.AppendLine();
//                newComponentStr.Append($"        public {bindComponent.Type} {bindComponent.Rect.name};");
//            }
//            //文本替换
//            temp = temp.Replace("#UIName", go.name);
//            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
//            temp = temp.Replace("#Component", newComponentStr.ToString());
//            //导出文件
//            File.WriteAllText(path + go.name + "_Bind.cs", temp, Encoding.UTF8);
//        }

//        /// <summary>
//        /// 将生成好的Mono绑定组件
//        /// </summary>
//        private static void BindUIComponent(GameObject go)
//        {
//            var assembly = Assembly.Load("Assembly-CSharp");
//            //反射拿到类
//            Type uiBindType = assembly.GetType($"GameData.{go.name}_Bind");
//            //添加绑定关系的组件
//            var uiBind = go.GetComponent(uiBindType);
//            if (uiBind == null)
//            {
//                uiBind = go.AddComponent(uiBindType);
//            }
//            //循环已有的字段列表绑定
//            for (int i = 0; i < BindDatas.Count; i++)
//            {
//                var bind = BindDatas[i];
//                //反射拿到类里的组件
//                var field = uiBindType.GetField(bind.Rect.name);
//                //为该组件赋值(实例对象，值)
//                var component = bind.Rect.GetComponent(bind.Type);
//                field.SetValue(uiBind, component);
//            }
//            //保存对象
//            EditorUtility.SetDirty(uiBind);
//        }

//        /// <summary>
//        /// 寻找全部绑定的组件
//        /// </summary>
//        private static void FindAllComponent(GameObject go)
//        {
//            BindDatas.Clear();
//            //游戏对象列表
//            var childList = go.GetComponentsInChildren<RectTransform>().ToList();
//            for (int i = 0; i < childList.Count; i++)
//            {
//                var child = childList[i];
//                //跳过自己
//                if (child.name == go.name)
//                {
//                    continue;
//                }
//                //下划线分割的组件才需要绑定
//                var strArr = child.name.Split('_');
//                if (strArr.Length != 2)
//                {
//                    continue;
//                }
//                //如果有Unit 就把Unit下面的绑定组件全部置空
//                if (child.name.StartsWith("Unit"))
//                {
//                    var unitChildArr = child.GetComponentsInChildren<RectTransform>();
//                    foreach (var unitChild in unitChildArr)
//                    {
//                        //跳过自己
//                        if (child.name == unitChild.name)
//                        {
//                            continue;
//                        }
//                        childList.Remove(unitChild);
//                    }
//                }
//                var bindData = new UIBindData()
//                {
//                    Rect = child,
//                    Type = GetTypeByStr(strArr[0]),
//                };
//                BindDatas.Add(bindData);
//            }
//        }

//        /// <summary>
//        /// 获取类型，通过前缀
//        /// </summary>
//        private static string GetTypeByStr(string str)
//        {
//            string type = string.Empty;
//            switch (str)
//            {
//                case "Btn":
//                    type = "ButtonEx";
//                    break;
//                case "Img":
//                    type = "ImageEx";
//                    break;
//                case "Lsr":
//                    type = "LoopScrollRect";
//                    break;
//                case "Sr":
//                    type = "ScrollRect";
//                    break;
//                case "Inf":
//                    type = "InputField";
//                    break;
//                case "Tg":
//                    type = "Toggle";
//                    break;
//                case "Sd":
//                    type = "Slider";
//                    break;
//                case "Tmp":
//                    type = "TextMeshProUGUI";
//                    break;
//                case "Txt":
//                    type = "Text";
//                    break;
//                case "Rt":
//                    type = "RectTransform";
//                    break;
//            }
//            return type;
//        }

//        public class UIBindData
//        {
//            public RectTransform Rect;
//            public string Type;
//        }
//    }
//}