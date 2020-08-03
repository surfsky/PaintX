using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 线
    /// </summary>
    public class SLine : SStrokable
    {
        public override ToolType Type => ToolType.Line;
        public override string Name => "普通直线";
        protected PointF[] Points { get; set; }
        private PointF[] SelectionPoints
        {
            get
            {
                PointF[] ps = Points;
                double angle = Math.Atan2(ps[1].Y - ps[0].Y, ps[1].X - ps[0].X);
                float r = XConsts.PenHitWidth / 2;
                float x1 = (float)(ps[1].X + r * Math.Cos(angle - Math.PI / 2));
                float y1 = (float)(ps[1].Y + r * Math.Sin(angle - Math.PI / 2));
                float x2 = (float)(ps[1].X + r * Math.Cos(angle + Math.PI / 2));
                float y2 = (float)(ps[1].Y + r * Math.Sin(angle + Math.PI / 2));
                float dx = ps[1].X - ps[0].X;
                float dy = ps[1].Y - ps[0].Y;
                float x3 = x1 - dx;
                float y3 = y1 - dy;
                float x4 = x2 - dx;
                float y4 = y2 - dy;

                return new PointF[] {
                    new PointF(x1,y1), new PointF(x2,y2),
                    new PointF(x4, y4), new PointF(x3, y3)
                };
            }
        }

        //
        public SLine(XKernel container, StrokableProperty property)
            : base(container, property)
        {
            Knobs = new Knob[]{
                new Knob(KnobType.Line),
                new Knob(KnobType.Line)
            };
        }

        /// <summary>设置终点（并构建路径）</summary>
        public override void SetEndPoint(Point pt)
        {
            base.BeforeTransform();
            base.Path.Reset();
            base.Path.AddLine(base.StartPt, pt);
            AfterTransform(TransformType.Scale, true);
        }

        /// <summary>移动手柄</summary>
        public override void MoveKnob(int index, Point newPos)
        {
            PointF[] pf = base.Path.PathPoints;
            if (index == 0)
                pf[0] = newPos;
            else
                pf[1] = newPos;

            base.BeforeTransform();
            base.Path.Reset();
            base.Path.AddLine(pf[0], pf[1]);
            AfterTransform(TransformType.Scale, true);
        }


        /// <summary>更新手柄后刷新路径点</summary>
        protected virtual void UpdateKnobs()
        {
            Points = base.Path.PathPoints;
        }

        /// <summary>变换后调用</summary>
        protected override void AfterTransform(TransformType type, bool refresh)
        {
            if (!IsCreating)
                UpdateKnobs();
            base.AfterTransform(type, refresh);
        }

        /// <summary>计算手柄坐标</summary>
        protected override void RecalcKnobs()
        {
            PointF[] ps = Points;
            int hw = XConsts.LineWidth;
            int w = hw * 2;
            Knobs[0].Rect = new Rectangle((int)(ps[0].X - hw), (int)(ps[0].Y - hw), w, w);
            Knobs[1].Rect = new Rectangle((int)(ps[1].X - hw), (int)(ps[1].Y - hw), w, w);
        }

        /// <summary>绘制选择框</summary>
        public override void DrawSelection(Graphics g, bool withKnobs)
        {
            PointF[] points = SelectionPoints;
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashPattern = new float[] { 4.0F, 4.0F, 4.0F, 4.0F };
                g.DrawLines(pen, points);
                g.DrawLine(pen, points[3], points[0]);
            }

            if (withKnobs)
            {
                var ps = new PointF[8];
                ps[0] = points[0];
                ps[1] = points[1];
                ps[2] = points[2];
                ps[3] = points[3];
                ps[4] = new PointF((points[0].X + points[1].X) / 2, (points[0].Y + points[1].Y) / 2);
                ps[5] = new PointF((points[1].X + points[2].X) / 2, (points[1].Y + points[2].Y) / 2);
                ps[6] = new PointF((points[2].X + points[3].X) / 2, (points[2].Y + points[3].Y) / 2);
                ps[7] = new PointF((points[3].X + points[0].X) / 2, (points[3].Y + points[0].Y) / 2);

                //
                int w = XConsts.LineWidth;
                var old = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(Brushes.White, ps[4].X - w / 2, ps[4].Y - w / 2, w, w);
                g.DrawEllipse(Pens.Black,    ps[4].X - w / 2, ps[4].Y - w / 2, w, w);
                g.FillEllipse(Brushes.White, ps[6].X - w / 2, ps[6].Y - w / 2, w, w);
                g.DrawEllipse(Pens.Black,    ps[6].X - w / 2, ps[6].Y - w / 2, w, w);
                g.SmoothingMode = old;
            }
        }

        /// <summary>是否与rect相交。由于直线是线段，其上的点是否被矩形包含需要特殊处理</summary>
        public override bool Intersect(Rectangle rect)
        {
            PointF[] pf = Points;
            Point linePt1 = new Point((int)pf[0].X, (int)pf[0].Y);
            Point linePt2 = new Point((int)pf[1].X, (int)pf[1].Y);
            return Painter.Contains(rect, linePt1, linePt2);
        }

        /// <summary>是否包含指定点</summary>
        public override bool Contains(Point point)
        {
            return base.Path.IsOutlineVisible(point, XConsts.PenHitDetect);
        }
    }
}
