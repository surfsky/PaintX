using System;
using System.Drawing;

namespace XPaint
{
    public class FillableProperty : StrokableProperty
    {
        public PaintType PaintType { get; set; }
        public FillType FillType { get; set; }
        public Color FillColor { get; set; }


        public override ShapePropertyType PropertyType
        {
            get { return ShapePropertyType.Fill; }
        }

        public FillableProperty()
        {
            PaintType = PaintType.Stroke;
            FillType = FillType.SolidColor;
            FillColor = Color.White;
        }

        public new FillableProperty Clone()
        {
            FillableProperty fp = new FillableProperty();
            fp.Antialias = this.Antialias;
            
            fp.PenWidth = this.PenWidth;
            fp.StrokeColor = this.StrokeColor;
            fp.LineDash = this.LineDash;
            fp.StartLineCap = this.StartLineCap;
            fp.EndLineCap = this.EndLineCap;
            fp.PenAlign = this.PenAlign;
            fp.HowLineJoin = this.HowLineJoin;
            
            fp.PaintType = this.PaintType;
            fp.FillType = this.FillType;
            fp.FillColor = this.FillColor;

            return fp;
        }
    }
}
