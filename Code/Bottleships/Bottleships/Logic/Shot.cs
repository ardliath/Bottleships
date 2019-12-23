using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Shot
    {
        public string FleetName { get; set; }
        public Coordinates Coordinates { get; set; }


        public override bool Equals(object obj)
        {
            var other = obj as Shot;
            return other != null
                && other.FleetName.Equals(this.FleetName)
                && other.Coordinates.Equals(this.Coordinates);
        }

        public override int GetHashCode()
        {
            return 27 * this.Coordinates.GetHashCode() * this.FleetName.GetHashCode();
        }

    }
}
