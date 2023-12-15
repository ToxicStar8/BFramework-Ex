/*********************************************
 * BFramework
 * 红点管理器，使用前缀树实现
 * 创建时间：2023/12/14 16:00:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 红点管理器
    /// </summary>
    public class RedPointManager : ManagerBase
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public TrieTreeNode RootNode { get; private set; }  

        public override void OnInit()
        {
            RootNode = new TrieTreeNode(null, "Root");
        }

        public override void OnUpdate()
        {

        }

        /// <summary>
        /// 添加节点，根据传入的路径路由
        /// </summary>
        /// <param name="paths">全路径</param>
        /// 例：UIMainMenu => UI_A1 => UI_a1  (UIMainMenu,UI_A1,UI_a1)
        ///                         => UI_a2  (UIMainMenu,UI_A1,UI_a2)
        ///                => UI_A2 => UI_aa  (UIMainMenu,UI_A2,UI_aa)
        ///                         => UI_ab  (UIMainMenu,UI_A2,UI_ab)
        public TrieTreeNode AddOrGetNodeByFullPath(params string[] paths)
        {
            //从根节点开始搜索
            var tempNode = RootNode;
            foreach (var key in paths)
            {
                //因为是全路径搜索，所以如果没有节点就直接添加，不会重复添加
                tempNode = tempNode.GetOrAddNode(key);
            }
            //全路径检索完，最后的就是刚添加的节点，返回
            return tempNode;
        }

        /// <summary>
        /// 添加节点,根据父节点名
        /// </summary>
        /// <param name="parent">目标父节点名</param>
        /// <param name="key">目标名</param>
        /// <returns></returns>
        public TrieTreeNode AddOrGetNodeByParentKey(string parentKey, string key)
        {
            var child = RootNode.FindNodeByKey(parentKey);
            if (child == null)
            {
                GameGod.Instance.Log(E_Log.Error, "没有找到父节点", parentKey);
                return null;
            }
            child = child.GetOrAddNode(key);
            return child;
        }

        /// <summary>
        /// 添加节点,根据父节点名
        /// </summary>
        /// <typeparam name="T">目标节点类</typeparam>
        /// <param name="parentKey">目标的父节点名</param>
        /// <returns></returns>
        public TrieTreeNode AddOrGetNodeByParentKey<T>(string parentKey)
        {
            var key = typeof(T).Name;
            var child = AddOrGetNodeByParentKey(parentKey, key);
            return child;
        }

        /// <summary>
        /// 添加节点,根据父节点
        /// </summary>
        /// <param name="parent">目标父节点</param>
        /// <param name="key">目标名</param>
        /// <returns></returns>
        public TrieTreeNode AddOrGetNodeByParent(TrieTreeNode parent, string key)
        {
            var child = AddOrGetNodeByParentKey(parent.Key, key);
            return child;
        }

        /// <summary>
        /// 移除节点,根据节点名
        /// </summary>
        public void RemoveNode(string key)
        {
            var child = RootNode.FindNodeByKey(key);
            if (child == null)
            {
                GameGod.Instance.Log(E_Log.Error, "没有找到节点", key);
                return;
            }
            child.Parent.RemoveNode(key);
        }

        /// <summary>
        /// 移除节点,根据节点名
        /// </summary>
        public void RemoveNode<T>()
        {
            var key = typeof(T).Name;
            RemoveNode(key);
        }

        /// <summary>
        /// 移除所有节点
        /// </summary>
        public void RemoveAllNode()
        {
            foreach (var node in RootNode.ChildrenDic)
            {
                node.Value.OnDispose();
            }
            RootNode.ChildrenDic.Clear();
        }

        public override void OnDispose()
        {
            RootNode.OnDispose();
            RootNode = null;
        }
    }
}