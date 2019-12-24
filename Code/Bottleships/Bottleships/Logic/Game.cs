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

        private Fleet _playerWhosTurnItIs;

        public Fleet PlayerWhosTurnItIs
        {
            get
            {
                if (_playerWhosTurnItIs == null)
                {
                    _playerWhosTurnItIs = this.Fleets.First();
                }
                return _playerWhosTurnItIs;
            }
        }

        public bool GameOver { get; set; }

        public IEnumerable<Fleet> Fleets { get; set; }

        public Game(params Player[] players)
        {
            this.Players = players;
        }

        public IEnumerable<Shot> CurrentPlayersShots { get; set; }
        public IEnumerable<Player> Players { get; private set; }

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


        public void MoveTurnOntoNextPlayer()
        {            
            Fleet nextPlayer = null;
            bool currentPlayerPassed = false;
            foreach(var fleet in this.Fleets)
            {
                if(fleet.Equals(this.PlayerWhosTurnItIs))
                {
                    currentPlayerPassed = true;
                    continue;
                }

                if(currentPlayerPassed && fleet.StillHasShipsAfloat)
                {
                    nextPlayer = fleet;
                    break;
                }
            }

            // if we've been all the way through with no joy (for example if the current player was at the end) then grab the first one
            if (nextPlayer == null)
            {                
                nextPlayer = this.Fleets.FirstOrDefault(f => f.StillHasShipsAfloat);
            }

            _playerWhosTurnItIs = nextPlayer;
        }               
    }
}
