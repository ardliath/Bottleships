using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bottleships
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.DrawMenu();
        }

        private void DrawMenu()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.DrawString("Bottleships", new Font(FontFamily.GenericMonospace, 24 , FontStyle.Regular), Brushes.Black, new PointF(10, 10));
            }

            UpdateScreen(bitmap);
        }


        public void DrawGameScreen(Fleet fleet)
        {            
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            
            var gameSize = (50 * 10) + 8;
            var xBuffer = (this.pictureBox1.Width - gameSize) / 2;
            var yBuffer = (this.pictureBox1.Height - gameSize) / 2;

            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.FillRectangle(Brushes.Aqua, new RectangleF(0, 0, this.pictureBox1.Width, this.pictureBox1.Height));

                for (int i = 1; i < 10; i++)
                {
                    gfx.DrawLine(Pens.Black, new Point(xBuffer, (i * 51) + yBuffer), new Point(this.pictureBox1.Width - xBuffer, (i * 51) + yBuffer));  // horizontal
                    gfx.DrawLine(Pens.Black, new Point((i * 51) + xBuffer, yBuffer), new Point((i * 51) + xBuffer, this.pictureBox1.Height - yBuffer)); // vertical
                }

                foreach (var ship in fleet.Ships)
                {
                    DrawShip(gfx, ship, xBuffer, yBuffer);
                }

            }


            this.UpdateScreen(bitmap);
        }

        private void DrawShip(Graphics gfx, Ship ship, int xBuffer, int yBuffer)
        {
            var shipSquares = ship.GetSquares();
            foreach (var coords in shipSquares)
            {
                gfx.FillRectangle(Brushes.Gray, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
            }
            gfx.FillRectangle(Brushes.Black, new Rectangle(1 + xBuffer + (ship.Coordinates.X * 51), 1 + yBuffer + (ship.Coordinates.Y * 51), 50, 50));
        }

        public delegate void UpdateScreenDelegate(Bitmap bitmap);

        public void UpdateScreen(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateScreenDelegate(UpdateScreen));
            }
            else
            {                
                this.pictureBox1.Image = bitmap;
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            var fleet = new Fleet
            {
                Player = new Player
                {
                    Name = "Adam"
                },
                Ships = new Ship[]
               {
                   new Ship
                   {
                       Class = Clazz.Battleship,
                       Coordinates = new Coordinates{ X = 3, Y = 4 },
                       Direction = Direction.Right
                   },
                   new Ship
                   {
                       Class = Clazz.Gunboat,
                       Coordinates = new Coordinates{ X = 7, Y = 3 },
                       Direction = Direction.Up
                   },
                   new Ship
                   {
                       Class = Clazz.Submarine,
                       Coordinates = new Coordinates{ X = 5, Y = 1 },
                       Direction = Direction.Up
                   },
                   new Ship
                   {
                       Class = Clazz.Frigate,
                       Coordinates = new Coordinates{ X = 1, Y = 8 },
                       Direction = Direction.Left
                   }
               }
            };

            DrawGameScreen(fleet);
        }
    }
}
