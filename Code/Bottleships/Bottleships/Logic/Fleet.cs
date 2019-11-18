using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships.Logic
{
    public class Fleet
    {
        public Player Player { get; set; }

        public IEnumerable<Ship> Ships { get; set; }

        public void ResolveShot(Coordinates coordinates)
        {
            foreach(var ship in Ships)
            {
                var squares = ship.GetSquares();
                foreach(var shipSpace in squares)
                {
                    if(shipSpace.Equals(coordinates))
                    {
                        ship.RegisterDamage(coordinates);
                    }
                }
            }
        }        
    }
}