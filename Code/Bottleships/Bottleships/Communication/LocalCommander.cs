using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class LocalCommander : ICommander
    {
        public LocalCommander(ICaptain captain)
        {
            Captain = captain;
        }

        public ICaptain Captain { get; private set; }

        public string GetName()
        {
            return Captain.GetName();
        }
    }
}
