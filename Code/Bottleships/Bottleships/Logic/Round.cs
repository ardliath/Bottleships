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

        public IEnumerable<Clazz> Classes { get; set; }

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
            }
            else
            {
                this.GameIndex = 0;
            }
        }
    }
}
