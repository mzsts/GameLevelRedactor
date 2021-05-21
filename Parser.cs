using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GameLevelRedactor.Data;

namespace GameLevelRedactor
{
    public static class Parser
    {
        public static string ToJson(string title, IList<Figure> figures)
        {
            LevelData levelData = new(figures);
            levelData.Title = title;

            return JsonSerializer.Serialize(levelData);
        }
        public static ObservableCollection<Figure> FromJson(string data)
        {
            LevelData ld = JsonSerializer.Deserialize<LevelData>(data);

            ObservableCollection<Figure> figures = new();

            foreach (FigureData item in ld.FiguresData)
                figures.Add(new Figure(item));

            return figures;
        }
        public static LevelData GetLevelData(string data)
        {
            return JsonSerializer.Deserialize<LevelData>(data);
        }
    }
}
