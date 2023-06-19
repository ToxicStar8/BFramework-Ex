/*********************************************
 * 
 * 地图管理器
 * 创建时间：2023/04/25 15:04:23
 *********************************************/
using Cinemachine;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameData
{
    public partial class GameManager
    {
        public class MonsterInfo
        {

        }

        //当前的地图
        public GameObject CurMapGo { private set; get; }
        public Transform CurMapTrans { private set; get; }
        public PolygonCollider2D CurMapEdge { private set; get; }
        //当前的怪物列表
        public List<MonsterInfo> MonsterInfoList { private set; get; }

        /// <summary>
        /// 初始化地图
        /// </summary>
        private void InitMap()
        {
            //后面可能想要 地图消失兑换伤害提高/伤害降低兑换地图增加

            //初始化地图
            CurMapGo = UnityEngine.Object.Instantiate(LoadHelper.LoadSync<GameObject>(AssetName.Prefab_Map_01));
            CurMapTrans = CurMapGo.transform;
            //地图边界初始化
            CurMapEdge = CurMapTrans.Find("Edge").GetComponent<PolygonCollider2D>();
            var edge = VirtualCamera.GetComponent<CinemachineConfiner2D>();
            edge.m_BoundingShape2D = CurMapEdge;
        }

        /// <summary>
        /// 初始化刷怪信息
        /// </summary>
        private void InitMonsterInfo()
        {
            //刷怪定时器
            var timeCountdown = GameGod.Instance.PoolManager.CreateClassObj<TimerInfo>();
            timeCountdown.Init(-1, 3, true, OnCreateMonster);
            AddTimer(ConstDefine.CreateMonsterTimeKey, timeCountdown);
        }

        /// <summary>
        /// 定时刷怪方法
        /// </summary>
        private void OnCreateMonster()
        {
            var monster = Monster_01.CreateEntity<Monster_01>(LoadHelper, AssetName.Prefab_Monster_01);
            monster.transform.position = FnGetRandomPosInMap();
            //monster.OnDispose();
            //RecycleClassObj(monster);
        }

        /// <summary>
        /// 获得地图边缘上随机一个点
        /// </summary>
        private Vector2 FnGetRandomPosInMap()
        {
            //随机一个点
            var x = CurMapEdge.bounds.size.x * 0.5f;
            var y = CurMapEdge.bounds.size.y * 0.5f;
            var rdX = UnityEngine.Random.Range(-x, x);
            var rdY = UnityEngine.Random.Range(-y, y);
            var rdPos = new Vector2(rdX, rdY);
            //边缘处理
            if (Mathf.Abs(rdPos.x) > Mathf.Abs(rdPos.y))
            {
                rdPos.x = rdPos.x > 0 ? x - 1 : -x + 1;
            }
            else
            {
                rdPos.y = rdPos.y > 0 ? y - 1 : -y + 1;
            }
            return rdPos;
        }
    }
}