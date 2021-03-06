﻿using Bottleships.AI;
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

        public List<ICaptain> LocalGameOpponents { get; set; }
        
        Bitmap MenuBackground { get; set; }

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
            
            this.DrawMainMenu();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(ScrollingXPos <= -200) ScrollingXPos = this.pictureBox1.Width;
            ScrollingXPos -= 2;

            if(this.OverrideMessage == "Playing Remote Game" && !(this.Client?.IsGameRunning ?? true))
            {
                this.OverrideMessage = null;
                this.Client?.EndGame(); // last ditch in case we've not shut things down properly
                this.Server?.StopListening();                
            }


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
                    EndGame();
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

            if(this.LocalGameOpponents != null)
            {
                DrawSelectLocalOpponentMenu();
                return;
            }

            this.DrawMainMenu();
        }

        private void StartNextGame()
        {
            this.Event.CurrentRound.MoveOntoNextGame();

            if (!this.Event.CurrentRound.RoundOver)
            {
                var gameFleets = new List<Fleet>();
                foreach (var player in this.CurrentGame.Players)
                {
                    player.StartGame(CurrentGame);
                    var fleet = player.GetFleet(this.Event.CurrentRound.Classes);
                    gameFleets.Add(fleet);
                }
                this.CurrentGame.Fleets = gameFleets;
            }
            else
            {
                foreach (var player in this.Event.Players)
                {
                    player.EndRound(this.Event.CurrentRound);
                }
            }
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

            foreach (var player in this.CurrentGame.Players)
            {
                player.EndGame(this.CurrentGame);
            }
        }

        private void DrawMainMenu()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            using (var gfx = Graphics.FromImage(bitmap))
            {

                DrawMenu(gfx,
                        bitmap,
                        true,
                        "Test Bot Locally",
                         "Connect Bot To Server",
                        "Host Server",
                        "Exit");                
            }

            UpdateScreen(bitmap);
        }

        private void DrawMenu(Graphics gfx, Bitmap bitmap, bool alignTop, int[] selectedIndex, params string[] menuItems)
        {
            DrawMenu(gfx, bitmap, alignTop, true, 0, selectedIndex, menuItems);
        }

        private void DrawMenu(Graphics gfx, Bitmap bitmap, bool alignTop, params string[] menuItems)
        {
            DrawMenu(gfx, bitmap, alignTop, true, 0, new int[] { }, menuItems);
        }

        private void DrawMenu(Graphics gfx, Bitmap bitmap, bool alignTop, bool drawBackground, int indexOffset, int[] selectedIndicies, params string[] menuItems)
        {
            if (Timer.Interval != 25)
            {
                this.Timer.Interval = 25;
            }


            if (BackgroundImage == null)
            {
                BackgroundImage = ShipPainter.GetBitmapResource("Menu");
            }

            var distanceFromTheTop = 275;
            var distanceFromTheBottom = 50;
            var spacing = 55;

            if (drawBackground == true)
            {
                gfx.DrawImage(BackgroundImage, new Rectangle(0, 0, this.pictureBox1.Width, this.pictureBox1.Height));
            }

            for (int i = 0; i < menuItems.Count(); i++)
            {
                int yPosition = alignTop
                    ? distanceFromTheTop + (i * spacing)
                    : this.pictureBox1.Height - distanceFromTheBottom - (spacing * menuItems.Count()) + (i * spacing);

                var selected = selectedIndicies.Contains(i + indexOffset);
                var highlighted = SelectedMenuIndex == i + indexOffset && (ScrollingXPos / 10) % 2 == 0;
                var brush = highlighted
                    ? Brushes.White : selected
                        ? Brushes.Green
                        : Brushes.Black;

                gfx.DrawString(menuItems.ElementAt(i),
                    new Font(FontFamily.GenericMonospace, 36, FontStyle.Bold),
                     brush,
                    new PointF(10, yPosition));
            }
        }



        private void DrawSelectLocalOpponentMenu()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            using (var gfx = Graphics.FromImage(bitmap))
            {

                var selectedIndex = new List<int>();
                if (this.LocalGameOpponents.OfType<RandomCaptain>().Any()) selectedIndex.Add(0);
                if (this.LocalGameOpponents.OfType<SimpleCaptain>().Any()) selectedIndex.Add(1);
                if (this.LocalGameOpponents.OfType<Nelson>().Any()) selectedIndex.Add(2);

                DrawMenu(gfx,
                        bitmap,
                        true,
                        selectedIndex.ToArray(),
                        "Random",
                        "Simple Captain",
                        "Nelson");                

                DrawMenu(gfx,
                        bitmap,
                        false,
                        false,
                        3,
                        new int[] { },
                        "Start Game",
                        "Exit");
            }

            UpdateScreen(bitmap);
        }

        private void DrawOverrideMessageScreen()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                DrawMenu(gfx, bitmap, true);

                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                gfx.DrawString(this.OverrideMessage, new Font(FontFamily.GenericMonospace, 48, FontStyle.Bold), Brushes.Black, new RectangleF(0, 0, this.pictureBox1.Width, this.pictureBox1.Height), format);
            }

            UpdateScreen(bitmap);
        }

        private void DrawServerScreen()
        {
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);
            var spaceForHeader = 275;
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                DrawMenu(gfx, bitmap, false, "Start Game", "Close Server");
                var brush = Brushes.Black;
                var font = new Font(FontFamily.GenericMonospace, 36, FontStyle.Bold);
                gfx.DrawString("Configuring Server", font, brush, new PointF(10, spaceForHeader + 10));

                var listenText = $"Server listening on http://{Environment.MachineName}:5999{"".PadRight(3 - Math.Abs(ScrollingXPos / 10) % 3, '.')}";
                gfx.DrawString(listenText, font, brush, new PointF(10, spaceForHeader + 75));

                gfx.DrawString("Connected Players:", font, brush, new PointF(10, spaceForHeader + 110));
                int i = 1;
                foreach(var player in this.Server.ConnectedPlayers)
                {
                    gfx.DrawString(player.Name, font, brush, new PointF(15, spaceForHeader + 110 + (55 * i)));
                    i++;
                }
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
            int xBuffer = (this.pictureBox1.Width - playersWidth - 300) / 2;
            int yBuffer = 100;

            int i = 0;
            int x = 0;
            int y = 0;
            StringFormat format;
            using (var gfx = Graphics.FromImage(bitmap))
            {
                gfx.FillRectangle(Brushes.Aqua, 0, 0, this.pictureBox1.Width, this.pictureBox1.Height);
                foreach (var fleet in this.CurrentGame.Fleets)
                {
                    var fleetScreen = DrawFleetScreen(fleet, shipPainter, 550, 550);

                    GetCoords(i, this.CurrentGame.Fleets.Count(), out x, out y);

                    // fleet board
                    gfx.DrawImage(fleetScreen,
                        new Rectangle(xBuffer + (x * 275), yBuffer + (y * (275 + 75)), 275, 275),
                        new Rectangle(0, 0, 550, 550),
                        GraphicsUnit.Pixel);

                    // ship's names
                    format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;
                    gfx.DrawString(fleet.Player.Name,
                        new Font(FontFamily.GenericMonospace, 12),
                        Brushes.Black,
                        new Rectangle(xBuffer + (x * 275), yBuffer + (y * (275 + 75)) + 275, 275, 75),
                        format);

                    // red border
                    if (this.CurrentGame.PlayerWhosTurnItIs.Equals(fleet))
                    {
                        gfx.DrawRectangle(Pens.Red, new Rectangle(xBuffer + (x * 275), yBuffer + (y * (275 + 75)), 274, 274));
                    }

                    i++;
                }

                // scores
                StringBuilder sb = new StringBuilder();
                foreach(var score in this.CurrentGame.ScoresPerPlayer.OrderByDescending(s => s.Value))
                {
                    sb.AppendLine($"{score.Value} - {score.Key.Name}");
                }
                format = new StringFormat();
                //format.LineAlignment = StringAlignment.Near;
                //format.Alignment = StringAlignment.Near;
                gfx.DrawString(sb.ToString(),
                    new Font(FontFamily.GenericMonospace, 12),
                    Brushes.Black,
                    new Rectangle(this.pictureBox1.Width - 300, yBuffer, 300, this.pictureBox1.Height - yBuffer),
                    format);

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


                //StringFormat format = new StringFormat();
                //format.LineAlignment = StringAlignment.Center;
                //format.Alignment = StringAlignment.Center;
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
                        if (result.WasFreshDamage) // only award points for fresh damage
                        {
                            this.CurrentGame.Scores.Add(new ScoreAwarded
                            {
                                Player = this.CurrentGame.PlayerWhosTurnItIs.Player,
                                Score = result.WasASink ? Scores.Sink : Scores.Hit
                            });
                        }

                        hitNotifications[fleetBeingShotAt.Player].Add(new HitNotification // but notify regardless
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
                            this.DrawMainMenu();
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
                    if (LocalGameOpponents != null) // if we're selecting opponents
                    {
                        switch (SelectedMenuIndex)
                        {
                            case 0:
                                AddRemoveClassFromLocalOpponents<RandomCaptain>();
                                break;

                            case 1:
                                AddRemoveClassFromLocalOpponents<SimpleCaptain>();
                                break;

                            case 2:
                                AddRemoveClassFromLocalOpponents<Nelson>();
                                break;

                            case 3:
                                this.Server = null;

                                this.OverrideMessage = "Starting Local Game";
                                this.DrawOverrideMessageScreen();
                                this.OverrideMessage = null;

                                this.Event = Event.CreateLocalGame(this.LocalGameOpponents);
                                this.StartNextGame();
                                this.Timer.Interval = TurnTickInterval;
                                this.Timer.Start();
                                break;

                            case 4:
                                this.LocalGameOpponents = null;
                                this.SelectedMenuIndex = 0;
                                this.RefreshScreen();
                                break;
                        }

                    }
                    else // otherwise it's the main menu
                    {
                        switch (SelectedMenuIndex)
                        {
                            case 0: // play locally

                                this.LocalGameOpponents = new List<ICaptain>();
                                this.RefreshScreen();
                                break;
                            case 1: // connect to server   

                                try
                                {
                                    var server = "http://localhost:5999"; // the server name should be editable
                                    RemoteCommander.RegisterCaptain(server);
                                    this.Client = new Client(server);
                                    this.Client.PlayGame();  // TODO: we need to disconnect the listener when the game ends or we'll have a problem                            

                                    this.OverrideMessage = "Playing Remote Game";
                                    this.RefreshScreen();
                                }
                                catch(Exception)
                                {
                                    MessageBox.Show("Unable to connect to remote server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

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
        }

        public void AddRemoveClassFromLocalOpponents<T>() where T : ICaptain, new()
        {
            if (!this.LocalGameOpponents.OfType<T>().Any())
            {
                var item = new T();
                this.LocalGameOpponents.Add(item);
            }
            else
            {
                var toRemove = this.LocalGameOpponents.OfType<T>().ToArray();
                foreach (var item in toRemove)
                {
                    this.LocalGameOpponents.Remove(item);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Client?.EndGame();
            this.Server?.StopListening(); // catch all

            base.OnClosing(e);
        }
    }
}
