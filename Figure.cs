using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using GameLevelRedactor.Data;

namespace GameLevelRedactor
{
    public class Figure : Border, ICloneable
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
        private double angle = 0;
        public double Angle 
        { 
            get { return angle; } 
            set
            {
                if (value >= 0 && value <= 359)
                {
                    angle = value;
                    Child.RenderTransform = new RotateTransform(value);
                }
            }
        }
        public string Title { get; set; }
        public int ZIndex { get; set; }
        public Point DrawPoint { get; set; }
        public ObservableCollection<Primitive> Primitives { get; set; }

        public Figure() : base()
        {
            id = ++counter;
            Title = "Фигура_" + id;
            Child = new Image() { Stretch = Stretch.Fill };
            AnchorFiguresId = new List<int>();
            Primitives = new ObservableCollection<Primitive>();

            Primitives.CollectionChanged += (s, e) =>
            {
                if (Primitives.Count > 0)
                {
                    DrawingGroup drawingGroup = new();
                    foreach (var primitive in Primitives)
                    {
                        drawingGroup.Children.Add(primitive.GeometryDrawing);
                    }

                    ((Image)this.Child).Source = new DrawingImage(drawingGroup);

                }
            };
        }
        public Figure(FigureData figureData) : base()
        {
            Child = new Image() { Stretch = Stretch.Fill };
            AnchorFiguresId = new List<int>();
            Primitives = new ObservableCollection<Primitive>();

            Primitives.CollectionChanged += (s, e) =>
            {
                if (Primitives.Count > 0)
                {
                    DrawingGroup drawingGroup = new();
                    foreach (var primitive in Primitives)
                    {
                        drawingGroup.Children.Add(primitive.GeometryDrawing);
                    }

                    ((Image)this.Child).Source = new DrawingImage(drawingGroup);

                }
            };

            Title = figureData.Title;
            DrawPoint = figureData.Drawpoint;
            ZIndex = figureData.ZIndex;
            Id = figureData.Id;
            counter = id > counter ? id : counter;
            MajorFigureId = figureData.MajorFigureId;
            AnchorFiguresId = figureData.AnchorFiguresId;
            AnchorPoint = figureData.AnchorPoint;

            foreach (PrimitiveData item in figureData.PrimitivesData)
                Primitives.Add(new Primitive(item));

            Angle = figureData.Angle;
        }
        public object Clone()
        {
            Figure newFigure = new()
            {
                Child = new Image(),
                Angle = Angle,
                DrawPoint = DrawPoint
            };

            foreach (var item in Primitives)
            {
                newFigure.Primitives.Add((Primitive)item.Clone());
            }

            return newFigure;
        }
    }
}
