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


        public void DrawGameScreen()
        {            
            var bitmap = new Bitmap(this.pictureBox1.Width, pictureBox1.Height);

            var gameSize = (25 * 10) + 8;
            var xBuffer = (this.pictureBox1.Width - gameSize) / 2;
            var yBuffer = (this.pictureBox1.Height - gameSize) / 2;

            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.FillRectangle(Brushes.Aqua, new RectangleF(0, 0, this.pictureBox1.Width, this.pictureBox1.Height));

                for (int i = 1; i < 10; i++)
                {
                    gfx.DrawLine(Pens.Black, new Point(xBuffer, (i * 26) + yBuffer), new Point(this.pictureBox1.Width - xBuffer, (i * 26) + yBuffer));
                    gfx.DrawLine(Pens.Black, new Point((i * 26) + xBuffer, yBuffer), new Point((i * 26) + xBuffer, this.pictureBox1.Height - yBuffer));
                }

            }


            this.UpdateScreen(bitmap);
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
            DrawGameScreen();
        }
    }
}
