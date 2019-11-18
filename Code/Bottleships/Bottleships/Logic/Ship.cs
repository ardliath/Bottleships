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
            var isOdd = Class.Size % 2 == 1;
            var isVertical = Direction == Direction.Down || Direction == Direction.Up;

            var coords = new List<Coordinates>();
            var currentLenth = 1;
            coords.Add(this.Coordinates);

            if (!isOdd)
            {
                currentLenth++;
                switch (Direction)
                {
                    case Direction.Up:
                        coords.Add(new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y - 1});
                        break;

                    case Direction.Down:
                        coords.Add(new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y + 1});
                        break;

                    case Direction.Left:
                        coords.Add(new Coordinates { X = this.Coordinates.X - 1, Y = this.Coordinates.Y });
                        break;

                    case Direction.Right:
                        coords.Add(new Coordinates { X = this.Coordinates.X + 1, Y = this.Coordinates.Y });
                        break;
                }
            }

            int i = 0;
            while (currentLenth <= Class.Size)
            {
                if (isVertical)
                {
                    coords.Add(new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y + i });
                    coords.Add(new Coordinates { X = this.Coordinates.X, Y = this.Coordinates.Y - i });
                }
                else
                {
                    coords.Add(new Coordinates { X = this.Coordinates.X + i, Y = this.Coordinates.Y });
                    coords.Add(new Coordinates { X = this.Coordinates.X - i, Y = this.Coordinates.Y - i });
                }

                i++;
                currentLenth += 2;
            }


            return coords;
        }
    }
}
