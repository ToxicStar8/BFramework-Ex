using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 按子项首选尺寸横向排列，并在宽度不足时自动换行。
    /// </summary>
    [AddComponentMenu("Layout/Grid Wrap Layout Group")]
    public class GridWrapLayoutGroup : LayoutGroup
    {
        [SerializeField] private float spacingX;
        [SerializeField] private float spacingY;

        public float SpacingX
        {
            get => spacingX;
            set => SetProperty(ref spacingX, value);
        }

        public float SpacingY
        {
            get => spacingY;
            set => SetProperty(ref spacingY, value);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
        }

        public override void CalculateLayoutInputVertical()
        {
            var height = Layout(false);
            SetLayoutInputForAxis(height, height, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            Layout(true);
        }

        public override void SetLayoutVertical()
        {
            Layout(true);
        }

        private float Layout(bool setPositions)
        {
            var width = rectTransform.rect.width - padding.horizontal;
            var x = (float)padding.left;
            var y = (float)padding.top;
            var lineHeight = 0f;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];
                var childWidth = LayoutUtility.GetPreferredSize(child, 0);
                var childHeight = LayoutUtility.GetPreferredSize(child, 1);

                if (x > padding.left && x + childWidth > width + padding.left)
                {
                    x = padding.left;
                    y += lineHeight + spacingY;
                    lineHeight = 0f;
                }

                if (setPositions)
                {
                    SetChildAlongAxis(child, 0, x, childWidth);
                    SetChildAlongAxis(child, 1, y, childHeight);
                }

                x += childWidth + spacingX;
                lineHeight = Mathf.Max(lineHeight, childHeight);
            }

            return y + lineHeight + padding.bottom;
        }
    }
}
