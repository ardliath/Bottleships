using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class HitNotification
    {
        public string Shooter { get; set; }

        public Coordinates Coordinates { get; set; }

        public bool WasASink { get; set; }

        public Clazz ClassHit { get; set; }
    }
}
