using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PixelAimbot.Classes.OpenCV
{
    public class DrawScreenWin
    {
        // drawing
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        public DrawScreenWin()
        {
            var f = new Form();
            f.FormBorderStyle = FormBorderStyle.None;
            f.Bounds = Screen.PrimaryScreen.Bounds;
            f.TopMost = true;
            f.BackColor = Color.LimeGreen;
            f.TransparencyKey = Color.LimeGreen;

            Application.EnableVisualStyles();
            //     Application.Run(f);

        }

        public void Draw(Form f, int x, int y, int width, int height, Pen p = null)
        {
            f.TopMost = true;
            var formGraphics = f.CreateGraphics();
            if (p is null)
            {
                p = new Pen(Color.Red, 3);
            }
            f.Invalidate();
            formGraphics.DrawRectangle(p, new Rectangle(x, y, width, height));
            formGraphics.Dispose();
            p.Dispose();

        }
    }
}