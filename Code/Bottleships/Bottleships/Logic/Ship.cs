using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Ship
    {
        public Coordinates Coordinates { get; set; }
        public Direction Direction { get; set; }
        public Clazz Class { get; set; }

        public IEnumerable<Coordinates> GetSquares()
        {
            var coords = new List<Coordinates>();
            
            var isOdd = this.Class.Size % 2 == 1;
            var halfRoundedDown = (this.Class.Size - (isOdd ? 1 : 0)) / 2;

            var backOfBoatOffset = halfRoundedDown;
            var frontOfBoatOffset = halfRoundedDown + (isOdd ? 0 : -1);

            int xOffset = 0;
            int yOffset = 0;
            Coordinates backOfBoat;

            switch(Direction)
            {
                case Direction.Up:
                    yOffset = -1;
                    backOfBoat = new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y + backOfBoatOffset };
                    break;

                case Direction.Down:
                    yOffset = 1;
                    backOfBoat = new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y - backOfBoatOffset };
                    break;
                case Direction.Left:
                    backOfBoat = new Coordinates { X = this.Coordinates.X + backOfBoatOffset, Y = this.Coordinates.Y};
                    xOffset = -1;
                    break;
                case Direction.Right:
                    backOfBoat = new Coordinates { X = this.Coordinates.X - backOfBoatOffset, Y = this.Coordinates.Y};
                    xOffset = 1;
                    break;

                default:
                    backOfBoat = new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y };
                    break;
            }

            for(int i = 0; i < this.Class.Size; i++)
            {
                coords.Add(new Coordinates
                {
                    X = backOfBoat.X + (xOffset * i),
                    Y = backOfBoat.Y + (yOffset * i)
                });
            }
            

            return coords;
        }
    }
}
