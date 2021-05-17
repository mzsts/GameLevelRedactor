using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLevelRedactor
{
    public class Primitive : ICloneable
    {
        public string Type { get; set; }

        public GeometryDrawing GeometryDrawing { get; set; }

        public object Clone()
        {
            return new Primitive() { Type = Type, GeometryDrawing = GeometryDrawing.Clone() };
        }
    }
}
