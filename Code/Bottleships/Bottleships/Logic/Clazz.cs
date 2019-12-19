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
        public static Clazz Gunboat = new Clazz("Gunboat", 2);

        public static IEnumerable<Clazz> AllClasses
        {
            get
            {
                return new Clazz[]
                {
                    AircraftCarrier,
                    Battleship,
                    Frigate,
                    Submarine,
                    Gunboat
                };
            }
        }


        public string Name { get; set; }
        public int Size { get; set; }

        public Clazz() // for serialising only
        {

        }

        protected Clazz(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}
