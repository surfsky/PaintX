using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 矩形
    /// </summary>
    public class RectShape : FillableShape
    {
        private Rectangle rect;
        private DraggableHotSpot[] _hotspots;

        public RectShape(XKernel container, FillableProperty pro)
            : base(container, pro)
        {
            initialProperties();
            initialHotspotRects(); 
        }        

        private void initialProperties()
        {
            RightSideAngle = 0;
            TopSideAngle = -Math.PI / 2;
        }

        protected virtual void initialHotspotRects()
        {
            _hotspots = new DraggableHotSpot[9];

            int i;
            for (i = 0; i <= _hotspots.Length - 2; i++)
            {
                _hotspots[i] = new DraggableHotSpot(HotSpotType.AnchorToScale);
            }
            // the last one is the rect used to rotate the shape
            _hotspots[i] = new DraggableHotSpot(HotSpotType.RotatingRect);
        }

        public override void SetStartPoint(Point pt)
        {
            base.SetStartPoint(pt);
            rect.Location = pt;
        }

        public override void SetEndPoint(Point pt)
        {
            rect.Width = pt.X - base.StartPoint.X;
            rect.Height = pt.Y - base.StartPoint.Y;

            rect.Location = base.StartPoint;
            if (rect.Width < 0)
            {
                rect.Width = -rect.Width;
                rect.X = base.StartPoint.X - rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Height = -rect.Height;
                rect.Y = base.StartPoint.Y - rect.Height;
            }

            ResetPath();
        }

        public override void SetNewPosForHotAnchor(int index, Point newPos)
        {
            if (index == _hotspots.Length - 1)
            {
                // shape rotating
                base.Rotate(newPos);
            }
            else
            {
                // shape scaling
                
                //PointF[] pf = TransformHelper.GetScaledRectPathPoints(base.Path, index,
                //    base.LastTransformPoint, newPos, false);
                PointF[] pf = GetScaledRectPath(base.Path, index, base.LastTransformPoint,
                    newPos, false, TopSideAngle, RightSideAngle);
                SetNewScaledPath(pf);
            }
            LastTransformPoint = newPos;
        }

        public override ToolType Type
        {
            get { return ToolType.Rectangle; }
        }

        public override string Name
        {
            get { return "矩形"; }
        }

        public override void DrawSelectedRect(Graphics g, bool withAnchors)
        {
            PointF[] pf = CornerAnchors;
            using (Pen p = new Pen(Color.Gray))
            {
                p.DashPattern = new float[] { 4.0F, 4.0F, 4.0F, 4.0F };
                g.DrawLines(p, pf);
                g.DrawLine(p, pf[3], pf[0]);

                if (withAnchors)
                    g.DrawLine(p, CenterPoint, RotateLocation);
            }

            // cross on center pont
            DrawCrossSign(g, CenterPoint, XConsts.CrossSignHalfWidth);            

            if (!withAnchors)
                return;
            
            DrawCrossSign(g, RotateLocation, XConsts.CrossSignHalfWidth);

            DraggableHotSpot[] hs = DraggableHotSpots;
            int i;
            for (i = 0; i < hs.Length - 1; i++)
            {
                if (!hs[i].Visible)
                    continue;

                g.FillRectangle(Brushes.White, hs[i].Rect);
                if (i < 4)
                    g.DrawRectangle(Pens.Red, hs[i].Rect);
                else
                    g.DrawRectangle(Pens.Black, hs[i].Rect);
            }
            g.DrawEllipse(Pens.Blue, hs[i].Rect);
        }

        public static void DrawCrossSign(Graphics g, PointF pt, int width)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(Pens.Blue, pt.X - width, pt.Y - width, pt.X + width, pt.Y + width);
            g.DrawLine(Pens.Blue, pt.X + width, pt.Y - width, pt.X - width, pt.Y + width);
            g.SmoothingMode = old;
        }

        protected override void AfterPathTransformed(TransformType transformType, bool refleshPath)
        {
            if (!IsInCreating)
            {
                ResetPathExtraInfo(transformType);
                if(transformType == TransformType.Scale)
                    ResetHotSpotsVisibility();
            }
            base.AfterPathTransformed(transformType, refleshPath);
        }

        protected virtual void ResetHotSpotsVisibility()
        {
            for (int i = 0; i <= _hotspots.Length - 2; i++)
                _hotspots[i].Visible = false;

            int w = XConsts.AnchorRectHalfWidth * 2;
            PointF[] pf = CornerAnchors;

            float dx = pf[1].X - pf[0].X;
            float dy = pf[1].Y - pf[0].Y;
            float top = (float)Math.Sqrt(dx * dx + dy * dy);
            dx = pf[3].X - pf[0].X;
            dy = pf[3].Y - pf[0].Y;
            float left = (float)Math.Sqrt(dx * dx + dy * dy);

            if (top < w * 4 && left < w * 4)
            {                
                _hotspots[2].Visible = true;
            }
            else if (top < w * 4 || left < w * 4)
            {
                _hotspots[2].Visible = true;
                _hotspots[0].Visible = true;
            }
            else if (top < w * 6 && left < w * 6)
            {
                _hotspots[2].Visible = true;
                _hotspots[0].Visible = true;
                _hotspots[1].Visible = true;
                _hotspots[3].Visible = true;
            }
            else
            {
                for (int i = 0; i <= 3; i++)
                    _hotspots[i].Visible = true;
                if (top > w * 6)
                {
                    _hotspots[4].Visible = true;
                    _hotspots[6].Visible = true;
                }
                if (left > w * 6)
                {
                    _hotspots[5].Visible = true;
                    _hotspots[7].Visible = true;
                }
            }
        }

        /// <summary>
        /// reset center point, rotate point, corner/side anchors, top/right side angle
        /// </summary>
        protected virtual void ResetPathExtraInfo(TransformType transType)
        {
            PointF[] pf = base.Path.PathPoints;
            PointF[] side = new PointF[4];
            base.CenterPoint = new PointF((pf[0].X + pf[2].X) / 2, (pf[1].Y + pf[3].Y) / 2);

            side[0] = new PointF((pf[0].X + pf[1].X) / 2, (pf[0].Y + pf[1].Y) / 2);
            side[1] = new PointF((pf[1].X + pf[2].X) / 2, (pf[1].Y + pf[2].Y) / 2);
            side[2] = new PointF((pf[2].X + pf[3].X) / 2, (pf[2].Y + pf[3].Y) / 2);
            side[3] = new PointF((pf[3].X + pf[0].X) / 2, (pf[3].Y + pf[0].Y) / 2);

            if (transType == TransformType.Rotate || transType == TransformType.Scale)
            {
                double tmp = Math.Atan2(pf[1].Y - pf[0].Y, pf[1].X - pf[0].X);
                if (tmp != 0)
                    RightSideAngle = tmp;
                tmp = Math.Atan2(pf[0].Y - pf[3].Y, pf[0].X - pf[3].X);
                if (tmp != 0)
                    TopSideAngle = tmp;
            }

            double angle = TopSideAngle + Math.PI;
            PointF bottomM = new PointF((pf[2].X + pf[3].X) / 2, (pf[2].Y + pf[3].Y) / 2);
            PointF rotatePoint = new PointF();
            rotatePoint.X = (float)(bottomM.X + XConsts.RotatingRectOffset * Math.Cos(angle));
            rotatePoint.Y = (float)(bottomM.Y + XConsts.RotatingRectOffset * Math.Sin(angle));

            base.RotateLocation = rotatePoint;
            CornerAnchors = pf;
            SideAnchors = side;
        }

        protected override void RecalculateDraggableHotSpots()
        {
            PointF[] ca = CornerAnchors;
            PointF[] sa = SideAnchors;

            PointF[] keyPoints = new PointF[8];
            for (int j = 0; j <= 7; j++)
                keyPoints[j] = (j < 4) ? ca[j] : sa[j - 4];            
            
            int hw = XConsts.AnchorRectHalfWidth;
            int w = hw * 2;
            int i;
            for (i = 0; i <= _hotspots.Length - 2; i++)
            {
                _hotspots[i].Rect = new Rectangle(
                    (int)(keyPoints[i].X - hw), (int)(keyPoints[i].Y - hw), w, w);
            }
            hw = XConsts.RotatingRectHalfWidth;
            w = hw * 2;
            _hotspots[i].Rect = new Rectangle(
                    (int)(RotateLocation.X - hw), (int)(RotateLocation.Y - hw), w, w);            
        }

        public override DraggableHotSpot[] DraggableHotSpots
        {
            get { return _hotspots; }
        }

        protected virtual void ResetPath()
        {
            base.BeforePathTransforming();
            base.Path.Reset();
            base.Path.AddRectangle(rect);

            this.AfterPathTransformed(TransformType.Scale, true);
        }

        /// <summary>
        /// 这个是给子类 EllipseTool 用的，用于 path.AddEllipse()
        /// </summary>
        protected Rectangle Rect
        {
            get { return rect; }
        }

        protected PointF[] CornerAnchors { get; set; }
        protected PointF[] SideAnchors { get; set; }
        protected double RightSideAngle { get; set; }
        protected double TopSideAngle { get; set; }

        /// <summary>
        /// 适用于矩形，返回按旋转前x, y坐标放大后的全部4个点
        /// </summary>
        public static PointF[] GetScaledRectPath(GraphicsPath path, int draggedPointIndex, PointF draggedPoint,
            Point mousePos, bool shift, double topAng, double rightAng)
        {
            if (draggedPointIndex > 3)
                return GetScaleRectSides(path, draggedPointIndex, draggedPoint, mousePos, topAng, rightAng);
            else
                return GetScaledRectCorners(path, draggedPointIndex, mousePos, shift, topAng, rightAng);
        }

        private static PointF[] GetScaleRectSides(GraphicsPath path, int draggedPointIndex,
            PointF draggedPoint, Point mousePos, double topAng, double rightAng)
        {
            int targetPt1Index = draggedPointIndex - 4;
            int targetPt2Index = (targetPt1Index + 1) % 4;

            PointF[] pf = path.PathPoints;

            double angle = 0;
            switch (draggedPointIndex)
            {
                case 4:
                    angle = topAng;
                    break;
                case 5:
                    angle = rightAng;
                    break;
                case 6:
                    angle = topAng + Math.PI;
                    break;
                case 7:
                    angle = rightAng + Math.PI;
                    break;
            }

            float dy = mousePos.Y - draggedPoint.Y;
            float dx = mousePos.X - draggedPoint.X;
            double mouseAngle = Math.Atan2(dy, dx);
            double r = Math.Sqrt(dy * dy + dx * dx);
            double length = r * Math.Cos(mouseAngle - angle);

            pf[targetPt1Index].X = (float)(pf[targetPt1Index].X + length * Math.Cos(angle));
            pf[targetPt1Index].Y = (float)(pf[targetPt1Index].Y + length * Math.Sin(angle));
            pf[targetPt2Index].X = (float)(pf[targetPt2Index].X + length * Math.Cos(angle));
            pf[targetPt2Index].Y = (float)(pf[targetPt2Index].Y + length * Math.Sin(angle));

            return pf;
        }

        private static PointF[] GetScaledRectCorners(GraphicsPath path, int draggedPointIndex,
            Point mousePos, bool shift, double topAng, double rightAng)
        {
            int targetPt1Index = (draggedPointIndex + 3) % 4;
            int targetPt2Index = (draggedPointIndex + 1) % 4;
            int pinPtIndex = (draggedPointIndex + 2) % 4;

            PointF[] pf = path.PathPoints;

            double angle1 = 0;
            double angle2 = 0;

            switch (draggedPointIndex)
            {
                case 0:
                    angle1 = rightAng + Math.PI;
                    angle2 = topAng;
                    break;
                case 1:
                    angle1 = topAng;
                    angle2 = rightAng;
                    break;
                case 2:
                    angle1 = rightAng;
                    angle2 = topAng + Math.PI;
                    break;
                case 3:
                    angle1 = topAng + Math.PI;
                    angle2 = rightAng + Math.PI;
                    break;
            }

            float dy = mousePos.Y - pf[pinPtIndex].Y;
            float dx = mousePos.X - pf[pinPtIndex].X;
            double mouseAngle = Math.Atan2(dy, dx);
            double r = Math.Sqrt(dy * dy + dx * dx);
            double length1 = r * Math.Cos(mouseAngle - angle1);
            double length2 = r * Math.Cos(mouseAngle - angle2);

            float x1 = (float)(pf[pinPtIndex].X + length1 * Math.Cos(angle1));
            float y1 = (float)(pf[pinPtIndex].Y + length1 * Math.Sin(angle1));
            float x2 = (float)(pf[pinPtIndex].X + length2 * Math.Cos(angle2));
            float y2 = (float)(pf[pinPtIndex].Y + length2 * Math.Sin(angle2));

            pf[targetPt1Index].X = x1;
            pf[targetPt1Index].Y = y1;
            pf[draggedPointIndex].X = mousePos.X;
            pf[draggedPointIndex].Y = mousePos.Y;
            pf[targetPt2Index].X = x2;
            pf[targetPt2Index].Y = y2;

            return pf;
        }
    }
}
