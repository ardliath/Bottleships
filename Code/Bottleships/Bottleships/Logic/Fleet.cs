using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships.Logic
{
    public class Fleet
    {
        public Player Player { get; set; }

        public IEnumerable<Ship> Ships { get; set; }
        public bool StillHasShipsAfloat
        {
            get
            {
                return this.Ships.Any(s => s.IsAfloat);
            }
        }

        public override string ToString()
        {
            return this.Player.ToString();
        }

        public ShotResult ResolveShot(Shot shot)
        {
            foreach(var ship in Ships)
            {
                var squares = ship.GetSquares();
                foreach(var shipSpace in squares)
                {
                    if(shipSpace.Equals(shot.Coordinates))
                    {
                        var damageResult = ship.RegisterDamage(shot.Coordinates);
                        return new ShotResult(shot, ship.Class, true, damageResult == DamageResult.Sank);
                    }
                }
            }

            return new ShotResult(shot, null, false, false);
        }

        public void SinkShipsWhichCollideOrFallOutOfBounds()
        {
            foreach(var ship in Ships)
            {
                var squares = ship.GetSquares();

                if(squares.Any(s => s.X < 0
                    || s.X > 9
                    || s.Y < 0
                    || s.Y > 9))
                {
                    ship.IsAfloat = false;
                    continue;
                }


                var otherSquares = this.Ships.Where(s => !s.Equals(ship)).SelectMany(s => s.GetSquares());
                var collisions = otherSquares.Where(s => squares.Contains(s));

                if(collisions.Any())
                {
                    ship.IsAfloat = false;
                    continue;
                }
            }
        }
    }
}