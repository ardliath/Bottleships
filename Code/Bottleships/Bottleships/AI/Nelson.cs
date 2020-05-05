using Bottleships.Communication;
using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.AI
{
    public class Nelson : ICaptain
    {

        public void StartGameNotification(GameStartNotification gameStartNotification)
        {
            this.ShotHistory = new List<Shot>();
            this.LastSearchShotTaken = new Dictionary<EnemyFleetInfo, Shot>();
        }



        public string GetName()
        {
            return "Nelson";
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var placements = new List<Placement>();
            var rand = new Random(Guid.NewGuid().GetHashCode());

            int xMin, xMax, yMin, yMax;


            foreach (var clazz in classes.OrderByDescending(s => s.Size))
            {
                var direction = (Direction)(1 + rand.Next(3));
                if (direction == Direction.Up || direction == Direction.Down)
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
            var shotsToFire = new List<Shot>();
            var weakestFleet = enemyFleetInfo
                .Where(f => f.NumberOfAfloatShipts > 0)
                .OrderByDescending(f => f.NumberOfAfloatShipts)
                .FirstOrDefault();

            
            if(!LastSearchShotTaken.ContainsKey(weakestFleet))
            {
                var openingShot = new Shot
                {
                    FleetName = weakestFleet.Name,
                    Coordinates = new Coordinates
                    {
                        X = 0,
                        Y = 0
                    }
                };
                LastSearchShotTaken[weakestFleet] = openingShot;
                shotsToFire.Add(openingShot);
            }

            while(shotsToFire.Count < numberOfShots)
            {                
                var newShot = GetNextShot(LastSearchShotTaken[weakestFleet]);
                shotsToFire.Add(newShot);
                LastSearchShotTaken[weakestFleet] = newShot;
            }

            return shotsToFire;
        }

        private Shot GetNextShot(Shot lastShot)
        {
            var evenSquares = lastShot.Coordinates.Y % 2 == 0;
            int y;
            if(lastShot.Coordinates.X >= 8)
            {
                y = Math.Min(lastShot.Coordinates.Y + 1, 9);
            }
            else
            {
                y = lastShot.Coordinates.Y;
            }

            int x;
            if(y == lastShot.Coordinates.Y)
            {
                x = lastShot.Coordinates.X + 2;
            }
            else
            {
                x = 0 + (evenSquares ? 0 : 1);
            }

            return new Shot
            {
                FleetName = lastShot.FleetName,
                Coordinates = new Coordinates
                {
                    X = x,
                    Y = y
                }
            };
        }

        public Dictionary<EnemyFleetInfo, Shot> LastSearchShotTaken { get; set; }

        public IEnumerable<Shot> GetShotsNotTaken(IEnumerable<EnemyFleetInfo> enemyFleetInfo)
        {
            var allShots = new List<Shot>();
            var fleetsToShootAt = enemyFleetInfo.Where(f => f.NumberOfAfloatShipts > 0);
            foreach (var enemy in fleetsToShootAt)
            {
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        allShots.Add(new Shot
                        {
                            Coordinates = new Coordinates
                            {
                                X = x,
                                Y = y
                            },
                            FleetName = enemy.Name
                        });
                    }
                }
            }


            return allShots.Except(this.ShotHistory);
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            // Doesn't care if we've hit something, not smart enough for that
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {

        }

    }
}
