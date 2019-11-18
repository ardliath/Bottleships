using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class Server
    {
        private List<ConnectedPlayer> _connectedPlayers { get; set; }

        private HttpListenerClass _listener;

        public IEnumerable<ConnectedPlayer> ConnectedPlayers
        {
            get
            {
                lock (_connectedPlayers)
                {
                    return _connectedPlayers;
                }
            }
        }

        public Server()
        {
            _connectedPlayers = new List<ConnectedPlayer>();
        }

        public void ListenForPlayers()
        {
            if (_listener == null)
            {
                _listener = new HttpListenerClass(3);
                _listener.Start(5999);
                _listener.ProcessRequest += Listener_ProcessRequest;
            }
        }

        private void Listener_ProcessRequest(System.Net.HttpListenerContext context)
        {
            string body = null;
            StreamReader sr = new StreamReader(context.Request.InputStream);
            using (sr)
            {
                body = sr.ReadToEnd();
            }

            var method = context.Request.Url.AbsolutePath.Replace("/", "").ToLower();
            if (method.Equals("registerplayer"))
            {
                var data = JsonConvert.DeserializeObject<ConnectedPlayer>(body);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/plain";
                using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.WriteLine(context.Request.RawUrl);
                }


                lock (_connectedPlayers)
                {
                    _connectedPlayers.Add(data);
                }
            }
        }

        public void StopListening()
        {
            if(_listener != null)
            {
                _listener.Stop();
                _listener.Dispose();
                _listener = null;
            }
        }
    }
}