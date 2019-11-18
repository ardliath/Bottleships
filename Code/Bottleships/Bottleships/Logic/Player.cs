using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships.Logic
{
    public class Player
    {
        public string Name { get; private set; }

        public ICommander Commander { get; private set; }

        public Player(ICommander commander)
        {
            this.Commander = commander;
            this.Name = this.Commander.GetName();
        }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            return Commander.GetShots(game, myFleet);
        }

        public Fleet GetFleet(IEnumerable<Clazz> classes)
        {
            var ships = new List<Ship>();
            var fleet = new Fleet
            {
                Player = this
            };

            var positions = Commander.GetPlacements(classes);

            foreach(var position in positions)
            {
                ships.Add(new Ship
                {
                    Class = position.Class,
                    Direction = position.Direction,
                    Coordinates = position.Coordinates
                });
            }                          

            fleet.Ships = ships;
            return fleet;
        }
    }
}