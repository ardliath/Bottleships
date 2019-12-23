using Bottleships.AI;
using Bottleships.Communication;
using Bottleships.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bottleships
{
    public partial class MainForm : Form
    {
        public int? FleetDisplayIndex { get; set; }

        public int? FleetShootingDisplayIndex { get; set; }

        public Queue<int> FleetsToShowShotsAt { get; set; }

        public Game Game { get; set; }

        public Client Client { get; set; }

        public Server Server { get; set; }

        public string OverrideMessage { get; set; }

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

            if(this.Game != null)
            {
                this.PlayGameTurn();
            }

            RefreshScreen();
        }

        public void RefreshScreen()
        {
            if(!string.IsNullOrWhiteSpace(this.OverrideMessage))
            {
                this.DrawOverrideMessageScreen();
                return;
            }

            if(this.Game != null)
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

        private void DrawOverrideMessageScreen()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                gfx.DrawString(this.OverrideMessage, new Font(FontFamily.GenericMonospace, 22), Brushes.Black, new RectangleF(0, 0, this.pictureBox1.Width, this.pictureBox1.Height), format);
            }

            UpdateScreen(bitmap);
        }

        private void DrawServerScreen()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.DrawString("Bottleships Server", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 10));

                var listenText = $"Server listening on http://{Environment.MachineName}:5999{"".PadRight(3 - Math.Abs(ScrollingXPos / 10) % 3, '.')}";
                gfx.DrawString(listenText, new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 75));

                gfx.DrawString("Connected Players:", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(10, 110));
                int i = 1;
                foreach(var player in this.Server.ConnectedPlayers)
                {
                    gfx.DrawString(player.Name, new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), Brushes.Black, new PointF(15, 110 + (35 * i)));
                    i++;
                }


                gfx.DrawString("Start Game", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 0 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White : Brushes.Black, new PointF(10, this.pictureBox1.Height - 75));
                gfx.DrawString("Close Server", new Font(FontFamily.GenericMonospace, 24, FontStyle.Regular), SelectedMenuIndex == 1 && (ScrollingXPos / 10) % 2 == 0 ? Brushes.White : Brushes.Black, new PointF(10, this.pictureBox1.Height - 45));
            }            

            UpdateScreen(bitmap);

            this.Server.ListenForPlayers();
        }

        public void DrawGameScreen()
        {
            var fleetIndex = FleetShootingDisplayIndex ?? FleetDisplayIndex ?? 0;
            var fleet = this.Game.Fleets.ElementAt(fleetIndex);
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
                        if (fleet.Player.Name.Equals(lastTurnShot.FleetName))
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
                var text = GetTitleText(fleet);
                gfx.DrawString(text, new Font(FontFamily.GenericMonospace, 22), Brushes.Black, new RectangleF(0, 0, this.pictureBox1.Width, yBuffer), format);

            }


            this.UpdateScreen(bitmap);
        }

        private string GetTitleText(Fleet fleet)
        {
            var isShooting = this.FleetShootingDisplayIndex == null;
            var fleetName = fleet.Player.Name;
            var shooter = this.Game.Fleets.ElementAt(this.FleetDisplayIndex.Value).Player.Name;
            return isShooting
                ? $"{fleetName} is taking aim..."
                : $"{fleetName} is taking fire from {shooter}";

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

        private void PlayGameTurn()
        {
            if (this.Game != null)
            {
                if (this.FleetsToShowShotsAt == null || this.FleetsToShowShotsAt.Count == 0)
                {
                    Game.SinkShipsWhichCollideOrFallOutOfBounds();
                    if (this.FleetShootingDisplayIndex == null) // first turn
                    {
                        this.FleetDisplayIndex = 0;
                    }
                    else if (this.FleetDisplayIndex == this.Game.Fleets.Count() - 1) // last one, flip back to start
                    {
                        this.FleetDisplayIndex = 0;
                    }
                    else // otherwise scroll right
                    {
                        this.FleetDisplayIndex++;
                    }

                    var activeFleet = this.Game.Fleets.ElementAt(this.FleetDisplayIndex.Value);
                    var shots = activeFleet.Player.GetShots(this.Game, activeFleet);
                    this.Game.LastTurnShots = shots;

                    this.FleetsToShowShotsAt = new Queue<int>();
                    int i = 0;
                    foreach (var fleet in this.Game.Fleets)
                    {
                        if (shots.Any(s => s.FleetName.Equals(fleet.Player.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            this.FleetsToShowShotsAt.Enqueue(i);
                        }
                        i++;
                    }

                    foreach (var shot in shots)
                    {
                        var fleet = Game.Fleets.SingleOrDefault(f => f.Player.Name.Equals(shot.FleetName));
                        if (fleet != null)
                        {
                            fleet.ResolveShot(shot.Coordinates);
                        }
                    }

                    FleetShootingDisplayIndex = null;
                }
                else
                {
                    this.FleetShootingDisplayIndex = this.FleetsToShowShotsAt.Dequeue();
                }


                if (Game.CheckForWinners(out Fleet winningFleet))
                {
                    this.Game = null;
                    OverrideMessage = winningFleet == null
                        ? "Draw!"
                        : $"{winningFleet.Player.Name} wins!";
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (Game != null)
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
                        var fleet = Game.Fleets.SingleOrDefault(f => f.Player.Name.Equals(shot.FleetName));
                        if (fleet != null)
                        {
                            fleet.ResolveShot(shot.Coordinates);
                        }
                    }
                }

                var fleetToDisplay = Game.Fleets.ElementAt(FleetDisplayIndex.Value);

                DrawGameScreen(fleetToDisplay);

                return;
            }
            if (Server != null)
            {
                if (e.KeyData == Keys.Up && SelectedMenuIndex > 0)
                {
                    SelectedMenuIndex--;
                }
                if (e.KeyData == Keys.Down && SelectedMenuIndex < 1)
                {
                    SelectedMenuIndex++;
                }
                if (e.KeyData == Keys.Enter)
                {
                    switch (SelectedMenuIndex)
                    {
                        case 0: // start a remote game
                            var player1 = new Player(new RemoteCommander(this.Server.ConnectedPlayers.FirstOrDefault()));
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

                            this.Game = new Game
                            {
                                Fleets = new Fleet[]
                                {
                                    fleet1,
                                    fleet2
                                }
                            };

                            this.Timer.Interval = 5000;
                            this.OverrideMessage = "Starting Hosted Game";
                            this.DrawOverrideMessageScreen();
                            this.OverrideMessage = null;

                            break;
                        case 1: // abort
                            this.Server.StopListening();
                            this.Server = null;
                            this.SelectedMenuIndex = 0;
                            this.DrawMenu();
                            break;
                    }
                }
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
                if (e.KeyData == Keys.Enter)
                {
                    switch (SelectedMenuIndex)
                    {
                        case 0: // play locally
                            this.Server = null;

                            this.OverrideMessage = "Starting Local Game";
                            this.DrawOverrideMessageScreen();
                            this.OverrideMessage = null;

                            Game = CreateLocalGame();
                            this.Timer.Interval = 5000;
                            this.Timer.Start();
                                                        
                            break;
                        case 1: // connect to server   
                            var server = "http://localhost:5999"; // the server name should be editable
                            RemoteCommander.RegisterCaptain(server);
                            this.Client = new Client(server);
                            this.Client.OnStatusUpdate += Client_OnStatusUpdate;
                            this.Client.PlayGame();  // TODO: we need to disconnect the listener when the game ends or we'll have a problem

                            this.OverrideMessage = "Playing Remote Game";
                            this.RefreshScreen();

                            break;
                        case 2: // host server
                            this.Game = null;
                            this.Server = new Server();
                            this.SelectedMenuIndex = 0;
                            this.Timer.Interval = 25;
                            this.DrawServerScreen();
                            break;
                        default:
                            this.Client?.EndGame(); // last ditch in case we've not shut things down properly
                            this.Server?.StopListening();
                            this.Close();                            
                            break;
                    }
                }
            }
        }

        private void Client_OnStatusUpdate(object sender, ClientUpdateEventArgs e)
        {
            
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

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Client?.EndGame();
            this.Server?.StopListening(); // catch all

            base.OnClosing(e);
        }
    }
}
