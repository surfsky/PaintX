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
        private enum SelectToolsLocation
        {
            Blank,
            UnselectedShape,
            SelectedShape,
            ShapeDraggableHotSpot
        }

        //--------------------------------------------
        // 成员
        //--------------------------------------------
        private EventHandlerList _events;
        private Bitmap finalBitmap;
        private Graphics graphics;
        private CursorType cursorType;
        private SelectToolsLocation selectToolLoc;
        private bool isInSelecting;

        private bool mousePressed;
        private bool isEverMove;
        private Shape shapeDrawing;
        private ToolType currentTool;
        private Point lastPt;
        private Point downPt;

        private int pressedHotSpotIndex;
        private PropertyCollector properties;


        // 公开属性
        public List<Shape> Shapes;    // TODO: 考虑用双向列表来容纳，便于调整层次
        private List<int> selectedIds;


        //--------------------------------------------
        // 构造函数
        //--------------------------------------------
        public XKernel()
            :this(new Size(400, 300)){}

        public XKernel(Size bitmapSize)
            :this(new Bitmap(bitmapSize.Width, bitmapSize.Height))
        {}

        public XKernel(Bitmap bitmap)
        {
            finalBitmap = bitmap;
            graphics = Graphics.FromImage(bitmap);
            _events = new System.ComponentModel.EventHandlerList();

            InitialValue();
        }

        private void InitialValue()
        {
            Shapes = new List<Shape>();
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            selectedIds = new List<int>();
            mousePressed = false;
            properties = new PropertyCollector();
        }


        #region inner methods

        private void ClearSelectedShapes(bool fireEvent=true)
        {
            if (selectedIds.Count > 0)
            {
                for (int i = 0; i < selectedIds.Count; i++)
                {
                    Shapes[selectedIds[i]].IsSelected = false;                    
                }
                selectedIds.Clear();
                //lastSelectedShapeIndex = -1;
                if (fireEvent)
                    OnSelectedShapesChanged(EventArgs.Empty);
            }
        }

        /// <summary>选择指定图层</summary>        
        private bool SelectShape(int shapeId, bool fireEvent)
        {
            if (shapeId >= 0 && shapeId < Shapes.Count)
            {
                if (!selectedIds.Contains(shapeId))
                {
                    Shapes[shapeId].IsSelected = true;
                    selectedIds.Add(shapeId);
                    if (fireEvent)
                        OnSelectedShapesChanged(EventArgs.Empty);
                    return true;
                }
            }
            return false;
        }

        /// <summary>选择指定图层</summary>
        private void SelectShapes(int[] ids)
        {
            bool ok = false;
            for (int i = 0; i < ids.Length; i++)
                ok = SelectShape(ids[i], false);

            if(ok)
                OnSelectedShapesChanged(EventArgs.Empty);
        }

        /// <summary>寻找指定点对应的图形</summary>
        private int GetFirstShape(Point pos)
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].Contains(pos))
                    return i;
            }
            return -1;
        }

        /// <summary>寻找指定区域内的所有图层</summary>
        private int[] GetShapeIds(Rectangle rect)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (Shapes[i].Intersect(rect))
                    list.Add(i);
            }
            return list.ToArray();
        }

        private void SelectTool_MouseDown(Point pos)
        {
            downPt = pos;
            bool inHotSpot = false;
            if (SelectedShapesCount == 1)
            {
                int i = selectedIds[0];
                Knob[] hs = Shapes[i].Knobs;
                for (int j = 0; j < hs.Length; j++)
                {
                    if (hs[j].Visible && hs[j].Rect.Contains(pos))
                    {
                        inHotSpot = true;
                        pressedHotSpotIndex = j;
                        Shapes[i].SetStartTransformPoint(pos);
                        selectToolLoc = SelectToolsLocation.ShapeDraggableHotSpot;
                        break;
                    }
                }
            }
            if (!inHotSpot)
            {
                int index = GetFirstShape(pos);
                if (index != -1)
                {
                    if (Shapes[index].IsSelected)
                    {
                        selectToolLoc = SelectToolsLocation.SelectedShape;
                    }
                    else
                    {
                        selectToolLoc = SelectToolsLocation.UnselectedShape;
                        ClearSelectedShapes(false);
                        SelectShape(index, true);
                        CursorType = CursorType.SelectMove;
                    }
                    lastPt = pos;
                }
                else
                {
                    selectToolLoc = SelectToolsLocation.Blank;
                }
            }
        }

        private void SelectTool_MouseMove(Point point)
        {
            if (mousePressed)
            {
                switch (selectToolLoc)
                {
                    case SelectToolsLocation.Blank:
                        lastPt = point;
                        isInSelecting = true;
                        OnSelecting(EventArgs.Empty);
                        break;
                    case SelectToolsLocation.UnselectedShape:
                    case SelectToolsLocation.SelectedShape:
                        for (int i = 0; i < selectedIds.Count; i++)
                        {
                            Shapes[selectedIds[i]].Move(
                                point.X - lastPt.X, 
                                point.Y - lastPt.Y,
                                i == selectedIds.Count - 1);
                        }                        
                        lastPt = point;
                        break;
                    case SelectToolsLocation.ShapeDraggableHotSpot:
                        Shapes[selectedIds[0]].MoveKnob(pressedHotSpotIndex, point);
                        break;
                }
            }
            else
            {
                CursorType type = CursorType.SelectDefault;
                if (SelectedShapesCount == 1)
                {
                    int index = selectedIds[0];
                    bool inHotSpot = false;
                    Knob[] hs = Shapes[index].Knobs;
                    
                    for (int j = 0; j < hs.Length; j++)
                    {
                        if (hs[j].Visible && hs[j].Rect.Contains(point))
                        {
                            switch (hs[j].Type)
                            {
                                case KnobType.Line:
                                    type = CursorType.SelectDragVertex;
                                    break;
                                case KnobType.Rotate:
                                    type = CursorType.SelectRotate;
                                    break;
                                case KnobType.Scale:
                                    type = CursorType.SelectScale;
                                    break;
                            }
                            inHotSpot = true;
                            break;
                        }
                    }

                    if (!inHotSpot && Shapes[index].Contains(point))
                    {
                        type = CursorType.SelectMove;
                    }
                }
                else if (SelectedShapesCount > 1)
                {
                    // multiple selected, then shapes can just be moved
                    for (int i = 0; i < selectedIds.Count; i++)
                    {
                        if (Shapes[selectedIds[i]].Contains(point))
                        {
                            type = CursorType.SelectMove;
                            break;
                        }
                    }
                }
                CursorType = type;
            }
        }

        private void SelectTool_MouseUp(Point pos)
        {
            if (!isEverMove)
            {
                switch (selectToolLoc)
                {
                    case SelectToolsLocation.Blank:
                        ClearSelectedShapes();
                        isInSelecting = false;
                        OnSelecting(EventArgs.Empty);
                        break;
                    case SelectToolsLocation.SelectedShape:
                        if (SelectedShapesCount > 0)
                        {
                            ClearSelectedShapes(false);
                            SelectShape(GetFirstShape(pos), true);
                        }
                        break;
                }
            }
            else
            {
                switch (selectToolLoc)
                {
                    case SelectToolsLocation.Blank:
                        ClearSelectedShapes();
                        int[] rst = GetShapeIds(SelectingRect);
                        if (rst.Length > 0)
                            SelectShapes(rst);
                        isInSelecting = false;
                        OnSelecting(EventArgs.Empty);
                        break;                    
                }
            }
        }

        private void SetShapeProperty(ShapeValueType type, object value)
        {
            switch (type)
            {
                case ShapeValueType.Antialias:
                    properties.Antialias = (bool)value;
                    break;

                case ShapeValueType.ArrowSize:
                    properties.ArrowSize = (ArrowSize)value;
                    break;                    

                case ShapeValueType.StrokeWidth:
                    properties.PenWidth = (float)value;
                    break;
                case ShapeValueType.StrokeColor:
                    properties.StrokeColor = (Color)value;
                    break;
                case ShapeValueType.StartLineCap:
                    properties.StartLineCap = (LineCapType)value;
                    break;
                case ShapeValueType.EndLineCap:
                    properties.EndLineCap = (LineCapType)value;
                    break;
                case ShapeValueType.LineDash:
                    properties.LineDash = (LineType)value;
                    break;
                case ShapeValueType.LineJoin:
                    properties.HowLineJoin = (LineJoin)value;
                    break;
                case ShapeValueType.PenAlignment:
                    properties.PenAlign = (PenAlignment)value;
                    break;

                case ShapeValueType.FillColor:
                    properties.FillColor = (Color)value;
                    break;
                case ShapeValueType.FillType:
                    properties.FillType = (FillType)value;
                    break;
                case ShapeValueType.PaintType:
                    properties.PaintType = (PaintType)value;
                    break;

                case ShapeValueType.RoundedRadius:
                    properties.RadiusAll = (int)value;
                    break;
            }
            ApplyNewProperty();
        }

        private void ApplyNewProperty()
        {
            if (SelectedShapesCount < 1)
                return;

            // currently only support one selected shape
            if (SelectedShapesCount == 1)
            {
                Shape shape = Shapes[selectedIds[0]];
                switch (shape.ShapeProperty.PropertyType)
                {
                    case ShapePropertyType.Arrow:
                        shape.ShapeProperty = properties.GetArrowProperty();
                        break;

                    case ShapePropertyType.Stroke:
                        shape.ShapeProperty = properties.GetStrokableProperty();
                        break;

                    case ShapePropertyType.Fill:
                        shape.ShapeProperty = properties.GetFillableProperty();
                        break;

                    case ShapePropertyType.RoundedRect:
                        shape.ShapeProperty = properties.GetRoundedRectProperty();
                        break;
                }
            }
        }

        /// <summary>检测当前图形的属性是否有变更，若有则触发事件</summary>
        private void CheckShapeProperty()
        {
            if (currentTool != ToolType.Select || SelectedShapesCount != 1)
                return;

            bool diff = false;
            BaseProperty basepro = Shapes[selectedIds[0]].ShapeProperty;
            if (properties.Antialias != basepro.Antialias)
            {
                properties.Antialias = basepro.Antialias;
                diff = true;
            }

            if(basepro is ArrowProperty)
            {
                ArrowProperty ip = (ArrowProperty)basepro;
                if (ip.LineColor != properties.StrokeColor ||
                    ip.LineSize != properties.ArrowSize)
                {
                    diff = true;
                    properties.StrokeColor=ip.LineColor;
                    properties.ArrowSize = ip.LineSize;
                }
            }

            if (basepro is StrokableProperty)
            {
                StrokableProperty sp = (StrokableProperty)basepro;
                if (properties.PenWidth != sp.PenWidth ||
                    properties.StrokeColor != sp.StrokeColor ||
                    properties.LineDash != sp.LineDash ||
                    properties.StartLineCap != sp.StartLineCap ||
                    properties.EndLineCap != sp.EndLineCap ||
                    properties.PenAlign != sp.PenAlign ||
                    properties.HowLineJoin != sp.HowLineJoin)
                {
                    diff = true;
                    properties.PenWidth = sp.PenWidth;
                    properties.StrokeColor = sp.StrokeColor;
                    properties.LineDash = sp.LineDash;
                    properties.StartLineCap = sp.StartLineCap;
                    properties.EndLineCap = sp.EndLineCap;
                    properties.PenAlign = sp.PenAlign;
                    properties.HowLineJoin = sp.HowLineJoin;
                }
            }

            if (basepro is FillableProperty)
            {
                FillableProperty fp = (FillableProperty)basepro;
                if (properties.PaintType != fp.PaintType ||
                    properties.FillColor != fp.FillColor ||
                    properties.FillType != fp.FillType)
                {
                    diff = true;
                    properties.PaintType = fp.PaintType;
                    properties.FillColor = fp.FillColor;
                    properties.FillType = fp.FillType;
                }
            }

            if (basepro is RoundedRectProperty)
            {
                RoundedRectProperty rp = (RoundedRectProperty)basepro;
                if (properties.RadiusAll != rp.RadiusAll)
                {
                    diff = true;
                    properties.RadiusAll = rp.RadiusAll;
                }
            }

            if (diff)
            {
                OnPropertyCollectorChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region private properties

        private Rectangle SelectingRect
        {
            get
            {
                int x = downPt.X;
                int y = downPt.Y;
                int w = lastPt.X - x;
                int h = lastPt.Y - y;
                if (w < 0)
                {
                    w = -w;
                    x -= w;
                }
                if (h < 0)
                {
                    h = -h;
                    y -= h;
                }
                return new Rectangle(x, y, w, h);
            }
        }

        #endregion

        #region public properties
        //----------------------------------------------------
        // Property
        //----------------------------------------------------

        /// <summary>
        /// 获取绘制了所有形状的最终的Bitmap
        /// </summary>
        public Bitmap FinalBitmap => finalBitmap;

        public Shape[] SelectedShapes
        {
            get
            {
                if (selectedIds.Count < 1)
                    return new Shape[] { };
                Shape[] shapes = new Shape[selectedIds.Count];
                int i = 0;
                foreach (int index in selectedIds)
                    shapes[i++] = this.Shapes[index];                

                return shapes;
            }
            set
            {
                selectedIds = new List<int>();
                if (value != null)
                {
                    foreach (var shape in value)
                    {
                        for (int i = 0; i < Shapes.Count; i++)
                        {
                            if (shape == Shapes[i])
                                selectedIds.Add(i);
                        }
                    }
                }
                OnSelectedShapesChanged(null);
            }
        }

        /// <summary>被选中的形状的个数</summary>
        public int SelectedShapesCount
        {
            get { return selectedIds.Count; }
        }

        /// <summary>当前需要的鼠标指针类型</summary>
        public CursorType CursorType
        {
            get { return cursorType; }
            private set
            {
                if (cursorType != value)
                {
                    cursorType = value;
                    OnCursorTypeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>是否处于拖动选择状态</summary>
        public bool IsInSelecting
        {
            get { return isInSelecting; }
        }

        public PropertyCollector ShapePropertyCollector
        {
            get { return properties.Clone(); }
        }

        #endregion

        #region public methods

        public void RefreshBitmap()
        {
            RefreshBitmap(Rectangle.Empty);
        }

        /// <summary>局部刷新图像</summary>
        /// <param name="rect">需刷新的区域</param>
        public void RefreshBitmap(Rectangle rect)
        {
            bool empty = rect.IsEmpty;
            if (!empty)
                graphics.SetClip(rect);

            graphics.FillRectangle(Brushes.White, new Rectangle(Point.Empty, finalBitmap.Size));

            // 只刷新相交的图层
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (empty || rect.IntersectsWith(Shapes[i].RefreshRect))
                    Shapes[i].Draw(graphics);
            }
            // 绘制正在创建的图层
            if (shapeDrawing != null && (empty || rect.IntersectsWith(shapeDrawing.RefreshRect)))
                shapeDrawing.Draw(graphics);

            OnFinalBitmapChanged(EventArgs.Empty);
            graphics.ResetClip();
        }

        public void MouseDown(Point pos)
        {
            mousePressed = true;
            isEverMove = false;            

            if(currentTool != ToolType.Select)
                ClearSelectedShapes();

            switch (currentTool)
            {
                case ToolType.Line:
                    shapeDrawing = new SLine(this, properties.GetStrokableProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;
                case ToolType.Polyline:
                    shapeDrawing = new SPolyline(this, properties.GetStrokableProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;
                case ToolType.Arrow:
                    shapeDrawing = new SArrow(this, properties.GetArrowProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;

                case ToolType.Rect:
                    shapeDrawing = new SRect(this, properties.GetFillableProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;
                case ToolType.RoundedRect:
                    shapeDrawing = new SRoundRect(this, properties.GetRoundedRectProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;
                case ToolType.Ellipse:
                    shapeDrawing = new SEllipse(this, properties.GetFillableProperty());
                    shapeDrawing.SetStartPoint(pos);
                    break;

                case ToolType.Select:
                    SelectTool_MouseDown(pos);                    
                    break;
            }
        }

        public void MouseMove(Point pos)
        {            
            isEverMove = true;

            switch (currentTool)
            {
                case ToolType.Line:
                case ToolType.Polyline:
                case ToolType.Arrow:
                case ToolType.Rect:
                case ToolType.RoundedRect:
                case ToolType.Ellipse:
                    if (!mousePressed)
                        return;
                    if (shapeDrawing != null)
                    {
                        shapeDrawing.SetEndPoint(pos);
                    }
                    break;

                case ToolType.Select:
                    SelectTool_MouseMove(pos);                    
                    break;
            }
        }

        public void MouseUp(Point pos)
        {            
            switch (currentTool)
            {
                case ToolType.Line:
                case ToolType.Polyline:
                case ToolType.Arrow:
                case ToolType.Rect:
                case ToolType.RoundedRect:
                case ToolType.Ellipse:
                    if (!isEverMove)
                    {
                        shapeDrawing = null;
                        mousePressed = false;
                        return;
                    }
                    if (shapeDrawing != null)
                    {                        
                        if (shapeDrawing.IsEndPointAcceptable(pos))
                        {
                            shapeDrawing.ID = SnowflakeID.Instance.NewID();
                            shapeDrawing.IsCreating = false;
                            shapeDrawing.SetEndPoint(pos);
                            Shapes.Add(shapeDrawing);
                            OnShapesChanged(EventArgs.Empty);
                            SelectShape(Shapes.Count - 1, true);                            
                        }
                        shapeDrawing = null;
                        currentTool = ToolType.Select;
                    }
                    break;
                case ToolType.Select:
                    SelectTool_MouseUp(pos);
                    break;
            }
            mousePressed = false;
        }

        /// <summary>设置当前工具模式</summary>
        /// <param name="type"></param>
        public void SetTool(ToolType type)
        {
            currentTool = type;
            switch (type)
            {
                case ToolType.Ellipse:
                    CursorType = CursorType.Ellipse;
                    break;
                case ToolType.RoundedRect:
                case ToolType.Rect:
                    CursorType = CursorType.Rect;
                    break;
                case ToolType.Arrow:
                case ToolType.Line:
                case ToolType.Polyline:
                    CursorType = CursorType.Line;
                    break;
                case ToolType.Select:
                    CursorType = CursorType.SelectDefault;
                    break;
                case ToolType.Hand:
                    CursorType = CursorType.Hand;
                    break;
                case ToolType.Custom:
                    CursorType = CursorType.Default;
                    break;
            }
        }

        /// <summary>Draw the sizable rects for selected shapes</summary>
        public void DrawSizableRects(Graphics g)
        {
            if (SelectedShapesCount < 1)
                return;

            bool one = (SelectedShapesCount == 1);
            for (int i = 0; i < selectedIds.Count; i++)
            {
                Shapes[selectedIds[i]].DrawSelection(g, one);
            }
        }

        /// <summary>绘制拖动时的选择矩形</summary>
        public void DrawSelectingRect(Graphics g)
        {
            using (Pen p = new Pen(Color.Gray))
            {
                p.DashPattern = new float[] { 4.0f, 4.0f };
                g.DrawRectangle(p, SelectingRect);
            }
        }

        public void SetValue(ShapeValueType type, object value)
        {
            switch (currentTool)
            {
                case ToolType.Line:
                case ToolType.Polyline:
                case ToolType.Arrow:
                case ToolType.Rect:
                case ToolType.Ellipse:
                case ToolType.RoundedRect:
                case ToolType.Select:
                    SetShapeProperty(type, value);
                    break;
            }
        }

        public void SelectAllShapes()
        {
            if (Shapes.Count == 0 || (Shapes.Count == selectedIds.Count))
                return;

            selectedIds.Clear();
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].IsSelected = true;
                selectedIds.Add(i);
            }
            OnSelectedShapesChanged(EventArgs.Empty);
        }

        public void ClearShapes()
        {
            if (Shapes.Count == 0)
                return;

            selectedIds.Clear();
            Shapes.Clear();
            RefreshBitmap();
            OnSelectedShapesChanged(EventArgs.Empty);
            OnShapesChanged(EventArgs.Empty);
        }

        public void DeleteSelectedShapes()
        {
            if (this.Shapes.Count == 0 || selectedIds.Count == 0)
                return;
            Shape[] shapes = new Shape[selectedIds.Count];
            for (int i = 0; i < selectedIds.Count; i++)
                shapes[i] = this.Shapes[selectedIds[i]];
            foreach (Shape b in shapes)
                this.Shapes.Remove(b);

            selectedIds.Clear();
            RefreshBitmap();
            OnSelectedShapesChanged(EventArgs.Empty);
            OnShapesChanged(EventArgs.Empty);
        }

        // tmp func
        public void GetTransparentBitmap(Bitmap bm)
        {
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                for (int i = 0; i < Shapes.Count; i++)
                {                    
                    Shapes[i].Draw(g);
                }
            }
        }

        #endregion


    }
}
