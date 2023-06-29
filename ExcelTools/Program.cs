using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using System.Globalization;

namespace ExcelTools
{
    public static class Program
    {
        private static string _toolsPath;       //工具路径
        private static string _excelPath;       //表格路径
        private static string _outTxtPath;      //导出Txt文件路径
        private static string _outScriptPath;   //导出代码文件路径

        public static void Main(string[] args)
        {
            _toolsPath = Directory.GetCurrentDirectory();
            //_toolsPath = "D:/Unity/CultivationRoguelike2D/ExcelTools";
            _excelPath = _toolsPath.Substring(0, _toolsPath.Length - "ExcelTools".Length);
            //_excelPath = _toolsPath.Substring(0, _toolsPath.Length - "ExcelTools".Length) + "/Table";
            _outScriptPath = _excelPath + "/ExcelTools/GameData/Scripts/Table";
            _outTxtPath = _excelPath + "/ExcelTools/GameData/Table";

            CreateAllTable(()=> {
                var config = File.OpenText(_toolsPath + "/config.txt");     //配置文件
                var src = config.ReadLine();

                var tablePath = _toolsPath + "/GameData/Scripts/Table";     //导出的表代码
                var gameTablePath = src + "/Scripts/Table";   //游戏的表代码
                Directory.Delete(gameTablePath, true);            //先删才能移
                Directory.Move(tablePath, gameTablePath);   //移动

                var txtPath = _toolsPath + "/GameData/Table";     //导出的表数据
                var gameTxtPath = src + "/Table";   //游戏的表数据
                Directory.Delete(gameTxtPath, true);            //先删才能移
                Directory.Move(txtPath, gameTxtPath);   //移动

                config.Close();
                Console.WriteLine("全部导出完成！");
            });
            Console.ReadKey();
        }

        /// <summary>
        /// 创建全部表格数据
        /// </summary>
        public static void CreateAllTable(Action completeAction)
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
            //先删除目录文件夹（清空数据） 再创建数据目录
            var gdPath = _toolsPath + "/GameData";
            if(Directory.Exists(gdPath))
            {
                Directory.Delete(gdPath,true);
            }
            Directory.CreateDirectory(gdPath);
            Directory.CreateDirectory(gdPath + "/Scripts");
            Directory.CreateDirectory(gdPath + "/Scripts/Table");
            Directory.CreateDirectory(gdPath + "/Table");

            bool isOutExcel = false;
            //遍历全部表
            var directoryInfo = new DirectoryInfo(_excelPath);
            var fileInfoArr = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            string typeofStr = string.Empty;
            foreach (var fileInfo in fileInfoArr)
            {
                if (fileInfo.Extension == ".xlsx" && !fileInfo.Name.Contains("~$"))
                {
                    var newPath = fileInfo.FullName + ".temp";
                    //如果有 先删除
                    if (Directory.Exists(newPath))
                    {
                        Directory.Delete(newPath, true);
                    }
                    File.Copy(fileInfo.FullName, newPath);
                    var newFileInfo = new FileInfo(newPath);

                    //通过读取到的文件信息，打开Excel
                    using (ExcelPackage excelPackage = new ExcelPackage(newFileInfo))
                    {
                        //循环遍历Sheet
                        for (int i = 0; i < excelPackage.Workbook.Worksheets.Count; i++)
                        {
                            var table = excelPackage.Workbook.Worksheets[i + 1];
                            Console.WriteLine("导出" + excelPackage.File.Name + "中...");
                            CreateTableTxt(table);
                            CreateTableScript(table, excelPackage.File.Name);
                            typeofStr +=  "typeof(" + CreateTableCtrlScript(table, excelPackage.File.Name) + "),\r\n\t\t\t";
                            Console.WriteLine(table.Name + "已完成...");

                            isOutExcel = true;
                        }
                    }

                    newFileInfo.Delete();
                }
            }

            if (!isOutExcel)
            {
                Console.WriteLine("无需转换！");
                return;
            }

            //TableTypes.cs
            typeofStr = typeofTemp.Replace("#typeofStr", typeofStr);
            typeofStr = typeofStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            var file = File.CreateText(_outScriptPath + $"/TableTypes.cs");
            file.WriteLine(typeofStr.ToString());
            file.Close();

            //完成回调
            completeAction?.Invoke();
        }

        /// <summary>
        /// 创建Txt文件
        /// </summary>
        private static void CreateTableTxt(ExcelWorksheet table)
        {
            StringBuilder sb = new StringBuilder();
            //End.Row获得当前表格的最大行数
            //第一行标识服务端|客户端数据 不参与导出
            for (int i = 2, row = table.Dimension.End.Row; i <= row; i++)
            {
                //备注行跳过
                if(i == 5)
                {
                    continue;
                }

                //End.Row获得当前表格的最大列数
                for (int j = 1, column = table.Dimension.End.Column; j <= column; j++)
                {
                    //空数据就结束
                    if (table.Cells[1, j].Value == null)
                    {
                        break;
                    }

                    //Cells是个二维数组，第一个参数是读取第几行，第二个参数是读取第几列（从1开始
                    string value = table.Cells[i, j].Value?.ToString();
                    //服务端数据不导出
                    if (table.Cells[1, j].Value.ToString() == "S" || table.Cells[1, j].Value.ToString() == "NO")
                    {
                        continue;
                    }

                    sb.Append(value);
                    //只要不是最后一列并且最后一列不为空 就添加逗号
                    if (j != column && table.Cells[1, j + 1].Value != null)
                    {
                        sb.Append('^');
                    }
                }

                //只要不是最后一行 就添加换行符
                if (i < row)
                {
                    sb.Append("\r\n");
                }
            }

            var file = File.CreateText(_outTxtPath + $"/{table.Name}.txt");
            file.WriteLine(sb.ToString());
            file.Close();
        }

        /// <summary>
        /// 创建表控制器文件
        /// </summary>
        /// <param name="table"></param>
        private static string CreateTableCtrlScript(ExcelWorksheet table,string excelName)
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
            //分割后按首字母大写转换
            string tableCtrlName = "Table";
            var strArr = table.Name.Remove(0, 3).Split('_');
            for (int i = 0, length = strArr.Length; i < length; i++)
            {
                tableCtrlName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strArr[i]);
            }
            //替换文本
            var outAllStr = allTemp.Replace("#TableName", tableCtrlName);
            tableCtrlName += "Ctrl";
            outAllStr = outAllStr.Replace("#TableCtrlName", tableCtrlName);
            outAllStr = outAllStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            outAllStr = outAllStr.Replace("#TextName", "\"" + table.Name + ".txt\"");
            outAllStr = outAllStr.Replace("#ExcelName", excelName);

            var file = File.CreateText(_outScriptPath + $"/{tableCtrlName}.cs");
            file.WriteLine(outAllStr.ToString());
            file.Close();

            return tableCtrlName;
        }

        /// <summary>
        /// 创建表文件
        /// </summary>
        /// <param name="table"></param>
        private static void CreateTableScript(ExcelWorksheet table, string excelName)
        {
            string allTemp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：#ExcelName
 * 修改时间：#Time
 *********************************************/

using Framework;

namespace GameData
{
    public partial class #TableName : TableBase
    {#ValueStr
        public override void OnInit(string[] group, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = group.Length; i < length; i++)
            {
                switch (group[i])
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

            //拿到变量名
            string outValueStr = string.Empty;
            for (int i = 1,column = column = table.Dimension.End.Column; i <= column; i++)
            {
                //空数据就结束
                if (table.Cells[1, i].Value == null)
                {
                    break;
                }

                //服务端数据不导出
                if (table.Cells[1, i].Value.ToString() == "S" || table.Cells[1, i].Value.ToString() == "NO")
                {
                    continue;
                }

                //具体导出
                var valueStr = valueTemp.Replace("#Type", GetValueType(table.Cells[2,i].Value.ToString()));
                valueStr = valueStr.Replace("#Name", GetValueName(table.Cells[2, i].Value.ToString(), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(table.Cells[3, i].Value.ToString())));
                valueStr = valueStr.Replace("#Remark", table.Cells[4, i].Value?.ToString());
                outValueStr += valueStr;
            }
            outValueStr = outValueStr.Replace(" int Id { private set; get; }", " override int Id { protected set; get; }");

            //拿到Case
            string outCaseStr = string.Empty;
            for (int i = 1, column = column = table.Dimension.End.Column; i <= column; i++)
            {
                //空数据就结束
                if (table.Cells[1, i].Value == null)
                {
                    break;
                }

                //服务端数据不导出
                if (table.Cells[1, i].Value.ToString() == "S" || table.Cells[1, i].Value.ToString() == "NO")
                {
                    continue;
                }

                var caseStr = caseTemp.Replace("#tableValueName", "\"" + table.Cells[3, i].Value.ToString()+ "\"");
                caseStr = caseStr.Replace("#Name", GetValueName(table.Cells[2, i].Value.ToString(), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(table.Cells[3, i].Value.ToString())));
                caseStr = caseStr.Replace("#Type", GetCaseType(table.Cells[2, i].Value.ToString()));
                outCaseStr += caseStr;
            }

            //表名分割后按首字母大写转换
            string tableName = "Table";
            var strArr = table.Name.Remove(0, 3).Split('_');
            for (int i = 0, length = strArr.Length; i < length; i++)
            {
                tableName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strArr[i]);
            }
            //替换文本
            var outAllStr = allTemp.Replace("#TableName", tableName);
            outAllStr = outAllStr.Replace("#Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            outAllStr = outAllStr.Replace("#ExcelName", excelName);
            outAllStr = outAllStr.Replace("#ValueStr", outValueStr);
            outAllStr = outAllStr.Replace("#CaseStr", outCaseStr);

            Console.WriteLine(_outScriptPath + $"/{tableName}.cs");
            var file = File.CreateText(_outScriptPath + $"/{tableName}.cs");
            file.WriteLine(outAllStr.ToString());
            file.Close();
        }

        /// <summary>
        /// 根据传入类型 返回转换文本
        /// </summary>
        private static string GetValueType(string valueStr)
        {
            string outValueStr = string.Empty;
            switch (valueStr)
            {
                case "int":
                    outValueStr = "int";
                    break;

                case "float":
                    outValueStr = "float";
                    break;

                case "string":
                    outValueStr = "string";
                    break;

                case "array":
                    outValueStr = "string";
                    break;
            }
            return outValueStr;
        }

        /// <summary>
        /// 根据传入名字 返回转换文本
        /// </summary>
        private static string GetValueName(string valueStr,string name)
        {
            string outValueStr = string.Empty;
            switch (valueStr)
            {
                case "int":
                    outValueStr = name;
                    break;

                case "float":
                    outValueStr = name;
                    break;

                case "string":
                    outValueStr = name;
                    break;

                case "array":
                    outValueStr = "Array_"+ name;
                    break;
            }
            return outValueStr;
        }

        /// <summary>
        /// 根据传入类型 返回转换文本
        /// </summary>
        private static string GetCaseType(string typeStr)
        {
            string outTypeStr = string.Empty;
            switch (typeStr)
            {
                case "int":
                    outTypeStr = "data[i].ToInt();";
                    break;

                case "float":
                    outTypeStr = "data[i].ToFloat();";
                    break;

                case "string":
                    outTypeStr = "data[i];";
                    break;

                case "array":
                    outTypeStr = "data[i];";
                    break;
            }
            return outTypeStr;
        }
    }
}
