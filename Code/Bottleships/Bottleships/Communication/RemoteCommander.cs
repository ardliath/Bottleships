using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public class RemoteCommander : ICommander
    {
        public string GetName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            throw new NotImplementedException();
        }
    }
}
