/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：D_地区表.xlsx
 * 修改时间：2024/08/02 11:15:17
 *********************************************/

using Framework;
using System.Collections.Generic;
using System.Linq;

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
        public List<string> List_Test1 { private set; get; }
        
        /// <summary>
        /// 测试2
        /// </summary>
        public List<int> List_Test2 { private set; get; }
        
        /// <summary>
        /// 测试3
        /// </summary>
        public List<long> List_Test3 { private set; get; }
        
        /// <summary>
        /// 测试4
        /// </summary>
        public List<float> List_Test4 { private set; get; }
        
        /// <summary>
        /// 测试5
        /// </summary>
        public List<string[]> List_Test5 { private set; get; }
        
        /// <summary>
        /// 测试6
        /// </summary>
        public List<int[]> List_Test6 { private set; get; }

        public override void OnInit(string[] nameGroupArr, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = nameGroupArr.Length; i < length; i++)
            {
                switch (nameGroupArr[i])
                {
                    case "id":
                        Id = data[i].ToInt();
                        break;

                    case "region":
                        Region = data[i];
                        break;

                    case "test1":
                        List_Test1 = data[i].Split(',').ToList();
                        break;

                    case "test2":
                        List_Test2 = data[i].SplitToIntList(',');
                        break;

                    case "test3":
                        List_Test3 = data[i].SplitToLongList(',');
                        break;

                    case "test4":
                        List_Test4 = data[i].SplitToFloatList(',');
                        break;

                    case "test5":
                        List_Test5 = data[i].SplitToStringArrList('|', ',');
                        break;

                    case "test6":
                        List_Test6 = data[i].SplitToIntArrList('|', ',');
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
