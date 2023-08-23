/*********************************************
 * BFramework
 * 图片扩展类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 图片扩展类
    /// </summary>
    public class ImageEx : Image
    {
        //倾斜偏移
        public Vector3[] Offset;

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Offset = new Vector3[4];
        }
#endif

        /// <summary>
        /// 更新渲染器网格
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            //一般图片为四个顶点 左下=0 左上=1 右上=2 右下=3
            for (int i = 0; i < Offset.Length; i++)
            {
                UIVertex vertex = new UIVertex();
                toFill.PopulateUIVertex(ref vertex, i);
                vertex.position += Offset[i];
                toFill.SetUIVertex(vertex, i);
            }
        }
    }
}
