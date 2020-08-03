using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 可描边图形（如线）
    /// </summary>
    public abstract class SStrokable : Shape
    {       
        private StrokableProperty _property;
        private Pen pen;

        public SStrokable(XKernel container, StrokableProperty property)
            : base(container, property)
        {
            _property = property;
            if (_property != null)
                SetNewPen();
        }

        protected override void AfterPropertyChanged(BaseProperty oldValue, BaseProperty newValue)
        {
            _property = (StrokableProperty)newValue;
            if (_property != null)
                SetNewPen();
        }

        private void SetNewPen()
        {
            if (pen != null)
                pen.Dispose();

            float width = Math.Min(_property.PenWidth, XConsts.MaxStrokeWidth);
            pen = new Pen(_property.StrokeColor, width);
            pen.Alignment = _property.PenAlign;
            pen.LineJoin = _property.HowLineJoin;

            switch (_property.LineDash)
            {
                case LineType.Solid:
                    pen.DashStyle = DashStyle.Solid;
                    break;
                case LineType.Dot:
                    pen.DashStyle = DashStyle.Dot;
                    break;
                case LineType.DashedDot:
                    pen.DashStyle = DashStyle.DashDot;
                    break;
                case LineType.DashedDotDot:
                    pen.DashStyle = DashStyle.DashDotDot;
                    break;
                case LineType.Dash1:
                    pen.DashPattern = new float[] { 2, 2 };
                    break;
                case LineType.Dash2:
                    pen.DashPattern = new float[] { 4, 4 };
                    break;
            }

            switch (_property.StartLineCap)
            {
                case LineCapType.Rounded:
                    pen.StartCap = LineCap.Round;
                    break;
                case LineCapType.Square:
                    pen.StartCap = LineCap.Square;
                    break;
                case LineCapType.LineArrow:
                    pen.CustomStartCap = new LineArrowCap();
                    break;
                case LineCapType.NormalArrow:
                    pen.CustomStartCap = new AdjustableArrowCap(5, 5, true);
                    break;
                case LineCapType.SharpArrow:
                    pen.CustomStartCap = new SharpArrowCap();
                    break;
                case LineCapType.SharpArrow2:
                    pen.CustomStartCap = new SharpArrowCap(8.0f, 6.4f, 4.2f);
                    break;
                case LineCapType.Rect:
                    pen.CustomStartCap = new RectangleCap();
                    break;
                case LineCapType.Circle:
                    pen.CustomStartCap = new CircleCap();
                    break;
                case LineCapType.Hip:
                    break;
            }

            switch (_property.EndLineCap)
            {
                case LineCapType.Rounded:
                    pen.EndCap = LineCap.Round;
                    break;
                case LineCapType.Square:
                    pen.EndCap = LineCap.Square;
                    break;
                case LineCapType.LineArrow:
                    pen.CustomEndCap = new LineArrowCap();
                    break;
                case LineCapType.NormalArrow:
                    pen.CustomEndCap = new AdjustableArrowCap(5, 5, true);
                    break;
                case LineCapType.SharpArrow:
                    pen.CustomEndCap = new SharpArrowCap();
                    break;
                case LineCapType.SharpArrow2:
                    pen.CustomEndCap = new SharpArrowCap(8.0f, 6.4f, 4.2f);
                    break;
                case LineCapType.Rect:
                    pen.CustomEndCap = new RectangleCap();
                    break;
                case LineCapType.Circle:
                    pen.CustomEndCap = new CircleCap();
                    break;
                case LineCapType.Hip:
                    break;
            }
        }

        /// <summary>
        /// 给后面的子类描边用
        /// </summary>
        protected Pen StrokePen
        { get { return pen; } }

        public override void Draw(Graphics g)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = (ShapeProperty.Antialias) ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            g.DrawPath(StrokePen, base.Path);
            g.SmoothingMode = old;
        }

        #region IDisposable interface

        public override void Dispose()
        {            
            if (pen != null)
                pen.Dispose();
            base.Dispose();
        }

        #endregion
    }
}
