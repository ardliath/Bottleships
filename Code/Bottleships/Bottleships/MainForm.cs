using Bottleships.AI;
using Bottleships.Communication;
using Bottleships.Logic;
using Bottleships.UI;
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
        public Event Event { get; set; }        

        public Client Client { get; set; }

        public Server Server { get; set; }

        public string OverrideMessage { get; set; }
        public int? RemainingTicksToDisplayOverrideMessage { get; set; }

        public Timer Timer { get; set; }

        public const int TurnTickInterval = 500;

        public int ScrollingXPos = 0;

        public int SelectedMenuIndex = 0;        

        public Game CurrentGame
        {
            get
            {
                return this.Event?.CurrentRound?.CurrentGame;
            }
        }

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

            if (this.CurrentGame != null)
            {
                
                    this.DoPreTurn();
                    this.DoTurn();
                    this.DrawGameScreen();
                    this.DoPostTurn();

                if (this.CurrentGame.Winner == null)
                {
                    this.CurrentGame.MoveTurnOntoNextPlayer();
                }
                else
                {
                    this.StartNextGame();
                }
            }
            else // if we're ticking but not in a game then just show the regular refresh screen
            {
                RefreshScreen();
            }
        }

        public void RefreshScreen()
        {
            if (!string.IsNullOrWhiteSpace(this.OverrideMessage))
            {                
                if (!RemainingTicksToDisplayOverrideMessage.HasValue // if it's an infinite message
                    || RemainingTicksToDisplayOverrideMessage.Value > 0) // or has time left to roll
                {
                    this.DrawOverrideMessageScreen();
                }

                if (RemainingTicksToDisplayOverrideMessage.HasValue) // decrement the counter
                {
                    RemainingTicksToDisplayOverrideMessage--;
                    if(RemainingTicksToDisplayOverrideMessage <=0)
                    {
                        RemainingTicksToDisplayOverrideMessage = null;
                        OverrideMessage = null;
                    }
                }

                return;
            }

            if(this.CurrentGame != null)
            {
                if (this.CurrentGame.GameOver)
                {
                    this.EndGame();
                    this.StartNextGame();
                }
                return;
            }

            if(this.Server != null)
            {
                DrawServerScreen();
                return;
            }

            this.DrawMenu();
        }

        private void StartNextGame()
        {
            this.Event.CurrentRound.MoveOntoNextGame();

            var gameFleets = new List<Fleet>();
            foreach (var player in this.CurrentGame.Players)
            {
                player.StartGame(CurrentGame);
                var fleet = player.GetFleet(this.Event.CurrentRound.Classes);
                gameFleets.Add(fleet);
            }
            this.CurrentGame.Fleets = gameFleets;
        }

        private void EndGame()
        {
            OverrideMessage = this.CurrentGame.Winner == null // then show the winner's message
                ? "Draw!"
                : $"{this.CurrentGame.Winner.Player.Name} wins!";
            RemainingTicksToDisplayOverrideMessage = 3;

            this.CurrentGame.Scores.Add(new ScoreAwarded
            {
                Player = this.CurrentGame.Winner.Player,
                Score = Scores.WinningGame
            });
        }

        private void DrawMenu()
        {
            if(Timer.Interval != 25)
            {
                this.Timer.Interval = 25;
            }
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
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            
            var shipPainter = new ShipPainter();

            bool twoRows = this.CurrentGame.Players.Count() > 3;
            int playersWidth = twoRows ? 3 * 275 : this.CurrentGame.Players.Count() * 275;
            int xBuffer = (this.pictureBox1.Width - playersWidth) / 2;

            int i = 0;
            int x = 0;
            int y = 0;
            foreach (var fleet in this.CurrentGame.Fleets)
            {
                var fleetScreen = DrawFleetScreen(fleet, shipPainter, 550, 550);
                using (var gfx = Graphics.FromImage(bitmap))
                {
                    GetCoords(i, this.CurrentGame.Fleets.Count(), out x, out y);

                    gfx.DrawImage(fleetScreen,
                        new Rectangle(xBuffer + (x * 275), y * 275, 275, 275),
                        new Rectangle(0, 0, 550, 550),
                        GraphicsUnit.Pixel);

                    if(this.CurrentGame.PlayerWhosTurnItIs.Equals(fleet))
                    {                        
                        gfx.DrawRectangle(Pens.Red, new Rectangle(xBuffer + (x * 275), y * 275, 274, 275));
                    }
                }

                i++;
            }

            this.UpdateScreen(bitmap);
        }

        private void GetCoords(int fleetIndex, int fleetCount, out int x, out int y)
        {
            if (fleetCount <= 3)
            {
                y = 0;
                x = fleetIndex;
            }
            else
            {
                y = fleetIndex < 3 ? 0 : 1;
                x = y == 1 ? fleetIndex - 3 : fleetIndex;
            }
        }

        public Bitmap DrawFleetScreen(Fleet fleet, ShipPainter shipPainter, int width, int height)
        {
            var bitmap = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.FillRectangle(Brushes.Aqua, new RectangleF(0, 0, width, height));

                foreach (var ship in fleet.Ships.Where(s => s.IsAfloat))
                {
                    shipPainter.DrawShip(gfx, ship);
                }

                if (this.CurrentGame.CurrentPlayersShots != null)
                {
                    var shotsAtThisPlayer = this.CurrentGame.CurrentPlayersShots.ContainsKey(fleet.Player)
                        ? this.CurrentGame.CurrentPlayersShots[fleet.Player]
                        : new List<HitNotification>();

                    foreach (var lastTurnShot in shotsAtThisPlayer)
                    {
                        if (lastTurnShot.WasAHit)
                        {
                            DrawSomething(gfx, lastTurnShot.Coordinates, "Explosion", Color.Black);
                        }
                        else
                        {
                            DrawSomething(gfx, lastTurnShot.Coordinates, "Splash", Color.FromArgb(13, 27, 39));
                        }
                    }
                }

                for (int i = 1; i < 10; i++)
                {
                    gfx.DrawLine(Pens.Black, new Point(0, (i * 51) ), new Point(this.pictureBox1.Width, (i * 51)));  // horizontal
                    gfx.DrawLine(Pens.Black, new Point((i * 51), 0), new Point((i * 51), this.pictureBox1.Height)); // vertical
                }


                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                //gfx.DrawString(GetTitleText(),
                //    new Font(FontFamily.GenericMonospace, 22),
                //    Brushes.Black,
                //    new RectangleF(0, 0, width, height),
                //    format);

            }

            return bitmap;
        }

        
        protected void DrawSomething(Graphics gfx, Coordinates lastTurnShotCoorinates, string what, Color transparent)
        {
            var image = ShipPainter.GetBitmapResource(what);
            image.MakeTransparent(transparent);

            gfx.DrawImage(image, new Point(lastTurnShotCoorinates.X * 51, lastTurnShotCoorinates.Y * 51));
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

        private void DoPreTurn()
        {
            this.CurrentGame.SinkShipsWhichCollideOrFallOutOfBounds();
            this.CurrentGame.CheckForWinners();
        }

        private void DoPostTurn()
        {
            this.CurrentGame.CheckForWinners();

            var fleetsBeingShotAt = new List<int>();
            int i = 0;
            foreach (var fleet in this.CurrentGame.Fleets)
            {
                if (this.CurrentGame.CurrentPlayersShots.ContainsKey(fleet.Player))
                {
                    fleetsBeingShotAt.Add(i);
                }
                i++;
            }
        }


        private void DoTurn()
        {
            var shots = this.CurrentGame.PlayerWhosTurnItIs.Player.GetShots(this.CurrentGame, this.CurrentGame.PlayerWhosTurnItIs);
            var results = new List<ShotResult>();
            var hitNotifications = new Dictionary<Player, List<HitNotification>>();

            foreach (var shot in shots)
            {                
                var fleetBeingShotAt = this.CurrentGame.Fleets.SingleOrDefault(f => f.Player.Name.Equals(shot.FleetName));
                if (fleetBeingShotAt != null)
                {
                    var result = fleetBeingShotAt.ResolveShot(shot);
                    results.Add(result);

                    if (!hitNotifications.ContainsKey(fleetBeingShotAt.Player))
                    {
                        hitNotifications.Add(fleetBeingShotAt.Player, new List<HitNotification>());
                    }

                    if (result.WasAHit)
                    {
                        this.CurrentGame.Scores.Add(new ScoreAwarded
                        {
                            Player = this.CurrentGame.PlayerWhosTurnItIs.Player,
                            Score = result.WasASink ? Scores.Sink : Scores.Hit
                        });                        

                        hitNotifications[fleetBeingShotAt.Player].Add(new HitNotification
                        {
                            Shooter = this.CurrentGame.PlayerWhosTurnItIs.Player.Name,
                            WasASink = result.WasASink,
                            Coordinates = shot.Coordinates,
                            ClassHit = result.Class,
                            WasAHit = true
                        });
                    }
                    else // record the miss
                    {
                        hitNotifications[fleetBeingShotAt.Player].Add(new HitNotification
                        {
                            Shooter = this.CurrentGame.PlayerWhosTurnItIs.Player.Name,
                            WasASink = false,
                            Coordinates = shot.Coordinates,
                            ClassHit = null,
                            WasAHit = false
                        });
                    }
                }
            }

            foreach(var playersHit in hitNotifications)
            {
                playersHit.Key.NotifyOfBeingHit(playersHit.Value.Where(h => h.WasAHit));
            }

            this.CurrentGame.CurrentPlayersShots = hitNotifications;
            this.CurrentGame.PlayerWhosTurnItIs.Player.RespondToShots(results);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
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
                            this.Event = Event.CreateEventSchedule(this.Server.ConnectedPlayers);                            
                            this.StartNextGame();

                            this.Timer.Interval = TurnTickInterval;
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

                            this.Event = Event.CreateLocalGame();
                            this.StartNextGame();
                            this.Timer.Interval = TurnTickInterval;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Client?.EndGame();
            this.Server?.StopListening(); // catch all

            base.OnClosing(e);
        }
    }
}
