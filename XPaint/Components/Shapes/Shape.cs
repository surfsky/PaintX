using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 图形基类
    /// </summary>
    public class Shape : IDisposable
    {
        #region Field

        /// <summary>表示该元素所依附的EPKernel类，该类负责把元素绘制到自己的Bitmap上</summary>
        private XKernel _container;
        private Matrix _matrix;
        private BaseProperty _property;

        /// <summary>唯一性标志（考虑用snowflakeid或guid）</summary>
        public long ID { get; set; }
        public bool IsCreating { get; set; }
        public bool IsSelected { get; set; }
        public bool IsLocked { get; set; }
        public bool IsVisible { get; set; }
        public virtual ToolType Type { get; }
        public virtual string Name { get; }
        public Knob[] Knobs { get; set; }
        protected Point StartPt { get; set; }
        protected GraphicsPath Path { get; set; }
        protected Point LastPt { get; set; }
        protected PointF CenterPt { get; set; }
        protected PointF RotaterPt { get; set; }
        /// <summary>
        /// 用于表示在元素变化前的范围边框。与元素变化后的范围边框进行Union，就可以确定
        /// 这次元素变化引发的需要刷新的视图区域了。该变量应该在path变化前，被赋予
        /// path.GetBounds()值，且该值应该在元素绘制后失效，可将其设置为empty
        /// </summary>
        protected Rectangle PreRect { get; set; }




        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 刷新区域。比 Path.Bounds 膨胀了 12 个像素。
        /// 这个矩形用于元素刷新操作。注意具体的元素其矩形是不相同的，比如当直线
        /// 带有箭头时，椭圆画线很粗时，这个矩形就要大于path.GetBounds()
        /// </summary>
        public virtual Rectangle RefreshRect
        {
            get
            {
                RectangleF r = Path.GetBounds();
                int x = (int)r.Left - 12;
                int y = (int)r.Top - 12;
                int w = (int)r.Width + 24;
                int h = (int)r.Height + 24;
                return new Rectangle(x, y, w, h);
            }
        }

        public virtual BaseProperty ShapeProperty
        {
            get { return _property; }
            set
            {
                BaseProperty old = _property;
                _property = value;
                AfterPropertyChanged(old, value);
                RefreshContainer();
            }
        }

        #endregion

        #region constructors

        public Shape(XKernel container, BaseProperty property) 
        {
            Path = new GraphicsPath();
            _matrix = new Matrix();
            _container = container;
            _property = property;

            IsVisible = true;
            IsCreating = true;
        }

        #endregion


        #region protected methods

        protected void SetNewScaledPath(PointF[] p)
        {
            byte[] type = Path.PathTypes;
            BeforeTransform();
            Path.Dispose();
            Path = new GraphicsPath(p, type);
            AfterTransform(TransformType.Scale, true);
        }

        protected void SetNewScaledPath(PointF[] p, byte[] types)
        {            
            BeforeTransform();
            Path.Dispose();
            Path = new GraphicsPath(p, types);
            AfterTransform(TransformType.Scale, true);
        }

        /// <summary>旋转图层</summary>
        /// <param name="newPt">新的光标点</param>
        protected void Rotate(Point newPt)
        {
            float a1 = (float)(Math.Atan2(LastPt.Y - CenterPt.Y, LastPt.X - CenterPt.X) * 180 / Math.PI);  // 原角度
            float a2 = (float)(Math.Atan2(newPt.Y - CenterPt.Y, newPt.X - CenterPt.X) * 180 / Math.PI);    // 新角度
            if (a1 < 0)
                a1 += 360;
            if (a2 < 0)
                a2 += 360;
            _matrix.Reset();
            _matrix.RotateAt(a2 - a1, CenterPt);
            BeforeTransform();
            Path.Transform(_matrix);
            AfterTransform(TransformType.Rotate, true);
        }

        /// <summary>路径变换前处理</summary>
        protected void BeforeTransform()
        {
            PreRect = RefreshRect;
        }

        /// <summary>路径变换后处理</summary>
        protected virtual void AfterTransform(TransformType type, bool refreshPath)
        {
            if (refreshPath)
                RefreshContainer();

            if (!IsCreating)
                RecalcKnobs();
        }

        /// <summary>
        /// 该方法必须在path变化之后(形状正在创建的除外)调用，以便及时刷新手柄区域
        /// </summary>
        protected virtual void RecalcKnobs()
        {

        }

        protected virtual void AfterPropertyChanged(BaseProperty oldValue, BaseProperty newValue)
        {

        }

        private void RefreshContainer()
        {
            //_container.RefleshBitmap();
            _container.RefreshBitmap(Rectangle.Union(PreRect, RefreshRect));
        }

        #endregion


        #region public methods

        public virtual void SetEndPoint(Point pt) { }
        public virtual void Draw(Graphics g) { }
        public virtual void MoveKnob(int index, Point newPos) { }

        public virtual void SetStartPoint(Point pt)
        {
            StartPt = pt;
        }

        public void SetStartTransformPoint(Point pt)
        {
            LastPt = pt;
        }

        public void DrawSelectedRect(Graphics g)
        {
            DrawSelection(g, true);
        }

        public virtual void DrawSelection(Graphics g, bool withAnchors)
        {
            
        }

        /// <summary>
        /// 判断形状是否有任意点被给定的矩形包含
        /// </summary>
        public virtual bool Intersect(Rectangle rect)
        {
            return Path.GetBounds().IntersectsWith(rect);
        }

        public virtual bool Contains(Point pos)
        {
            return Path.IsVisible(pos);
        }

        public virtual bool IsEndPointAcceptable(Point endPoint)
        {
            int deltax = Math.Abs(endPoint.X - StartPt.X);
            int deltay = Math.Abs(endPoint.Y - StartPt.Y);
            int d = XConsts.AcceptableMinMoveDistance;
            return (deltax >= d || deltay >= d);
        }

        public void Move(int offsetx, int offsety)
        {
            Move(offsetx, offsety, true);
        }

        public void Move(int offsetx, int offsety, bool reflesh)
        {
            _matrix.Reset();
            _matrix.Translate(offsetx, offsety);
            BeforeTransform();
            Path.Transform(_matrix);
            AfterTransform(TransformType.Move,reflesh);
        }

        #endregion

        #region IDisposable interface
        
        public virtual void Dispose()
        {
            if (Path != null)
                Path.Dispose();
            if (_matrix != null)
                _matrix.Dispose();
        }

        #endregion        
    }
}
