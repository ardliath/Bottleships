using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Game
    {
        public IEnumerable<Fleet> Fleets { get; set; }

        public IEnumerable<Shot> LastTurnShots { get; set; }
        
    }
}
