﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class ShotRequest
    {
        public int NumberOfShots { get; set; }
        public IEnumerable<EnemyFleetInfo> EnemyFleets { get; set; }
    }
}
