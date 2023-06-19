/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：K_卡牌表.xlsx.temp
 * 修改时间：2023/04/26 04:06:42
 *********************************************/

using Framework;

namespace GameData
{
    public partial class TableSuitCard : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 卡牌名
        /// </summary>
        public string Name { private set; get; }
        
        /// <summary>
        /// 花色类型
        /// </summary>
        public int Suit_Type { private set; get; }
        
        /// <summary>
        /// 卡牌说明
        /// </summary>
        public string Desc { private set; get; }
        
        /// <summary>
        /// 卡牌图片
        /// </summary>
        public string Asset { private set; get; }

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

                    case "suit_type":
                        Suit_Type = int.Parse(data[i]);;
                        break;

                    case "desc":
                        Desc = data[i];;
                        break;

                    case "asset":
                        Asset = data[i];;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
