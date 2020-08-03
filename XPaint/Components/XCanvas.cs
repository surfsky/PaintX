using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    public class XCanvas : Panel
    {
        //----------------------------------------------------
        // Field
        //----------------------------------------------------
        // static
        static readonly int DW = 120;
        static readonly int DH = 80;

        // private
        Size _imageSize;
        Rectangle _imageRect;
        Rectangle _frameRect;

        // public
        public XKernel Kernel;

        //----------------------------------------------------
        // Constructor
        //----------------------------------------------------
        public XCanvas()
        {
            base.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer, true);
            base.ResizeRedraw = true;
            base.BorderStyle = BorderStyle.FixedSingle;
            base.AutoScrollMinSize = Size.Empty;
            InitialValue();
        }

        private void InitialValue()
        {
            Kernel = new XKernel(new Size(600, 480));
            _imageSize = Kernel.FinalBitmap.Size;
            base.AutoScrollMinSize = new Size(_imageSize.Width + DW, _imageSize.Height + DH);
            RecalcCanvas();

            Kernel.FinalBitmapChanged += new EventHandler(kernal_FinalBitmapChanged);
            Kernel.SelectedShapesChanged += new EventHandler(kernel_SelectedShapesChanged);
            Kernel.Selecting += new EventHandler(kernel_Selecting);
            Kernel.CursorTypeChanged += new EventHandler(kernel_CursorTypeChanged);
            Kernel.SetTool(ToolType.Line);
        }


        #region 文档传递上来的事件处理
        //----------------------------------------------------
        // 文档传递上来的事件处理
        //----------------------------------------------------
        private void kernal_FinalBitmapChanged(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        private void kernel_SelectedShapesChanged(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        private void kernel_Selecting(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        private void kernel_CursorTypeChanged(object sender, EventArgs e)
        {
            switch (Kernel.CursorType)
            {
                case CursorType.Ellipse:
                    base.Cursor = XCursors.ToolEllipse;
                    break;
                case CursorType.Line:
                    base.Cursor = XCursors.ToolLine;
                    break;
                case CursorType.Rect:
                    base.Cursor = XCursors.ToolRect;
                    break;
                case CursorType.Default:
                    base.Cursor = Cursors.Default;
                    break;
                case CursorType.Hand:
                    base.Cursor = XCursors.Pan;
                    break;
                case CursorType.SelectMove:
                    base.Cursor = Cursors.SizeAll;
                    break;
                case CursorType.SelectDragVertex:
                    base.Cursor = XCursors.Select2;
                    break;
                case CursorType.SelectScale:
                    base.Cursor = XCursors.Size0;
                    break;
                case CursorType.SelectRotate:
                    base.Cursor = XCursors.Rotate;
                    break;
                case CursorType.SelectDefault:
                    base.Cursor = XCursors.Select;
                    break;
                case CursorType.Custom:
                    //..
                    break;

            }
        }

        #endregion

        #region 页面事件处理

        //----------------------------------------------------
        // 页面事件处理
        //----------------------------------------------------
        /// <summary>绘制背景（灰黑色）</summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(Point.Empty, ClientSize));
        }

        /// <summary>绘制（中间的画板）</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            // 绘制白色背景、外框、图片（在图片坐标下绘制？）
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            g.FillRectangle(Brushes.White, _imageRect);
            g.DrawRectangle(Pens.DarkGray, _frameRect);
            g.DrawImage(Kernel.FinalBitmap, _imageRect.Location);
            g.ResetTransform();

            // 绘制选区、选择层（在屏幕坐标下绘制？）
            g.TranslateTransform(
                _imageRect.Location.X + AutoScrollPosition.X,
                _imageRect.Location.Y + AutoScrollPosition.Y);

            // 绘制选择区域矩形
            if (Kernel.IsInSelecting)
                Kernel.DrawSelectingRect(g);

            // 绘制已选择对象矩形
            if (Kernel.SelectedShapesCount > 0)
                Kernel.DrawSizableRects(g);

            g.ResetTransform();
        }

        /// <summary>面板尺寸变更（重新计算画板的位置）</summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            RecalcCanvas();
        }

        /// <summary>鼠标事件（传递给处理器）</summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Kernel.MouseDown(ToImagePoint(e.Location));
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Kernel.MouseMove(ToImagePoint(e.Location));
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Kernel.MouseUp(ToImagePoint(e.Location));
        }

        #endregion

        #region private methods

        //----------------------------------------------------
        // 辅助方法
        //----------------------------------------------------
        /// <summary>将屏幕坐标转化为图片坐标</summary>
        private Point ToImagePoint(Point point)
        {
            point.Offset(
                -_imageRect.Location.X - AutoScrollPosition.X, 
                -_imageRect.Location.Y - AutoScrollPosition.Y
                );
            return point;
        }

        /// <summary>重新计算画板参数</summary>
        private void RecalcCanvas()
        {
            int dw = Math.Max(ClientSize.Width  - _imageSize.Width,  DW);  // 宽度差值
            int dh = Math.Max(ClientSize.Height - _imageSize.Height, DH);  // 高度差值
            //var canvasSize = new Size(_imageSize.Width + dw, _imageSize.Height + dh);   // 画板大小
            _imageRect = new Rectangle(dw/2, dh/2, _imageSize.Width, _imageSize.Height);  // 图片区域（居中放置）
            _frameRect = new Rectangle(dw/2 - 1, dh/2 - 1, _imageSize.Width + 1, _imageSize.Height + 1);  // 外框区域
        }


        #endregion

    }
}
