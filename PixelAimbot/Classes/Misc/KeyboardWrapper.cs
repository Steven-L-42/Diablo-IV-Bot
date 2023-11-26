using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = System.Windows.Point;


namespace PixelAimbot.Classes.Misc
{
    public class KeyboardWrapper
    {
        public static bool holdKeyDown = false;
        public static bool isWalking = false;
        /// <summary>
        ///     Key down flag
        /// </summary>
        private const int KEY_DOWN_EVENT = 0x0001;

        private const int KEY_UP_EVENT = 0x0002;

        private const int PauseBetweenStrokes = 50;
        public const int MOUSEEVENTF_WHEEL = 2048;
        public const byte VK_LBUTTON = 0x01;
        public const byte VK_ALT = 0xA4;
        public const byte VK_TAB = 0x09;

        public const byte VK_RBUTTON = 0x02;
        public const byte VK_SPACE = 0x20;
        public const byte VK_ESCAPE = 0x1B;
        public const byte VK_RETURN = 0x0D;
        public const byte VK_BACK = 0x08;

        public const byte VK_A = 0x41;
        public const byte VK_B = 0x42;
        public const byte VK_C = 0x43;
        public const byte VK_D = 0x44;
        public const byte VK_E = 0x45;
        public const byte VK_F = 0x46;
        public const byte VK_G = 0x47;
        public const byte VK_H = 0x48;
        public const byte VK_I = 0x49;
        public const byte VK_J = 0x4A;
        public const byte VK_K = 0x4B;
        public const byte VK_L = 0x4C;
        public const byte VK_P = 0x50;
        public const byte VK_O = 0x4F;
        public const byte VK_N = 0x4E;
        public const byte VK_Q = 0x51;
        public const byte VK_R = 0x52;
        public const byte VK_S = 0x53;
        public const byte VK_V = 0x56;
        public const byte VK_W = 0x57;
        public const byte VK_Y = 0x59;
        public const byte VK_Z = 0x5A;

        public const byte VK_F10 = 0x79;
        public const byte VK_F9 = 0x78;
        public const byte VK_F1 = 0x70;
        public const byte VK_1 = 0x31;
        public const byte VK_2 = 0x32;
        public const byte VK_3 = 0x33;
        public const byte VK_4 = 0x34;
        public const byte VK_5 = 0x35;
        public const byte VK_6 = 0x36;
        public const byte VK_7 = 0x37;
        public const byte VK_8 = 0x38;
        public const byte VK_9 = 0x39;

        public string LAYOUTS { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /// <summary>
        ///     Will hold a key down for a number of milliseconds
        /// </summary>
        /// <param name="key">byte value for key. can cast like this: (byte)System.Windows.Forms.Keys.F24</param>
        /// <param name="duration">ms to hold key down for</param>
        /// <example>
        ///     <code>
        /// Keyboard.KeyUp((byte)Keys.F24,5000);
        /// </code>
        /// </example>
        /// 


        public static void HoldKey(byte key, int duration)
        {
            var totalDuration = 0;
            while (totalDuration < duration)
            {
                keybd_event(key, 0, KEY_DOWN_EVENT, 0);
                keybd_event(key, 0, KEY_UP_EVENT, 0);
                Thread.Sleep(PauseBetweenStrokes);
                totalDuration += PauseBetweenStrokes;
            }
        }

        public static async Task HoldKeyBool(byte key)
        {
            var isMouseButton = key == KeyboardWrapper.VK_LBUTTON || key == KeyboardWrapper.VK_RBUTTON;

            if (isMouseButton)
            {
                if (key == KeyboardWrapper.VK_LBUTTON)
                {
                    VirtualMouse.LeftDown();
                }
                else if (key == KeyboardWrapper.VK_RBUTTON)
                {
                    VirtualMouse.RightDown();
                }
            }
            else
            {
                keybd_event(key, 0, KEY_DOWN_EVENT, 0);
            }

            
            while (holdKeyDown)
            {
                await Task.Delay(PauseBetweenStrokes);
            }
            
            
            if (isMouseButton)
            {
                if (key == KeyboardWrapper.VK_LBUTTON)
                {
                    VirtualMouse.LeftUp();
                }
                else if (key == KeyboardWrapper.VK_RBUTTON)
                {
                    VirtualMouse.RightUp();
                }
            }
            else
            {
                keybd_event(key, 0, KEY_UP_EVENT, 0);
            }
            holdKeyDown = false;
        }
        public static async Task AlternateHoldKey(byte key, int duration)
        {
            var totalDuration = 0;
            var isMouseButton = key == KeyboardWrapper.VK_LBUTTON || key == KeyboardWrapper.VK_RBUTTON;

            if (isMouseButton)
            {
                if (key == KeyboardWrapper.VK_LBUTTON)
                {
                    VirtualMouse.LeftDown();
                }
                else if (key == KeyboardWrapper.VK_RBUTTON)
                {
                    VirtualMouse.RightDown();
                }
            }
            else
            {
                keybd_event(key, 0, KEY_DOWN_EVENT, 0);
            }

            while (totalDuration < duration)
            {
                await Task.Delay(PauseBetweenStrokes);
                totalDuration += PauseBetweenStrokes;
            }

            if (isMouseButton)
            {
                if (key == KeyboardWrapper.VK_LBUTTON)
                {
                    VirtualMouse.LeftUp();
                }
                else if (key == KeyboardWrapper.VK_RBUTTON)
                {
                    VirtualMouse.RightUp();
                }
            }
            else
            {
                keybd_event(key, 0, KEY_UP_EVENT, 0);
            }
        }
        /// <summary>
        ///     Will press a key
        /// </summary>
        /// <param name="key">byte value for key. can cast like this: (byte)System.Windows.Forms.Keys.F24</param>
        /// <example>
        ///     <code>
        /// Keyboard.PressKey((byte)Keys.F24);
        /// </code>
        /// </example>
        /// 
        public static void MultiplePressKey(byte key1, byte key2)
        {
            keybd_event(key1, 0, KEY_DOWN_EVENT, 0);
            keybd_event(key2, 0, KEY_DOWN_EVENT, 0);
            keybd_event(key2, 0, KEY_UP_EVENT, 0);
            keybd_event(key1, 0, KEY_UP_EVENT, 0);
        }
        public static void PressKey(byte key)
        {
            if (key == KeyboardWrapper.VK_LBUTTON)
            {
                VirtualMouse.LeftClick();
            }
            else
            {
                keybd_event(key, 0, KEY_DOWN_EVENT, 0);
                keybd_event(key, 0, KEY_UP_EVENT, 0);
            }
        }

        /// <summary>
        ///     Will trigger the KeyUp event for a key. Easy way to keep the computer awake without sending any input.
        /// </summary>
        /// <param name="key">byte value for key. can cast like this: (byte)System.Windows.Forms.Keys.F24</param>
        /// <example>
        ///     <code>
        /// Keyboard.KeyUp((byte)Keys.F24);
        /// </code>
        /// </example>
        public static void KeyUp(byte key)
        {
            keybd_event(key, 0, KEY_UP_EVENT, 0);
        }

        public static void KeyDown(byte key)
        {
            keybd_event(key, 0, KEY_DOWN_EVENT, 0);
        }
    }


    public static class VirtualMouse
    {
        // constants for the mouse_input() API function
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_WHEEL = 0x0800;


        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        // import the necessary API function so .NET can
        // marshall parameters appropriately
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        public static void MoveTo(int x, int y, int nSpeed = 0)
        {
            Point ptCur;
            var rect = Screen.PrimaryScreen.Bounds;
            int xCur, yCur;
            int delta;
            const int nMinSpeed = 32;

            x = 65535 * x / (rect.Right - 1) + 1;
            y = 65535 * y / (rect.Bottom - 1) + 1;

            if (nSpeed == 0)
            {
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
                Task.Delay(10).Wait();
                return;
            }

            if (nSpeed < 0 || nSpeed > 100)
                nSpeed = 10; // Default is speed 10

            ptCur = GetCursorPosition();
            xCur = (int) ptCur.X * 65535 / (rect.Right - 1) + 1;
            yCur = (int) ptCur.Y * 65535 / (rect.Bottom - 1) + 1;

            // Mouse Calculation magic fickt meinen kopf ... im out now
            while (xCur != x || yCur != y)
            {
                if (xCur < x)
                {
                    delta = (x - xCur) / nSpeed;
                    if (delta == 0 || delta < nMinSpeed)
                        delta = nMinSpeed;
                    if (xCur + delta > x)
                        xCur = x;
                    else
                        xCur += delta;
                }
                else if (xCur > x)
                {
                    delta = (xCur - x) / nSpeed;
                    if (delta == 0 || delta < nMinSpeed)
                        delta = nMinSpeed;
                    if (xCur - delta < x)
                        xCur = x;
                    else
                        xCur -= delta;
                }

                if (yCur < y)
                {
                    delta = (y - yCur) / nSpeed;
                    if (delta == 0 || delta < nMinSpeed)
                        delta = nMinSpeed;
                    if (yCur + delta > y)
                        yCur = y;
                    else
                        yCur += delta;
                }
                else if (yCur > y)
                {
                    delta = (yCur - y) / nSpeed;
                    if (delta == 0 || delta < nMinSpeed)
                        delta = nMinSpeed;
                    if (yCur - delta < y)
                        yCur = y;
                    else
                        yCur -= delta;
                }

                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, xCur, yCur, 0, 0);

                Task.Delay(10).Wait();
            }
        }

        // simulates a click-and-release action of the left mouse
        // button at its current position
        public static void Scroll(int scroll)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scroll, 0);  // -Value für ScrollDown und +Value für ScrollUp 
                                                              // Bsp.: VirtualMouse.Scroll(-120); VirtualMouse.Scroll(+120);
        }
        public static void LeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }  
        public static void LeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }
        public static void LeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void RightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }    
        public static void RightDown()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            
        }
        public static void RightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
    }
}