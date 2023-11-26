using System.Runtime.InteropServices;
using System.Drawing;
using System;

public class DrawScreen
{
    SolidBrush b = new SolidBrush(Color.Red);
    

    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("User32.dll")]
    public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

    public void Draw(int x, int y, int width, int height, Pen p = null)
    {
        if (p is null)
        {
            p = new Pen(Color.Red, 3);
        }

        IntPtr desktopPtr = GetDC(IntPtr.Zero);
        Graphics g = Graphics.FromHdc(desktopPtr);

        g.DrawRectangle(p, new Rectangle(x,y, width, height));

        g.Dispose();
        ReleaseDC(IntPtr.Zero, desktopPtr);
    }
}