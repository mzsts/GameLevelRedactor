using System;
using System.Windows;
using System.Windows.Media;

using GameLevelRedactor.Data;

namespace GameLevelRedactor
{
    public class Primitive : ICloneable
    {
        public string Type { get; set; }
        public GeometryDrawing GeometryDrawing { get; set; }

        public Primitive(PrimitiveData primitiveData)
        {
            GeometryDrawing = new GeometryDrawing()
            {
                Brush = new SolidColorBrush((Color)ColorConverter
                    .ConvertFromString(primitiveData.FillColor)),
                Pen = new Pen(new SolidColorBrush((Color)ColorConverter
                    .ConvertFromString(primitiveData.BorderColor)), primitiveData.BorderWidth)
            };

            if (primitiveData.Type == "Rectangle")
            {
                Type = "Прямоугольник";
                GeometryDrawing.Geometry = new RectangleGeometry(primitiveData.Bounds);
            }
            if (primitiveData.Type == "Ellipse")
            {
                Type = "Овал";
                GeometryDrawing.Geometry = new EllipseGeometry(primitiveData.Bounds);
            }
            if (primitiveData.Type == "Line")
            {
                Type = "Линия";
                GeometryDrawing.Geometry = new LineGeometry(primitiveData.Points[0], primitiveData.Points[1]);
            }
            if (primitiveData.Type == "Triangle")
            {
                Type = "Треугольник";

                PathFigure pf_triangle = new();
                pf_triangle.IsClosed = true;
                pf_triangle.StartPoint = primitiveData.Points[0];

                for (int i = 1; i < primitiveData.Points.Count; i++)
                    pf_triangle.Segments.Add(new LineSegment(primitiveData.Points[i], true));

                GeometryDrawing.Geometry = new PathGeometry();
                ((PathGeometry)GeometryDrawing.Geometry).Figures.Add(pf_triangle);
            }
        }
        public Primitive()
        { }

        public object Clone()
        {
            return new Primitive() { Type = Type, GeometryDrawing = GeometryDrawing.Clone() };
        }
    }
}
