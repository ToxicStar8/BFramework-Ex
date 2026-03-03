/*********************************************
 * BFramework
 * UI代码生成
 * 创建时间：2022/12/25 20:40:23
 *********************************************/
using System;
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
        /// <summary>
        /// 暂用绑定列表
        /// </summary>
        public static Dictionary<GameObject, List<UIBindData>> BindDataDic = new();

        /// <summary>
        /// 是否生成UI组件
        /// </summary>
        public static bool IsGenerateDesign;

        /// <summary>
        /// 是否连带生成Unit
        /// </summary>
        public static bool IsGenerateUnit;

        [MenuItem("GameObject/生成UI代码", false, 10000)]
        public static void GenerateUIAndDesign()
        {
            IsGenerateDesign = true;
            IsGenerateUnit = true;
            GenerateUI();
        }

        [MenuItem("GameObject/生成UI代码(不生成组件)", false, 10001)]
        public static void GenerateUINotDesign()
        {
            IsGenerateDesign = false;
            IsGenerateUnit = true;
            GenerateUI();
        }

        [MenuItem("GameObject/生成UI代码(不生成Unit)", false, 10002)]
        public static void GenerateUINotUnit()
        {
            IsGenerateDesign = true;
            IsGenerateUnit = false;
            GenerateUI();
        }

        public static void GenerateUI()
        {
            var go = Selection.activeGameObject;
            if (go == null || !(go.name.StartsWith("UI") || go.name.StartsWith("Unit")))
            {
                Debug.LogError("未选中UI/Unit！");
                return;
            }

            if (go.name.EndsWith("(Clone)"))
            {
                Debug.LogError("不要选中克隆体进行生成！");
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

            BindDataDic.Clear();

            if (go.name.StartsWith("UI"))
            {
                //创建目录
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //寻找全部绑定的组件
                FindAllComponent(go);

                CreateUIScript(go, path);
                if (IsGenerateDesign)
                    CreateUIDesignScript(go, path);
            }
            else if (go.name.StartsWith("Unit"))
            {
                if (!IsGenerateUnit)
                {
                    Debug.LogError("使用了不支持的方式生成Unit");
                    return;
                }

                //寻找全部绑定的组件
                FindAllComponent(go);

                CreateUnitScript(go, path);
                if (IsGenerateDesign)
                    CreateUnitDesignScript(go, path);
            }

            try
            {
                //绑定组件代码
                BindUIComponent();

                BindDataDic.Clear();

                Debug.Log(go.name + "生成完毕！如没有绑定成功，请再生成一次！");
                //回收资源
                System.GC.Collect();

                //刷新编辑器
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void CreateUIScript(GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 
 * 脚本名：#UIName.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class #UIName : GameUIBase
    {
        public override void OnAwake()
        {
            rectTransform = GetComponent<RectTransform>();#PoolBindFunc#ButtonBindFunc
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
                if (BindDataDic.TryGetValue(go, out var list))
                {
                    //按钮绑定方法
                    string btnBindFunc = "\r\n";
                    string poolBindFunc = "\r\n";
                    string btnOnClickStr = string.Empty;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var bindData = list[i];
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
                        //如果是Unit 就直接池
                        if (bindData.Type == "Unit")
                        {
                            poolBindFunc += "\r\n            " + bindData.Rect.name + "Pool = new(" + bindData.Rect.name + ", LoadHelper, rectTransform);";
                            continue;
                        }
                    }
                    //如果为空 为了美观 换行
                    poolBindFunc = poolBindFunc == "\r\n" ? "" : poolBindFunc;
                    btnBindFunc = btnBindFunc == "\r\n" ? "" : btnBindFunc;

                    //替换文本
                    temp = temp.Replace("#UIName", go.name);
                    temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    temp = temp.Replace("#PoolBindFunc", poolBindFunc); //预留池子绑定
                    temp = temp.Replace("#ButtonBindFunc", btnBindFunc);
                    temp = temp.Replace("#ButtonOnClick", btnOnClickStr);
                    //导出文件
                    File.WriteAllText(path + "/" + go.name + ".cs", temp, new UTF8Encoding(false));
                }
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
using MainPackage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class #UIName
    {#Component
    }
}
";
            string newComponentStr;
            //生成绑定关系
            GenerateBind(go, path, out newComponentStr);

            //文本替换
            temp = temp.Replace("#UIName", go.name);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#Component", newComponentStr);
            //导出文件
            var savePath = path + "/" + go.name + ".Design.cs";
            if (CheckCodeIsUnchanged(savePath, temp))
            {
                return;
            }
            File.WriteAllText(savePath, temp, new UTF8Encoding(false));
        }

        /// <summary>
        /// 生成绑定关系
        /// </summary>
        /// <param name="go">目标对象</param>
        /// <param name="path">脚本路径</param>
        /// <param name="newComponentStr">拿去替换的组件文本</param>
        private static void GenerateBind(GameObject go, string path, out string newComponentStr)
        {
            string componentStr = @"
        public #Component #Name;
";

            newComponentStr = string.Empty;
            if (BindDataDic.TryGetValue(go, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var bindData = list[i];
                    //声明变量
                    var tempComponentStr = componentStr.Replace("#Name", bindData.Rect.name);
                    var componentType = bindData.Type == "Unit" ? bindData.Rect.name : bindData.Type;
                    tempComponentStr = tempComponentStr.Replace("#Component", componentType);
                    newComponentStr += tempComponentStr;

                    //Unit类型
                    if (bindData.Type == "Unit")
                    {
                        //CreateUnitScript(go.name, bindData.Rect.gameObject, path);
                        //CreateUnitDesignScript(go.name, bindData.Rect.gameObject, path);

                        var poolName = bindData.Rect.name + "Pool";
                        var className = bindData.Rect.name;

                        //追加一个Pool
                        var tempComponentPoolStr = componentStr.Replace("#Name", poolName);
                        tempComponentPoolStr = tempComponentPoolStr.Replace("#Component", "UnitPool<" + className + ">");
                        tempComponentPoolStr = tempComponentPoolStr.Replace("#Content", "");
                        newComponentStr += tempComponentPoolStr;

                        var unitGo = bindData.Rect.gameObject;
                        //寻找全部绑定的组件
                        FindAllComponent(unitGo);

                        //路径特殊处理，方便找Unit
                        var unitPath = path;
                        var lastFolderName = Path.GetFileName(unitPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                        //Debug.Log("当前最后一级目录名称: " + lastFolderName);
                        // 如果最后一级不是 Unit，则追加 Unit 子目录
                        if (!string.Equals(lastFolderName, "Unit", StringComparison.Ordinal))
                        {
                            unitPath = Path.Combine(unitPath, "Unit");
                        }

                        if (IsGenerateUnit)
                        {
                            CreateUnitScript(unitGo, unitPath);
                            if (IsGenerateDesign)
                                CreateUnitDesignScript(unitGo, unitPath);
                        }
                    }
                }
            }
        }

        private static void CreateUnitScript(GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 
 * 脚本名：#UnitName.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class #UnitName : UnitBase
    {
        public override void OnAwake()
        {
            rectTransform = GetComponent<RectTransform>();#PoolBindFunc
        }

        public void OnShow()
        {
            
        }

        public override void OnRecycle()
        {
            
        }

        public override void OnBeforeDestroy()
        {
            
        }
    }
}
";

            //创建目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //导出文件
            var saveName = go.name + ".cs";
            var savePath = path + "/" + saveName;
            //检查是否有同名文件，存到那边，不然就存到既定路径
            var directoryInfo = new DirectoryInfo(Application.dataPath);
            foreach (var item in directoryInfo.GetFiles(saveName, SearchOption.AllDirectories))
            {
                if (item.Name == saveName)
                {
                    savePath = item.FullName;
                    break;
                }
            }

            //已经生成过了就不覆盖了
            if (!File.Exists(savePath))
            {
                if (BindDataDic.TryGetValue(go, out var list))
                {
                    string poolBindFunc = "\r\n";
                    for (int i = 0; i < list.Count; i++)
                    {
                        var bindData = list[i];
                        //空就跳过
                        if (bindData == null)
                        {
                            continue;
                        }
                        //如果是Unit 就直接池
                        if (bindData.Type == "Unit")
                        {
                            poolBindFunc += "\r\n            " + bindData.Rect.name + "Pool = new(" + bindData.Rect.name + " , LoadHelper, rectTransform);";
                            continue;
                        }
                    }
                    poolBindFunc = poolBindFunc== "\r\n" ? "" : poolBindFunc;

                    //替换文本
                    temp = temp.Replace("#UnitName", go.name);
                    temp = temp.Replace("#PoolBindFunc", poolBindFunc); //预留池子绑定
                    temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    //导出文件
                    File.WriteAllText(savePath, temp, new UTF8Encoding(false));
                }
            }
        }

        /// <summary>
        /// 生成Unit绑定关系 每次覆盖
        /// </summary>
        private static void CreateUnitDesignScript(GameObject go, string path)
        {
            string temp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：#UnitName.cs
 * 修改时间：#Time
 *********************************************/
using Framework;
using MainPackage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class #UnitName
    {#Component
    }
}
";

            string newComponentStr;
            //生成绑定关系
            GenerateBind(go, path, out newComponentStr);

            //创建目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //文本替换
            temp = temp.Replace("#UnitName", go.name);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#Component", newComponentStr);

            //导出文件
            var saveName = go.name + ".Design.cs";
            var savePath = path + "/" + saveName;
            //检查是否有同名文件，存到那边，不然就存到既定路径
            var directoryInfo = new DirectoryInfo(Application.dataPath);
            foreach (var item in directoryInfo.GetFiles(saveName, SearchOption.AllDirectories))
            {
                if (item.Name == saveName)
                {
                    savePath = item.FullName;
                    break;
                }
            }
            if (CheckCodeIsUnchanged(savePath, temp))
            {
                return;
            }
            File.WriteAllText(savePath, temp, new UTF8Encoding(false));
        }

        /// <summary>
        /// 将生成好的Mono绑定组件
        /// </summary>
        private static void BindUIComponent()
        {
            foreach (var item in BindDataDic)
            {
                var go = item.Key;
                var list = item.Value;

                //var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
                var assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

                //反射拿到类
                var bindTypeStr = $"GameData.{go.name}";
                Type uiBindType = assembly.GetType(bindTypeStr);
                //添加绑定关系的组件
                var uiBind = go.GetComponent(uiBindType);
                uiBind ??= go.AddComponent(uiBindType);

                //循环已有的字段列表绑定
                for (int i = 0; i < list.Count; i++)
                {
                    var bind = list[i];
                    try
                    {
                        //反射拿到类里的组件
                        var field = uiBindType.GetField(bind.fielType);
                        //如果找不到字段，跳过
                        if (field == null)
                        {
                            continue;
                        }

                        //Unit特殊处理
                        var typeStr = bind.Type;
                        UnityEngine.Object component;
                        if (typeStr == "GameObject")
                        {
                            component = bind.Rect.gameObject;
                            //为该组件赋值(实例对象，值)
                            field.SetValue(uiBind, component);
                        }
                        else if (typeStr.Contains("[]"))
                        {
                            typeStr = typeStr.Replace("[]", "");

                            component = bind.Rect.GetComponent(typeStr);
                            var data = field.GetValue(uiBind);

                            Array array = data as Array;

                            // Create a new array with one more element than the original array
                            Type elementType = component.GetType();

                            var Length = 0;

                            if (array != null && bind.index != 0)
                            {
                                Length = array.Length;
                            }

                            Array newArray = Array.CreateInstance(elementType, Length + 1);

                            if (array != null && bind.index != 0)
                                // Copy the original array elements to the new array
                                Array.Copy(array, newArray, array.Length);

                            // Add the new element to the new array
                            newArray.SetValue(component, Length);

                            // Set the new array back to the field
                            field.SetValue(uiBind, newArray);
                        }
                        else if (typeStr == "Unit")
                        {
                            component = bind.Rect.GetComponent(bind.Rect.name);
                            //为该组件赋值(实例对象，值)
                            field.SetValue(uiBind, component);
                        }
                        else
                        {
                            component = bind.Rect.GetComponent(typeStr);
                            //为该组件赋值(实例对象，值)
                            if (component is null)
                            {
                                var resolvedType = ResolveTypeByName(typeStr);
                                if (resolvedType is null)
                                {
                                    throw new Exception($"无法解析类型: {typeStr}");
                                }
                                component = bind.Rect.GetComponent(resolvedType);
                            }
                            //为该组件赋值(实例对象，值)
                            field.SetValue(uiBind, component);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"<color=#FF7070>Name={bind.Rect.name} Type={bind.Type}</color>\n===> {e} prefab: {go.name}");
                    }
                }

                //保存对象
                EditorUtility.SetDirty(uiBind);

            }
        }

        private static Type ResolveTypeByName(string typeName)
        {
            // 直接尝试简单名称
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            // 遍历已加载的所有程序集按名称匹配类型
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                type = asm.GetTypes().FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static string GetParentDirName(string goName, string prefabsPath)
        {
            foreach (var file in Directory.GetFiles(prefabsPath, "*.prefab", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == goName + ".prefab")
                {
                    var strs = file.Replace("\\", "/").Split('/');
                    return strs[^2];
                }
            }

            return "";
        }

        /// <summary>
        /// 检查代码文件是否未改动
        /// </summary>
        private static bool CheckCodeIsUnchanged(string savePath, string temp)
        {
            if (File.Exists(savePath))
            {
                using (var txt = File.OpenText(savePath))
                {
                    var oldCode = txt.ReadToEnd().Remove(0, 180);
                    temp = temp.Remove(0, 180);
                    if (oldCode == temp)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 寻找全部绑定的组件
        /// </summary>
        private static void FindAllComponent(GameObject go)
        {
            //游戏对象列表
            var list = new List<UIBindData>();
            var childList = go.GetComponentsInChildren<RectTransform>(true).ToList();
            for (int i = 0; i < childList.Count; i++)
            {
                var child = childList[i];
                //跳过自己
                if (child.name == go.name)
                {
                    continue;
                }

                //下划线分割的组件才需要绑定
                var strArr = child.name.Split('_');
                if (strArr.Length != 2 && strArr.Length != 3)
                {
                    continue;
                }

                //如果有Unit 就把Unit下面的绑定组件全部置空 / 如果是自定义组件也置空
                if (child.name.StartsWith("Unit") || child.name.StartsWith("Custom"))
                {
                    var unitChildArr = child.GetComponentsInChildren<RectTransform>(true);
                    foreach (var unitChild in unitChildArr)
                    {
                        //跳过自己
                        if (child.name == unitChild.name)
                        {
                            continue;
                        }
                        childList.Remove(unitChild);
                    }
                }

                var bindData = new UIBindData()
                {
                    Rect = child,
                    Type = GetTypeByStr(strArr),
                };

                if (bindData.Type.Contains("[]"))
                {
                    var childArr = child.name.Split("_");
                    if (childArr.Length < 3)
                    {
                        Debug.LogError($"生成绑定失败 {child.name} {go.name}");
                        return;
                    }

                    if (int.TryParse(childArr[2], out int value))
                    {
                        bindData.index = value;
                    }
                    else
                    {
                        Debug.LogError($"生成绑定失败 {child.name} {go.name}");
                    }

                    bindData.fielType = $"{childArr[0]}_{childArr[1]}";
                }
                else
                {
                    bindData.fielType = child.name;
                }

                list.Add(bindData);
            }
            BindDataDic[go] = list;
        }

        /// <summary>
        /// 获取类型，通过前缀
        /// </summary>
        private static string GetTypeByStr(string[] strArr)
        {
            switch (strArr[0])
            {
                case "Btn":
                    return "ButtonEx";
                case "Btns":
                    return "ButtonEx[]";
                case "Pcl":
                    return "PointerClickListener";
                case "Img":
                    return "ImageEx";
                case "Imgs":
                    return "ImageEx[]";
                case "Raw":
                    return "RawImage";
                case "Raws":
                    return "RawImage[]";
                case "Inf":
                    return "InputField";
                case "Infs":
                    return "InputField[]";
                case "Txt":
                    return "Text";
                case "Txts":
                    return "Text[]";
                case "Tmp":
                    return "TmpEx";
                case "Tmps":
                    return "TmpEx[]";
                case "TmpInf":
                    return "TMP_InputField";
                case "TmpInfs":
                    return "TMP_InputField[]";
                case "Tg":
                    return "Toggle";
                case "Tgs":
                    return "Toggle[]";
                case "Sd":
                    return "Slider";
                case "Sv":
                    return "ScrollRect";
                case "Lvs":
                    return "SQQ.Core.LoopVerticalScroll";
                case "Lgs":
                    return "SQQ.Core.LoopVGridScroll";
                case "Unit":
                    return "Unit";
                case "Rt":
                    return "RectTransform";
                case "Rts":
                    return "RectTransform[]";
                case "Vlg":
                    return "VerticalLayoutGroup";
                case "Le":
                    return "LayoutElement";
                case "Go":
                    return "GameObject";
                case "Gos":
                    return "GameObject[]";
                case "TmpDp":
                    return "TMP_Dropdown";
                case "Dp":
                    return "Dropdown";

                //自定义组件直接返回类型
                case "Custom":
                    return strArr[1];
            }
            return "Error";
        }

        public class UIBindData
        {
            public RectTransform Rect;
            public string Type;
            public int index = -1;
            public string fielType;
        }
    }
}