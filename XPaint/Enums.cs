using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPaint
{
    /// <summary>
    /// 橡皮筋区域热点类别
    /// </summary>
    public enum HotSpotType
    {
        LineVertex,        // 线段终点热点
        RotatingRect,      // 旋转热点
        AnchorToScale      // 放缩热点
    }

    /// <summary>
    /// 箭头大小
    /// </summary>
    public enum ArrowSize
    {
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// 线帽类别
    /// </summary>
    public enum LineCapType
    {
        Square,
        Rounded,
        Rectangle,
        Circle,
        LineArrow,
        NormalArrow,
        SharpArrow,
        SharpArrow2,
        Hip
    }

    /// <summary>
    /// 线型类别
    /// </summary>
    public enum LineDashType
    {
        Solid,
        DashedDot,
        DashedDotDot,
        Dot,
        Dash1,
        Dash2
    }

    /// <summary>
    /// 变形方式
    /// </summary>
    public enum TransformType
    {
        Move,
        Scale,
        Rotate
    }

    /// <summary>
    /// 填充方式
    /// </summary>
    public enum ShapeFillType
    {
        SolidColor,
        LinearGradient,
        PathGradient
    }

    /// <summary>
    /// 绘制方式（描边、填充）
    /// </summary>
    public enum ShapePaintType
    {
        Stroke,
        Fill,
        StrokeAndFill
    }

    /// <summary>
    /// 形状的属性类别（隐藏、描边、填充、标志、圆角）
    /// </summary>
    public enum ShapePropertyType
    {
        NotDrawable,
        StrokableProperty,
        FillableProperty,
        IndicatorArrowProperty,
        RoundedRectProperty
    }

    /// <summary>
    /// 形状属性值类别
    /// </summary>
    public enum ShapePropertyValueType
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

    /// <summary>
    /// 工具类别
    /// </summary>
    public enum ToolType
    {
        Hand,
        Line,
        BrokenLine,
        Arrow,
        Rectangle,
        RoundedRect,
        Ellipse,
        ShapeSelect,
        Custom
    }


    /// <summary>
    /// 工具光标类别
    /// </summary>
    public enum ToolCursorType
    {
        Default,
        LineTool,
        RectTool,
        EllipseTool,
        HandTool,
        ShapeSelect_Default,

        ShapeSelect_Move,
        ShapeSelect_Rotate,
        ShapeSelect_Scale,
        ShapeSelect_DragLineVertex,

        CustomTool
    }

}
