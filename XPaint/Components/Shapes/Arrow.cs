using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 箭头
    /// </summary>
    public class Arrow : LineShape
    {
        private byte[] types = new byte[] { 0, 1, 1, 1, 1, 1, 0x81 };
        private ArrowProperty _property;

        public Arrow(XKernel container, ArrowProperty pro)
            :base(container, null)
        {
            _property = pro;
        }

        public override ToolType Type
        {
            get { return ToolType.Arrow; }
        }

        public override string Name
        {
            get { return "提示箭头"; }
        }

        public override BaseProperty ShapeProperty
        {
            get
            {
                return _property;
            }
            set
            {
                _property = (ArrowProperty)value;

                // reset the size
                Point p = new Point((int)VertexAnchors[0].X, (int)VertexAnchors[0].Y);
                SetNewPosForHotAnchor(0, p);
            }
        }

        public override void SetEndPoint(Point pt)
        {
            PointF[] pf = GetArrowPath(base.StartPoint, pt, _property.LineSize);
            base.SetNewScaledPath(pf, types);
        }

        public static PointF[] GetArrowPath(PointF p1, PointF p2, ArrowSize size)
        {
            int hw = 0;
            float tw = 0;

            switch (size)
            {
                case ArrowSize.Small:
                    hw = 10;
                    tw = 0.8f;
                    break;
                case ArrowSize.Medium:
                    hw = 16;
                    tw = 0.8f;
                    break;
                case ArrowSize.Large:
                    hw = 22;
                    tw = 0.8f;
                    break;
            }

            PointF[] pf = new PointF[7];
            PointF tmp = new PointF();
            double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            tmp.X = p2.X + (float)(hw * Math.Cos(angle + Math.PI));
            tmp.Y = p2.Y + (float)(hw * Math.Sin(angle + Math.PI));

            pf[0] = new PointF(
                p1.X + (float)(tw * Math.Cos(angle - Math.PI / 2)),
                p1.Y + (float)(tw * Math.Sin(angle - Math.PI / 2)));
            pf[6] = new PointF(
                p1.X + (float)(tw * Math.Cos(angle + Math.PI / 2)),
                p1.Y + (float)(tw * Math.Sin(angle + Math.PI / 2)));

            pf[3] = p2;

            pf[1] = new PointF(
                tmp.X + (float)(hw / 4 * Math.Cos(angle - Math.PI / 2)),
                tmp.Y + (float)(hw / 4 * Math.Sin(angle - Math.PI / 2)));
            pf[2] = new PointF(
                tmp.X + (float)(hw / 2 * Math.Cos(angle - Math.PI / 2)),
                tmp.Y + (float)(hw / 2 * Math.Sin(angle - Math.PI / 2)));
            pf[4] = new PointF(
                tmp.X + (float)(hw / 2 * Math.Cos(angle + Math.PI / 2)),
                tmp.Y + (float)(hw / 2 * Math.Sin(angle + Math.PI / 2)));
            pf[5] = new PointF(
                tmp.X + (float)(hw / 4 * Math.Cos(angle + Math.PI / 2)),
                tmp.Y + (float)(hw / 4 * Math.Sin(angle + Math.PI / 2)));

            return pf;
        }

        public override void SetNewPosForHotAnchor(int index, Point newPos)
        {
            PointF[] pf = base.VertexAnchors;
            pf[index] = newPos;

            PointF[] newps = GetArrowPath(pf[0], pf[1], _property.LineSize);
            base.SetNewScaledPath(newps, types);
        }

        //protected override void AfterPropertyChanged(BaseProperty oldValue, BaseProperty newValue)
        //{
        //    _property = (IndicatorArrowProperty)newValue;
        //}

        protected override void UpdateVertexAnchors()
        {
            PointF[] pf = Path.PathPoints;
            PointF[] va = new PointF[2];
            va[0] = new PointF((pf[0].X + pf[6].X) / 2, (pf[0].Y + pf[6].Y) / 2);
            va[1] = pf[3];
            VertexAnchors = va;
        }

        public override void Draw(Graphics g)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = (_property.Antialias) ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            
            using (SolidBrush sb = new SolidBrush(_property.LineColor))
            {
                g.FillPath(sb, Path);
            }

            g.SmoothingMode = old;
        }

        public override bool ContainsPoint(Point pos)
        {
            return base.Path.IsVisible(pos);
        }
    }
}
