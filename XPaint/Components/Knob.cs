using System;
using System.Drawing;

namespace XPaint
{
    /// <summary>
    /// 手柄类别
    /// </summary>
    public enum KnobType
    {
        Line,      // 线段终点
        Rotate,    // 旋转点
        Scale      // 放缩点
    }

    /// <summary>
    /// 手柄（顶点、旋转、放缩等）
    /// </summary>
    public struct Knob
    {
        public KnobType Type;
        public Rectangle Rect;
        public bool Visible;

        public Knob(KnobType type)
        {
            Rect = Rectangle.Empty;
            Type = type;
            Visible = true;
        }
        public Knob(Rectangle rect, KnobType type)
        {
            Rect = rect;
            Type = type;
            Visible = true;
        }

    }
}
