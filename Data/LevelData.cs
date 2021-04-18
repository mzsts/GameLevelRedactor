using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLevelRedactor.Data
{
    [Serializable]
    public class LevelData
    {
        public string Title { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public List<FigureData> FiguresData { get; set; }
        public LevelData()
        {
        }
        public LevelData(IList<Figure> figures)
        {
            FiguresData = new();

            Point leftTop = new();
            Point rightBottom = new();

            leftTop = rightBottom = figures[0].DrawPoint;

            foreach (var item in figures)
            {
                if (item.DrawPoint.X > rightBottom.X)
                    rightBottom.X = (int)item.DrawPoint.X + 1;
                if (item.DrawPoint.Y > rightBottom.Y)
                    rightBottom.Y = (int)item.DrawPoint.Y + 1;

                if (item.DrawPoint.X + item.ActualWidth > rightBottom.X)
                    rightBottom.X = (int)(item.DrawPoint.X + item.ActualWidth) + 1;
                if (item.DrawPoint.Y + item.ActualHeight > rightBottom.Y)
                    rightBottom.Y = (int)(item.DrawPoint.Y + item.ActualHeight) + 1;
            }

            foreach (var item in figures)
            {
                FiguresData.Add(new(item, leftTop));
            }

            Width = (int)(rightBottom.X - leftTop.X);
            Height = (int)(rightBottom.Y - leftTop.Y);
        }
    }
}
