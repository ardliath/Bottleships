﻿using Bottleships.Logic;
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
    public class Client
    {
        private string _serverUrl;

        private ICaptain _myCaptain;

        private HttpTransmitter _transmitter;
        private HttpListenerClass _listener;

        public Client(string serverUrl)
        {
            _serverUrl = serverUrl;
            _myCaptain = new MyCaptain();
            _transmitter = new HttpTransmitter();
            _listener = new HttpListenerClass(3);
        }

        public event EventHandler<ClientUpdateEventArgs> OnStatusUpdate;

        public void PlayGame()
        {
            _listener.Start(6999);
            _listener.ProcessRequest += HttpListener_ProcessRequest;
        }

        private void HttpListener_ProcessRequest(System.Net.HttpListenerContext context)
        {
            string body = null;
            StreamReader sr = new StreamReader(context.Request.InputStream);
            using (sr)
            {
                body = sr.ReadToEnd();
            }

            var method = context.Request.Url.AbsolutePath.Replace("/", "").ToLower();
            if (method.Equals("getplacements"))
            {
                var data = JsonConvert.DeserializeObject<PlacementRequest>(body);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/plain";

                var placements = _myCaptain.GetPlacements(new Clazz[] { });                

                using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(placements));
                }
            }
        }
    }
}
