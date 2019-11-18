using Bottleships.AI;
using Bottleships.Communication;
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

        public Server Server { get; set; }

        public Timer Timer { get; set; }

        public int ScrollingXPos = 0;

        public int SelectedMenuIndex = 0;

        public MainForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            

            this.Timer = new Timer();
            this.Timer.Tick += Timer_Tick;
            this.Timer.Interval = 25;
            this.Timer.Start();
            
            this.DrawMenu();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(ScrollingXPos <= -200) ScrollingXPos = this.pictureBox1.Width;
            ScrollingXPos -= 2;

            RefreshScreen();
        }

        public void RefreshScreen()
        {
            if(this.Game!= null)
            {
                this.DrawGameScreen();
                return;
            }

            if(this.Server != null)
            {
                DrawServerScreen();
                return;
            }

            this.DrawMenu();
        }

        private void DrawMenu()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.DrawString("Bottleships", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 10));

                gfx.DrawString("Test Bot Locally", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 0 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White: Brushes.Black, new PointF(10, 75));
                gfx.DrawString("Connect Bot To Server", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 1 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White : Brushes.Black, new PointF(10, 110));
                gfx.DrawString("Host Server", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 2 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White : Brushes.Black, new PointF(10, 145));
                gfx.DrawString("Exit", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 3 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White : Brushes.Black, new PointF(10, 180));
            }

            UpdateScreen(bitmap);
        }

        private void DrawServerScreen()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.DrawString("Bottleships Server", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 10));

                var listenText = $"Server listening on http://{Environment.MachineName}:5999{"".PadRight(3 - Math.Abs(ScrollingXPos) % 3, '.')}";
                gfx.DrawString(listenText, new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 75));

                gfx.DrawString("Connected Players:", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 110));
                int i = 1;
                foreach(var player in this.Server.ConnectedPlayers)
                {
                    gfx.DrawString(player.Name, new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(15, 110 + (35 * i)));
                    i++;
                }
            }            

            UpdateScreen(bitmap);
        }

        public void DrawGameScreen()
        {
            var fleet = this.Game.Fleets.ElementAt(FleetDisplayIndex);
            this.DrawGameScreen(fleet);
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


                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;                
                gfx.DrawString(fleet.Player.Name, new Font(FontFamily.GenericMonospace, 22), Brushes.Black, new RectangleF(0, 0, this.pictureBox1.Width, yBuffer), format);

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
            if(Game != null)
           {
                Game.SinkShipsWhichCollideOrFallOutOfBounds();
                if (e.KeyData == Keys.Left && FleetDisplayIndex > 0)
                {
                    FleetDisplayIndex--;
                }
                if (e.KeyData == Keys.Right && FleetDisplayIndex + 2 <= Game.Fleets.Count())
                {
                    FleetDisplayIndex++;
                }

                // get shots
                if (e.KeyData == Keys.Space)
                {
                    List<Shot> shots = new List<Shot>();
                    foreach (var fleet in Game.Fleets)
                    {
                        shots.AddRange(fleet.Player.GetShots(Game, fleet));
                    }

                    Game.LastTurnShots = shots;
                    foreach (var shot in shots)
                    {
                        shot.Fleet.ResolveShot(shot.Coordinates);
                    }
                }

                var fleetToDisplay = Game.Fleets.ElementAt(FleetDisplayIndex);

                DrawGameScreen(fleetToDisplay);

                return;
            }
            if(Server != null)
            {                
                return;
            }
            else // main menu
            {
                if (e.KeyData == Keys.Up && SelectedMenuIndex > 0)
                {
                    SelectedMenuIndex--;
                }
                if (e.KeyData == Keys.Down && SelectedMenuIndex < 4)
                {
                    SelectedMenuIndex++;
                }
                if(e.KeyData == Keys.Enter)
                {
                    switch(SelectedMenuIndex)
                    {
                        case 0: // play locally
                            this.Server = null;
                            Game = CreateLocalGame();
                            this.Timer.Interval = 5000;
                            this.DrawGameScreen(this.Game.Fleets.FirstOrDefault());
                            break;
                        case 1: // connect to server
                            break;
                        case 2: // host server
                            this.Game = null;
                            this.Server = new Server();
                            this.Timer.Interval = 500;
                            this.DrawServerScreen();
                            break;
                        default:
                            this.Close();
                            break;
                    }
                }
            }            
        }

        private Game CreateLocalGame()
        {
            var player1 = new Player(new LocalCommander(new MyCaptain()));
            var player2 = new Player(new LocalCommander(new RandomCaptain()));

            var classes = new Clazz[]
            {
                Clazz.AircraftCarrier,
                Clazz.Battleship,
                Clazz.Frigate,
                Clazz.Gunboat,
                Clazz.Submarine
            };
            var fleet1 = player1.GetFleet(classes);
            var fleet2 = player2.GetFleet(classes);

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
