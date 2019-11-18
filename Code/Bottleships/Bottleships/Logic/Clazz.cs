using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Logic
{
    public class Clazz
    {
        public static Clazz AircraftCarrier = new Clazz("Aircraft Carrier", 5);
        public static Clazz Battleship = new Clazz("Battleship", 4);
        public static Clazz Frigate = new Clazz("Frigate", 3);
        public static Clazz Submarine = new Clazz("Submarine", 3);
        public static Clazz Gunboat = new Clazz("Submarine", 2);


        public string Name { get; protected set; }
        public int Size { get; protected set; }

        protected Clazz(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}
