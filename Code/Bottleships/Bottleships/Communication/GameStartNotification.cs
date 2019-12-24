using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class GameStartNotification
    {
        public IEnumerable<string> PlayerNames { get; set; }
        public int NumberOfPlayers { get; set; }
    }
}
