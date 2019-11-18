using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.AI
{
    public class RandomCaptain : ICaptain
    {
        public string GetName()
        {
            return "Random Captain";
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var placements = new List<Placement>();
            var rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var clazz in classes.OrderByDescending(s => s.Size))
            {
                var ship = new Placement
                {
                    Class = clazz,
                    Direction = (Direction)(1 + rand.Next(3)),
                    Coordinates = new Coordinates
                    {
                        X = rand.Next(9),
                        Y = rand.Next(9)
                    }
                };
                placements.Add(ship);
            }

            return placements;
        }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            var rand = new Random();
            var shots = new List<Shot>();
            var target = game.Fleets.Where(f => !f.Equals(myFleet)).FirstOrDefault();

            for (int i = 0; i < myFleet.Ships.Count(s => s.IsAfloat); i++)
            {
                var coords = new Coordinates
                {
                    X = rand.Next(0, 9),
                    Y = rand.Next(0, 9)
                };

                shots.Add(new Shot
                {
                    Coordinates = coords,
                    Fleet = target
                });
            }

            return shots;
        }
    }
}
