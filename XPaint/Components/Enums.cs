using App.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPaint
{

    /// <summary>
    /// 箭头大小
    /// </summary>
    public enum ArrowSize
    {
        [UI("小")] Small,
        [UI("中")] Medium,
        [UI("大")] Large
    }

    /// <summary>
    /// 线型类别
    /// </summary>
    public enum LineType
    {
        [UI("-")]       Solid,
        [UI("-.")]      DashedDot,
        [UI("-..")]     DashedDotDot,
        [UI(".")]       Dot,
        [UI("-.-.")]    Dash1,
        [UI("-..-..")]  Dash2
    }


    /// <summary>
    /// 线帽类别
    /// </summary>
    public enum LineCapType
    {
        [UI("")] Square,
        [UI("")] Rounded,
        [UI("")] Rect,
        [UI("")] Circle,
        [UI("")] LineArrow,
        [UI("")] NormalArrow,
        [UI("")] SharpArrow,
        [UI("")] SharpArrow2,
        [UI("")] Hip
    }


    /// <summary>
    /// 变形方式
    /// </summary>
    public enum TransformType
    {
        [UI("移动")] Move,
        [UI("放缩")] Scale,
        [UI("旋转")] Rotate
    }

    /// <summary>
    /// 填充方式
    /// </summary>
    public enum FillType
    {
        [UI("填充色")]   SolidColor,
        [UI("线性渐变")] LinearGradient,
        [UI("路径渐变")] PathGradient
    }

    /// <summary>
    /// 绘制方式（描边、填充）
    /// </summary>
    public enum PaintType
    {
        [UI("描边")]       Stroke,
        [UI("填充")]       Fill,
        [UI("描边及填充")] StrokeAndFill
    }

    /// <summary>
    /// 图形的属性类别（描边、填充、标志、圆角）
    /// </summary>
    public enum ShapePropertyType
    {
        [UI("无")]        Empty,
        [UI("描边")]      Stroke,
        [UI("填充")]      Fill,
        [UI("箭头")]      Arrow,
        [UI("圆角矩形")]  RoundedRect
    }

    /// <summary>
    /// 图形属性值类别
    /// </summary>
    public enum ShapeValueType
    {
        Antialias,

        StrokeWidth,
        StrokeColor,
        LineDash,
        StartLineCap,
        EndLineCap,
        PenAlignment,
        LineJoin,

        PaintType,
        FillType,
        FillColor,

        ArrowSize,
        RoundedRadius
    }


}
