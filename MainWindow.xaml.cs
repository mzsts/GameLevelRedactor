using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System;

namespace GameLevelRedactor
{
    public enum Tools
    {
        Rect,
        Ellipse,
        Line,
        Triengle,
        Arrow,
    }
    public enum HitTypes
    {
        None,
        Body
    }
    public partial class MainWindow : Window
    {
        private Figure currentFigure;
        private Figure tempFigure;
        private ObservableCollection<Figure> figures;

        private HitTypes hitType; 
        private Tools currentTool;

        private int lastZIndex;

        private bool IsDraging;
        private bool IsDrawing;
        private bool IsUnitFigures;
        private bool IsLinking;

        public MainWindow()
        {
            InitializeComponent();

            InitButtons();
            InitHotKeys();
            currentTool = Tools.Arrow;

            fillColor.SelectedColor = Colors.Blue;
            borderColor.SelectedColor = Colors.Black;
            borderWidth.Value = 2;

            figures = new ObservableCollection<Figure>();
            FiguresVisualTreeView.ItemsSource = figures;

            FiguresVisualTreeView.SelectedItemChanged += (s, e) =>
            {
                if (FiguresVisualTreeView.SelectedItem is Figure figure)
                    currentFigure = figure;

                if (FiguresVisualTreeView.SelectedItem is Primitive primitive)
                {
                    fillColor.SelectedColor = ((SolidColorBrush)primitive.GeometryDrawing.Brush).Color;
                    borderColor.SelectedColor = ((SolidColorBrush)primitive.GeometryDrawing.Pen.Brush).Color;
                    borderWidth.Value = (int)primitive.GeometryDrawing.Pen.Thickness;
                }
            };
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(canvas);

            if (currentTool != Tools.Arrow)
            {
                IsDrawing = true;
                currentFigure = new Figure
                {
                    DrawPoint = currentPoint
                };
                currentFigure.Primitives.Add(new Primitive()
                {
                    GeometryDrawing = new GeometryDrawing
                    {
                        Brush = new SolidColorBrush((Color)fillColor.SelectedColor),
                        Pen = new Pen(new SolidColorBrush((Color)borderColor.SelectedColor), (double)borderWidth.Value)
                    }
                });

                canvas.Children.Add(currentFigure);

                Canvas.SetTop(currentFigure, currentPoint.Y);
                Canvas.SetLeft(currentFigure, currentPoint.X);
                Canvas.SetZIndex(currentFigure, ++lastZIndex);

                

                return;
            }

            if (IsUnitFigures)
            {
                UnitFigures(currentPoint);
                return;
            }

            if (IsLinking)
            {
                LinkFigures(currentPoint);
                return;
            }

            if (currentTool == Tools.Arrow && (currentFigure = GetFigure(currentPoint)) != null)
            {

                hitType = SetHitType(currentFigure, currentPoint);
                SetCursor();

                if (hitType == HitTypes.Body)
                {
                    IsDraging = true;
                    Canvas.SetZIndex(currentFigure, ++lastZIndex);

                    return;
                }
            }

            hitType = SetHitType(currentFigure, currentPoint);
            SetCursor();
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(canvas);

            if (IsDrawing)
            {
                Draw(currentPoint);
                return;
            }

            if (IsDraging)
            {
                Drag(currentPoint);
                return;
            }

            hitType = SetHitType(currentFigure, currentPoint);
            SetCursor();
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDrawing)
            {
                currentFigure.DrawPoint = currentFigure.Primitives[0].GeometryDrawing.Bounds.TopLeft;
                IsDrawing = false;

                figures.Add(currentFigure);
            }

            if (IsDraging)
            {
                Point newDrawPoint = new();

                newDrawPoint.X = Canvas.GetLeft(currentFigure);
                newDrawPoint.Y = Canvas.GetTop(currentFigure);

                MakeNewPrimitive(newDrawPoint);

                currentFigure.DrawPoint = newDrawPoint;

                IsDraging = false;
            }
        }
        
        private void Draw(Point currentPoint)
        {
            switch (currentTool)
            {
                case Tools.Rect:
                    currentFigure.Primitives[0].Type = "Прямоугольник";
                    currentFigure.Primitives[0].GeometryDrawing.Geometry =
                        new RectangleGeometry(MakeRightRect(currentFigure.DrawPoint, currentPoint));
                    break;
                case Tools.Line:
                    currentFigure.Primitives[0].Type = "Линия";
                    currentFigure.Primitives[0].GeometryDrawing.Geometry =
                        new LineGeometry(currentFigure.DrawPoint, currentPoint);
                    break;
                case Tools.Ellipse:
                    currentFigure.Primitives[0].Type = "Овал";
                    currentFigure.Primitives[0].GeometryDrawing.Geometry =
                        new EllipseGeometry(MakeRightRect(currentFigure.DrawPoint, currentPoint));
                    break;
                case Tools.Triengle:
                    currentFigure.Primitives[0].Type = "Треугольник";
                    currentFigure.Primitives[0].GeometryDrawing.Geometry =
                        MakeRightTriangle(currentFigure.DrawPoint, currentPoint);
                    break;
                default:
                    break;
            }

            Canvas.SetTop(currentFigure, currentFigure.Primitives[0].GeometryDrawing.Bounds.Top);
            Canvas.SetLeft(currentFigure, currentFigure.Primitives[0].GeometryDrawing.Bounds.Left);
        }
        private void Drag(Point currentPoint)
        {
            double width = currentFigure.ActualWidth;
            double height = currentFigure.ActualHeight;

            double x_pos = currentPoint.X - width / 2;
            double y_pos = currentPoint.Y - height / 2;

            if (x_pos >= 0 && y_pos >= 0 
                && x_pos + width <= canvas.ActualWidth 
                && y_pos + height <= canvas.ActualHeight)
            {
                Canvas.SetTop(currentFigure, y_pos);
                Canvas.SetLeft(currentFigure, x_pos);
            }
        }
        private void UnitFigures(Point currentPoint)
        {
            Figure temp = GetFigure(currentPoint);

            foreach (Primitive primitive in currentFigure.Primitives)
            {
                temp.Primitives.Add(primitive);
            }

            Point newFigurePos = new();

            if (temp.DrawPoint.X < currentFigure.DrawPoint.X)
            {
                newFigurePos.X = temp.DrawPoint.X;
            }
            else
            {
                newFigurePos.X = currentFigure.DrawPoint.X;
            }

            if (temp.DrawPoint.Y < currentFigure.DrawPoint.Y)
            {
                newFigurePos.Y = temp.DrawPoint.Y;
            }
            else
            {
                newFigurePos.Y = currentFigure.DrawPoint.Y;
            }

            temp.DrawPoint = newFigurePos;

            canvas.Children.Remove(currentFigure);
            figures.Remove(currentFigure);

            Canvas.SetLeft(temp, newFigurePos.X);
            Canvas.SetTop(temp, newFigurePos.Y);

            currentFigure = temp;

            IsUnitFigures = false;
        }
        private void LinkFigures(Point currentPoint)
        {
            Figure temp = GetFigure(currentPoint);

            if (temp != null)
            {
                currentFigure.MajorFigureId = temp.Id;

                temp.AnchorFiguresId.Add(currentFigure.Id);

                currentFigure.AnchorPoint = new(temp.DrawPoint.X - currentFigure.DrawPoint.X,
                                                temp.DrawPoint.Y - currentFigure.DrawPoint.Y);

                IsLinking = false;
            }
        }
        private Rect MakeRightRect(Point p1, Point p2)
        {
            if (p1.X < p2.X)
            {
                if (p1.Y > p2.Y)
                    return new Rect(p1, p2);
                else
                    return new Rect(new Point(p1.X, p2.Y), new Point(p2.X, p1.Y));
            }
            else
            {
                if (p1.Y > p2.Y)
                    return new Rect(new Point(p2.X, p1.Y), new Point(p1.X, p2.Y));
                else
                    return new Rect(p2, p1);
            }
        }
        private PathGeometry MakeRightTriangle(Point p1, Point p2)
        {
            PathFigure pf_triangle = new PathFigure();
            pf_triangle.IsClosed = true;

            Rect rect = MakeRightRect(p1, p2);

            Point tr_point = rect.TopLeft;
            tr_point.X += rect.Width / 2;

            pf_triangle.StartPoint = rect.BottomLeft;
            pf_triangle.Segments.Add(new LineSegment(rect.BottomRight, true));
            pf_triangle.Segments.Add(new LineSegment(tr_point, true));

            PathGeometry triangle = new PathGeometry();
            triangle.Figures.Add(pf_triangle);

            return triangle;
        }
        private void MakeNewPrimitive(Point currentPoint)
        {
            double differenceX = (currentPoint.X - currentFigure.DrawPoint.X);
            double differenceY = (currentPoint.Y - currentFigure.DrawPoint.Y);

            foreach (Primitive primitive in currentFigure.Primitives)
            {
                if (primitive.GeometryDrawing.Geometry is RectangleGeometry rectangle)
                {
                    double newX = rectangle.Rect.Left + differenceX;
                    double newY = rectangle.Rect.Top + differenceY;
                    primitive.GeometryDrawing.Geometry =
                        new RectangleGeometry(new Rect(newX, newY,
                        rectangle.Rect.Width, rectangle.Rect.Height));
                }
                if (primitive.GeometryDrawing.Geometry is LineGeometry line)
                {
                    Point newStartPoint = new(line.StartPoint.X + differenceX,
                        line.StartPoint.Y + differenceY);
                    Point newEndPoint = new(line.EndPoint.X + differenceX,
                        line.EndPoint.Y + differenceY);

                    primitive.GeometryDrawing.Geometry = new LineGeometry(newStartPoint, newEndPoint);
                }
                if (primitive.GeometryDrawing.Geometry is EllipseGeometry ellipse)
                {
                    double newX = ellipse.Bounds.Left + differenceX;
                    double newY = ellipse.Bounds.Top + differenceY;

                    Rect newRect = new(newX, newY, ellipse.Bounds.Width, ellipse.Bounds.Height);

                    primitive.GeometryDrawing.Geometry = new EllipseGeometry(newRect);
                }
                if (primitive.GeometryDrawing.Geometry is PathGeometry triengle)
                {
                    double newX = triengle.Bounds.Left + differenceX;
                    double newY = triengle.Bounds.Top + differenceY;

                    Point point1 = new(newX, newY);
                    Point point2 = new(newX + triengle.Bounds.Width, newY + triengle.Bounds.Height);

                    primitive.GeometryDrawing.Geometry = MakeRightTriangle(point1, point2);
                }
            }
        }
        private Figure GetFigure(in Point point)
        {
            if (VisualTreeHelper.HitTest(canvas, point) is var hit && hit == null || hit.VisualHit == canvas)
            {
                return null;
            }
            return (Figure)((Image)hit.VisualHit).Parent;
        }
        private static HitTypes SetHitType(Figure figure, Point point)
        {
            if (figure == null)
                return HitTypes.None;

            double left = figure.DrawPoint.X;
            double top = figure.DrawPoint.Y;
            double right = left + figure.ActualWidth;
            double bottom = top + figure.ActualHeight;

            if (point.X < left)
                return HitTypes.None;

            if (point.X > right)
                return HitTypes.None;

            if (point.Y < top)
                return HitTypes.None;

            if (point.Y > bottom)
                return HitTypes.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP)
                    return HitTypes.None;

                if (bottom - point.Y < GAP)
                    return HitTypes.None;

                return HitTypes.None;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP)
                    return HitTypes.None;

                if (bottom - point.Y < GAP)
                    return HitTypes.None;

                return HitTypes.None;
            }
            if (point.Y - top < GAP)
                return HitTypes.None;

            if (bottom - point.Y < GAP)
                return HitTypes.None;

            return HitTypes.Body;
        }
        private void SetCursor()
        {
            if (currentFigure == null || currentTool != Tools.Arrow)
            {
                Cursor = Cursors.Arrow;
                return;
            }

            switch (hitType)
            {
                case HitTypes.None:
                    Cursor = Cursors.Arrow;
                    break;
                case HitTypes.Body:
                    Cursor = Cursors.SizeAll;
                    break;
                default:
                    break;
            }
        }
        private void SetZIndexes()
        {
            foreach (var item in figures)
            {
                item.ZIndex = Canvas.GetZIndex(item);
            }
        }
        private void SaveFile(object sender, EventArgs e)
        {
            SetZIndexes();

            string jsonString = Parser.ToJson("Car", figures);
            File.WriteAllText(@"C:\Users\MZSTS\Desktop\Car.json", jsonString);
        }
        private void InitHotKeys()
        {
            CommandBinding copyCommand = new CommandBinding() { Command = ApplicationCommands.Copy };
            copyCommand.Executed += CopyFigure;

            CommandBindings.Add(copyCommand);

            CommandBinding pasteCommand = new CommandBinding() { Command = ApplicationCommands.Paste };
            copyCommand.Executed += PasteFigure;

            CommandBindings.Add(pasteCommand);
        }
        private void InitButtons()
        {
            arrowButton.Click += (s, e) => currentTool = Tools.Arrow;
            ellipseButton.Click += (s, e) => currentTool = Tools.Ellipse;
            rectButton.Click += (s, e) => currentTool = Tools.Rect;
            triangleButton.Click += (s, e) => currentTool = Tools.Triengle;
            lineButton.Click += (s, e) => currentTool = Tools.Line;

            unitFiguresButton.Click += (s, e) => IsUnitFigures = true;
            setMajorFigureButton.Click += (s, e) => IsLinking = true;
        }
        private void CopyFigure(object sender, ExecutedRoutedEventArgs e) { } /*=>*/
            /*tempFigure = currentFigure.Clone()*/
        private void PasteFigure(object sender, ExecutedRoutedEventArgs e)
        {
            //if (tempFigure != null)
            //{
            //    figures.Add(tempFigure);
            //    canvas.Children.Add(tempFigure);

            //    Canvas.SetLeft(tempFigure, tempFigure.DrawPoint.X);
            //    Canvas.SetTop(tempFigure, tempFigure.DrawPoint.Y);
            //    Canvas.SetZIndex(tempFigure, ++lastZIndex);

            //    currentFigure = tempFigure;
            //}
        }
    }
}
