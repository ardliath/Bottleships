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

        protected int GameIndex { get; set; }

        public Game CurrentGame
        {
            get
            {
                return this.Games.ElementAt(this.GameIndex);
            }
        }

        public void MoveOntoNextGame()
        {
            this.GameIndex++;
        }
    }
}
