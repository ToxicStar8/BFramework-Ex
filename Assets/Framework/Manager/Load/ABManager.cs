/*********************************************
 * BFramework
 * AB包管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ABManager : ManagerBase
    {
        /// <summary>
        /// AB包信息
        /// </summary>
        public ABInfo ABInfo { private set; get; }

        /// <summary>
        /// 缓存，避免线性查找依赖
        /// </summary>
        public Dictionary<string, ABInfo.ABRelyInfo> ABRelyInfoDic { private set; get; }

        public override void OnAwake()
        {
            if (!GameEntry.Instance.IsEditorMode || GameEntry.Instance.IsRunABPackage)
            {
                //非编辑器模式或者加载AB包模式下直接加载AB包索引信息
                var abPackage = AssetBundle.LoadFromFile(GameEntry.Instance.DowloadManager.SavePath + ConstDefine.ABInfoName);
                if (abPackage == null)
                {
                    GameManager.Instance.Log(E_Log.Error, "没有找到AB包！");
                    return;
                }
                var textAsset = abPackage.LoadAsset<TextAsset>(ConstDefine.ABInfoName + ".bytes");
                //转化为ABInfo（AB包的索引
                ABInfo = JsonConvert.DeserializeObject<ABInfo>(textAsset.text);
                abPackage.Unload(true);

                ABRelyInfoDic = new Dictionary<string, ABInfo.ABRelyInfo>();
                foreach (var info in ABInfo.ABRelyInfoList)
                {
                    ABRelyInfoDic[info.ABName] = info;
                }
            }
        }

        public override void OnUpdate() { }
        public override void OnDispose() { }
    }
}
