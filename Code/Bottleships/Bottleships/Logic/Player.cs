﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships.Logic
{
    public class Player
    {
        public string Name { get; set; }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            var rand = new Random();
            var shots = new List<Shot>();
            var target = game.Fleets.Where(f => !f.Equals(myFleet)).FirstOrDefault();

            for (int i = 0; i < myFleet.Ships.Count(s => s.IsAfloat); i++)
            {
                var coords = new Coordinates
                {
                    X = rand.Next(0, 10),
                    Y = rand.Next(0, 10)
                };

                shots.Add(new Shot
                {
                    Coordinates = coords,
                    Fleet = target
                });
            }

            return shots;
        }

        public Fleet GetFleet(IEnumerable<Clazz> classes)
        {
            var ships = new List<Ship>();
            var fleet = new Fleet
            {
                Player = this
            };

            var rand = new Random();
            foreach(var clazz in classes.OrderByDescending(s => s.Size))
            {
                var ship = new Ship
                {
                    Class = clazz,
                    Direction = (Direction)(1 + rand.Next(3)),
                    Coordinates = new Coordinates
                    {
                        X = rand.Next(9),
                        Y = rand.Next(9)
                    }
                };
                ships.Add(ship);
            }


            fleet.Ships = ships;
            return fleet;
        }
    }
}