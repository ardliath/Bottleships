using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.UI
{
    public class ShipPainter
    {
        private int xBuffer;
        private int yBuffer;

        public ShipPainter(int xBuffer, int yBuffer)
        {
            this.xBuffer = xBuffer;
            this.yBuffer = yBuffer;
        }

        public void DrawShip(Graphics gfx, Ship ship)
        {
            if (ship.Class.Equals(Clazz.Gunboat))
            {
                //Bitmap shipBitmap = GetBitmapResource("Gunboat");
                //if (shipBitmap != null)
                //{
                //    shipBitmap.MakeTransparent(Color.FromArgb(255, 174, 201));

                //    var point = new Point(xBuffer + (ship.FrontOfBoat.X * 51), yBuffer + (ship.FrontOfBoat.Y * 51));
                //    gfx.DrawImage(shipBitmap, point);
                //}

                var shipSquares = ship.GetSquares();
                foreach (var coords in shipSquares)
                {
                    var brush = Brushes.Gray;
                    gfx.FillRectangle(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
                    if (coords.IsDamaged)
                    {
                        brush = Brushes.Orange;
                        gfx.FillEllipse(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
                    }
                }
            }
            else
            {
                var shipSquares = ship.GetSquares();
                foreach (var coords in shipSquares)
                {
                    var brush = Brushes.Gray;
                    gfx.FillRectangle(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));

                    if (coords.IsDamaged)
                    {
                        brush = Brushes.Orange;
                        gfx.FillEllipse(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
                    }
                }
            }
        }

        private static Bitmap GetBitmapResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string strBaseName = assembly.GetName().Name + ".Properties.Resources";
            ResourceManager rm = new ResourceManager(strBaseName, assembly);
            rm.IgnoreCase = true;

            return (Bitmap)rm.GetObject(name);
        }
    }
}
