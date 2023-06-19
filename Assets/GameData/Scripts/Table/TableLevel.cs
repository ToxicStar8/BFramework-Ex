/*********************************************
 * 自动生成代码，禁止手动修改文件
 * Excel表名：G_关卡表.xlsx.temp
 * 修改时间：2023/04/26 04:06:42
 *********************************************/

using Framework;

namespace GameData
{
    public partial class TableLevel : TableBase
    {        
        /// <summary>
        /// 
        /// </summary>
        public override int Id { protected set; get; }
        
        /// <summary>
        /// 难度级别
        /// </summary>
        public int Difficulty { private set; get; }
        
        /// <summary>
        /// 轮数
        /// </summary>
        public int Class { private set; get; }
        
        /// <summary>
        /// 本轮开始后的刷新时间
        /// </summary>
        public int Exec_Time { private set; get; }
        
        /// <summary>
        /// 怪物种类和数量
        /// </summary>
        public string Array_Monster_Info { private set; get; }
        
        /// <summary>
        /// 轮数难度系数（百分比）
        /// </summary>
        public int Class_Factor { private set; get; }

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

                    case "difficulty":
                        Difficulty = int.Parse(data[i]);;
                        break;

                    case "class":
                        Class = int.Parse(data[i]);;
                        break;

                    case "exec_time":
                        Exec_Time = int.Parse(data[i]);;
                        break;

                    case "monster_info":
                        Array_Monster_Info = data[i];;
                        break;

                    case "class_factor":
                        Class_Factor = int.Parse(data[i]);;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
