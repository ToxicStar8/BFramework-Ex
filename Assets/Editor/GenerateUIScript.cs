/*********************************************
 * BFramework
 * UI代码生成
 * 创建时间：2022/12/25 20:40:23
 *********************************************/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// UI代码生成
    /// </summary>
    public class GenerateUIScript : Editor
    {
        [MenuItem("GameObject/生成UI代码", false, 10000)]
        public static void GenerateUI()
        {
            var go = Selection.activeGameObject;
            if (go == null || !(go.name.StartsWith("UI") || go.name.StartsWith("Unit")))
            {
                Debug.LogError("未选中UI/Unit！");
                return;
            }

            //寻找到当前obj的路径
            string prefabName = go.name + ".prefab";
            var directoryInfo = new DirectoryInfo(Application.dataPath);
            string path = string.Empty;
            foreach (var item in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (item.Name == prefabName)
                {
                    //预制体路径
                    path = item.FullName;
                    if (go.name.StartsWith("UI"))
                    {
                        //替换成脚本路径
                        path = path.Replace("GameData\\Prefabs", "GameData\\Scripts").Substring(0, path.Length - prefabName.Length);
                    }
                    else
                    {
                        path = path.Replace("GameData\\Prefabs", "GameData\\Scripts").Substring(0, path.Length - prefabName.Length - 6);
                    }
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("UI内的Unit请直接使用UI生成代码");
                return;
            }

            //创建目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (go.name.StartsWith("UI"))
            {
                //创建目录
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                CreateUIScript(go, path);
                CreateUIDesignScript(go, path);
            }
            else if (go.name.StartsWith("Unit"))
            {
                //没有父物体就直接用父文件夹名
                var parentName = new DirectoryInfo(path).Name;
                CreateUnitScript(parentName, go, path);
                CreateUnitDesignScript(parentName, go, path);
            }

            Debug.Log(go.name + "生成完毕！");
            //回收资源
            System.GC.Collect();
            //刷新编辑器
            AssetDatabase.Refresh();
        }

        private static void CreateUIScript(GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 
 * 脚本名：#UIName.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class #UIName : GameUIBase
    {
        public override void OnInit()
        {#ButtonBindFunc
        }

        public override void OnShow(params object[] args)
        {
            
        }
#ButtonOnClick
        protected override void OnBeforeDestroy()
        {
            
        }
    }
}
";
            if (!File.Exists(path + go.name + ".cs"))
            {
                //按钮绑定方法
                string btnBindFunc = string.Empty;
                string btnOnClickStr = string.Empty;
                var bindDatas = FindAllComponent(go);
                for (int i = 0; i < bindDatas.Count; i++)
                {
                    var bindData = bindDatas[i];
                    //空就跳过
                    if (bindData == null)
                    {
                        continue;
                    }
                    //如果是按钮 就直接追加方法
                    if (bindData.Type == "ButtonEx")
                    {
                        string funcName = "OnClick_" + bindData.Rect.name;
                        btnBindFunc += "\r\n            " + bindData.Rect.name + ".AddListener(" + funcName + ");";
                        btnOnClickStr += "\r\n        private void " + funcName + "()\r\n        {\r\n            Log(\"点击了" + bindData.Rect.name + "\");\r\n        }\r\n";
                        continue;
                    }
                }
                //如果为空 加个回车 美观
                btnBindFunc = string.IsNullOrWhiteSpace(btnBindFunc) ? "\r\n" : btnBindFunc;

                //替换文本
                temp = temp.Replace("#UIName", go.name);
                temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                temp = temp.Replace("#ButtonBindFunc", btnBindFunc);
                temp = temp.Replace("#ButtonOnClick", btnOnClickStr);
                //导出文件
                File.WriteAllText(path + go.name + ".cs", temp, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成UI绑定关系 每次覆盖
        /// </summary>
        /// <param name="go"></param>
        /// <param name="path"></param>
        private static void CreateUIDesignScript(GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：#UIName.Design.cs
 * 修改时间：#Time
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class #UIName
    {#Component
        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            #Find
        }
    }
}
";
            string newComponentStr;
            string newFindStr;
            //生成绑定关系
            GenerateBind(go, path, out newComponentStr, out newFindStr);

            //文本替换
            temp = temp.Replace("#UIName", go.name);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#Component", newComponentStr);
            temp = temp.Replace("#Find", newFindStr);
            //导出文件
            var savePath = path + go.name + ".Design.cs";
            if (IsNeedGenerateText(savePath, temp))
            {
                return;
            }
            File.WriteAllText(savePath, temp, Encoding.UTF8);
        }

        /// <summary>
        /// 生成绑定关系
        /// </summary>
        /// <param name="go">目标对象</param>
        /// <param name="path">脚本路径</param>
        /// <param name="newComponentStr">拿去替换的组件文本</param>
        /// <param name="newFindStr">拿去替换的寻找文本</param>
        private static void GenerateBind(GameObject go, string path, out string newComponentStr, out string newFindStr)
        {
            string componentStr = @"
        public #Component #Name;
";
            string UnitStr = "#Name = rectTransform.Find(\"#Path\").gameObject;";
            string FindStr = "#Name = rectTransform.Find(\"#Path\").GetComponent<#Component>();";

            string PoolStr = "#PoolName = new UnitPool<#UnitName>(this,#Obj);";

            newComponentStr = string.Empty;
            newFindStr = string.Empty;
            var bindDatas = FindAllComponent(go);
            for (int i = 0; i < bindDatas.Count; i++)
            {
                var bindData = bindDatas[i];
                //声明变量
                var tempComponentStr = componentStr.Replace("#Name", bindData.Rect.name);
                var componentType = bindData.Type == "Unit" ? "GameObject" : bindData.Type;
                tempComponentStr = tempComponentStr.Replace("#Component", componentType);
                newComponentStr += tempComponentStr;

                //寻找对象
                var targetStr = bindData.Type == "Unit" ? UnitStr : FindStr;
                var tempFindStr = targetStr.Replace("#Component", componentType);
                tempFindStr = tempFindStr.Replace("#Name", bindData.Rect.name);
                tempFindStr = tempFindStr.Replace("#Path", FindComponentInPrefabPath(go.name, bindData.Rect.gameObject));
                tempFindStr += "\r\n\t\t\t";
                newFindStr += tempFindStr;

                //Unit类型
                if (bindData.Type == "Unit")
                {
                    CreateUnitScript(go.name, bindData.Rect.gameObject, path);
                    CreateUnitDesignScript(go.name, bindData.Rect.gameObject, path);

                    var poolName = bindData.Rect.name + "Pool";
                    var className = bindData.Rect.name;

                    //追加一个Pool
                    var tempComponentPoolStr = componentStr.Replace("#Name", poolName);
                    tempComponentPoolStr = tempComponentPoolStr.Replace("#Component", "UnitPool<" + className + ">");
                    tempComponentPoolStr = tempComponentPoolStr.Replace("#Content", "");
                    newComponentStr += tempComponentPoolStr;

                    var tempFindPoolStr = PoolStr.Replace("#PoolName", poolName);
                    tempFindPoolStr = tempFindPoolStr.Replace("#UnitName", className);
                    tempFindPoolStr = tempFindPoolStr.Replace("#Obj", bindData.Rect.name);
                    tempFindPoolStr += "\r\n\t\t\t";
                    newFindStr += tempFindPoolStr;
                }
            }
        }

        private static void CreateUnitScript(string uiName, GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 
 * 脚本名：#UnitName.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class #UnitName : UnitBase
    {
        public override void OnInit()
        {
            
        }

        public void OnShow()
        {
            
        }
    }
}
";

            var unitPath = path + "/Unit";
            //创建目录
            if (!Directory.Exists(unitPath))
            {
                Directory.CreateDirectory(unitPath);
            }

            var savePath = unitPath + "/" + go.name + ".cs";
            //已经生成过了就不覆盖了
            if (!File.Exists(savePath))
            {
                //替换文本
                temp = temp.Replace("#UnitName", go.name);
                temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                //导出文件
                File.WriteAllText(savePath, temp, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成Unit绑定关系 每次覆盖
        /// </summary>
        private static void CreateUnitDesignScript(string uiName, GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：#UnitName.cs
 * 修改时间：#Time
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class #UnitName
    {#Component
        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            #Find
        }
    }
}
";

            string newComponentStr;
            string newFindStr;
            //生成绑定关系
            GenerateBind(go, path, out newComponentStr, out newFindStr);

            var unitPath = path + "/Unit";
            //创建目录
            if (!Directory.Exists(unitPath))
            {
                Directory.CreateDirectory(unitPath);
            }

            //文本替换
            temp = temp.Replace("#UnitName", go.name);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#Component", newComponentStr);
            temp = temp.Replace("#Find", newFindStr);
            //导出文件
            var savePath = unitPath + "/" + go.name + ".Design.cs";
            if (IsNeedGenerateText(savePath, temp))
            {
                return;
            }
            File.WriteAllText(savePath, temp, Encoding.UTF8);
        }

        /// <summary>
        /// 循环父物体拿到组件路径
        /// </summary>
        private static string FindComponentInPrefabPath(string scriptName, GameObject go)
        {
            //拿到父物体 修改路径 设置跳出值
            var parentGo = go.transform.parent;
            string path = parentGo.name + "/" + go.name;
            bool isParent = true;

            while (isParent)
            {
                //如果父物体名字等于传进来的对象名 就退出
                if (parentGo.name == scriptName)
                {
                    isParent = false;
                    break;
                }

                //循环添加路径
                parentGo = parentGo.transform.parent;
                path = parentGo.name + "/" + path;
            }

            //删除最上级物体名
            path = path.Substring(parentGo.name.Length + 1);

            return path;
        }

        /// <summary>
        /// 检测是否需要生成代码
        /// </summary>
        private static bool IsNeedGenerateText(string savePath, string newText)
        {
            if (!File.Exists(savePath))
            {
                return false;
            }

            using (var txt = File.OpenText(savePath))
            {
                var oldText = txt.ReadToEnd();
                oldText = oldText.Substring(200, oldText.Length - 200);
                var temp = newText.Substring(200, newText.Length - 200);
                if (oldText.Equals(temp))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<BindData> FindAllComponent(GameObject go)
        {
            var bindDatas = new List<BindData>();
            var childs = go.GetComponentsInChildren<RectTransform>(true).ToList();
            for (int i = 0; i < childs.Count; i++)
            {
                var child = childs[i];
                //跳过自己
                if (child.name == go.name)
                {
                    continue;
                }
                //切割类型和名字
                var strArr = child.name.Split('_');
                if (strArr.Length != 2)
                {
                    continue;
                }
                //如果UI下有Unit 就把Unit下面的绑定组件全部置空
                if (child.name.StartsWith("Unit"))
                {
                    var unitChildArr = child.GetComponentsInChildren<RectTransform>(true);
                    foreach (var unitChild in unitChildArr)
                    {
                        //跳过自己
                        if (child.name == unitChild.name)
                        {
                            continue;
                        }
                        childs.Remove(unitChild);
                    }
                }

                bindDatas.Add(new BindData()
                {
                    Rect = child,
                    Type = GetTypeByStr(strArr[0])
                });
            }
            return bindDatas;
        }

        private static string GetTypeByStr(string type)
        {
            switch (type)
            {
                case "Lsr":
                    return "LoopScrollRect";
                case "Sv":
                    return "ScrollRect";
                case "Btn":
                    return "ButtonEx";
                case "Inf":
                    return "InputField";
                case "TmpInf":
                    return "TMP_InputField";
                case "Img":
                    return "Image";
                case "Raw":
                    return "RawImage";
                case "Tg":
                    return "Toggle";
                case "Sd":
                    return "Slider";
                case "Tmp":
                    return "TmpEx";
                case "Txt":
                    return "TextEx";
                case "TmpDp":
                    return "TMP_Dropdown";
                case "Dp":
                    return "Dropdown";
                case "Rt":
                    return "RectTransform";
                case "Unit":
                    return "Unit";
                default:
                    return "Error";
            }
        }
    }

    public class BindData
    {
        public RectTransform Rect;
        public string Type;
    }
}
