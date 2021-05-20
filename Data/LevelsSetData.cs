using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLevelRedactor.Data
{
    [Serializable]
    public class LevelsSetData
    {
        public string Tematic { get; set; }
        public List<LevelData> Levels { get; set; }

        public LevelsSetData()
        { }
    }
}
