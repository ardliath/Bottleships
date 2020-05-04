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
            var shipSquares = ship.GetSquares();
            foreach (var coords in shipSquares)
            {
                var brush = Brushes.Gray;
                if (coords.Equals(shipSquares.First()))
                {
                    DrawProw(gfx, ship.Class, ship.Direction, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
                }
                else if (coords.Equals(shipSquares.Last()))
                {
                    DrawStern(gfx, ship.Class, ship.Direction, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));                    
                }
                else
                {
                    gfx.FillRectangle(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
                    if(ship.Class.HasChimneys)
                    {
                        gfx.FillEllipse(Brushes.DarkGray, new Rectangle(1 + xBuffer + (coords.X * 51) + 10, 1 + yBuffer + (coords.Y * 51) + 10, 30, 30));
                        gfx.DrawEllipse(Pens.Black, new Rectangle(1 + xBuffer + (coords.X * 51) + 10, 1 + yBuffer + (coords.Y * 51) + 10, 30, 30));
                    }
                }

                if (coords.IsDamaged)
                {
                    var image = ShipPainter.GetBitmapResource("Damage");
                    image.MakeTransparent(Color.White);

                    gfx.DrawImage(image, new Point(xBuffer + (coords.X * 51), yBuffer + (coords.Y * 51)));
                }
            }
        }

        private void DrawProw(Graphics gfx, Clazz clazz, Direction direction, Rectangle rectangle)
        {

            var prowBitmap = new Bitmap(50, 50);
            using (var prowGfx = Graphics.FromImage(prowBitmap))
            {                
                prowGfx.FillPolygon(Brushes.Gray, new Point[]
                {
                    new Point(0, 0),
                    new Point(25, 25),
                    new Point(50, 0),
                });

                RotateFlipType? flip;
                switch(direction)
                {
                    case Direction.Down:
                        flip = RotateFlipType.RotateNoneFlipY;
                        break;
                    case Direction.Left:
                        flip = RotateFlipType.Rotate270FlipNone;
                        break;
                    case Direction.Right:
                        flip = RotateFlipType.Rotate90FlipNone;
                        break;
                    default:
                        flip = null;
                        break;
                }
                if (flip.HasValue)
                {
                    prowBitmap.RotateFlip(flip.Value);
                }
            }
            gfx.DrawImage(prowBitmap, rectangle);
        }

        private void DrawStern(Graphics gfx, Clazz clazz, Direction direction, Rectangle rectangle)
        {

            var prowBitmap = new Bitmap(50, 50);
            using (var prowGfx = Graphics.FromImage(prowBitmap))
            {
                prowGfx.FillPolygon(Brushes.Gray, new Point[]
                {
                    new Point(0, 0),
                    new Point(5, 35),
                    new Point(45, 35),
                    new Point(50, 0),
                });

                RotateFlipType? flip;
                switch (direction)
                {
                    case Direction.Up:
                        flip = RotateFlipType.RotateNoneFlipY;
                        break;
                    case Direction.Right:
                        flip = RotateFlipType.Rotate270FlipNone;
                        break;
                    case Direction.Left:
                        flip = RotateFlipType.Rotate90FlipNone;
                        break;
                    default:
                        flip = null;
                        break;
                }
                if (flip.HasValue)
                {
                    prowBitmap.RotateFlip(flip.Value);
                }
            }
            gfx.DrawImage(prowBitmap, rectangle);
        }

        public static Bitmap GetBitmapResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string strBaseName = assembly.GetName().Name + ".Properties.Resources";
            ResourceManager rm = new ResourceManager(strBaseName, assembly);
            rm.IgnoreCase = true;

            return (Bitmap)rm.GetObject(name);
        }
    }
}
