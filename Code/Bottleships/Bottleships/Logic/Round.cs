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

        public bool RoundOver { get; set; }

        public IEnumerable<Clazz> Classes { get; set; }

        public IEnumerable<KeyValuePair<Player, int>> ScoresPerPlayer
        {
            get
            {
                return this.Games.SelectMany(s => s.ScoresPerPlayer)
                    .GroupBy(g => g.Key)
                    .Select(g => new KeyValuePair<Player, int>(g.Key, g.Select(x => x.Value).Sum()))
                    .OrderByDescending( s => s.Value);
            }
        }

        public Round(params Clazz[] classes)
        {
            this.Classes = classes;
        }

        protected int? GameIndex { get; set; }

        public Game CurrentGame
        {
            get
            {
                return this.GameIndex.HasValue
                    ? this.Games.ElementAt(this.GameIndex.Value)
                    : null;
            }
        }

        public void MoveOntoNextGame()
        {
            if (this.GameIndex.HasValue)
            {                
                this.GameIndex++;
                if (this.Games.Count() == this.GameIndex)
                {
                    this.GameIndex = null;
                    this.RoundOver = true;
                }
            }
            else
            {
                this.GameIndex = 0;
            }
        }
    }
}
