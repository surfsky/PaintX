﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 折线
    /// </summary>
    public class SPolyline : SStrokable
    {
        //public Knob[] Knobs { get; set; }

        public SPolyline(XKernel container, StrokableProperty pro)
            :base(container, pro)
        {
            Knobs = new Knob[4];
            for (int i = 0; i < 4; i++)
                Knobs[i] = new Knob(KnobType.Line);
        }

        public override ToolType Type
        {
            get { return ToolType.Polyline; }
        }

        public override string Name
        {
            get { return "折线"; }
        }

        public override void SetEndPoint(Point pt)
        {
            if (IsCreating)
            {
                base.BeforeTransform();
                base.Path.Reset();
                base.Path.AddLine(base.StartPt, pt);
                AfterTransform(TransformType.Scale, true);
            }
            else
            {
                PointF[] pf = GetBrokenLinePath(StartPt, pt);
                base.SetNewScaledPath(pf, new byte[] { 0, 1, 1, 1 });
            }
        }

        public static PointF[] GetBrokenLinePath(Point p1, Point p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            PointF p3 = new PointF(
                p1.X + (float)(length / 3 * Math.Cos(angle)),
                p1.Y + (float)(length / 3 * Math.Sin(angle)));
            PointF p4 = new PointF(
                p1.X + (float)(length / 3 * 2 * Math.Cos(angle)),
                p1.Y + (float)(length / 3 * 2 * Math.Sin(angle)));
            return new PointF[] { p1, p3, p4, p2 };
        }

        public override void MoveKnob(int index, Point newPos)
        {
            PointF[] pf = base.Path.PathPoints;
            pf[index] = newPos;
            base.SetNewScaledPath(pf, new byte[] { 0, 1, 1, 1 });
        }

        protected override void RecalcKnobs()
        {
            PointF[] pf = Path.PathPoints;
            int hw = XConsts.KnobSize;
            int w = hw * 2;
            for (int i = 0; i < Knobs.Length; i++)
                Knobs[i].Rect = new Rectangle((int)(pf[i].X - hw), (int)(pf[i].Y - hw), w, w);
        }

        public override void DrawSelection(Graphics g, bool withAnchors)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            for (int i = 0; i < Knobs.Length; i++)
            {
                g.FillRectangle(Brushes.White, Knobs[i].Rect);
                g.DrawRectangle(Pens.Black, Knobs[i].Rect);
            }
            g.SmoothingMode = old;
        }
        
        public override bool Intersect(Rectangle rect)
        {
            PointF[] pf = Path.PathPoints;
            for (int i = 0; i < pf.Length - 1; i++)
            {
                Point linePt1 = new Point((int)pf[i].X, (int)pf[i].Y);
                Point linePt2 = new Point((int)pf[i + 1].X, (int)pf[i + 1].Y);
                if (Painter.Contains(rect, linePt1, linePt2))
                    return true;
            }
            return false;
        }

        public override bool Contains(Point pos)
        {
            return base.Path.IsOutlineVisible(pos, XConsts.PenHitDetect);
        }
    }
}
