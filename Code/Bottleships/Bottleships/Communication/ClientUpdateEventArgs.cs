﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class ClientUpdateEventArgs : EventArgs
    {
        public string MessageForScreen { get; set; }
    }
}
