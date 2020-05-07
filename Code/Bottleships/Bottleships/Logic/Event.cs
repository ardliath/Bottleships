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

        public IEnumerable<KeyValuePair<Player, int>> ScoresPerPlayer
        {
            get
            {
                return this.Rounds.SelectMany(s => s.ScoresPerPlayer)
                    .GroupBy(g => g.Key)
                    .Select(g => new KeyValuePair<Player, int>(g.Key, g.Select(x => x.Value).Sum()))
                    .OrderByDescending(s => s.Value);
            }
        }

        public IEnumerable<Player> Players
        {
            get
            {
                return this.Rounds
                    .SelectMany(r => r.Games)
                    .SelectMany(r => r.Players)
                    .Distinct();
            }
        }

        public void MoveOntoNextRound()
        {
            this.RoundIndex++;
        }

        public static Event CreateEventSchedule(IEnumerable<ConnectedPlayer> connectedPlayers)
        {
            var remotePlayers = connectedPlayers.Select(cp => new Player(new RemoteCommander(cp)));
            var houseRobots = new Player[]
                {
                    new Player(new LocalCommander(new Nelson())),
                    //new Player(new LocalCommander(new RandomCaptain()))
                };

            var allplayers = remotePlayers.Union(houseRobots);
            var pairedGames = CreateGamesForPlayerPairs(allplayers);

            return new Event
            {
                Rounds = new Round[]
                {
                    new Round(Clazz.AllClasses.ToArray())
                    {
                        Games = pairedGames
                    },
                    new Round(Clazz.AllClasses.ToArray())
                    {
                        Games = new Game[]
                        {
                            new Game(allplayers.ToArray())
                        }
                    }
                }
            };
        }

        private static IEnumerable<Game> CreateGamesForPlayerPairs(IEnumerable<Player> players)
        {

            var games = new List<Game>();

            for (int i = 0; i < players.Count(); i++)
            {
                for (int j = 0; j < players.Count(); j++)
                {
                    if (j >= i) continue;

                    games.Add(new Game(new Player[]
                    {
                        players.ElementAt(i),
                        players.ElementAt(j)
                    }));
                }
            }

            return games;
        }

        public static Event CreateLocalGame()
        {
            var player1 = new Player(new LocalCommander(new MyCaptain()));
            var player2 = new Player(new LocalCommander(new SimpleCaptain()));
            var player3 = new Player(new LocalCommander(new Nelson()));
            var player4 = new Player(new LocalCommander(new RandomCaptain()));
            var player5 = new Player(new LocalCommander(new RandomCaptain()));
            var player6 = new Player(new LocalCommander(new RandomCaptain()));

            return new Event
            {
                Rounds = new Round[]
                {
                    new Round(Clazz.AllClasses.ToArray())
                    {
                        Games = new Game[]
                        {
                            new Game(player1, player3),
                            new Game(player1, player2, player3, player4, player5, player6),
                            new Game(player1, player2, player3, player4, player6),
                            new Game(player1, player2),

                            new Game(player1, player2, player3),
                            new Game(player1, player2, player3, player4),
                        }
                    }
                }
            };
        }
    }
}
