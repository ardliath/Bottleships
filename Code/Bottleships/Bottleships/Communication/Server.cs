using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class Server
    {
        private List<ConnectedPlayer> _connectedPlayers { get; set; }

        public IEnumerable<ConnectedPlayer> ConnectedPlayers
        {
            get
            {
                return _connectedPlayers;
            }
        }

        public Server()
        {
            _connectedPlayers = new List<ConnectedPlayer>();
        }
    }
}
