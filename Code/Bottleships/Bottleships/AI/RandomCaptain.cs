using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.AI
{
    public class RandomCaptain : ICaptain
    {
        public string GetName()
        {
            return "Random Captain";
        }
    }
}
