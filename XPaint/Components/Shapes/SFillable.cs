using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 可填冲的图形（如矩形）
    /// </summary>
    public abstract class SFillable : SStrokable
    {
        private Brush fillBrush;
        private FillableProperty _pro;
        protected Brush FillBrush
        {
            get { return fillBrush; }
        }

        //
        public SFillable(XKernel container, FillableProperty property)
            :base(container, property)
        {
            _pro = property;
            SetNewBrush();
        }

        private void SetNewBrush()
        {
            if (fillBrush != null)
                fillBrush.Dispose();

            fillBrush = new SolidBrush(_pro.FillColor);
        }

        protected override void AfterPropertyChanged(BaseProperty oldValue, BaseProperty newValue)
        {            
            base.AfterPropertyChanged(oldValue, newValue);
            _pro = (FillableProperty)newValue;
            SetNewBrush();
        }

        public override void Draw(Graphics g)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = (ShapeProperty.Antialias) ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            
            if (_pro.PaintType == PaintType.Fill || _pro.PaintType == PaintType.StrokeAndFill)
                g.FillPath(FillBrush, base.Path);

            if (_pro.PaintType == PaintType.Stroke || _pro.PaintType == PaintType.StrokeAndFill)
                base.Draw(g);

            g.SmoothingMode = old;
        }

        #region IDisposable interface

        public override void Dispose()
        {            
            if (fillBrush != null)
                fillBrush.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
