using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships
{
    public class MyCaptain : ICaptain
    {
        public string GetName()
        {
            return "MyCaptain";
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            var placements = new List<Placement>();
            int i = 0;
            foreach (var clazz in classes)
            {
                placements.Add(new Placement
                {
                    Class = clazz,
                    Coordinates = new Coordinates { X = i * 2, Y = 5 },
                    Direction = Direction.Up                
                });
                i++;
            }

            return placements;
        }
    }
}
