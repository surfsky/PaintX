using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using App.Core;
using XPaint.Components;

namespace XPaint
{
    public partial class FormMain : Form
    {
        private XCanvas _canvas;
        private PropertyCollector _properties;
        private bool comingBackFromShape;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitCanvas();
            InitData();
            InitTools();
            this.KeyPreview = true;
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
            _canvas.AllowDrop = true;
            this.panMain.Controls.Add(_canvas);

            _canvas.Kernel.SelectedShapesChanged += new EventHandler(Kernel_SelectedShapesChanged);
            _canvas.Kernel.PropertyCollectorChanged += new EventHandler(Kernel_PropertyCollectorChanged);
            _canvas.Kernel.ShapesChanged += new EventHandler(Kernel_ShapesChanged);
            SetDrag();
        }

        /// <summary>初始化工具列表</summary>
        void InitTools()
        {
            this.lbTool.Items.Clear();
            foreach (var item in XTool.All)
            {
                this.lbTool.Items.Add(item);
            }
        }

        /// <summary>初始化属性数据</summary>
        private void InitData()
        {
            _properties = new PropertyCollector();
            for (int i = 1; i <= 24; i++)
                cmbStrokeWidth.Items.Add(i);
            cmbStrokeWidth.SelectedIndex = 0;

            //
            UI.BindEnum(cmbPaintType, typeof(PaintType));
            UI.BindEnum(cmbArrowSize, typeof(ArrowSize));

            // 圆角矩形
            cmbRoundRadius.Items.Add(4);
            cmbRoundRadius.Items.Add(6);
            cmbRoundRadius.Items.Add(8);
            cmbRoundRadius.SelectedIndex = 2;

            // antialias
            menuAnliaTrue.Tag = true;
            menuAliaFalse.Tag = false;

            //UI.BindEnum(cmbStartCap, typeof(LineCapType));
            //UI.BindEnum(cmbEndCap, typeof(LineCapType));
            //UI.BindEnum(cmbLine, typeof(LineType));

            // line startcap
            menuStartcapRound.Tag = LineCapType.Rounded;
            menuStartcapSquare.Tag = LineCapType.Square;
            menuStartcapRect.Tag = LineCapType.Rect;
            menuStartcapCircle.Tag = LineCapType.Circle;
            menuStartcapLineArrow.Tag = LineCapType.LineArrow;
            menuStartcapNormalArrow.Tag = LineCapType.NormalArrow;
            menuStartcapSharpArrow.Tag = LineCapType.SharpArrow;
            menuStartcapSharpArrow2.Tag = LineCapType.SharpArrow2;

            // line endcap
            menuEndcapRound.Tag = LineCapType.Rounded;
            menuEndcapSqare.Tag = LineCapType.Square;
            menuEndcapRect.Tag = LineCapType.Rect;
            menuEndcapCircle.Tag = LineCapType.Circle;
            menuEndcapLineArrow.Tag = LineCapType.LineArrow;
            menuEndcapNormalArrow.Tag = LineCapType.NormalArrow;
            menuEndcapSharpArrow.Tag = LineCapType.SharpArrow;
            menuEndcapSharpArrow2.Tag = LineCapType.SharpArrow2;

            // line dash type
            menuLinedashSolid.Tag = LineType.Solid;
            menuLinedashDot.Tag = LineType.Dot;
            menuLinedashDashDot.Tag = LineType.DashedDot;
            menuLinedashDashDotDot.Tag = LineType.DashedDotDot;
            menuLinedashDash1.Tag = LineType.Dash1;
            menuLinedashDash2.Tag = LineType.Dash2;
        }


        //---------------------------------------
        // drag and drop
        //---------------------------------------
        // 启动拖动
        private void lbTool_MouseDown(object sender, MouseEventArgs e)
        {
            var o = lbTool.SelectedItem as XTool;
            if (o != null)
            {
                SelectTool(o.Type);
                _canvas.DoDragDrop(o.Type.ToString(), DragDropEffects.Copy);
            }
        }

        void SetDrag()
        {
            _canvas.AllowDrop = true;
            _canvas.DragDrop += Canvas_DragDrop;
            _canvas.DragEnter += Canvas_DragEnter;
        }
        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {
            // 放下文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (Array)e.Data.GetData(DataFormats.FileDrop);
                foreach (object f in files)
                {
                    var fileName = f.ToString();
                    var p = this.PointToClient(new Point(e.X, e.Y));
                    Debug.WriteLine(string.Format("{0} ({1},{2})", fileName, p.X, p.Y));
                }
            }
            // 放置文本
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // 如果是字符 { 开头的，解析为 json 再处理
                // 否则创建文本控件
                var p = new Point(e.X, e.Y);
                _canvas.Kernel.MouseDown(p);
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                // 创建图片图形
            }
        }




        //---------------------------------------
        // init controls
        //---------------------------------------

        /// <summary>选中工具</summary>
        private void SelectTool(ToolType type)
        {
            _canvas.Kernel.SetTool(type);
            switch (type)
            {
                case ToolType.Line:
                    SwitchPropertyCtrls(ShapePropertyType.Stroke);
                    break;
                case ToolType.Arrow:
                    SwitchPropertyCtrls(ShapePropertyType.Arrow);
                    break;
                case ToolType.Polyline:
                    SwitchPropertyCtrls(ShapePropertyType.Stroke);
                    break;
                case ToolType.Rect:
                    SwitchPropertyCtrls(ShapePropertyType.Fill);
                    break;
                case ToolType.RoundedRect:
                    SwitchPropertyCtrls(ShapePropertyType.RoundedRect);
                    break;
                case ToolType.Ellipse:
                    SwitchPropertyCtrls(ShapePropertyType.Fill);
                    break;
                case ToolType.Select:
                    if (_canvas.Kernel.SelectedShapesCount != 1)
                        SwitchPropertyCtrls(ShapePropertyType.Empty);
                    break;
            }
        }

        /// <summary>显隐当前图形对应的属性控件</summary>
        private void SwitchPropertyCtrls(ShapePropertyType type)
        {
            switch (type)
            {
                case ShapePropertyType.Empty:
                    ShowAntialiasCtrls(false);
                    ShowStrokeCtrls(false);
                    ShowFillCtrls(false);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.Stroke:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(false);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.Fill:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(true);
                    ShowRoundCtrls(false);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.RoundedRect:
                    ShowAntialiasCtrls(true);
                    ShowStrokeCtrls(true);
                    ShowFillCtrls(true);
                    ShowRoundCtrls(true);
                    ShowArrowCtrls(false);
                    break;
                case ShapePropertyType.Arrow:
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
            bool isnull = (_properties == null);

            // anti-alias
            if (isnull || _properties.Antialias != ps.Antialias)
                btnAntialias.Image = XRes.GetAntialiasImage(ps.Antialias);

            // indicator arrow size
            if (isnull || _properties.ArrowSize != ps.ArrowSize)
                UI.SetEnumValue(cmbArrowSize, ps.ArrowSize);

            // stroke color
            if (isnull || _properties.StrokeColor != ps.StrokeColor)
            {
                MainColor.StrokeColor = ps.StrokeColor;
            }

            // stroke width
            if (isnull || _properties.PenWidth != ps.PenWidth)
            {
                cmbStrokeWidth.Text = ps.PenWidth.ToString();
            }

            // line start cap
            if (isnull || _properties.StartLineCap != ps.StartLineCap)
                btnStartCapType.Image = XRes.GetLineStartCapImage(ps.StartLineCap);

            // line end cap
            if (isnull || _properties.EndLineCap != ps.EndLineCap)
                btnEndCapType.Image = XRes.GetLineEndCapImage(ps.EndLineCap);

            // line dash type
            if (isnull || _properties.LineDash != ps.LineDash)
                btnLineType.Image = XRes.GetDashLineImage(ps.LineDash);

            // paint type
            if (isnull || _properties.PaintType != ps.PaintType)
            {
                UI.SetEnumValue(cmbPaintType, ps.PaintType);
            }

            // fill color
            if (isnull || _properties.FillColor != ps.FillColor)
            {
                MainColor.FillColor = ps.FillColor;
            }

            // round radius
            if (isnull || _properties.RadiusAll != ps.RadiusAll)
            {
                cmbRoundRadius.Text = ps.RadiusAll.ToString();
            }

            _properties = ps;
            comingBackFromShape = false;
        }



        //------------------------------------------------
        // Canvas 事件
        //------------------------------------------------
        /// <summary>图形列表变更事件</summary>
        private void Kernel_ShapesChanged(object sender, EventArgs e)
        {
            // 新增或删除形状后重新设为选择模式 surfsky 2020-07-10
            this.lbShapes.SelectedIndex = -1;
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
            //if (rdoSelect.Checked)
            //if (lbShapes.SelectedIndex == 0)
            {
                ShowSelectedShapes();
                if (_canvas.Kernel.SelectedShapesCount == 1)
                {
                    Shape shape = _canvas.Kernel.SelectedShapes[0];
                    SwitchPropertyCtrls(shape.ShapeProperty.PropertyType);
                }
                else
                {
                    SwitchPropertyCtrls(ShapePropertyType.Empty);
                }
            }
        }

        /// <summary>（选中图形）属性变更事件</summary>
        private void Kernel_PropertyCollectorChanged(object sender, EventArgs e)
        {
            SetProperties(_canvas.Kernel.ShapePropertyCollector);
        }

        //------------------------------------------------
        // 图层
        //------------------------------------------------
        // 选择图层
        private void lbShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbShapes.SelectedIndex != -1)
            {
                var shape = lbShapes.SelectedItem as Shape;
                _canvas.Kernel.SelectedShapes = new Shape[] { shape };  // 会触发Kernel_SelectedShapesChanged 事件
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



        //------------------------------------------------
        // 属性控制
        //------------------------------------------------
        // 抗锯齿
        private void antiAlia_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnAntialias.Image = mi.Image;

            bool v = (bool)mi.Tag;
            _canvas.Kernel.SetValue(ShapeValueType.Antialias, v);
            _properties.Antialias = v;
        }

        // 起始线帽
        private void startcapRound_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnStartCapType.Image = mi.Image;

            LineCapType type = (LineCapType)mi.Tag;
            _canvas.Kernel.SetValue(ShapeValueType.StartLineCap, type);
            _properties.StartLineCap = type;
        }

        // 点画线
        private void dashlineSolid_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnLineType.Image = mi.Image;

            LineType type = (LineType)mi.Tag;
            _canvas.Kernel.SetValue(ShapeValueType.LineDash, type);
            _properties.LineDash = type;
        }

        // 结束点
        private void endcapRound_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            btnEndCapType.Image = mi.Image;

            LineCapType type = (LineCapType)mi.Tag;
            _canvas.Kernel.SetValue(ShapeValueType.EndLineCap, type);
            _properties.EndLineCap = type;
        }

        // 填充色变更
        private void MainColor_FillColorChange(object sender, EventArgs e)
        {
            _canvas.Kernel.SetValue(ShapeValueType.FillColor, MainColor.FillColor);
            _properties.FillColor = MainColor.FillColor;
        }

        // 线段色变更
        private void MainColor_StrokeColorChange(object sender, EventArgs e)
        {
            _canvas.Kernel.SetValue(ShapeValueType.StrokeColor, MainColor.StrokeColor);
            _properties.StrokeColor = MainColor.StrokeColor;
        }

        // 箭头大小
        private void arrowSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            ArrowSize size = (ArrowSize)UI.GetEnumValue(cmbArrowSize);
            _canvas.Kernel.SetValue(ShapeValueType.ArrowSize, size);
            _properties.ArrowSize = size;
        }

        // 线宽
        private void strokeWidth_TextChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            float v;
            if(float.TryParse(cmbStrokeWidth.Text,out v))
            {
                _canvas.Kernel.SetValue(ShapeValueType.StrokeWidth, v);
                _properties.PenWidth = v;
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

            PaintType type = (PaintType)UI.GetEnumValue(cmbPaintType);
            _canvas.Kernel.SetValue(ShapeValueType.PaintType, type);
            _properties.PaintType = type;
        }

        // 圆角
        private void roundRadius_TextChanged(object sender, EventArgs e)
        {
            if (comingBackFromShape)
                return;

            int v;
            if (int.TryParse(cmbRoundRadius.Text, out v))
            {
                _canvas.Kernel.SetValue(ShapeValueType.RoundedRadius, v);
                _properties.RadiusAll = v;
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

        //--------------------------------------------
        // 按键处理
        //--------------------------------------------
        // 按键处理( 注意要设置 Form.KeyPreview = true)
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
        }
        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                _canvas.Kernel.SelectAllShapes();
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                _canvas.Kernel.DeleteSelectedShapes();
        }

    }
}
