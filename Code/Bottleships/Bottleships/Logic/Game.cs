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

        public void SinkShipsWhichCollideOrFallOutOfBounds()
        {
            foreach(var fleet in Fleets)
            {
                fleet.SinkShipsWhichCollideOrFallOutOfBounds();
            }
        }

        /// <summary>
        /// Checks if the game is over
        /// </summary>
        /// <param name="winningFleet">The winning fleet (null if a draw or no winner)</param>
        /// <returns>Whether the game is over</returns>
        public bool CheckForWinners(out Fleet winningFleet)
        {
            winningFleet = null;
            var fleetsWithShips = this.Fleets.Where(f => f.StillHasShipsAfloat);
            if (fleetsWithShips.Count() == 0)
            {
                return true;
            }
            else if (fleetsWithShips.Count() == 1)
            {
                winningFleet = fleetsWithShips.Single();
                return true;
            }

            return false;
        }
    }
}
