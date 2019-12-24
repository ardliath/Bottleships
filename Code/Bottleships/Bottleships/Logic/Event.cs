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

        protected int RoundIndex { get; set; }

        public Round CurrentRound
        {
            get
            {
                return this.Rounds.ElementAt(this.RoundIndex);
            }
        }

        public void MoveOntoNextRound()
        {
            this.RoundIndex++;
        }

        public static Event CreateEventSchedule(IEnumerable<ConnectedPlayer> connectedPlayers)
        {
            if (connectedPlayers.Count() > 1) throw new NotImplementedException("Not implemented for multiple players");
            var player1 = new Player(new RemoteCommander(connectedPlayers.Single()));
            var simplePlayer = new Player(new LocalCommander(new SimpleCaptain()));
            var randomPlayer = new Player(new LocalCommander(new RandomCaptain()));

            return new Event
            {
                Rounds = new Round[]
                {
                    new Round(Clazz.AllClasses.ToArray())
                    {
                        Games = new Game[]
                        {
                            new Game(player1, simplePlayer),
                            new Game(player1, randomPlayer),
                            new Game(player1, simplePlayer, randomPlayer)
                        }
                    }
                }
            };
        }

        public static Event CreateLocalGame()
        {
            var player1 = new Player(new LocalCommander(new MyCaptain()));
            var player2 = new Player(new LocalCommander(new SimpleCaptain()));
            var player3 = new Player(new LocalCommander(new RandomCaptain()));

            return new Event
            {
                Rounds = new Round[]
                {
                    new Round(Clazz.AllClasses.ToArray())
                    {
                        Games = new Game[]
                        {
                            new Game(player1, player2),
                            new Game(player1, player3),
                            new Game(player1, player2, player3)
                        }
                    }
                }
            };
        }
    }
}
