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

        public List<int> DamageIndicies { get; set; }

        public bool IsAfloat { get; set; }

        public Ship()
        {
            this.IsAfloat = true;
            this.DamageIndicies = new List<int>();
        }

        public IEnumerable<Square> GetSquares()
        {
            var coords = new List<Square>();
            
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
                var square = new Square
                {
                    X = backOfBoat.X + (xOffset * i),
                    Y = backOfBoat.Y + (yOffset * i),
                    PositionIndex = i
                };                
                coords.Add(square);

                if(square.Equals(this.Coordinates))
                {
                    square.IsCentre = true;
                }
                if(DamageIndicies.Contains(i))
                {
                    square.IsDamaged = true;
                }
            }            
            

            return coords;
        }

        public DamageResult RegisterDamage(Coordinates coordinates)
        {
            if (this.IsAfloat)
            {
                var position = this.GetSquares();
                foreach (var shipPosition in position)
                {
                    if (shipPosition.Equals(coordinates) && !shipPosition.IsDamaged)
                    {
                        this.DamageIndicies.Add(shipPosition.PositionIndex);                        
                    }
                }

                IsAfloat = this.Class.Size > this.DamageIndicies.Count();
                if (!IsAfloat)
                {
                    return DamageResult.Sank;
                }

                return DamageResult.Hit;
            }
            else
            {
                return DamageResult.None;
            }
        }
    }
}
