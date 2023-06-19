/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：G_关卡表.xlsx.temp
 * 修改时间：2023/04/26 04:06:42
 *********************************************/

using Framework;

namespace GameData
{
    public partial class TableDifficulty : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 难度名
        /// </summary>
        public string Name { private set; get; }
        
        /// <summary>
        /// 难度系数（百分比）
        /// </summary>
        public int Factor { private set; get; }

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

                    case "name":
                        Name = data[i];;
                        break;

                    case "factor":
                        Factor = int.Parse(data[i]);;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
