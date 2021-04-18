using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        //public static IList<Figure> FromJson(string levelData)
        //{

        //}
    }
}
