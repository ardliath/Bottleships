using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships
{
    public class MyCaptain : ICaptain
    {
        public string Name { get; set; }

        public string GetName()
        {
            return this.Name;
        }

        public MyCaptain()
        {
            this.Name = "MyCaptain";
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var placements = new List<Placement>();
            int i = 0;
            foreach (var clazz in classes)
            {
                placements.Add(new Placement
                {
                    Class = clazz,
                    Coordinates = new Coordinates { X = i * 2, Y = 5 },
                    Direction = Direction.Up                
                });
                i++;
            }

            return placements;
        }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {
            var rand = new Random();
            var shots = new List<Shot>();
            var target = enemyFleetInfo.FirstOrDefault();

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

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            
        }

        public void StartGameNotification(GameStartNotification gameStartNotification)
        {
            
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {

        }
    }
}
