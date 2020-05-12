using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public class GameEndNotification
    {
        public IEnumerable<KeyValuePair<Player, int>> Scores { get; internal set; }
    }
}
