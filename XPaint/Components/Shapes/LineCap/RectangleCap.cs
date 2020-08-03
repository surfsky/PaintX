﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    public class RectangleCap : CustomLineCap
    {
        public RectangleCap()
            : this(4.0f, 4.0f)
        {
        }

        public RectangleCap(float width, float height)
            : base(GetFillPath(width, height), null)
        {
        }

        private static GraphicsPath GetFillPath(float w, float h)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(-h / 2, -w / 2, h, w));
            return path;
        }
    }
}
