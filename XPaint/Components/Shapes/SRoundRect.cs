using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XPaint
{
    /// <summary>
    /// 圆角矩形（建议和 RectShape 直接合并）
    /// </summary>
    public class SRoundRect : SRect
    {
        #region var
        private byte[] _types;
        private RoundedRectProperty _pro;
        public override ToolType Type => ToolType.RoundedRect;
        public override string Name => "圆角矩形";

        #endregion

        #region constructors

        public SRoundRect(XKernel container, RoundedRectProperty pro)
            : base(container, pro)
        {
            _pro = pro;
            _types = new byte[] { 0, 3, 3, 3, 1, 3, 3, 3, 1, 3, 3, 3, 1, 3, 3, 0x83 };
        }

        #endregion

        #region private methods

        private PointF[] GetScaledPoints_Sides(PointF[] pf ,int draggedIndex, Point mousePos)
        {
            PointF draggedPoint = SideKnobs[draggedIndex - 4];
            double angle = 0;
            switch (draggedIndex)
            {
                case 4:
                    angle = RightSideAngle - Math.PI / 2;
                    break;
                case 5:
                    angle = RightSideAngle;
                    break;
                case 6:
                    angle = RightSideAngle + Math.PI / 2;
                    break;
                case 7:
                    angle = RightSideAngle + Math.PI;
                    break;
            }
            double mouseAngle = Math.Atan2(mousePos.Y - draggedPoint.Y, mousePos.X - draggedPoint.X);
            float dx = mousePos.X - draggedPoint.X;
            float dy = mousePos.Y - draggedPoint.Y;
            double draggedLength = Math.Sqrt(dx * dx + dy * dy);
            double shadowLength = draggedLength * Math.Cos(mouseAngle - angle);

            float offsetx = (float)(shadowLength * Math.Cos(angle));
            float offsety = (float)(shadowLength * Math.Sin(angle));
            
            int start = (draggedIndex - 4) * 4;
            int end = start + 7;
            for (int i = start; i <= end; i++)
            {
                int j = i % 16;
                pf[j].X += offsetx;
                pf[j].Y += offsety;
            }
            return pf;
        }

        private PointF[] GetScaledPoints_Corner(int draggedIndex, Point mousePos, bool shift)
        {
            int[] ex = new int[] { 7, 4, 4, 5, 5, 6, 6, 7 };
            PointF[] pf = GetScaledPoints_Sides(Path.PathPoints, ex[draggedIndex * 2], mousePos);
            return GetScaledPoints_Sides(pf, ex[draggedIndex * 2 + 1], mousePos);
        }

        #endregion


        /// <summary>移动某个手柄</summary>
        public override void MoveKnob(int index, Point newPt)
        {
            if (index == Knobs.Length - 1)
            {
                base.Rotate(newPt);
            }
            else
            {
                PointF[] pf;
                if (index > 3)
                    pf = GetScaledPoints_Sides(Path.PathPoints, index, newPt);
                else
                    pf = GetScaledPoints_Corner(index, newPt, false);
                base.SetNewScaledPath(pf);
            }
            LastPt = newPt;
        }        

        /// <summary>
        /// reset center point, rotate point, corner/side anchors, top/right side angle
        /// </summary>
        protected override void SetKnobs(TransformType transType)
        {
            PointF[] ps = base.Path.PathPoints;
            PointF[] corner = new PointF[4];
            PointF[] side = new PointF[4];

            // 右边倾角
            if (transType == TransformType.Rotate)
                RightSideAngle = Math.Atan2(ps[4].Y - ps[3].Y, ps[4].X - ps[3].X);

            // 4各顶点的坐标
            corner[0] = new PointF(
                ps[3].X + (float)(_pro.RadiusTL * Math.Cos(RightSideAngle + Math.PI)),
                ps[3].Y + (float)(_pro.RadiusTL * Math.Sin(RightSideAngle + Math.PI)));
            corner[1] = new PointF(
                ps[4].X + (float)(_pro.RadiusTR * Math.Cos(RightSideAngle)),
                ps[4].Y + (float)(_pro.RadiusTR * Math.Sin(RightSideAngle)));
            corner[2] = new PointF(
                ps[11].X + (float)(_pro.RadiusBR * Math.Cos(RightSideAngle)),
                ps[11].Y + (float)(_pro.RadiusBR * Math.Sin(RightSideAngle)));
            corner[3] = new PointF(
                ps[12].X + (float)(_pro.RadiusBL * Math.Cos(RightSideAngle + Math.PI)),
                ps[12].Y + (float)(_pro.RadiusBL * Math.Sin(RightSideAngle + Math.PI)));

            // 4条边的中点坐标
            side[0] = new PointF((corner[0].X + corner[1].X) / 2, (corner[0].Y + corner[1].Y) / 2);
            side[1] = new PointF((corner[2].X + corner[1].X) / 2, (corner[2].Y + corner[1].Y) / 2);
            side[2] = new PointF((corner[2].X + corner[3].X) / 2, (corner[2].Y + corner[3].Y) / 2);
            side[3] = new PointF((corner[0].X + corner[3].X) / 2, (corner[0].Y + corner[3].Y) / 2);            
            
            // 旋转点的坐标
            double angle = RightSideAngle + Math.PI / 2;
            base.RotaterPt = new PointF(
                (float)(side[2].X + XConsts.RotateRectOffset * Math.Cos(angle)),
                (float)(side[2].Y + XConsts.RotateRectOffset * Math.Sin(angle)));

            // 中心点的坐标
            base.CenterPt = new PointF((side[3].X + side[1].X) / 2, (side[0].Y + side[2].Y) / 2);
            CornerKnobs = corner;
            SideKnobs = side;
        }

        protected override void AfterPropertyChanged(BaseProperty oldValue, BaseProperty newValue)
        {
            _pro = (RoundedRectProperty)newValue;
            base.AfterPropertyChanged(oldValue, newValue);
        }

        /// <summary>设置路径</summary>
        protected override void SetPath()
        {
            PointF[] pts = GetRoundedRectPath(_pro.RadiusAll, Rect.Location, new PointF(Rect.Right, Rect.Bottom));
            base.SetNewScaledPath(pts, _types);           
        }

        public static PointF[] GetRoundedRectPath(int radius, PointF pt0, PointF pt2)
        {
            PointF[] pf = new PointF[16];
            PointF pt1 = new PointF(pt2.X, pt0.Y);
            PointF pt3 = new PointF(pt0.X, pt2.Y);

            float magic = 0.55f;
            float len = radius * magic;

            pf[0] = new PointF(pt0.X, pt0.Y + radius);
            pf[1] = new PointF(pt0.X, pf[0].Y - len);
            pf[3] = new PointF(pt0.X + radius, pt0.Y);
            pf[2] = new PointF(pf[3].X - len, pt0.Y);

            pf[4] = new PointF(pt1.X - radius, pt1.Y);
            pf[5] = new PointF(pf[4].X + len, pt1.Y);
            pf[7] = new PointF(pt1.X, pt1.Y + radius);
            pf[6] = new PointF(pt1.X, pf[7].Y - len);

            pf[8] = new PointF(pt2.X, pt2.Y - radius);
            pf[9] = new PointF(pt2.X, pf[8].Y + len);
            pf[11] = new PointF(pt2.X - radius, pt2.Y);
            pf[10] = new PointF(pf[11].X + len, pt2.Y);

            pf[12] = new PointF(pt3.X + radius, pt3.Y);
            pf[13] = new PointF(pf[12].X - len, pt3.Y);
            pf[15] = new PointF(pt3.X, pt3.Y - radius);
            pf[14] = new PointF(pt3.X, pf[15].Y + len);

            return pf;
        }
    }
}
