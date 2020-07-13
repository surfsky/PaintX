using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPaint
{
    /// <summary>
    /// 资源匹配管理
    /// </summary>
    public class XRes
    {
        public static Bitmap GetAntialiasImage(bool b)
        {
            return b ? Properties.Resources.anti_true : Properties.Resources.anti_false;
        }

        public static Bitmap GetDashLineImage(LineType dashType)
        {
            switch (dashType)
            {
                case LineType.Solid:        return Properties.Resources.solid;
                case LineType.Dot:          return Properties.Resources.dot;
                case LineType.DashedDot:    return Properties.Resources.dash_dot;
                case LineType.DashedDotDot: return Properties.Resources.dash_dot_dot;
                case LineType.Dash1:        return Properties.Resources.dash;
                case LineType.Dash2:        return Properties.Resources.dash2;
                default:                        return Properties.Resources.solid;
            }
        }

        public static Bitmap GetLineEndCapImage(LineCapType capType)
        {
            switch (capType)
            {
                case LineCapType.Rounded:       return Properties.Resources.cap_round_right;
                case LineCapType.Square:        return Properties.Resources.cap_square_right;
                case LineCapType.Rect:     return Properties.Resources.cap_rect_right;
                case LineCapType.Circle:        return Properties.Resources.cap_circle_right;
                case LineCapType.LineArrow:     return Properties.Resources.line_arrow_right;
                case LineCapType.SharpArrow:    return Properties.Resources.sharp_arrow_right;
                case LineCapType.SharpArrow2:   return Properties.Resources.sharp_arrow2_right;
                case LineCapType.NormalArrow:   return Properties.Resources.normal_arrow_right;
                default:                        return null;
            }
        }

        public static Bitmap GetLineStartCapImage(LineCapType capType)
        {
            switch (capType)
            {
                case LineCapType.Rounded:       return Properties.Resources.cap_round_left;
                case LineCapType.Square:        return Properties.Resources.cap_square_left;
                case LineCapType.Rect:     return Properties.Resources.cap_rect_left;
                case LineCapType.Circle:        return Properties.Resources.cap_circle_left;
                case LineCapType.LineArrow:     return Properties.Resources.line_arrow_left;
                case LineCapType.SharpArrow:    return Properties.Resources.sharp_arrow_left;
                case LineCapType.SharpArrow2:   return Properties.Resources.sharp_arrow2_left;
                case LineCapType.NormalArrow:   return Properties.Resources.normal_arrow_left;
                default:                        return null;
            }
        }
    }
}
