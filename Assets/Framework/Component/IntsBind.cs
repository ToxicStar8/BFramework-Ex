/*********************************************
 * BFramework
 * Int数组绑定
 * 创建时间：2026/11/02 14:58:10
 *********************************************/
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Int数组绑定，需要在预制体上配置多个整数时使用
    /// </summary>
    public class IntsBind : MonoBehaviour
    {
        [SerializeField]
        public int[] Values;

        /// <summary>
        /// 说明
        /// </summary>
        [SerializeField]
        public string Desc;
    }
}
