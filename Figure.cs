using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameLevelRedactor
{
    public class Figure : Border
    {
        private static int counter;
        private int id;
        public int Id
        {
            get => id;
            set => id = value;
        }
        public int MajorFigureId { get; set; }
        public Point AnchorPoint { get; set; }
        public List<int> AnchorFiguresId { get; set; }
        public double Angle { get; set; }
        public string Title { get; set; }
        public int ZIndex { get; set; }
        public Point DrawPoint { get; set; }
        public ObservableCollection<Primitive> Primitives { get; set; }

        public Figure() : base()
        {
            id = ++counter;
            Title = "Фигура_" + id;
            Child = new Image();
            AnchorFiguresId = new List<int>();
            Primitives = new ObservableCollection<Primitive>();

            Primitives.CollectionChanged += (s, e) =>
            {
                if (Primitives.Count > 0)
                {
                    DrawingGroup drawingGroup = new DrawingGroup();
                    foreach (var primitive in Primitives)
                    {
                        drawingGroup.Children.Add(primitive.GeometryDrawing);
                    }

                    ((Image)this.Child).Source = new DrawingImage(drawingGroup);

                }
            };
        }

        //public Figure Clone()
        //{
        //    return new Figure()
        //    {
        //        Child = Child,
        //        Primitives = Primitives,
        //        Angle = Angle,
        //        DrawPoint = DrawPoint
        //    };
        //}
    }
}
