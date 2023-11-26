using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;

namespace PixelAimbot
{
    partial class DiabloBot
    {


        private async Task Game_Restart()
        {
            try
            {

                if (_gameCrashed)
                {

                    DiscordSendMessage("Game is restarting");
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Game restart in 30 seconds."));

                    await Task.Delay(30000);

                    Process.Start(steampath, "steam://rungameid/1599340");
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Game is starting..."));

                    await Task.Delay(60000);

                    //var template = null;
                    //var detector = new ScreenDetector(template, null, 0.90f,
                    //    DiabloBot.Recalc(879),
                    //    DiabloBot.Recalc(1003, false),
                    //    DiabloBot.Recalc(65, true, true),
                    //    DiabloBot.Recalc(43, false, true));

                    //detector.setMyPosition(new Point(DiabloBot.Recalc(500), DiabloBot.Recalc(390, false)));
                    //bool game_restart = true;
                    //while (game_restart)
                    //{
                    //    try
                    //    {
                    //        using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    //        {
                    //            var item = detector.GetClosest(screenCapture);
                    //            if (item.HasValue && game_restart)
                    //            {

                    //                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Server selection..."));
                    //                game_restart = false;
                    //                VirtualMouse.MoveTo(Recalc(959), Recalc(925, false), 10);
                    //                KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //      //   ExceptionHandler.SendException(ex);
                    //        int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                    //        Debug.WriteLine("[" + line + "]" + ex.Message);
                    //    }
                    //    Random random = new Random();
                    //    var sleepTime = random.Next(5000, 5500);
                    //    await Task.Delay(sleepTime);
                    //}
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Load character selection...15 sec."));
                    await Task.Delay(15000);
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Select character..."));

                    int x = Convert.ToInt16(txCharSelect.Text);
                    switch (x)
                    {
                        case 1:
                            VirtualMouse.MoveTo(Recalc(342), Recalc(925, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        case 2:
                            VirtualMouse.MoveTo(Recalc(600), Recalc(924, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        case 3:
                            VirtualMouse.MoveTo(Recalc(840), Recalc(922, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        case 4:
                            VirtualMouse.MoveTo(Recalc(1085), Recalc(924, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        case 5:
                            VirtualMouse.MoveTo(Recalc(1334), Recalc(922, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        case 6:
                            VirtualMouse.MoveTo(Recalc(1579), Recalc(931, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            VirtualMouse.MoveTo(Recalc(859), Recalc(1015, false), 10);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);
                            break;
                        default:
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Wrong Charnumber is set!"));
                            break;
                    }
                    if (x > 0 && x < 7)
                    {
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Loadingscreen..."));
                        CheckIfLoadScreen();

                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Closing Window..."));
                        await Task.Delay(3000);
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Bot is ready!"));

                        VirtualMouse.MoveTo(Recalc(1482), Recalc(107, false), 10);
                        KeyboardWrapper.PressKey(KeyboardWrapper.VK_LBUTTON);

                        _start = false;
                        _stopped = false;
                        _stop = false;
                        _restart = false;
                        _logout = false;

                        Cts = new CancellationTokenSource();
                        await Task.Delay(_humanizer.Next(10, 240) + 5000);
                        KeyboardWrapper.PressKey(KeyboardWrapper.VK_F9);
                        _stop = true;
                        DiscordSendMessage("Bot is Running!");
                    }
                }

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


