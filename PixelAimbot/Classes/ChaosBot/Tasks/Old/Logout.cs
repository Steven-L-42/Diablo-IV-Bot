using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        private async Task Logout(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "LOGOUT starts in 20 Seconds..."));
                await Task.Delay(_humanizer.Next(10, 240) + 20000, token);
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                lbStatus.Invoke((MethodInvoker) (() => lbStatus.Text = "LOGOUT Process starts..."));
                KeyboardWrapper.PressKey(KeyboardWrapper.VK_ESCAPE);
                await Task.Delay(_humanizer.Next(10, 240) + 2000, token);
                VirtualMouse.MoveTo(Recalc(1471), Recalc(723, false), 5); 
                KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                await Task.Delay(_humanizer.Next(10, 240) + 2000, token);
                VirtualMouse.MoveTo(Recalc(906), Recalc(575, false), 5);
                KeyboardWrapper.PressKey(KeyboardWrapper.VK_RETURN);
                await Task.Delay(_humanizer.Next(10, 240) + 1000, token);
                DiscordSendMessage("Bot logged you out!");
                lbStatus.Invoke((MethodInvoker) (() => lbStatus.Text = "You are logged out!"));
                _start = false;
                for (int i = 0; i < 50; i++)
                {
                    Cts.Cancel();
                    CtsBoss.Cancel();
                    CtsSkills.Cancel();
                    await Task.Delay(100);
                }
                Cts.Dispose();
                CtsBoss.Dispose();
                CtsSkills.Dispose();
               
            }
            catch (AggregateException)
            {
                Debug.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Bug");
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}