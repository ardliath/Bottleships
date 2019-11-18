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
        public int FleetDisplayIndex;
        public Game Game { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Game = CreateGame();
            this.DrawMenu();
        }

        private void DrawMenu()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.DrawString("Bottleships", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 10));
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

                if (Game.LastTurnShots != null)
                {
                    foreach (var lastTurnShot in this.Game.LastTurnShots)
                    {
                        if (fleet.Equals(lastTurnShot.Fleet))
                        {
                            gfx.FillRectangle(Brushes.DarkBlue, new RectangleF(xBuffer + (lastTurnShot.Coordinates.X * 51), yBuffer + (lastTurnShot.Coordinates.Y * 51), 50, 50));
                        }
                    }
                }

                for (int i = 1; i < 10; i++)
                {
                    gfx.DrawLine(Pens.Black, new Point(xBuffer, (i * 51) + yBuffer), new Point(this.pictureBox1.Width - xBuffer, (i * 51) + yBuffer));  // horizontal
                    gfx.DrawLine(Pens.Black, new Point((i * 51) + xBuffer, yBuffer), new Point((i * 51) + xBuffer, this.pictureBox1.Height - yBuffer)); // vertical
                }

                foreach (var ship in fleet.Ships.Where(s => s.IsAfloat))
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
                var brush = Brushes.Gray;
                if (coords.IsCentre) brush = Brushes.Black;
                if (coords.IsDamaged) brush = Brushes.Red;

                gfx.FillRectangle(brush, new Rectangle(1 + xBuffer + (coords.X * 51), 1 + yBuffer + (coords.Y * 51), 50, 50));
            }
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



        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Game.SinkShipsWhichCollideOrFallOutOfBounds();
            if(e.KeyData == Keys.Left && FleetDisplayIndex > 0)
            {
                FleetDisplayIndex--;
            }
            if(e.KeyData == Keys.Right && FleetDisplayIndex + 2 <= Game.Fleets.Count())
            {
                FleetDisplayIndex++;
            }

            // get shots
            if(e.KeyData == Keys.Space)
            {
                List<Shot> shots = new List<Shot>();
                foreach(var fleet in Game.Fleets)
                {
                    shots.AddRange(fleet.Player.GetShots(Game, fleet));
                }

                Game.LastTurnShots = shots;
                foreach(var shot in shots)
                {
                    shot.Fleet.ResolveShot(shot.Coordinates);
                }
            }

            var fleetToDisplay = Game.Fleets.ElementAt(FleetDisplayIndex);

            DrawGameScreen(fleetToDisplay);
        }

        private Game CreateGame()
        {
            var adam = new Player { Name = "Adam" };
            var joe = new Player { Name = "Joe" };

            var classes = new Clazz[]
            {
                Clazz.AircraftCarrier,
                Clazz.Battleship,
                Clazz.Frigate,
                Clazz.Gunboat,
                Clazz.Submarine
            };
            var fleet1 = adam.GetFleet(classes);
            var fleet2 = joe.GetFleet(classes);

            var game = new Game
            {
                Fleets = new Fleet[]
                {
                   fleet1,
                   fleet2
                }
            };

            return game;
        }

    }
}
