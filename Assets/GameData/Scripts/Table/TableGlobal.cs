/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：Q_全局表.xlsx.temp
 * 修改时间：2023/04/26 04:06:42
 *********************************************/

using Framework;

namespace GameData
{
    public partial class TableGlobal : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 层，行，列
        /// </summary>
        public string Map_Max { private set; get; }

        public override void OnInit(string[] group, string dataStrArr)
        {
            var data = dataStrArr.Split('^');
            for (int i = 0,length = group.Length; i < length; i++)
            {
                switch (group[i])
                {
                    case "id":
                        Id = int.Parse(data[i]);;
                        break;

                    case "map_max":
                        Map_Max = data[i];;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
