using System;
using System.Drawing;
using System.Windows.Forms;

namespace XPaint
{
    public partial class FormMain : Form
    {
        private XCanvas _canvas;
        private PropertyCollector properties;
        private bool comingBackFromShape;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitCanvas();
            InitData();
            rdoArrow.Checked = true;
        }

        /// <summary>初始化画板</summary>
        private void InitCanvas()
        {
            _canvas = new XCanvas();
            _canvas.Size = new Size(526, 382);
            _canvas.Location = new Point(140, 81);
            _canvas.AutoScroll = true;
            _canvas.BorderStyle = BorderStyle.FixedSingle;
            _canvas.Dock = DockStyle.Fill;
            this.panMain.Controls.Add(_canvas);

            _canvas.Kernel.SelectedShapesChanged += new EventHandler(Kernel_SelectedShapesChanged);
            _canvas.Kernel.PropertyCollectorChanged += new EventHandler(Kernel_PropertyCollectorChanged);
            _canvas.Kernel.ShapesChanged += new EventHandler(Kernel_ShapesChanged);
        }                

        /// <summary>初始化属性数据</summary>
        private void InitData()
        {
            properties = new PropertyCollector();
            for (int i = 1; i <= 24; i++)
                cmbStrokeWidth.Items.Add(i);
            cmbStrokeWidth.SelectedIndex = 0;

            cmbArrowSize.SelectedIndex = 2;
            cmbPaintType.SelectedIndex = 0;
            cmbRoundRadius.Items.Add(4);
            cmbRoundRadius.Items.Add(6);
            cmbRoundRadius.Items.Add(8);
            cmbRoundRadius.SelectedIndex = 2;

            // antialias
            menuAnliaTrue.Tag = true;
            menuAliaFalse.Tag = false;

            // line startcap
            menuStartcapRound.Tag = LineCapType.Rounded;
            menuStartcapSquare.Tag = LineCapType.Square;
            menuStartcapRect.Tag = LineCapType.Rectangle;
            menuStartcapCircle.Tag = LineCapType.Circle;
            menuStartcapLineArrow.Tag = LineCapType.LineArrow;
            menuStartcapNormalArrow.Tag = LineCapType.NormalArrow;
            menuStartcapSharpArrow.Tag = LineCapType.SharpArrow;
            menuStartcapSharpArrow2.Tag = LineCapType.SharpArrow2;

            // line endcap
            menuEndcapRound.Tag = LineCapType.Rounded;
            menuEndcapSqare.Tag = LineCapType.Square;
            menuEndcapRect.Tag = LineCapType.Rectangle;
            menuEndcapCircle.Tag = LineCapType.Circle;
            menuEndcapLineArrow.Tag = LineCapType.LineArrow;
            menuEndcapNormalArrow.Tag = LineCapType.NormalArrow;
            menuEndcapSharpArrow.Tag = LineCapType.SharpArrow;
            menuEndcapSharpArrow2.Tag = LineCapType.SharpArrow2;

            // line dash type
            menuLinedashSolid.Tag = LineDashType.Solid;
            menuLinedashDot.Tag = LineDashType.Dot;
            menuLinedashDashDot.Tag = LineDashType.DashedDot;
            menuLinedashDashDotDot.Tag = LineDashType.DashedDotDot;
            menuLinedashDash1.Tag = LineDashType.Dash1;
            menuLinedashDash2.Tag = LineDashType.Dash2;
        }

        /// <summary>选中工具</summary>
        private void SelectTool(int index)
        {
            switch (index)
            {
                case 0: // line
                    _canvas.Kernel.SetTool(ToolType.Line);
                    SwitchPropertyCtrls(ShapePropertyType.StrokableProperty);
                    break;
                case 1: // indicator arrow
                    _canvas.Kernel.SetTool(ToolType.Arrow);
                    SwitchPropertyCtrls(ShapePropertyType.IndicatorArrowProperty);
                    break;
                case 2: // broken line
                    _canvas.Kernel.SetTool(ToolType.BrokenLine);
                    SwitchPropertyCtrls(ShapePropertyType.StrokableProperty);
                    break;
                case 3: // rect
                    _canvas.Kernel.SetTool(ToolType.Rectangle);
                    SwitchPropertyCtrls(ShapePropertyType.FillableProperty);
                    break;
                case 4: // rounded rect
                    _canvas.Kernel.SetTool(ToolType.RoundedRect);
                    SwitchPropertyCtrls(ShapePropertyType.RoundedRectProperty);
                    break;
                case 5: // ellipse
                    _canvas.Kernel.SetTool(ToolType.Ellipse);
                    SwitchPropertyCtrls(ShapePropertyType.FillableProperty);
                    break;
                case 6: // shape select
                    _canvas.Kernel.SetTool(ToolType.ShapeSelect);
                    if (_canvas.Kernel.SelectedShapesCount != 1)
                        SwitchPropertyCtrls(ShapePropertyType.NotDrawable);
                    break;
            }
        }

        /// <summary>显隐当前图形对应的属性控件</summary>
        private void SwitchPropertyCtrls(ShapePropertyType type)
        {
            switch (type)
            {
                case ShapePropertyType.NotDrawable:
                    ShowAntialiasCtrls(false);
                    ShowStrokeCtrls(false);
                    ShowFillCtrls(false);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.StrokableProperty:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(false);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.FillableProperty:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(true);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.RoundedRectProperty:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(true);
                    ShowRoundCtrls(true);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.IndicatorArrowProperty:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(false);
                    ShowFillCtrls(false);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(true);
                    break;
            }
        }

        private void ShowAntialiasCtrls(bool v)
        {
            btnAntialias.Visible = v;
        }

        private void ShowArrowCtrls(bool v)
        {            
            sp.Visible = v;
            lblArrowSize.Visible = v;
            cmbArrowSize.Visible = v;
        }

        private void ShowRoundCtrls(bool v)
        {
            lblRoundRadius.Visible = v;
            cmbRoundRadius.Visible = v;
        }

        private void ShowFillCtrls(bool v)
        {
            spFill.Visible = v;
            lblPaintType.Visible = v;
            cmbPaintType.Visible = v;
        }

        private void ShowStrokeCtrls(bool v)
        {
            lblStrokeWidth.Visible = v;
            cmbStrokeWidth.Visible = v;
            
            lblLineStyle.Visible = v;
            btnStartCapType.Visible = v;
            btnLineType.Visible = v;
            btnEndCapType.Visible = v;
        }        

        /// <summary>设置属性相关控件的值</summary>
        private void SetProperties(PropertyCollector ps)
        {
            comingBackFromShape = true;
            bool isnull = (properties == null);

            // anti-alias
            if (isnull || properties.Antialias != ps.Antialias)
            {
                btnAntialias.Image = ps.Antialias ? 
                    Properties.Resources.anti_true : Properties.Resources.anti_false;
            }

            // indicator arrow size
            if (isnull || properties.IndicatorLineSize != ps.IndicatorLineSize)
            {
                switch (ps.IndicatorLineSize)
                {
                    case ArrowSize.Small:
                        cmbArrowSize.SelectedIndex = 0;
                        break;
                    case ArrowSize.Medium:
                        cmbArrowSize.SelectedIndex = 1;
                        break;
                    case ArrowSize.Large:
                        cmbArrowSize.SelectedIndex = 2;
                        break;
                }
            }

            // stroke color
            if (isnull || properties.StrokeColor != ps.StrokeColor)
            {
                MainColor.StrokeColor = ps.StrokeColor;
            }

            // stroke width
            if (isnull || properties.PenWidth != ps.PenWidth)
            {
                cmbStrokeWidth.Text = ps.PenWidth.ToString();
            }

            // line start cap
            if (isnull || properties.StartLineCap != ps.StartLineCap)
            {
                Bitmap bm = null;
                switch (ps.StartLineCap)
                {
                    case LineCapType.Rounded:
                        bm = Properties.Resources.cap_round_left;
                        break;
                    case LineCapType.Square:
                        bm = Properties.Resources.cap_square_left;
                        break;
                    case LineCapType.Rectangle:
                        bm = Properties.Resources.cap_rect_left;
                        break;
                    case LineCapType.Circle:
                        bm = Properties.Resources.cap_circle_left;
                        break;
                    case LineCapType.LineArrow:
                        bm = Properties.Resources.line_arrow_left;
                        break;
                    case LineCapType.SharpArrow:
                        bm = Properties.Resources.sharp_arrow_left;
                        break;
                    case LineCapType.SharpArrow2:
                        bm = Properties.Resources.sharp_arrow2_left;
                        break;
                    case LineCapType.NormalArrow:
                        bm = Properties.Resources.normal_arrow_left;
                        break;
                }
                btnStartCapType.Image = bm;
            }

            // line end cap
            if (isnull || properties.EndLineCap != ps.EndLineCap)
            {
                Bitmap bm = null;
                switch (ps.EndLineCap)
                {
                    case LineCapType.Rounded:
                        bm = Properties.Resources.cap_round_right;
                        break;
                    case LineCapType.Square:
                        bm = Properties.Resources.cap_square_right;
                        break;
                    case LineCapType.Rectangle:
                        bm = Properties.Resources.cap_rect_right;
                        break;
                    case LineCapType.Circle:
                        bm = Properties.Resources.cap_circle_right;
                        break;
                    case LineCapType.LineArrow:
                        bm = Properties.Resources.line_arrow_right;
                        break;
                    case LineCapType.SharpArrow:
                        bm = Properties.Resources.sharp_arrow_right;
                        break;
                    case LineCapType.SharpArrow2:
                        bm = Properties.Resources.sharp_arrow2_right;
                        break;
                    case LineCapType.NormalArrow:
                        bm = Properties.Resources.normal_arrow_right;
                        break;
                }
                btnEndCapType.Image = bm;
            }

            // line dash type
            if (isnull || properties.LineDash != ps.LineDash)
            {
                Bitmap bm = null;
                switch (ps.LineDash)
                {
                    case LineDashType.Solid:
                        bm = Properties.Resources.solid;
                        break;
                    case LineDashType.Dot:
                        bm = Properties.Resources.dot;
                        break;
                    case LineDashType.DashedDot:
                        bm = Properties.Resources.dash_dot;
                        break;
                    case LineDashType.DashedDotDot:
                        bm = Properties.Resources.dash_dot_dot;
                        break;
                    case LineDashType.Dash1:
                        bm = Properties.Resources.dash;
                        break;
                    case LineDashType.Dash2:
                        bm = Properties.Resources.dash2;
                        break;
                }
                btnLineType.Image = bm;
            }

            // paint type
            if (isnull || properties.PaintType != ps.PaintType)
            {
                switch (ps.PaintType)
                {
                    case ShapePaintType.Stroke:
                        cmbPaintType.SelectedIndex = 0;
                        break;
                    case ShapePaintType.Fill:
                        cmbPaintType.SelectedIndex = 1;
                        break;
                    case ShapePaintType.StrokeAndFill:
                        cmbPaintType.SelectedIndex = 2;
                        break;
                }
            }

            // fill color
            if (isnull || properties.FillColor != ps.FillColor)
            {
                MainColor.FillColor = ps.FillColor;
            }

            // round radius
            if (isnull || properties.RadiusAll != ps.RadiusAll)
            {
                cmbRoundRadius.Text = ps.RadiusAll.ToString();
            }

            properties = ps;
            comingBackFromShape = false;
        }

        /// <summary>图形列表变更事件</summary>
        private void Kernel_ShapesChanged(object sender, EventArgs e)
        {
            // 新增或删除形状后重新设为选择模式 surfsky 2020-07-10
            this.rdoSelect.Checked = true;

            ShowShapes();
            ShowSelectedShapes();
        }

        // 显示所有图层
        private void ShowShapes()
        {
            var shapes = _canvas.Kernel.Shapes;
            lbShapes.Items.Clear();
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    lbShapes.Items.Insert(0, shape);
                }
            }
        }

        /// <summary>选中的图形变更事件</summary>
        private void Kernel_SelectedShapesChanged(object sender, EventArgs e)
        {
            if (rdoSelect.Checked)
            {
                if (_canvas.Kernel.SelectedShapesCount == 1)
                {
                    Shape shape = _canvas.Kernel.SelectedShapes[0];
                    SwitchPropertyCtrls(shape.ShapeProperty.PropertyType);
                    ShowSelectedShapes();
                }
                else
                {
                    SwitchPropertyCtrls(ShapePropertyType.NotDrawable);
                }
            }
        }

        // 显示选中图层
        private void ShowSelectedShapes()
        {
            var selectedShapes = _canvas.Kernel.SelectedShapes;
            if (selectedShapes.Length > 0)
            {
                lbShapes.SelectedIndexChanged -= new EventHandler(lbShapes_SelectedIndexChanged);
                lbShapes.SelectedItem = selectedShapes[0];
                lbShapes.SelectedIndexChanged += new EventHandler(lbShapes_SelectedIndexChanged);
            }
        }


        // 选择图层
        private void lbShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbShapes.SelectedIndex != -1)
            {
                var shape = lbShapes.SelectedItem as Shape;
                _canvas.Kernel.SelectedShapes = new Shape[] { shape };  // 会触发Kernel_SelectedShapesChanged 事件
            }
        }


        /// <summary>（选中图形）属性变更事件</summary>
        private void Kernel_PropertyCollectorChanged(object sender, EventArgs e)
        {
            SetProperties(_canvas.Kernel.ShapePropertyCollector);
        }

        //-----------------------------------------
        // 工具栏控制
        //-----------------------------------------
        // 工具栏（形状）选择事件
        private void rdoTool_CheckedChanged(object sender, EventArgs e)
        {
            SelectTool(((Control)sender).TabIndex);
        }

        // 抗锯齿
        private void antiAlia_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnAntialias.Image = mi.Image;

            bool v = (bool)mi.Tag;
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.Antialias, v);
            properties.Antialias = v;
        }

        // 起始线帽
        private void startcapRound_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnStartCapType.Image = mi.Image;

            LineCapType type = (LineCapType)mi.Tag;
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.StartLineCap, type);
            properties.StartLineCap = type;
        }

        // 点画线
        private void dashlineSolid_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnLineType.Image = mi.Image;

            LineDashType type = (LineDashType)mi.Tag;
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.LineDash, type);
            properties.LineDash = type;
        }

        // 结束点
        private void endcapRound_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnEndCapType.Image = mi.Image;

            LineCapType type = (LineCapType)mi.Tag;
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.EndLineCap, type);
            properties.EndLineCap = type;
        }

        // 填充色变更
        private void MainColor_FillColorChange(object sender, EventArgs e)
        {
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.FillColor, MainColor.FillColor);
            properties.FillColor = MainColor.FillColor;
        }

        // 线段色变更
        private void MainColor_StrokeColorChange(object sender, EventArgs e)
        {
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.StrokeColor, MainColor.StrokeColor);
            properties.StrokeColor = MainColor.StrokeColor;
        }

        // 箭头大小
        private void arrowSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            ArrowSize size;
            switch (cmbArrowSize.SelectedIndex)
            {
                case 0:
                    size = ArrowSize.Small;
                    break;
                case 1:
                    size = ArrowSize.Medium;
                    break;
                case 2:
                default:
                    size = ArrowSize.Large;
                    break;
            }
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.ArrowSize, size);
            properties.IndicatorLineSize = size;
        }

        // 线宽
        private void strokeWidth_TextChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            float v;
            if(float.TryParse(cmbStrokeWidth.Text,out v))
            {
                _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.StrokeWidth, v);
                properties.PenWidth = v;
            }
            else
            {

            }
        }

        // 绘制模式（填充、描边、皆有）
        private void paintType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            ShapePaintType type;
            switch (cmbPaintType.SelectedIndex)
            {
                case 0:
                    type = ShapePaintType.Stroke;
                    break;
                case 1:
                    type = ShapePaintType.Fill;
                    break;
                case 2:
                default:
                    type = ShapePaintType.StrokeAndFill;
                    break;
            }
            _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.PaintType, type);
            properties.PaintType = type;
        }

        // 圆角
        private void roundRadius_TextChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            int v;
            if (int.TryParse(cmbRoundRadius.Text, out v))
            {
                _canvas.Kernel.SetPropertyValue(ShapePropertyValueType.RoundedRadius, v);
                properties.RadiusAll = v;
            }
            else
            {

            }
        }

        //--------------------------------------------
        // 菜单
        //--------------------------------------------
        // 全选
        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            _canvas.Kernel.SelectAllShapes();
        }

        // 清空
        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            _canvas.Kernel.ClearShapes();
        }

        // 删除
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            _canvas.Kernel.DeleteSelectedShapes();
        }

        // 保存
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "png(*.png)|*.png";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fn = dlg.FileName;
                Bitmap bm = _canvas.Kernel.FinalBitmap;
                bm.Save(fn, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        // 退出
        private void ExistMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // 关于
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            (new FormAbout()).ShowDialog();
        }
    }
}
