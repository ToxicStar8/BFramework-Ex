/*********************************************
 * BFramework
 * Excel配表导出工具（EditorWindow版）
 * 基于ExcelTools控制台工具改造
 *********************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Excel配表导出EditorWindow
    /// </summary>
    public class ExcelToolsWindow : EditorWindow
    {
        /// <summary>
        /// Excel表格所在目录
        /// </summary>
        private string _excelPath;

        /// <summary>
        /// 导出的脚本目录
        /// </summary>
        private string _outScriptPath;

        /// <summary>
        /// 导出的Txt/bytes目录
        /// </summary>
        private string _outTxtPath;

        /// <summary>
        /// 导出根目录（GameData）
        /// </summary>
        private string _gameDataPath;

        /// <summary>
        /// 日志滚动位置
        /// </summary>
        private Vector2 _logScrollPos;

        /// <summary>
        /// 日志内容
        /// </summary>
        private readonly List<string> _logs = new List<string>();

        /// <summary>
        /// 是否正在导出
        /// </summary>
        private bool _isExporting;

        // EditorPrefs keys
        private const string PrefKeyExcelPath = "ExcelTools_ExcelPath";
        private const string PrefKeyOutScriptPath = "ExcelTools_OutScriptPath";
        private const string PrefKeyOutTxtPath = "ExcelTools_OutTxtPath";
        private const string SessionKeyLogs = "ExcelTools_Logs";

        [MenuItem("BFramework/Excel导表工具", false, 200)]
        private static void OpenWindow()
        {
            var win = GetWindow<ExcelToolsWindow>("Excel导表工具");
            win.ClearLog();
            win.minSize = new Vector2(500, 400);
            win.Show();
        }

        private void OnEnable()
        {
            // 读取上次保存的路径，若无则使用默认值
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;

            _excelPath = EditorPrefs.GetString(PrefKeyExcelPath, Path.Combine(projectRoot,"Table").Replace("\\", "/"));
            _outScriptPath = EditorPrefs.GetString(PrefKeyOutScriptPath, Path.Combine(Application.dataPath, "GameData", "Scripts", "Table").Replace("\\", "/"));
            _outTxtPath = EditorPrefs.GetString(PrefKeyOutTxtPath, Path.Combine(Application.dataPath, "GameData", "Table").Replace("\\", "/"));

            var savedLogs = SessionState.GetString(SessionKeyLogs, string.Empty);
            _logs.Clear();
            if (!string.IsNullOrEmpty(savedLogs))
            {
                _logs.AddRange(savedLogs.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField("Excel 配表导出工具", EditorStyles.boldLabel);
            GUILayout.Space(4);

            // ====== 路径配置 ======
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("路径配置", EditorStyles.boldLabel);
                GUILayout.Space(2);

                _excelPath = DrawPathField("Excel表格目录", _excelPath, PrefKeyExcelPath);
                _outScriptPath = DrawPathField("导出脚本目录", _outScriptPath, PrefKeyOutScriptPath);
                _outTxtPath = DrawPathField("导出数据目录", _outTxtPath, PrefKeyOutTxtPath);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(6);

            // ====== 操作按钮 ======
            EditorGUI.BeginDisabledGroup(_isExporting);
            {
                if (GUILayout.Button("导出全部配表", GUILayout.Height(36)))
                {
                    ExportAll();
                }
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(6);

            // ====== 日志输出 ======
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("日志输出", EditorStyles.boldLabel);
                if (GUILayout.Button("清空", GUILayout.Width(50)))
                {
                    ClearLog();
                }
                EditorGUILayout.EndHorizontal();

                _logScrollPos = EditorGUILayout.BeginScrollView(_logScrollPos, GUILayout.ExpandHeight(true));
                foreach (var log in _logs)
                {
                    EditorGUILayout.LabelField(log, EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制路径选择行
        /// </summary>
        private string DrawPathField(string label, string path, string prefKey)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(90));
            EditorGUILayout.TextField(path);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                var selected = EditorUtility.OpenFolderPanel(label, path, "");
                if (!string.IsNullOrEmpty(selected))
                {
                    path = selected;
                    EditorPrefs.SetString(prefKey, path);
                }
            }
            EditorGUILayout.EndHorizontal();
            return path;
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        private void Log(string message)
        {
            _logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            SaveLogsToSession();
            // 自动滚动到底部
            _logScrollPos.y = float.MaxValue;
            Repaint();
        }

        private void SaveLogsToSession()
        {
            SessionState.SetString(SessionKeyLogs, string.Join("\n", _logs));
        }

        private void ClearLog()
        {
            _logs.Clear();
            SaveLogsToSession();
        }

        #region 导出逻辑

        /// <summary>
        /// 导出全部配表
        /// </summary>
        private void ExportAll()
        {
            if (string.IsNullOrWhiteSpace(_excelPath) || !Directory.Exists(_excelPath))
            {
                EditorUtility.DisplayDialog("错误", "Excel表格目录不存在，请检查路径配置。", "确定");
                return;
            }

            _isExporting = true;
            _logs.Clear();
            Log("开始导出...");

            try
            {
                _gameDataPath = Directory.GetParent(_outTxtPath).FullName;
                CreateAllTable(() =>
                {
                    Log("全部导出完成！");
                    AssetDatabase.Refresh();
                });
            }
            catch (Exception e)
            {
                Log($"导出异常：{e.Message}");
                Debug.LogException(e);
            }
            finally
            {
                _isExporting = false;
                Repaint();
            }
        }

        /// <summary>
        /// 创建全部表格数据
        /// </summary>
        private void CreateAllTable(Action completeAction)
        {
            string typeofTemp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：TableTypes.cs
 * 修改时间：#Time
 *********************************************/
using System;

namespace GameData
{
    /// <summary>
    /// 所有表格的Type
    /// </summary>
    public class TableTypes
    {
        public static Type[] TableCtrlTypeArr = new Type[]
        {
            #typeofStr
        };
    }
}";
            // 先删除目录文件夹（清空导出目录） 再创建数据目录
            if (Directory.Exists(_outScriptPath))
                Directory.Delete(_outScriptPath, true);
            if (Directory.Exists(_outTxtPath))
                Directory.Delete(_outTxtPath, true);
            Directory.CreateDirectory(Path.Combine(_gameDataPath, "Scripts"));
            Directory.CreateDirectory(_outScriptPath);
            Directory.CreateDirectory(_outTxtPath);

            bool isOutExcel = false;
            // 遍历全部表
            var directoryInfo = new DirectoryInfo(_excelPath);
            var fileInfoArr = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            string typeofStr = string.Empty;
            foreach (var fileInfo in fileInfoArr)
            {
                if (fileInfo.Extension == ".xlsx" && !fileInfo.Name.Contains("~$"))
                {
                    var newPath = fileInfo.FullName + ".temp";
                    // 如果有 先删除
                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                    }
                    File.Copy(fileInfo.FullName, newPath, true);
                    var newFileInfo = new FileInfo(newPath);

                    // 通过读取到的文件信息，打开Excel
                    using (ExcelPackage excelPackage = new ExcelPackage(newFileInfo))
                    {
                        // 循环遍历Sheet
                        for (int i = 0; i < excelPackage.Workbook.Worksheets.Count; i++)
                        {
                            var table = excelPackage.Workbook.Worksheets[i + 1];
                            var excelName = excelPackage.File.Name.Replace(".temp", "");
                            Log($"导出{excelName}中...");
                            CreateTableTxt(table);
                            CreateTableScript(table, excelName);
                            typeofStr += "typeof(" + CreateTableCtrlScript(table, excelName) + "),\r\n\t\t\t";
                            Log(table.Name + "已完成...");

                            isOutExcel = true;
                        }
                    }

                    newFileInfo.Delete();
                }
            }

            if (!isOutExcel)
            {
                Log("无需转换！");
                return;
            }

            // TableTypes.cs
            typeofStr = typeofTemp.Replace("#typeofStr", typeofStr);
            typeofStr = typeofStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            var file = File.CreateText(_outScriptPath + "/TableTypes.cs");
            file.WriteLine(typeofStr);
            file.Close();

            // 完成回调
            completeAction?.Invoke();
        }

        /// <summary>
        /// 创建Txt文件
        /// </summary>
        private void CreateTableTxt(ExcelWorksheet table)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 3, row = table.Dimension.End.Row; i <= row; i++)
            {
                // 如果首行是空行或注释行，就跳过
                var temp = table.Cells[i, 1].Value?.ToString();
                if (i > 5 && (temp == null || temp.StartsWith("#")))
                {
                    continue;
                }

                // 类型名的行 和 中文名的行 和 备注的行 跳过
                if (i == 4 || i == 5)
                {
                    continue;
                }

                for (int j = 1, column = table.Dimension.End.Column; j <= column; j++)
                {
                    // 空数据就结束
                    if (table.Cells[1, j].Value == null)
                    {
                        break;
                    }

                    string value = table.Cells[i, j].Value?.ToString().Trim();
                    // 服务端数据不导出
                    if (table.Cells[1, j].Value.ToString() == "S" || table.Cells[1, j].Value.ToString() == "NO")
                    {
                        continue;
                    }

                    sb.Append(value);
                    if (j != column && table.Cells[1, j + 1].Value != null)
                    {
                        sb.Append('^');
                    }
                }

                sb.Append("`");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            var outStr = sb.ToString().Trim();
            // 导出二进制文件
            using (var file = File.Create(_outTxtPath + "/" + table.Name + ".bytes"))
            {
                byte[] dataBuffer = Encoding.UTF8.GetBytes(outStr);
                file.Write(dataBuffer, 0, dataBuffer.Length);
                file.Close();
            }
        }

        /// <summary>
        /// 创建表控制器文件
        /// </summary>
        private string CreateTableCtrlScript(ExcelWorksheet table, string excelName)
        {
            string allTemp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：#ExcelName
 * 修改时间：#Time
 *********************************************/

using Framework;

namespace GameData
{
    public partial class #TableCtrlName : TableCtrlBase<#TableName>
    {
        public override string TableName => #TextName;
    }
}";
            // 分割后按首字母大写转换
            string tableCtrlName = "Table";
            var strArr = table.Name.Remove(0, 3).Split('_');
            for (int i = 0, length = strArr.Length; i < length; i++)
            {
                tableCtrlName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strArr[i]);
            }
            // 替换文本
            var outAllStr = allTemp.Replace("#TableName", tableCtrlName);
            tableCtrlName += "Ctrl";
            outAllStr = outAllStr.Replace("#TableCtrlName", tableCtrlName);
            outAllStr = outAllStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            outAllStr = outAllStr.Replace("#TextName", "\"" + table.Name + "\"");
            outAllStr = outAllStr.Replace("#ExcelName", excelName);

            var file = File.CreateText(_outScriptPath + "/" + tableCtrlName + ".cs");
            file.WriteLine(outAllStr);
            file.Close();

            return tableCtrlName;
        }

        /// <summary>
        /// 创建表文件
        /// </summary>
        private void CreateTableScript(ExcelWorksheet table, string excelName)
        {
            string allTemp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：#ExcelName
 * 修改时间：#Time
 *********************************************/

using Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameData
{
    public partial class #TableName : TableBase
    {#ValueStr
        public override void OnAwake(string[] nameGroupArr, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = nameGroupArr.Length; i < length; i++)
            {
                switch (nameGroupArr[i])
                {#CaseStr
                    default:
                        break;
                }
            }
        }
    }
}";
            string valueTemp = @"        
        /// <summary>
        /// #Remark
        /// </summary>
        public #Type #Name { private set; get; }
";

            string caseTemp = @"
                    case #tableValueName:
                        #Name = #Type;
                        break;
";

            // 拿到变量名
            string outValueStr = string.Empty;
            for (int i = 1, column = table.Dimension.End.Column; i <= column; i++)
            {
                if (table.Cells[1, i].Value == null)
                {
                    break;
                }

                if (table.Cells[1, i].Value.ToString() == "S" || table.Cells[1, i].Value.ToString() == "NO")
                {
                    continue;
                }

                var valueStr = valueTemp.Replace("#Type", GetValueType(table.Cells[2, i].Value.ToString()));
                valueStr = valueStr.Replace("#Name", table.Cells[3, i].Value.ToString());
                valueStr = valueStr.Replace("#Remark", table.Cells[4, i].Value?.ToString());
                outValueStr += valueStr;
            }
            outValueStr = outValueStr.Replace(" int Id { private set; get; }", " override int Id { protected set; get; }");

            // 拿到Case
            string outCaseStr = string.Empty;
            for (int i = 1, column = table.Dimension.End.Column; i <= column; i++)
            {
                if (table.Cells[1, i].Value == null)
                {
                    break;
                }

                if (table.Cells[1, i].Value.ToString() == "S" || table.Cells[1, i].Value.ToString() == "NO")
                {
                    continue;
                }

                var caseStr = caseTemp.Replace("#tableValueName", "\"" + table.Cells[3, i].Value.ToString() + "\"");
                caseStr = caseStr.Replace("#Name", table.Cells[3, i].Value.ToString());
                caseStr = caseStr.Replace("#Type", GetCaseType(table.Cells[2, i].Value.ToString()));
                outCaseStr += caseStr;
            }

            // 表名分割后按首字母大写转换
            string tableName = "Table";
            var strArr = table.Name.Remove(0, 3).Split('_');
            for (int i = 0, length = strArr.Length; i < length; i++)
            {
                tableName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strArr[i]);
            }
            // 替换文本
            var outAllStr = allTemp.Replace("#TableName", tableName);
            outAllStr = outAllStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            outAllStr = outAllStr.Replace("#ExcelName", excelName);
            outAllStr = outAllStr.Replace("#ValueStr", outValueStr);
            outAllStr = outAllStr.Replace("#CaseStr", outCaseStr);

            Log(_outScriptPath + "/" + tableName + ".cs");
            var file = File.CreateText(_outScriptPath + "/" + tableName + ".cs");
            file.WriteLine(outAllStr);
            file.Close();
        }

        #endregion

        #region 类型映射

        private static string GetValueType(string valueStr)
        {
            switch (valueStr)
            {
                case "int": return "int";
                case "long": return "long";
                case "float": return "float";
                case "bool": return "bool";
                case "string": return "string";
                case "vector2": return "Vector2";
                case "vector3": return "Vector3";
                case "array_string": return "List<string>";
                case "array_int": return "List<int>";
                case "array_long": return "List<long>";
                case "array_float": return "List<float>";
                case "composite_string": return "List<string[]>";
                case "composite_int": return "List<int[]>";
                default: return string.Empty;
            }
        }

        private static string GetCaseType(string typeStr)
        {
            switch (typeStr)
            {
                case "int": return "data[i].ToInt()";
                case "long": return "data[i].ToLong()";
                case "float": return "data[i].ToFloat()";
                case "bool": return "data[i].ToBool()";
                case "string": return "data[i]";
                case "vector2": return "data[i].ToVector2(Vector2.zero)";
                case "vector3": return "data[i].ToVector3(Vector3.zero)";
                case "array_string": return "data[i].Split(',').ToList()";
                case "array_int": return "data[i].SplitToIntList(',')";
                case "array_long": return "data[i].SplitToLongList(',')";
                case "array_float": return "data[i].SplitToFloatList(',')";
                case "composite_string": return "data[i].SplitToStringArrList('|', ',')";
                case "composite_int": return "data[i].SplitToIntArrList('|', ',')";
                default: return string.Empty;
            }
        }

        #endregion
    }
}
