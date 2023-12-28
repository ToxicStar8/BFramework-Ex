/*********************************************
 * BFramework
 * 前缀树节点
 * 创建时间：2023/12/14 16:01:23
 *********************************************/
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 前缀树节点
    /// </summary>
    public class TrieTreeNode
    {
        public TrieTreeNode(TrieTreeNode parent, string key)
        {
            Parent = parent;
            Key = key;
            Value = 0;
            ChildrenDic = new Dictionary<string, TrieTreeNode>();
        }

        /// <summary>
        /// 节点名
        /// </summary>
        public string Key { private set; get; }

        /// <summary>
        /// 值
        /// </summary>
        public int Value { private set; get; }

        /// <summary>
        /// 父节点
        /// </summary>
        public TrieTreeNode Parent { private set; get; }

        /// <summary>
        /// 子节点字典
        /// </summary>
        public Dictionary<string, TrieTreeNode> ChildrenDic { private set; get; }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器用 是否展开
        /// </summary>
        public bool IsExpanded = true;
#endif

        /// <summary>
        /// 获取或添加子节点
        /// </summary>
        /// <param name="key">节点名</param>
        public TrieTreeNode GetOrAddNode(string key)
        {
            if (!ChildrenDic.TryGetValue(key, out var child))
            {
                child = new TrieTreeNode(this, key);
                ChildrenDic.Add(key, child);
            }
            return child;
        }

        /// <summary>
        /// 递归遍历树，获取节点
        /// </summary>
        /// <param name="key">目标节点的名字</param>
        /// <returns></returns>
        public TrieTreeNode FindNodeByKey(string key)
        {
            //是当前节点，直接返回
            if(key == Key)
            {
                return this;
            }

            //遍历子节点寻找
            TrieTreeNode tempNode = null;
            foreach (var node in ChildrenDic)
            {
                tempNode = node.Value.FindNodeByKey(key);
                //找到了
                if (tempNode != null && tempNode.Key == key)
                {
                    return tempNode;
                }
            }
            //没找着
            return null;
        }

        /// <summary>
        /// 移除指定节点
        /// </summary>
        public void RemoveNode(string key)
        {
            if (ChildrenDic.TryGetValue(key, out var child))
            {
                child.OnDispose();
                ChildrenDic.Remove(key);
            }
        }

        public void OnDispose()
        {
            foreach (var node in ChildrenDic)
            {
                node.Value.OnDispose();
            }
            ChildrenDic.Clear();
            ChildrenDic = null;
        }
    }
}