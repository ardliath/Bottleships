﻿using Bottleships.Communication;
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
        public string GetName()
        {
            return "MyCaptain";
        }

        public string GetSecretCode() // this should be abstracted down and away from the player
        {
            return "123";
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
    }
}
