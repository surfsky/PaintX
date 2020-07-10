using System;
using System.Drawing;

namespace XPaint
{
    /// <summary>
    /// 绘制常量
    /// </summary>
    public class XConsts
    {        
        public static readonly Pen PenHitDetect;
        public const int PenHitWidth = 14;
        public const int AcceptableMinMoveDistance = 4;
        public const int AnchorRectHalfWidth = 3;
        public const int LineVertexHalfWidth = 5;
        public const int RotatingRectOffset = 60;
        public const int RotatingRectHalfWidth = 12;
        public const int CrossSignHalfWidth = 2;
        public const float MagicBezier = 0.55f;
        public const int MaxStrokeWidth = 120;

        static XConsts()
        {
            PenHitDetect = new Pen(Color.Black, PenHitWidth);
        }
    }
}
