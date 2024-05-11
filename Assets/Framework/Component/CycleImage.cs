/*********************************************
 * BFramework
 * 把矩形Image改成圆形image，圆形大小内嵌于矩形框内。由于顶点数量增多，不宜大量使用，但是减少了DC。
 * 创建时间：2024/05/11 16:10:23
 *********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

public class CycleImage : Image
{
    // 每个角最大的三角形数
    [Range(1, 100)] public int cornerSegments = 30;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Rect rect = this.rectTransform.rect;
        float left = rect.xMin;
        float right = rect.xMax;
        float top = rect.yMax;
        float bottom = rect.yMin;

        Vector4 outerUV = this.overrideSprite != null ? DataUtility.GetOuterUV(this.overrideSprite) : new Vector4(0, 0, 1, 1);
        var color32 = color;
        float r = Mathf.Min(rect.width, rect.height) / 2;
        // 计算uv中对应的半径值坐标轴的半径
        float uvRadiusX = r / (right - left) * (outerUV.z - outerUV.x);
        float uvRadiusY = r / (top - bottom) * (outerUV.w - outerUV.y);

        vh.Clear();

        // 从左往右
        // 0 1 2 3
        //Vector3 centerPos = new Vector3(left, bottom) + new Vector3(rect.width, rect.height)/2;       // 中心点位置
        Vector3 centerPos = new Vector3(0, 0);
        Vector2 centerUV = new Vector2(outerUV.x + (outerUV.z - outerUV.x) / 2, outerUV.y + (outerUV.w - outerUV.y) / 2);
        vh.AddVert(centerPos, color32, centerUV);

        float degreeDelta = Mathf.PI * 2 / this.cornerSegments;
        float cornerDegree = 0;
        // 多两段补全第一个和最后一个
        for (int i = 0; i < this.cornerSegments + 2; i++)
        {
            float cos = Mathf.Cos(cornerDegree);
            float sin = Mathf.Sin(cornerDegree);
            Vector3 position = new Vector3(centerPos.x + cos * r, centerPos.y + sin * r);
            Vector3 uv0 = new Vector2(centerUV.x + cos * uvRadiusX, centerUV.y + sin * uvRadiusY);
            vh.AddVert(position, color32, uv0);
            cornerDegree += degreeDelta;
            if (i > 0)
            {
                vh.AddTriangle(0, i, i - 1);
            }
        }
    }
    
    /// <summary>
    /// 设置圆形细节
    /// </summary>
    /// <param name="cornerSeg"></param>
    public void SetDetail(int cornerSeg)
    {
        cornerSegments = cornerSeg;
        SetVerticesDirty();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CycleImage), true)]
    public class CycleImageEditor : ImageEditor
    {
        SerializedProperty _sprite;
        SerializedProperty _cornerSegments;

        protected override void OnEnable()
        {
            base.OnEnable();

            this._sprite = this.serializedObject.FindProperty("m_Sprite");
            this._cornerSegments = this.serializedObject.FindProperty("cornerSegments");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            this.SpriteGUI();
            this.AppearanceControlsGUI();
            this.RaycastControlsGUI();
            bool showNativeSize = this._sprite.objectReferenceValue != null;
            this.m_ShowNativeSize.target = showNativeSize;
            this.MaskableControlsGUI();
            this.NativeSizeButtonGUI();
            EditorGUILayout.PropertyField(this._cornerSegments);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}