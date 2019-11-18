using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships
{
    public class MyCaptain : ICaptain
    {
        public string GetName()
        {
            return "MyCaptain";
        }
    }
}
