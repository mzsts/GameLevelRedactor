using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace GameLevelRedactor.Data
{
    [Serializable]
    public class PrimitiveData
    {
        private Point _parentDrawpoint;
        public int BorderWidth { get; set; }
        public string FillColor { get; set; }
        public string BorderColor { get; set; }
        public string Type { get; set; }
        //public Point DrawPoint { get; set; }
        public Rect Bounds { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public PrimitiveData()
        {
        }
        public PrimitiveData(Primitive primitive, Point parentDrawpoint)
        {
            _parentDrawpoint = parentDrawpoint;
            Parameters = new Dictionary<string, object>();

            Bounds = new()
            {
                Width = (int)primitive.GeometryDrawing.Bounds.Width,
                Height = (int)primitive.GeometryDrawing.Bounds.Height,
                X = (int)primitive.GeometryDrawing.Bounds.Left - _parentDrawpoint.X,
                Y = (int)primitive.GeometryDrawing.Bounds.Top - _parentDrawpoint.Y
            };

            FillColor = primitive.GeometryDrawing.Brush.ToString();
            BorderColor = primitive.GeometryDrawing.Pen.Brush.ToString();
            BorderWidth = (int)primitive.GeometryDrawing.Pen.Thickness;

            SetParameters(primitive);
        }
        private void SetParameters(Primitive primitive)
        {
            if (primitive.GeometryDrawing.Geometry is RectangleGeometry)
            {
                Type = "Rectangle";
            }
            if (primitive.GeometryDrawing.Geometry is EllipseGeometry)
            {
                Type = "Ellipse";
            }
            if (primitive.GeometryDrawing.Geometry is LineGeometry line)
            {
                Type = "Line";

                Point startPoint = new((int)(line.StartPoint.X - _parentDrawpoint.X),
                                       (int)(line.StartPoint.Y - _parentDrawpoint.Y));
                Point endPoint = new((int)(line.EndPoint.X - _parentDrawpoint.X),
                                     (int)(line.EndPoint.Y - _parentDrawpoint.Y));

                Parameters.Add("StartPoint", startPoint);
                Parameters.Add("EndPoint", endPoint);
            }
            if (primitive.GeometryDrawing.Geometry is PathGeometry pathGeometry)
            {
                Type = "Triengle";

                List<Point> points = new();

                PathFigure pf_triangle = pathGeometry.Figures[0];

                Point startPoint = new((int)(pf_triangle.StartPoint.X - _parentDrawpoint.X),
                                       (int)(pf_triangle.StartPoint.Y - _parentDrawpoint.Y));

                points.Add(startPoint);

                foreach (var point in pf_triangle.Segments)
                {
                    Point temp = ((LineSegment)point).Point;

                    points.Add(new Point((int)(temp.X - _parentDrawpoint.X), 
                                         (int)(temp.Y - _parentDrawpoint.Y)));
                }

                Parameters.Add("Points", points);
            }
        }
    }
}
