using System;
using System.Drawing;

namespace XPaint
{
    public class ArrowProperty : BaseProperty
    {
        public Color LineColor { get; set; }
        public ArrowSize LineSize { get; set; }


        public override ShapePropertyType PropertyType
        {
            get { return ShapePropertyType.Arrow; }
        }

        public ArrowProperty()
        {
            LineColor = Color.Gray;
            LineSize = ArrowSize.Large;
        }

        public ArrowProperty Clone()
        {
            ArrowProperty iap = new ArrowProperty();
            iap.Antialias = this.Antialias;
            iap.LineColor = this.LineColor;
            iap.LineSize = this.LineSize;
            return iap;
        }
    }
}
