using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            var shipSquares = ship.GetSquares();
            foreach (var coords in shipSquares)
            {
                var brush = Brushes.Gray;
                if (coords.IsCentre) brush = Brushes.Black;
                if (coords.IsDamaged) brush = Brushes.Orange;

                gfx.FillRectangle(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
            }
        }
    }
}
