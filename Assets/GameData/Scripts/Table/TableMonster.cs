/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：G_怪物表.xlsx.temp
 * 修改时间：2023/04/26 04:06:42
 *********************************************/

using Framework;

namespace GameData
{
    public partial class TableMonster : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 怪物名
        /// </summary>
        public string Name { private set; get; }
        
        /// <summary>
        /// 怪物资源
        /// </summary>
        public string Asset { private set; get; }
        
        /// <summary>
        /// 怪物说明
        /// </summary>
        public string Desc { private set; get; }
        
        /// <summary>
        /// 血量基础值
        /// </summary>
        public int Hp { private set; get; }
        
        /// <summary>
        /// 速度基础值
        /// </summary>
        public int Speed { private set; get; }
        
        /// <summary>
        /// 被影响的系数比（百分比）
        /// </summary>
        public int Factor_Scale { private set; get; }

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

                    case "asset":
                        Asset = data[i];;
                        break;

                    case "desc":
                        Desc = data[i];;
                        break;

                    case "hp":
                        Hp = int.Parse(data[i]);;
                        break;

                    case "speed":
                        Speed = int.Parse(data[i]);;
                        break;

                    case "factor_scale":
                        Factor_Scale = int.Parse(data[i]);;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
