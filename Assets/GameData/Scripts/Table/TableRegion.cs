/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：D_地区表.xlsx
 * 修改时间：2026/03/03 05:44:57
 *********************************************/

using Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameData
{
    public partial class TableRegion : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 地区
        /// </summary>
        public string Region { private set; get; }
        
        /// <summary>
        /// 测试1
        /// </summary>
        public List<string> Test1 { private set; get; }
        
        /// <summary>
        /// 测试2
        /// </summary>
        public List<int> Test2 { private set; get; }
        
        /// <summary>
        /// 测试3
        /// </summary>
        public List<long> Test3 { private set; get; }
        
        /// <summary>
        /// 测试4
        /// </summary>
        public List<float> Test4 { private set; get; }
        
        /// <summary>
        /// 测试5
        /// </summary>
        public List<string[]> Test5 { private set; get; }
        
        /// <summary>
        /// 测试6
        /// </summary>
        public List<int[]> Test6 { private set; get; }
        
        /// <summary>
        /// 测试7
        /// </summary>
        public Vector2 Test7 { private set; get; }
        
        /// <summary>
        /// 测试8
        /// </summary>
        public Vector3 Test8 { private set; get; }

        public override void OnAwake(string[] nameGroupArr, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = nameGroupArr.Length; i < length; i++)
            {
                switch (nameGroupArr[i])
                {
                    case "Id":
                        Id = data[i].ToInt();
                        break;

                    case "Region":
                        Region = data[i];
                        break;

                    case "Test1":
                        Test1 = data[i].Split(',').ToList();
                        break;

                    case "Test2":
                        Test2 = data[i].SplitToIntList(',');
                        break;

                    case "Test3":
                        Test3 = data[i].SplitToLongList(',');
                        break;

                    case "Test4":
                        Test4 = data[i].SplitToFloatList(',');
                        break;

                    case "Test5":
                        Test5 = data[i].SplitToStringArrList('|', ',');
                        break;

                    case "Test6":
                        Test6 = data[i].SplitToIntArrList('|', ',');
                        break;

                    case "Test7":
                        Test7 = data[i].ToVector2(Vector2.zero);
                        break;

                    case "Test8":
                        Test8 = data[i].ToVector3(Vector3.zero);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
