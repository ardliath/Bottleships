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
        private string _name;

        public RandomCaptain()
        {
            this._name = $"Random {Guid.NewGuid().ToString()}";
        }

        public string GetName()
        {
            return _name;
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

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {
            var rand = new Random();
            var shots = new List<Shot>();
            var target = enemyFleetInfo // target
                .Where(f => f.NumberOfAfloatShipts > 0) // any enemy with ships afloat
                .OrderBy(f => Guid.NewGuid()) // and pick a random one for all shots
                .FirstOrDefault();

            for (int i = 0; i < numberOfShots; i++)
            {
                var coords = new Coordinates
                {
                    X = rand.Next(0, 9),
                    Y = rand.Next(0, 9)
                };

                shots.Add(new Shot
                {
                    Coordinates = coords,
                    FleetName = target.Name
                });
            }

            return shots;
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            
        }

        public void StartGameNotification(GameStartNotification gameStartNotification)
        {
            
        }
    }
}
