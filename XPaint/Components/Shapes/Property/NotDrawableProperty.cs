using System;

namespace XPaint
{
    public class NotDrawableProperty : BaseProperty
    {
        public override ShapePropertyType PropertyType
        {
            get { return ShapePropertyType.NotDrawable; }
        }
    }
}
