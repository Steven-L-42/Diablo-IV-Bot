using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PixelAimbot
{
    public partial class frmMinimized : Form
    {
        public Stopwatch sw = new Stopwatch();
        public string title = "Symbiotic D4 : ";
        public frmMinimized()
        {
            InitializeComponent();
            this.Text = FrmLogin.RandomString(15);
            this.labelTitle.Text = title;
        }

        private void timerRuntimer_Tick(object sender, EventArgs e)
        {
            labelRuntimer.Text = sw.Elapsed.Hours.ToString("D2") + ":" + sw.Elapsed.Minutes.ToString("D2") + ":" + sw.Elapsed.Seconds.ToString("D2") + " running";
        }

        public void updateLabel(string label)
        {
            this.labelTitle.Text = label;
        }
        public void updateImage(Bitmap image)
        {
            this.imageBgra.BackgroundImage = image;
        }

    }
}
