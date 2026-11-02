/*********************************************
 * BFramework
 * Float绑定
 * 创建时间：2026/11/02 14:58:10
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// Float绑定
    /// </summary>
    public class FloatBind : MonoBehaviour
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [SerializeField]
        public float Value;

        /// <summary>
        /// 说明
        /// </summary>
        [SerializeField]
        public string Desc;
    }
}
