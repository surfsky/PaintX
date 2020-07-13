using App.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XPaint
{
    /// <summary>
    /// 工具类别
    /// </summary>
    public enum ToolType
    {
        [UI("平移")] Hand,
        [UI("选择")] Select,
        [UI("自定义")] Custom,

        //
        [UI("文本")] Text,
        [UI("线段")] Line,
        [UI("箭头")] Arrow,
        [UI("矩形")] Rect,
        [UI("圆角矩形")] RoundedRect,
        [UI("椭圆")] Ellipse,

        //
        [UI("三角形")] Triangle,
        [UI("星形")]   Star,
        [UI("多边线")] Polyline,
        [UI("多边形")] Polygon
    }

    /// <summary>
    /// 工具
    /// </summary>
    public class XTool
    {
        public ToolType Type { get; set; }
        public Image Icon { get; set; }

        public override string ToString()
        {
            return this.Type.GetTitle();
        }
        public XTool() { }
        public XTool(ToolType type, Image icon)
        {
            this.Type = type;
            this.Icon = icon;
        }

        //
        public static List<XTool> All = GetAll();
        public static List<XTool> GetAll()
        {
            var items = new List<XTool>();
            items.Add(new XTool(ToolType.Select, Properties.Resources.icon_cursor_2x));
            items.Add(new XTool(ToolType.Text, Properties.Resources.icon_text_2x));
            items.Add(new XTool(ToolType.Rect, Properties.Resources.icon_rect_2x));
            items.Add(new XTool(ToolType.RoundedRect, Properties.Resources.icon_round_rect_2x));
            items.Add(new XTool(ToolType.Polyline, Properties.Resources.icon_polyline_2x));
            items.Add(new XTool(ToolType.Polygon, Properties.Resources.icon_polygon_2x));
            items.Add(new XTool(ToolType.Star, Properties.Resources.icon_star_2x));
            items.Add(new XTool(ToolType.Arrow, Properties.Resources.icon_arrow_2x));
            return items;
        }
    }

}
