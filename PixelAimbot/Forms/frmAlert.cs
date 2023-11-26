using System;
using System.Drawing;
using System.Windows.Forms;
using PixelAimbot.Properties;

namespace PixelAimbot
{
    public partial class FrmAlert : Form
    {
        public FrmAlert()
        {
            InitializeComponent();
        }
        
        public enum EnmAction
        {
            Wait,
            Start,
            Close
        }

        public enum EnmType
        {
            Success,
            Warning,
            Error,
            Info
        }
        private EnmAction _action;

        private int _x, _y;

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(this._action)
            {
                case EnmAction.Wait:
                    timer1.Interval = 5000;
                    _action = EnmAction.Close;
                    break;
                case EnmAction.Start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this._x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if (this.Opacity == 1.0)
                        {
                            _action = EnmAction.Wait;
                        }
                    }
                    break;
                case EnmAction.Close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (Opacity == 0.0)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            _action = EnmAction.Close;
        }

        public void ShowAlert(string msg, EnmType type)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for (int i = 1; i < 10; i++)
            {
                fname = "alert" + i.ToString();
                FrmAlert frm = (FrmAlert)Application.OpenForms[fname];

                if (frm == null)
                {
                    this.Name = fname;
                    this._x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + 15;
                    this._y = this.Height * i + 5 * i;
                    this.Location = new Point(this._x, this._y);
                    break;

                }

            }
            this._x = Screen.PrimaryScreen.WorkingArea.Width - Width - 5;

            switch(type)
            {
                case EnmType.Success:
                    this.pictureBox1.Image = Resources.success;
                    this.BackColor = Color.SeaGreen;
                    break;
                case EnmType.Error:
                    this.pictureBox1.Image = Resources.error;
                    this.BackColor = Color.DarkRed;
                    break;
                case EnmType.Info:
                    this.pictureBox1.Image = Resources.info;
                    this.BackColor = Color.RoyalBlue;
                    break;
                case EnmType.Warning:
                    this.pictureBox1.Image = Resources.warning;
                    this.BackColor = Color.DarkOrange;
                    break;
            }


            this.lblMsg.Text = msg;

            this.Show();
            this._action = EnmAction.Start;
            this.timer1.Interval = 1;
            this.timer1.Start();
        }
    }
}