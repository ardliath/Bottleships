using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

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
            throw new NotImplementedException();
        }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            throw new NotImplementedException();
        }
    }
}
