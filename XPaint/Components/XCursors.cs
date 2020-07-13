using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace XPaint
{
    /// <summary>
    /// 光标类别
    /// </summary>
    public enum CursorType
    {
        Default,
        Hand,
        SelectDefault,
        SelectMove,
        SelectRotate,
        SelectScale,
        SelectDragVertex,
        Custom,


        Line,
        Rect,
        Ellipse,
        Star,
        Polygon
    }

    /// <summary>
    /// 光标
    /// </summary>
    public static class XCursors
    {
        public static Cursor Pan = LoadCursor(Properties.Resources.pan);
        public static Cursor Rotate = LoadCursor(Properties.Resources.rotate);
        public static Cursor Size0 = LoadCursor(Properties.Resources.size_0);
        public static Cursor Size90 = LoadCursor(Properties.Resources.size_90);
        public static Cursor Select = LoadCursor(Properties.Resources.select);
        public static Cursor Select2 = LoadCursor(Properties.Resources.select2);
        public static Cursor ToolLine = LoadCursor(Properties.Resources.tool_line);
        public static Cursor ToolRect = LoadCursor(Properties.Resources.tool_rect);
        public static Cursor ToolEllipse = LoadCursor(Properties.Resources.tool_ellipse);


        /// <summary>加载字节数组为鼠标指针对象</summary>
        static Cursor LoadCursor(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return new Cursor(stream);
        }
    }
}
