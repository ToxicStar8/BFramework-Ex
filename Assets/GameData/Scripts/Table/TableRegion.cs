/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：D_地区表.xlsx.temp
 * 修改时间：2024/07/28 11:24:39
 *********************************************/

using Framework;

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
        /// 毒株名
        /// </summary>
        public string Poison_Name { private set; get; }
        
        /// <summary>
        /// 毒株说明
        /// </summary>
        public string Poison_Desc { private set; get; }

        public override void OnInit(string[] group, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = group.Length; i < length; i++)
            {
                switch (group[i])
                {
                    case "id":
                        Id = data[i].ToInt();;
                        break;

                    case "region":
                        Region = data[i];;
                        break;

                    case "poison_name":
                        Poison_Name = data[i];;
                        break;

                    case "poison_desc":
                        Poison_Desc = data[i];;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
