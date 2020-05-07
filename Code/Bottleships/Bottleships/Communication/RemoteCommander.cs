using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;
using Newtonsoft.Json;

namespace Bottleships.Communication
{
    public class RemoteCommander : ICommander
    {
        private ConnectedPlayer _connectedPlayer;
        public RemoteCommander(ConnectedPlayer connectedPlayer)
        {
            _connectedPlayer = connectedPlayer;
        }

        public string GetName()
        {
            return _connectedPlayer.Name;
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "getplacements", new PlacementRequest
            {
                Classes = classes.Select(c => c.Name)
            });
            
            return JsonConvert.DeserializeObject<IEnumerable<Placement>>(data);
        }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "getshots", new ShotRequest
            {
                NumberOfShots = numberOfShots,
                EnemyFleets = enemyFleetInfo
            });

            return JsonConvert.DeserializeObject<IEnumerable<Shot>>(data);
        }

        public static void RegisterCaptain(string serverUrl)
        {
            var bot = new MyCaptain();
            var thisUrl = $"http://{Environment.MachineName}:6999";
            new HttpTransmitter().SendMessage(serverUrl, "registerplayer", new ConnectedPlayer
            {
                Name = bot.GetName(),
                Url = thisUrl
            });
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "shotresult", results);
        }

        public void StartGame(GameStartNotification gameStartNotification)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "startgame", gameStartNotification);
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "hitnotification", hits);
        }

        public void EndGame(GameEndNotification gameEndNotification)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "endgame", gameEndNotification);
        }

        public void EndRound(RoundEndNotification roundEndNotification)
        {
            var data = new HttpTransmitter().SendMessage(_connectedPlayer.Url, "endround", roundEndNotification);
        }
    }
}
