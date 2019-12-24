using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships.Logic
{
    public class Player
    {
        public string Name { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

        protected ICommander Commander { get; private set; }

        public Player(ICommander commander)
        {
            this.Commander = commander;
            this.Name = this.Commander.GetName();
        }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            var numberOfShots = myFleet.Ships.Count(s => s.IsAfloat);
            var enemyFleets = game.Fleets.Where(f => !f.Equals(myFleet)).Select(f => new EnemyFleetInfo
            {
                Name = f.Player.Name,
                NumberOfAfloatShipts = f.Ships.Count(s => s.IsAfloat)
            });

            return Commander.GetShots(enemyFleets, numberOfShots);
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

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            this.Commander.RespondToShots(results);
        }

        public void StartGame(Game currentGame)
        {
            this.Commander.StartGame(new GameStartNotification
            {
                NumberOfPlayers = currentGame.Players.Count(),
                PlayerNames = currentGame.Players.Select(p => p.Name)
            });
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            this.Commander.NotifyOfBeingHit(hits);
        }
    }
}