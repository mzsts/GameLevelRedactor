using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace GameLevelRedactor.Data
{
    [Serializable]
    public class PrimitiveData
    {
        private Point parentDrawpoint;
        public int BorderWidth { get; set; }
        public string FillColor { get; set; }
        public string BorderColor { get; set; }
        public string Type { get; set; }
        public Rect Bounds { get; set; }
        public List<Point> Points { get; set; }

        public PrimitiveData()
        {
        }
        public PrimitiveData(Primitive primitive, Point parentDrawpoint)
        {
            this.parentDrawpoint = parentDrawpoint;
            Points = new ();

            Bounds = new()
            {
                Width = (int)primitive.GeometryDrawing.Bounds.Width,
                Height = (int)primitive.GeometryDrawing.Bounds.Height,
                X = (int)primitive.GeometryDrawing.Bounds.Left - parentDrawpoint.X,
                Y = (int)primitive.GeometryDrawing.Bounds.Top - parentDrawpoint.Y
            };

            FillColor = primitive.GeometryDrawing.Brush.ToString();
            BorderColor = primitive.GeometryDrawing.Pen.Brush.ToString();
            BorderWidth = (int)primitive.GeometryDrawing.Pen.Thickness;

            SetParameters(primitive);
        }
        private void SetParameters(Primitive primitive)
        {
            if (primitive.GeometryDrawing.Geometry is RectangleGeometry)
                Type = "Rectangle";
            
            if (primitive.GeometryDrawing.Geometry is EllipseGeometry)
                Type = "Ellipse";
            
            if (primitive.GeometryDrawing.Geometry is LineGeometry line)
            {
                Type = "Line";

                Point startPoint = new((int)(line.StartPoint.X - parentDrawpoint.X),
                                       (int)(line.StartPoint.Y - parentDrawpoint.Y));
                Point endPoint = new((int)(line.EndPoint.X - parentDrawpoint.X),
                                     (int)(line.EndPoint.Y - parentDrawpoint.Y));

                Points.Add(startPoint);
                Points.Add(endPoint);
            }
            
            if (primitive.GeometryDrawing.Geometry is PathGeometry pathGeometry)
            {
                Type = "Triangle";

                PathFigure pf_triangle = pathGeometry.Figures[0];

                Point startPoint = new((int)(pf_triangle.StartPoint.X - parentDrawpoint.X),
                                       (int)(pf_triangle.StartPoint.Y - parentDrawpoint.Y));

                Points.Add(startPoint);

                foreach (var point in pf_triangle.Segments)
                {
                    Point temp = ((LineSegment)point).Point;

                    Points.Add(new Point((int)(temp.X - parentDrawpoint.X), 
                                         (int)(temp.Y - parentDrawpoint.Y)));
                }
            }
        }
    }
}
