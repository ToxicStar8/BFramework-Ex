/*********************************************
 * BFramework
 * 把矩形的四个直角修改成圆角，由于顶点数量增多，不宜大量使用，但是减少了DC。
 * 创建时间：2024/05/11 16:11:23
 *********************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CornerRadiusImage : Image
{
    public float cornerRadius = 8;

    // 每个角最大的三角形数
    [Range(1, 40)] public int cornerSegments = 8;

    public bool leftTop = true;
    public bool rightTop = true;
    public bool leftBottom = true;
    public bool rightBottom = true;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Rect rect = this.rectTransform.rect;
        //Debug.Log(rect);
        float left = rect.xMin;
        float right = rect.xMax;
        float top = rect.yMax;
        float bottom = rect.yMin;

        Vector4 outerUV = this.overrideSprite != null ? DataUtility.GetOuterUV(this.overrideSprite) : new Vector4(0, 0, 1, 1);
        var color32 = color;
        float r = this.cornerRadius;
        // 计算uv中对应的半径值坐标轴的半径
        float uvRadiusX = r / (right - left) * (outerUV.z - outerUV.x);
        float uvRadiusY = r / (top - bottom) * (outerUV.w - outerUV.y);

        vh.Clear();

        // 从左往右
        // 0 1 2 3
        vh.AddVert(new Vector3(left, top), color32, new Vector2(outerUV.x, outerUV.w));
        vh.AddVert(new Vector3(left, top - r), color32, new Vector2(outerUV.x, outerUV.w - uvRadiusY));
        vh.AddVert(new Vector3(left, bottom + r), color32, new Vector2(outerUV.x, outerUV.y + uvRadiusY));
        vh.AddVert(new Vector3(left, bottom), color32, new Vector2(outerUV.x, outerUV.y));

        // 4 5 6 7
        vh.AddVert(new Vector3(left + r, top), color32, new Vector2(outerUV.x + uvRadiusX, outerUV.w));
        vh.AddVert(new Vector3(left + r, top - r), color32,
            new Vector2(outerUV.x + uvRadiusX, outerUV.w - uvRadiusY));
        vh.AddVert(new Vector3(left + r, bottom + r), color32,
            new Vector2(outerUV.x + uvRadiusX, outerUV.y + uvRadiusY));
        vh.AddVert(new Vector3(left + r, bottom), color32, new Vector2(outerUV.x + uvRadiusX, outerUV.y));

        // 8 9 10 11
        vh.AddVert(new Vector3(right - r, top), color32, new Vector2(outerUV.z - uvRadiusX, outerUV.w));
        vh.AddVert(new Vector3(right - r, top - r), color32,
            new Vector2(outerUV.z - uvRadiusX, outerUV.w - uvRadiusY));
        vh.AddVert(new Vector3(right - r, bottom + r), color32,
            new Vector2(outerUV.z - uvRadiusX, outerUV.y + uvRadiusY));
        vh.AddVert(new Vector3(right - r, bottom), color32, new Vector2(outerUV.z - uvRadiusX, outerUV.y));

        // 12 13 14 15
        vh.AddVert(new Vector3(right, top), color32, new Vector2(outerUV.z, outerUV.w));
        vh.AddVert(new Vector3(right, top - r), color32, new Vector2(outerUV.z, outerUV.w - uvRadiusY));
        vh.AddVert(new Vector3(right, bottom + r), color32, new Vector2(outerUV.z, outerUV.y + uvRadiusY));
        vh.AddVert(new Vector3(right, bottom), color32, new Vector2(outerUV.z, outerUV.y));

        // 左边矩形
        vh.AddTriangle(2, 5, 1);
        vh.AddTriangle(2, 5, 6);
        // 中间矩形
        vh.AddTriangle(7, 8, 4);
        vh.AddTriangle(7, 8, 11);
        // 右边矩形
        vh.AddTriangle(10, 13, 9);
        vh.AddTriangle(10, 13, 14);

        List<Vector2> positionList = new List<Vector2>();
        List<Vector2> uvList = new List<Vector2>();
        List<int> vertexList = new List<int>();

        // 右上角圆心
        positionList.Add(new Vector2(right - r, top - r));
        uvList.Add(new Vector2(outerUV.z - uvRadiusX, outerUV.w - uvRadiusY));
        vertexList.Add(9);

        // 左上角圆心
        positionList.Add(new Vector2(left + r, top - r));
        uvList.Add(new Vector2(outerUV.x + uvRadiusX, outerUV.w - uvRadiusY));
        vertexList.Add(5);

        // 左下角圆心
        positionList.Add(new Vector2(left + r, bottom + r));
        uvList.Add(new Vector2(outerUV.x + uvRadiusX, outerUV.y + uvRadiusY));
        vertexList.Add(6);

        // 右下角圆心
        positionList.Add(new Vector2(right - r, bottom + r));
        uvList.Add(new Vector2(outerUV.z - uvRadiusX, outerUV.y + uvRadiusY));
        vertexList.Add(10);

        // 每个三角形角度
        float degreeDelta = Mathf.PI / 2 / this.cornerSegments;

        // 当前角度
        float degree = -Mathf.PI / 2;
        for (int i = 0; i < vertexList.Count; i++)
        {
            degree += Mathf.PI / 2;
            if (i == 0 && !this.rightTop)
            {
                vh.AddTriangle(vertexList[i], 8, 12);
                vh.AddTriangle(vertexList[i], 12, 13);
                continue;
            }

            if (i == 1 && !this.leftTop)
            {
                vh.AddTriangle(vertexList[i], 0, 4);
                vh.AddTriangle(vertexList[i], 0, 1);
                continue;
            }

            if (i == 2 && !this.leftBottom)
            {
                vh.AddTriangle(vertexList[i], 3, 2);
                vh.AddTriangle(vertexList[i], 3, 7);
                continue;
            }

            if (i == 3 && !this.rightBottom)
            {
                vh.AddTriangle(vertexList[i], 15, 14);
                vh.AddTriangle(vertexList[i], 15, 11);
                continue;
            }

            int currVertCount = vh.currentVertCount;
            float cornerDegree = degree;
            for (int j = 0; j <= this.cornerSegments; j++)
            {
                float cos = Mathf.Cos(cornerDegree);
                float sin = Mathf.Sin(cornerDegree);
                Vector3 position = new Vector3(positionList[i].x + cos * r, positionList[i].y + sin * r);
                Vector3 uv0 = new Vector2(uvList[i].x + cos * uvRadiusX, uvList[i].y + sin * uvRadiusY);
                vh.AddVert(position, color32, uv0);
                cornerDegree += degreeDelta;
                if (j > 0)
                {
                    vh.AddTriangle(vertexList[i], currVertCount + j, currVertCount + j - 1);
                }
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
    [UnityEditor.CustomEditor(typeof(CornerRadiusImage), true)]
    public class CornerRadiusImageEditor : UnityEditor.UI.ImageEditor
    {
        UnityEditor.SerializedProperty _sprite;
        UnityEditor.SerializedProperty _cornerRadius;
        UnityEditor.SerializedProperty _cornerSegments;

        protected override void OnEnable()
        {
            base.OnEnable();

            this._sprite = this.serializedObject.FindProperty("m_Sprite");
            this._cornerRadius = this.serializedObject.FindProperty("cornerRadius");
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
            if(this._cornerRadius.floatValue < 0)
            {
                this._cornerRadius.floatValue = 0;
            }
            UnityEditor.EditorGUILayout.PropertyField(this._cornerRadius);
            UnityEditor.EditorGUILayout.PropertyField(this._cornerSegments);
            UnityEditor.EditorGUILayout.PropertyField(this.serializedObject.FindProperty("leftTop"));
            UnityEditor.EditorGUILayout.PropertyField(this.serializedObject.FindProperty("rightTop"));
            UnityEditor.EditorGUILayout.PropertyField(this.serializedObject.FindProperty("leftBottom"));
            UnityEditor.EditorGUILayout.PropertyField(this.serializedObject.FindProperty("rightBottom"));
            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}