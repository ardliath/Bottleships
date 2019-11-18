using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

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

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            return Captain.GetPlacements(classes);
        }

        public IEnumerable<Shot> GetShots(Game game, Fleet myFleet)
        {
            return Captain.GetShots(game, myFleet);
        }
    }
}
