using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 矩形
    /// </summary>
    public class SRect : SFillable
    {
        //----------------------------------------------
        // Field
        //----------------------------------------------
        public override string Name => "矩形";
        public override ToolType Type => ToolType.Rect;


        /// <summary>范围。这个是给子类 EllipseTool 用的，用于 path.AddEllipse()</summary>
        protected Rectangle Rect;//{ get; set; }

        /// <summary>四角控制点</summary>
        protected PointF[] CornerKnobs { get; set; }

        /// <summary>四边中点控制点</summary>
        protected PointF[] SideKnobs { get; set; }

        /// <summary>右边倾斜角度</summary>
        protected double RightSideAngle { get; set; }

        /// <summary>顶边倾斜角度</summary>
        protected double TopSideAngle { get; set; }


        //----------------------------------------------
        // Constructor
        //----------------------------------------------
        public SRect(XKernel container, FillableProperty pro)
            : base(container, pro)
        {
            InitialProperties();
            initialHotspotRects(); 
        }        

        private void InitialProperties()
        {
            RightSideAngle = 0;
            TopSideAngle = -Math.PI / 2;
        }

        protected virtual void initialHotspotRects()
        {
            Knobs = new Knob[9];
            int i;
            for (i = 0; i <= Knobs.Length - 2; i++)
            {
                Knobs[i] = new Knob(KnobType.Scale);
            }
            // the last one is the rect used to rotate the shape
            Knobs[i] = new Knob(KnobType.Rotate);
        }

        public override void SetStartPoint(Point pt)
        {
            base.SetStartPoint(pt);
            Rect.Location = pt;
        }

        public override void SetEndPoint(Point pt)
        {
            Rect.Width = pt.X - base.StartPt.X;
            Rect.Height = pt.Y - base.StartPt.Y;

            Rect.Location = base.StartPt;
            if (Rect.Width < 0)
            {
                Rect.Width = -Rect.Width;
                Rect.X = base.StartPt.X - Rect.Width;
            }
            if (Rect.Height < 0)
            {
                Rect.Height = -Rect.Height;
                Rect.Y = base.StartPt.Y - Rect.Height;
            }

            SetPath();
        }

        public override void MoveKnob(int index, Point newPos)
        {
            if (index == Knobs.Length - 1)
            {
                // shape rotating
                base.Rotate(newPos);
            }
            else
            {
                // shape scaling
                //PointF[] pf = TransformHelper.GetScaledRectPathPoints(base.Path, index,
                //    base.LastTransformPoint, newPos, false);
                PointF[] ps = GetScaleRect(
                    base.Path, index, base.LastPt,
                    newPos, false, TopSideAngle, RightSideAngle);
                SetNewScaledPath(ps);
            }
            LastPt = newPos;
        }


        /// <summary>绘制图层选择工具</summary>
        public override void DrawSelection(Graphics g, bool withKnobs)
        {
            PointF[] pf = CornerKnobs;
            using (Pen p = new Pen(Color.Gray))
            {
                p.DashPattern = new float[] { 4.0F, 4.0F, 4.0F, 4.0F };
                g.DrawLines(p, pf);
                g.DrawLine(p, pf[3], pf[0]);

                if (withKnobs)
                    g.DrawLine(p, CenterPt, RotaterPt);
            }

            // cross on center pont
            DrawCrossSign(g, CenterPt, XConsts.CrossSignHalfWidth);            

            if (!withKnobs)
                return;
            
            DrawCrossSign(g, RotaterPt, XConsts.CrossSignHalfWidth);

            Knob[] hs = Knobs;
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

        /// <summary>画一个叉叉</summary>
        public static void DrawCrossSign(Graphics g, PointF pt, int width)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(Pens.Blue, pt.X - width, pt.Y - width, pt.X + width, pt.Y + width);
            g.DrawLine(Pens.Blue, pt.X + width, pt.Y - width, pt.X - width, pt.Y + width);
            g.SmoothingMode = old;
        }

        protected override void AfterTransform(TransformType type, bool refresh)
        {
            if (!IsCreating)
            {
                SetKnobs(type);
                if(type == TransformType.Scale)
                    SetKnobsVisibility();
            }
            base.AfterTransform(type, refresh);
        }

        /// <summary>设置手柄的可见性</summary>
        protected virtual void SetKnobsVisibility()
        {
            for (int i = 0; i <= Knobs.Length - 2; i++)
                Knobs[i].Visible = false;

            int w = XConsts.KnobSize * 2;
            PointF[] pf = CornerKnobs;

            float dx = pf[1].X - pf[0].X;
            float dy = pf[1].Y - pf[0].Y;
            float top = (float)Math.Sqrt(dx * dx + dy * dy);
            dx = pf[3].X - pf[0].X;
            dy = pf[3].Y - pf[0].Y;
            float left = (float)Math.Sqrt(dx * dx + dy * dy);

            if (top < w * 4 && left < w * 4)
            {                
                Knobs[2].Visible = true;
            }
            else if (top < w * 4 || left < w * 4)
            {
                Knobs[2].Visible = true;
                Knobs[0].Visible = true;
            }
            else if (top < w * 6 && left < w * 6)
            {
                Knobs[2].Visible = true;
                Knobs[0].Visible = true;
                Knobs[1].Visible = true;
                Knobs[3].Visible = true;
            }
            else
            {
                for (int i = 0; i <= 3; i++)
                    Knobs[i].Visible = true;
                if (top > w * 6)
                {
                    Knobs[4].Visible = true;
                    Knobs[6].Visible = true;
                }
                if (left > w * 6)
                {
                    Knobs[5].Visible = true;
                    Knobs[7].Visible = true;
                }
            }
        }

        /// <summary>设置各个手柄点的位置、边倾斜角度</summary>
        protected virtual void SetKnobs(TransformType transType)
        {
            PointF[] pf = base.Path.PathPoints;
            base.CenterPt = new PointF((pf[0].X + pf[2].X) / 2, (pf[1].Y + pf[3].Y) / 2);

            // 四条边中点的位置
            PointF[] side = new PointF[4];
            side[0] = new PointF((pf[0].X + pf[1].X) / 2, (pf[0].Y + pf[1].Y) / 2);
            side[1] = new PointF((pf[1].X + pf[2].X) / 2, (pf[1].Y + pf[2].Y) / 2);
            side[2] = new PointF((pf[2].X + pf[3].X) / 2, (pf[2].Y + pf[3].Y) / 2);
            side[3] = new PointF((pf[3].X + pf[0].X) / 2, (pf[3].Y + pf[0].Y) / 2);

            // 计算边倾斜角度
            if (transType == TransformType.Rotate || transType == TransformType.Scale)
            {
                double tmp = Math.Atan2(pf[1].Y - pf[0].Y, pf[1].X - pf[0].X);
                if (tmp != 0)
                    RightSideAngle = tmp;
                tmp = Math.Atan2(pf[0].Y - pf[3].Y, pf[0].X - pf[3].X);
                if (tmp != 0)
                    TopSideAngle = tmp;
            }

            // 计算旋转手柄的位置
            double angle = TopSideAngle + Math.PI;
            PointF bottomM = new PointF((pf[2].X + pf[3].X) / 2, (pf[2].Y + pf[3].Y) / 2);
            PointF rotatePoint = new PointF();
            rotatePoint.X = (float)(bottomM.X + XConsts.RotateRectOffset * Math.Cos(angle));
            rotatePoint.Y = (float)(bottomM.Y + XConsts.RotateRectOffset * Math.Sin(angle));
            base.RotaterPt = rotatePoint;

            //
            CornerKnobs = pf;
            SideKnobs = side;
        }

        protected override void RecalcKnobs()
        {
            PointF[] ca = CornerKnobs;
            PointF[] sa = SideKnobs;

            PointF[] keyPoints = new PointF[8];
            for (int j = 0; j <= 7; j++)
                keyPoints[j] = (j < 4) ? ca[j] : sa[j - 4];            
            
            int hw = XConsts.KnobSize;
            int w = hw * 2;
            int i;
            for (i = 0; i <= Knobs.Length - 2; i++)
            {
                Knobs[i].Rect = new Rectangle(
                    (int)(keyPoints[i].X - hw), (int)(keyPoints[i].Y - hw), w, w);
            }
            hw = XConsts.RotateRectHalfWidth;
            w = hw * 2;
            Knobs[i].Rect = new Rectangle(
                    (int)(RotaterPt.X - hw), (int)(RotaterPt.Y - hw), w, w);            
        }

        /// <summary>设置路径</summary>
        protected virtual void SetPath()
        {
            base.BeforeTransform();
            base.Path.Reset();
            base.Path.AddRectangle(Rect);
            this.AfterTransform(TransformType.Scale, true);
        }


        /// <summary>
        /// 适用于矩形，返回按旋转前x, y坐标放大后的全部4个点
        /// </summary>
        public static PointF[] GetScaleRect(
            GraphicsPath path, 
            int spotIndex, PointF spotPt, Point mousePt, 
            bool shift, double topAng, double rightAng)
        {
            if (spotIndex > 3)
                return GetRectBySide(path, spotIndex, spotPt, mousePt, topAng, rightAng);
            else
                return GetRectByCorner(path, spotIndex, mousePt, shift, topAng, rightAng);
        }

        /// <summary>计算拖拽四边热点后的顶点</summary>
        private static PointF[] GetRectBySide(
            GraphicsPath path, 
            int spotIndex, PointF spotPt, Point mousePt, 
            double topAng, double rightAng)
        {
            // 计算旋转角度、距离
            double angle = 0;
            switch (spotIndex)
            {
                case 4: angle = topAng; break;
                case 5: angle = rightAng; break;
                case 6: angle = topAng + Math.PI; break;
                case 7: angle = rightAng + Math.PI; break;
            }
            float dy = mousePt.Y - spotPt.Y;
            float dx = mousePt.X - spotPt.X;
            double mouseAngle = Math.Atan2(dy, dx);
            double r = Math.Sqrt(dy * dy + dx * dx);
            double length = r * Math.Cos(mouseAngle - angle);

            //
            PointF[] ps = path.PathPoints;
            int n1 = spotIndex - 4;  // 右侧的顶点编号
            int n2 = (n1 + 1) % 4;   // 左侧的顶点编号
            ps[n1].X = (float)(ps[n1].X + length * Math.Cos(angle));
            ps[n1].Y = (float)(ps[n1].Y + length * Math.Sin(angle));
            ps[n2].X = (float)(ps[n2].X + length * Math.Cos(angle));
            ps[n2].Y = (float)(ps[n2].Y + length * Math.Sin(angle));
            return ps;
        }

        /// <summary>计算拖拽四角热点后的顶点</summary>
        private static PointF[] GetRectByCorner(
            GraphicsPath path, int spotIndex,
            Point mousePos, bool shift, double topAng, double rightAng)
        {
            PointF[] pf = path.PathPoints;
            double angle1 = 0;
            double angle2 = 0;
            switch (spotIndex)
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

            int n1 = (spotIndex + 3) % 4; // 左手边的点
            int n2 = (spotIndex + 1) % 4; // 右手边的点
            int n3 = (spotIndex + 2) % 4; // 斜对面的点
            float dy = mousePos.Y - pf[n3].Y;
            float dx = mousePos.X - pf[n3].X;
            double mouseAngle = Math.Atan2(dy, dx);
            double r = Math.Sqrt(dy * dy + dx * dx);
            double length1 = r * Math.Cos(mouseAngle - angle1);
            double length2 = r * Math.Cos(mouseAngle - angle2);

            float x1 = (float)(pf[n3].X + length1 * Math.Cos(angle1));
            float y1 = (float)(pf[n3].Y + length1 * Math.Sin(angle1));
            float x2 = (float)(pf[n3].X + length2 * Math.Cos(angle2));
            float y2 = (float)(pf[n3].Y + length2 * Math.Sin(angle2));

            pf[n1].X = x1;
            pf[n1].Y = y1;
            pf[spotIndex].X = mousePos.X;
            pf[spotIndex].Y = mousePos.Y;
            pf[n2].X = x2;
            pf[n2].Y = y2;

            return pf;
        }
    }
}
