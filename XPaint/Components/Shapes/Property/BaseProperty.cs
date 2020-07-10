using System;

namespace XPaint
{
    public abstract class BaseProperty
    {
        public abstract ShapePropertyType PropertyType { get; }
        public bool Antialias { get; set; }

        public BaseProperty()
        {
            Antialias = true;
        }
    }
}
