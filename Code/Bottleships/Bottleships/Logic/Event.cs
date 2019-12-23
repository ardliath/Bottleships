using Bottleships.AI;
using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Event
    {
        public IEnumerable<Round> Rounds { get; set; }

        public Round CurrentRound
        {
            get
            {
                return this.Rounds.Single();
            }
        }

        public static Event CreateEventSchedule(IEnumerable<ConnectedPlayer> connectedPlayers)
        {
            if(connectedPlayers.Count() > 1) throw new NotImplementedException("Not implemented for multiple players");
            var player1 = new Player(new RemoteCommander(connectedPlayers.Single()));
            var aiPlayer = new Player(new LocalCommander(new SimpleCaptain()));

            var classes = new Clazz[]
            {
                                Clazz.AircraftCarrier,
                                Clazz.Battleship,
                                Clazz.Frigate,
                                Clazz.Gunboat,
                                Clazz.Submarine
            };
            var fleet1 = player1.GetFleet(classes);
            var fleet2 = aiPlayer.GetFleet(classes);

            var game = new Game
            {
                Fleets = new Fleet[]
               {
                                    fleet1,
                                    fleet2
               }
            };

            return CreateEventFromSingleGame(game);
        }

        public static Event CreateLocalGame()
        {
            var player1 = new Player(new LocalCommander(new MyCaptain()));
            var player2 = new Player(new LocalCommander(new SimpleCaptain()));
            var player3 = new Player(new LocalCommander(new RandomCaptain()));

            var classes = new Clazz[]
            {
                Clazz.AircraftCarrier,
                Clazz.Battleship,
                Clazz.Frigate,
                Clazz.Gunboat,
                Clazz.Submarine
            };
            var fleet1 = player1.GetFleet(classes);
            var fleet2 = player2.GetFleet(classes);
            var fleet3 = player3.GetFleet(classes);

            var game = new Game
            {
                Fleets = new Fleet[]
                {
                   fleet1,
                   fleet2,
                   fleet3
                }
            };

            return CreateEventFromSingleGame(game);
        }

        private static Event CreateEventFromSingleGame(Game game)
        {
            return new Event
            {
                Rounds = new Round[]
                {
                    new Round
                    {
                        Games = new Game[]
                        {
                            game
                        }
                    }
                }
            };
        }
    }
}
