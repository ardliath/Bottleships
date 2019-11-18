using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Square : Coordinates
    {
        public bool IsCentre { get; set; }
        public bool IsDamaged { get; set; }

        public int PositionIndex { get; set; }
    }
}
