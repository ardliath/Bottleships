﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public interface ICaptain
    {
        string GetName();
        IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes);
        IEnumerable<Shot> GetShots(Game game, Fleet myFleet);
    }
}
