using PixelAimbot.Classes.Misc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    partial class DiabloBot
    {

        private async Task AutoSetup()
        {
            try
            {
                Process[] processName = Process.GetProcessesByName("Diablo IV");
                if (processName.Length == 1)
                {
                    handle = processName[0].MainWindowHandle;
                    SetForegroundWindow(handle);
                    await Task.Delay(1000);
                    KeyboardWrapper.PressKey(KeyboardWrapper.VK_ESCAPE);
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(320), Recalc(607, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(829), Recalc(29, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(1178), Recalc(939, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(1103), Recalc(504, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(958), Recalc(593, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    VirtualMouse.MoveTo(Recalc(757), Recalc(1050, false), 5);
                    VirtualMouse.LeftClick();
                    await Task.Delay(250);
                    KeyboardWrapper.PressKey(KeyboardWrapper.VK_ESCAPE);
                    await Task.Delay(500);
                    KeyboardWrapper.PressKey(KeyboardWrapper.VK_ESCAPE);
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Game setup successful!"));
                }
                else
                {
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Diablo IV is not running!"));
                }
               


            }
            catch (Exception ex)
            {
                ExceptionHandler.SendException(ex);
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}