using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace XPaint
{
    public partial class XKernel
    {
        /// <summary>最后的图片有变更</summary>
        public event EventHandler FinalBitmapChanged;

        /// <summary>选中的图层有变更</summary>
        public event EventHandler SelectedShapesChanged;

        /// <summary>光标类型变更</summary>
        public event EventHandler CursorTypeChanged;

        /// <summary>正在选择图层</summary>
        public event EventHandler Selecting;

        /// <summary>选中图层的属性有变更</summary>
        public event EventHandler PropertyCollectorChanged;

        /// <summary>图层列表有变更</summary>
        public event EventHandler ShapesChanged;



        //--------------------------------------------
        // 内部调用触发事件的方法
        //--------------------------------------------
        protected virtual void OnFinalBitmapChanged(EventArgs e)
        {
            if (FinalBitmapChanged != null)
                FinalBitmapChanged(this, e);
        }

        protected virtual void OnSelectedShapesChanged(EventArgs e)
        {
            CheckShapeProperty();
            if (SelectedShapesChanged != null)
                SelectedShapesChanged(this, e);
        }

        protected virtual void OnCursorTypeChanged(EventArgs e)
        {
            if (CursorTypeChanged != null)
                CursorTypeChanged(this, e);
        }

        protected virtual void OnSelecting(EventArgs e)
        {
            if (Selecting != null)
                Selecting(this, e);
        }

        protected virtual void OnPropertyCollectorChanged(EventArgs e)
        {
            if (PropertyCollectorChanged != null)
                PropertyCollectorChanged(this, e);
        }

        protected virtual void OnShapesChanged(EventArgs e)
        {
            if (ShapesChanged != null)
                ShapesChanged(this, e);
        }
    }
}
