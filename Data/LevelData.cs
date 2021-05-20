using System;
using System.Collections.Generic;
using System.Windows;

namespace GameLevelRedactor.Data
{
    [Serializable]
    public class LevelData
    {
        public string Tag { get; set; }
        public string Title { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public DateTime ModificationDate { get; set; }
        public Point DrawPoint { get; set; }
        public List<FigureData> FiguresData { get; set; }
        public LevelData()
        {
        }
        public LevelData(IList<Figure> figures)
        {
            FiguresData = new();

            Point leftTop = figures[0].DrawPoint;
            Point rightBottom = figures[0].DrawPoint;

            foreach (var item in figures)
            {
                if (item.DrawPoint.X < leftTop.X)
                    leftTop.X = (int)item.DrawPoint.X - 1;

                if (item.DrawPoint.Y < leftTop.Y)
                    leftTop.Y = (int)item.DrawPoint.Y - 1;

                if (item.DrawPoint.X + item.ActualWidth > rightBottom.X)
                    rightBottom.X = (int)(item.DrawPoint.X + item.ActualWidth) + 1;

                if (item.DrawPoint.Y + item.ActualHeight > rightBottom.Y)
                    rightBottom.Y = (int)(item.DrawPoint.Y + item.ActualHeight) + 1;

                FiguresData.Add(new(item));
            }

            Width = (int)(rightBottom.X - leftTop.X);
            Height = (int)(rightBottom.Y - leftTop.Y);
            DrawPoint = leftTop;

            ModificationDate = DateTime.Now;
        }
    }
}
