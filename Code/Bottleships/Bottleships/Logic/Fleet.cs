using System.Collections.Generic;

namespace Bottleships.Logic
{
    public class Fleet
    {
        public Player Player { get; set; }

        public IEnumerable<Ship> Ships { get; set; }
    }
}