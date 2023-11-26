using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot.Classes.Misc
{
    public partial class SearchColorOverlay : Form
    {
        public SearchColorOverlay()
        {
            InitializeComponent();
        }

        private void SearchColorOverlay_Load(object sender, EventArgs e)
        {
            this.Opacity = (float)0.01f;
        }
        public event EventHandler<Point> MouseClickHandled;

        private void SearchColorOverlay_MouseClick(object sender, MouseEventArgs e)
        {
            Point clickedPoint = e.Location;
            MouseClickHandled?.Invoke(this, clickedPoint);
            this.Close();

        }
    }
}
