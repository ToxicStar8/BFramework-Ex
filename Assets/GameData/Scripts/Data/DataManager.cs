/*********************************************
 * BFramework
 * 数据管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using Framework;
using LitJson;
using MainPackage;
using System.IO;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public class DataList
    {
        /// <summary>
        /// 游玩总时间
        /// </summary>
        public float GamingTime;
        //直接在这里添加新的数据类型
        public PlayData PlayData;
        //统一回收
        public void OnDispose()
        {
            PlayData?.OnDispose();
        }
    }

    /// <summary>
    /// 数据管理器
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        /// <summary>
        /// 最后存档的时间（Time.time）
        /// </summary>
        public float LastSaveTime;

        /// <summary>
        /// 当前进行中的数据
        /// </summary>
        public static DataManager Instance { private set; get; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public DataList Data { private set; get; }

        public bool IsNullData => Data == null;

        #region 数据快捷访问
        //随用随取
        //这种方式获取避免新模块没有初始化
        public PlayData PlayData
        {
            get
            {
                if (Data.PlayData== null)
                {
                    Data.PlayData = new PlayData();
                    Data.PlayData.OnInit();
                }
                return Data.PlayData;
            }
        }
        #endregion

        /// <summary>
        /// 存档的路径
        /// </summary>
        private string _archivalPath;

        private void Awake()
        {
            Instance = this;
            gameObject.SetParent(GameEntry.Instance.transform);
            GameEntry.Instance.DisposeCallback += OnDispose;
            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //WebGL不允许使用IO类函数
            var jsonData = PlayerPrefs.GetString(ConstDefine.Archival, null);
            if(jsonData != null)
            {
                try
                {
                    Data = JsonMapper.ToObject<DataList>(jsonData);
                }
                catch
                {
                    GameEntry.Instance.Log(E_Log.Error, "游戏存档损坏，请检查");
                }
            }
#else
            //存档路径
            _archivalPath = Application.persistentDataPath + "/Archival.Json";
            //简单的存档模式：
            var fileInfo = new FileInfo(_archivalPath);
            //如果有存档 直接加载存档
            if (fileInfo.Exists)
            {
                using (var text = fileInfo.OpenText())
                {
                    var jsonData = text.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(jsonData))
                    {
                        try
                        {
                            Data = JsonMapper.ToObject<DataList>(jsonData);
                        }
                        catch
                        {
                            GameEntry.Instance.Log(E_Log.Error, "游戏存档损坏，请检查");
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 新建存档
        /// </summary>
        public DataList GetNewData()
        {
            //新存档需要将旧数据绑定的定时器之类的先取消
            Data?.OnDispose();
            //创建新存档
            LastSaveTime = Time.time;
            Data = new DataList();
            SaveDataToFile();
            return Data;
        }

        /// <summary>
        /// 存档
        /// </summary>
        public void SaveData()
        {
            //游玩时长
            Data.GamingTime += Time.time - LastSaveTime;
            //更新记录开始的时间
            LastSaveTime = Time.time;
            SaveDataToFile();
        }

        /// <summary>
        /// 存档至文件
        /// </summary>
        private void SaveDataToFile()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //WebGL不允许使用IO类函数
            PlayerPrefs.SetString(ConstDefine.Archival, JsonMapper.ToJson(Data));
#else
            File.WriteAllText(_archivalPath, JsonMapper.ToJson(Data));
#endif
        }

        /// <summary>
        /// 退出方法
        /// </summary>
        private void OnDispose()
        {
            SaveData();
            Data?.OnDispose();
        }
    }
}
