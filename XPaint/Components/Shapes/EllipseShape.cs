using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 椭圆（包括圆形）
    /// </summary>
    public class EllipseShape : RectShape
    {
        public EllipseShape(XKernel container, FillableProperty pro)
            : base(container, pro)
        { }

        public override ToolType Type
        {
            get { return ToolType.Ellipse; }
        }

        public override string Name
        {
            get { return "椭圆"; }
        }

        public override void SetNewPosForHotAnchor(int index, Point newPos)
        {
            if (index == DraggableHotSpots.Length - 1)
            {
                base.Rotate(newPos);
            }
            else
            {
                //PointF[] pf = TransformHelper.GetScaledEllipsePathPoints(base.Path, index, newPos, false);
                PointF[] pf = GetScaledEllipsePath(base.Path, index, newPos, false, 
                    TopSideAngle, RightSideAngle);
                base.SetNewScaledPath(pf);
            }
            LastTransformPoint = newPos;
        }        

        /// <summary>
        /// reset center point, rotate point, corner/side anchors, top/right side angle
        /// </summary>
        protected override void ResetPathExtraInfo(TransformType transType)
        {
            PointF[] pf = base.Path.PathPoints;
            PointF[] corner = new PointF[4];
            PointF[] side = new PointF[4];

            base.CenterPoint = new PointF((pf[6].X + pf[0].X) / 2, (pf[9].Y + pf[3].Y) / 2);

            side[0] = pf[9];
            side[1] = pf[0];
            side[2] = pf[3];
            side[3] = pf[6];

            float rdx = pf[0].X - CenterPoint.X;
            float rdy = pf[0].Y - CenterPoint.Y;
            float ldx = pf[6].X - CenterPoint.X;
            float ldy = pf[6].Y - CenterPoint.Y;

            corner[0] = new PointF(pf[9].X + ldx, pf[9].Y + ldy);
            corner[1] = new PointF(pf[9].X + rdx, pf[9].Y + rdy);
            corner[2] = new PointF(pf[3].X + rdx, pf[3].Y + rdy);
            corner[3] = new PointF(pf[3].X + ldx, pf[3].Y + ldy);

            CornerAnchors = corner;
            SideAnchors = side;

            if (transType == TransformType.Rotate || transType == TransformType.Scale)
            {
                double tmp = Math.Atan2(pf[0].Y - pf[6].Y, pf[0].X - pf[6].X);
                if (tmp != 0)
                    RightSideAngle = tmp;
                tmp = Math.Atan2(pf[9].Y - pf[3].Y, pf[9].X - pf[3].X);
                if (tmp != 0)
                    TopSideAngle = tmp;
            }

            double angle = TopSideAngle + Math.PI;
            PointF rotatePoint = new PointF(
                (float)(pf[3].X + XConsts.RotatingRectOffset * Math.Cos(angle)),
                (float)(pf[3].Y + XConsts.RotatingRectOffset * Math.Sin(angle)));
            base.RotateLocation = rotatePoint;
        }

        protected override void ResetPath()
        {
            base.BeforePathTransforming();
            base.Path.Reset();
            base.Path.AddEllipse(base.Rect);
            AfterPathTransformed(TransformType.Scale, true);
        }

        /// <summary>
        /// 返回按旋转前x, y坐标放大后的椭圆的数据点
        /// </summary>
        public static PointF[] GetScaledEllipsePath(GraphicsPath path, int index, Point mousePos,
            bool shift, double topAng, double rightAng)
        {
            PointF[] pf = path.PathPoints;
            if (index > 3)
            {
                int[] ex = new int[] { 9, 0, 3, 6 };
                return GetScaledEllipseSides(pf, ex[(index - 4)], mousePos, topAng, rightAng);
            }
            else
            {
                return GetScaledEllipseCorners(pf, index, mousePos, shift, topAng, rightAng);
            }
        }

        private static PointF[] GetScaledEllipseCorners(PointF[] pf, int index, Point mousePos,
            bool shift, double topAng, double rightAng)
        {
            int[] ex = new int[] { 6, 9, 9, 0, 0, 3, 3, 6 };
            pf = GetScaledEllipseSides(pf, ex[index * 2], mousePos, topAng, rightAng);
            return GetScaledEllipseSides(pf, ex[index * 2 + 1], mousePos, topAng, rightAng);
        }

        private static PointF[] GetScaledEllipseSides(PointF[] pf, int index, Point mousePos,
            double topAng, double rightAng)
        {
            int index2 = (index + 6) % 12;
            float dx = mousePos.X - pf[index].X;
            float dy = mousePos.Y - pf[index].Y;
            double r = Math.Sqrt(dx * dx + dy * dy);
            double angle = 0;

            switch (index)
            {
                case 0:
                    angle = rightAng;
                    break;
                case 3:
                    angle = topAng + Math.PI;
                    break;
                case 6:
                    angle = rightAng + Math.PI;
                    break;
                case 9:
                    angle = topAng;
                    break;
            }

            double mouseAngle = Math.Atan2(mousePos.Y - pf[index].Y, mousePos.X - pf[index].X);
            double length = r * Math.Cos(mouseAngle - angle);
            dx = (float)(length * Math.Cos(angle));
            dy = (float)(length * Math.Sin(angle));

            int[] indices = new int[] { 9, 11, 12, 13, 15 };
            foreach (int i in indices)
            {
                int j = (index + i) % 12;
                if (i == 11 || i == 12 || i == 13)
                {
                    pf[j].X += dx;
                    pf[j].Y += dy;
                }
                else
                {
                    pf[j].X += dx / 2;
                    pf[j].Y += dy / 2;
                }
            }

            dx = pf[index].X - pf[index2].X;
            dy = pf[index].Y - pf[index2].Y;
            length = Math.Sqrt(dy * dy + dx * dx);
            float len = (float)(length / 2 * XConsts.MagicBezier);
            angle = Math.Atan2(pf[index].Y - pf[index2].Y, pf[index].X - pf[index2].X);
            int t1 = (index + 8) % 12;
            int t2 = (index + 9) % 12;
            int t3 = (index + 10) % 12;
            pf[t3].X = (float)(pf[t2].X + len * Math.Cos(angle));
            pf[t3].Y = (float)(pf[t2].Y + len * Math.Sin(angle));
            pf[t1].X = (float)(pf[t2].X + len * Math.Cos(angle + Math.PI));
            pf[t1].Y = (float)(pf[t2].Y + len * Math.Sin(angle + Math.PI));

            int s1 = (index + 4) % 12;
            int s2 = (index + 3) % 12;
            int s3 = (index + 2) % 12;
            dx = pf[s2].X - pf[t2].X;
            dy = pf[s2].Y - pf[t2].Y;
            pf[s3].X = pf[t3].X + dx;
            pf[s3].Y = pf[t3].Y + dy;
            pf[s1].X = pf[t1].X + dx;
            pf[s1].Y = pf[t1].Y + dy;

            pf[12] = pf[0];
            return pf;
        }

    }
}
