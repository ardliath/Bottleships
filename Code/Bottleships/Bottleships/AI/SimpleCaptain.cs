using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.AI
{
    public class SimpleCaptain : ICaptain
    {
        public SimpleCaptain()
        {
            this.ShotHistory = new List<Shot>(); // TODO: I need to remember that when we play multiple games in a round this must be reset!
        }



        public string GetName()
        {
            return "Simple Captain";
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var placements = new List<Placement>();
            var rand = new Random(Guid.NewGuid().GetHashCode());

            int xMin, xMax, yMin, yMax;
            

            foreach (var clazz in classes.OrderByDescending(s => s.Size))
            {
                var direction = (Direction)(1 + rand.Next(3));
                if(direction == Direction.Up || direction == Direction.Down)
                {
                    xMin = 0;
                    xMax = 9;
                    yMin = (clazz.Size / 2) + 1;
                    yMax = 9 - (clazz.Size / 2);
                }
                else
                {
                    yMin = 0;
                    yMax = 9;
                    xMin = (clazz.Size / 2) + 1;
                    xMax = 9 - (clazz.Size / 2);
                }

                var ship = new Placement
                {
                    Class = clazz,
                    Direction = direction,
                    Coordinates = new Coordinates
                    {
                        X = rand.Next(xMin, xMax),
                        Y = rand.Next(yMin, yMax)
                    }
                };
                placements.Add(ship);
            }

            return placements;
        }

        public List<Shot> ShotHistory { get; set; }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {            
            var shots = new List<Shot>();
            var rand = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < numberOfShots;  i++)
            {
                int x = rand.Next(0, 9);
                int y = rand.Next(0, 9);

                var newCoord = new Coordinates { X = x, Y = y };
                var enemy = enemyFleetInfo.ElementAt(rand.Next(enemyFleetInfo.Count()));

                var shot = new Shot
                {
                    Coordinates = newCoord,
                    FleetName = enemy.Name
                };

                if(ShotHistory.Contains(shot))
                {
                    i--;
                }
                else
                {
                    ShotHistory.Add(shot);
                    shots.Add(shot);
                }
            }

            return shots;
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            // Doesn't care if we've hit something, not smart enough for that
        }
    }
}
