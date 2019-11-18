using System;

namespace Bottleships.Communication
{
    public class ConnectedPlayer
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public string SecretCode { get; set; }

        public DateTime FirstConnected { get; set; }

        public DateTime LastConnected { get; set; }
    }
}