using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Round
    {
        public IEnumerable<Game> Games { get; set; }

        public Game CurrentGame
        {
            get
            {
                return this.Games.Single();
            }
        }

        public void MoveOntoNextGame()
        {
            //this.CurrentGame = null; // TODO: this needs to expand to support more games see issue #18
        }
    }
}
