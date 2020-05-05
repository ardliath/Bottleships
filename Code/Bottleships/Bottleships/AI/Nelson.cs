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
        public class Find
        {
            public ShotResult OriginalHittingShot { get; set; }
            
            public Stack<Shot> UpcomingShots { get; set; }

            public Find()
            {
                this.UpcomingShots = new Stack<Shot>();
            }
        }


        public void StartGameNotification(GameStartNotification gameStartNotification)
        {
            this.ShotHistory = new List<Shot>();
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

        public string LastHittMe { get; set; }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {
            if (this.SearchShots == null)
            {
                CreateSearchShots(enemyFleetInfo);
            }
            if(LastHittMe != null)
            {
                // if the person who last shot at me has no ships then go back to the weakest
                var shooter = enemyFleetInfo.Single(s => s.Name == LastHittMe);
                if(shooter.NumberOfAfloatShipts == 0)
                {
                    LastHittMe = null;
                }
            }

            var shotsToFire = new List<Shot>();

            string target = LastHittMe;
            if (target == null)
            {
                target = enemyFleetInfo
                    .Where(f => f.NumberOfAfloatShipts > 0)
                    .OrderByDescending(f => f.NumberOfAfloatShipts)
                    .FirstOrDefault()
                    ?.Name;
            }

            // see if we've got any finds first
            foreach (var find in Finds)
            {
                while (shotsToFire.Count < numberOfShots && find.UpcomingShots.Any())
                {
                    var nextShot = find.UpcomingShots.Pop();
                    shotsToFire.Add(nextShot);
                }
            }

            // then fill up with searches
            for (int i = shotsToFire.Count; i < numberOfShots; i++)
            {
                if (SearchShots.ContainsKey(target)
                    && SearchShots[target].Any())
                {
                    shotsToFire.Add(SearchShots[target].Pop());
                }
            }

            return shotsToFire;
        }
        private void CreateSearchShots(IEnumerable<EnemyFleetInfo> fleets)
        {
            SearchShots = new Dictionary<string, Stack<Shot>>();
            this.Finds = new List<Find>();
            Stack<Shot> searchShots;

            foreach (var fleet in fleets)
            {
                searchShots = new Stack<Shot>();

                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        searchShots.Push(new Shot
                        {
                            FleetName = fleet.Name,
                            Coordinates = new Coordinates
                            {
                                X = (x * 2) + (y % 2 == 0 ? 0 : 1),
                                Y = y
                            }
                        });
                    }
                }

                SearchShots[fleet.Name] = searchShots;
            }
        }

        Dictionary<string, Stack<Shot>> SearchShots { get; set; }

        public List<Find> Finds { get; set; }



        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            foreach (var result in results)
            {
                if (result.WasAHit) // if we've hit
                {
                    if (result.WasASink) // and sunk
                    {
                        foreach (var find in Finds.Where(f => f.OriginalHittingShot.Equals(result.Shot))) // then cancel all other shots aiming from that find
                        {
                            this.Finds.Remove(find);
                        }
                    }


                    var originatingShot = this.Finds.SingleOrDefault(f => f.OriginalHittingShot.Equals(result.Shot));
                    if (originatingShot == null) // it was a searching shot which hit
                    {
                        var find = new Find // create a new find
                        {
                            OriginalHittingShot = result
                        };

                        this.Finds.Add(find);

                        // with four upcoming shots which spiral out from it
                        find.UpcomingShots.Push(new Shot
                        {
                            FleetName = result.Shot.FleetName,
                            Coordinates = new Coordinates { X = result.Shot.Coordinates.X + 1, Y = result.Shot.Coordinates.Y }
                        });
                        find.UpcomingShots.Push(new Shot
                        {
                            FleetName = result.Shot.FleetName,
                            Coordinates = new Coordinates { X = result.Shot.Coordinates.X - 1, Y = result.Shot.Coordinates.Y }
                        });
                        find.UpcomingShots.Push(new Shot
                        {
                            FleetName = result.Shot.FleetName,
                            Coordinates = new Coordinates { X = result.Shot.Coordinates.X, Y = result.Shot.Coordinates.Y + 1 }
                        });
                        find.UpcomingShots.Push(new Shot
                        {
                            FleetName = result.Shot.FleetName,
                            Coordinates = new Coordinates { X = result.Shot.Coordinates.X, Y = result.Shot.Coordinates.Y - 1 }
                        });
                    }
                    else // if we do know where it came from
                    {
                        // create another shot extending in that direction
                        if(result.Shot.Coordinates.X > originatingShot.OriginalHittingShot.Shot.Coordinates.X) // if it's a shot to the right
                        {
                            originatingShot.UpcomingShots.Push(new Shot
                            {
                                FleetName = result.Shot.FleetName,
                                Coordinates = new Coordinates { X = result.Shot.Coordinates.X + 1, Y = result.Shot.Coordinates.Y }
                            });
                        }

                        if (result.Shot.Coordinates.X < originatingShot.OriginalHittingShot.Shot.Coordinates.X) // if it's a shot to the left
                        {
                            originatingShot.UpcomingShots.Push(new Shot
                            {
                                FleetName = result.Shot.FleetName,
                                Coordinates = new Coordinates { X = result.Shot.Coordinates.X - 1, Y = result.Shot.Coordinates.Y }
                            });
                        }

                        if (result.Shot.Coordinates.Y > originatingShot.OriginalHittingShot.Shot.Coordinates.Y)
                        {
                            originatingShot.UpcomingShots.Push(new Shot
                            {
                                FleetName = result.Shot.FleetName,
                                Coordinates = new Coordinates { X = result.Shot.Coordinates.X, Y = result.Shot.Coordinates.Y + 1 }
                            });
                        }

                        if (result.Shot.Coordinates.Y < originatingShot.OriginalHittingShot.Shot.Coordinates.Y)
                        {
                            originatingShot.UpcomingShots.Push(new Shot
                            {
                                FleetName = result.Shot.FleetName,
                                Coordinates = new Coordinates { X = result.Shot.Coordinates.X, Y = result.Shot.Coordinates.Y - 1 }
                            });
                        }
                    }
                }
            }
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            // swap to shoot at whoever is shooting at me
            LastHittMe = hits.LastOrDefault()?.Shooter;
        }
    }
}
