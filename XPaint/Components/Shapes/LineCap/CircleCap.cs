using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    public class CircleCap : CustomLineCap
    {
        public CircleCap()
            : this(5.0f, 5.0f)
        {
        }

        public CircleCap(float width, float height)
            : base(GetFillPath(width, height), null)
        {
        }

        private static GraphicsPath GetFillPath(float w, float h)
        {
            var path = new GraphicsPath();
            path.AddEllipse(new RectangleF(-h / 2, -w / 2, h, w));
            return path;
        }
    }
}
