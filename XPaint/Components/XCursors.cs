using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace XPaint
{
    /// <summary>
    /// 绘图使用到的图标
    /// </summary>
    public static class XCursors
    {
        // private
        private static Cursor _pan;
        private static Cursor _line;
        private static Cursor _rect;
        private static Cursor _ellipse;
        private static Cursor _rotate;
        private static Cursor _size_0;
        private static Cursor _size_90;
        private static Cursor _select;
        private static Cursor _select2;

        // public
        public static Cursor Pan { get { return _pan; } }
        public static Cursor ToolLine { get { return _line; } }
        public static Cursor ToolRect { get { return _rect; } }
        public static Cursor ToolEllipse { get { return _ellipse; } }
        public static Cursor Rotate { get { return _rotate; } }
        public static Cursor Size0 { get { return _size_0; } }
        public static Cursor Size90 { get { return _size_90; } }
        public static Cursor Select { get { return _select; } }
        public static Cursor Select2 { get { return _select2; } }

        //
        static XCursors()
        {
            _pan     = LoadCursor(Properties.Resources.pan);
            _line    = LoadCursor(Properties.Resources.tool_line);
            _rect    = LoadCursor(Properties.Resources.tool_rect);
            _ellipse = LoadCursor(Properties.Resources.tool_ellipse);
            _rotate  = LoadCursor(Properties.Resources.rotate);
            _size_0  = LoadCursor(Properties.Resources.size_0);
            _size_90 = LoadCursor(Properties.Resources.size_90);
            _select  = LoadCursor(Properties.Resources.select);
            _select2 = LoadCursor(Properties.Resources.select2);
        }

        /// <summary>加载字节数组为鼠标指针对象</summary>
        static Cursor LoadCursor(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return new Cursor(stream);
        }
    }
}
