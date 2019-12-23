using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Game
    {
        public Fleet Winner { get; set; }

        public bool GameOver { get; set; }

        public IEnumerable<Fleet> Fleets { get; set; }

        public IEnumerable<Shot> LastTurnShots { get; set; }

        public void SinkShipsWhichCollideOrFallOutOfBounds()
        {
            foreach (var fleet in Fleets)
            {
                fleet.SinkShipsWhichCollideOrFallOutOfBounds();
            }
        }

        /// <summary>
        /// Checks if the game is over and sets the Winner/GameOver properties
        /// </summary>
        public void CheckForWinners()
        {
            this.Winner = null;
            var fleetsWithShips = this.Fleets.Where(f => f.StillHasShipsAfloat);
            if (fleetsWithShips.Count() == 0)
            {
                this.GameOver = true;
                return;
            }
            else if (fleetsWithShips.Count() == 1)
            {
                this.Winner = fleetsWithShips.Single();
                this.GameOver = true;
                return;
            }

            this.GameOver = false;
        }
    }
}
