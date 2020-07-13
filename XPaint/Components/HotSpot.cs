using System;
using System.Drawing;

namespace XPaint
{
    /// <summary>
    /// 热点类别
    /// </summary>
    public enum HotSpotType
    {
        LineVertex,        // 线段终点热点
        RotatingRect,      // 旋转热点
        AnchorToScale      // 放缩热点
    }

    /// <summary>
    /// 可调整热点（顶点、旋转、放缩等）
    /// </summary>
    public struct HotSpot
    {
        public Rectangle Rect;
        HotSpotType _type;
        public int AnchorAngle;
        public bool Visible;

        public HotSpotType Type { get { return _type; } }

        public HotSpot(HotSpotType type)
        {
            Rect = Rectangle.Empty;
            _type = type;
            AnchorAngle = 0;
            Visible = true;
        }

        public HotSpot(Rectangle rect, HotSpotType type)
        {
            Rect = rect;
            _type = type;
            AnchorAngle = 0;
            Visible = true;
        }

    }
}
