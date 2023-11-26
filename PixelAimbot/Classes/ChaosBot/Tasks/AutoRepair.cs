using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;

namespace PixelAimbot
{
    partial class DiabloBot
    {

        private async Task AutoRepair(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                var template = ImageDestroyedArmor;
                var detector = new TestDetectors(template, null, 0.85f,
                     Recalc(1102),
                     Recalc(870, false),
                     Recalc(53),
                     Recalc(59, false));

                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    if (!playerDead)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1, token);
                        using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                        {
                            Point item = detector.ClickIfFound(screenCapture, false, true);
                            if (item.X > 0 && item.Y > 0)
                            {

                                _RepairProcess = true;
                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Armor destroyed..."));

                                KeyboardWrapper.PressKey(KeyboardWrapper.VK_TAB);
                                await Task.Delay(1000, token);
                                token.ThrowIfCancellationRequested();
                                await Task.Delay(1, token);

                                template = ImageTeleporter;
                                detector = new TestDetectors(template, null, 0.85f,
                                     Recalc(288),
                                     Recalc(139, false),
                                     Recalc(1343),
                                     Recalc(825, false));


                                while (_stopped == false && _RepairProcess && !playerDead)
                                {
                                    try
                                    {
                                       processName = Process.GetProcessesByName("Diablo IV");
                                        if (processName.Length != 1 && !_gameCrashed)
                                            await GameCrashed();
                                        if (processName.Length != 1 && _gameCrashed)
                                            return;

                                        token.ThrowIfCancellationRequested();
                                        await Task.Delay(1, token);
                                        countLookedForTeleporter++;
                                        using (var screenCapture2 = _globalScreenPrinter.CaptureScreenImage())
                                        {
                                            item = detector.ClickIfFound(screenCapture2, false, true);
                                            if (item.X > 0 && item.Y > 0)
                                            {
                                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Teleporter found..."));

                                                // CLick on Teleporter on Map
                                                VirtualMouse.MoveTo(Recalc(item.X), Recalc(item.Y, false), 5);
                                                VirtualMouse.LeftClick();

                                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Start teleport..."));
                                                // CLick on Accept
                                                await Task.Delay(500, token);
                                                VirtualMouse.MoveTo(Recalc(873), Recalc(600, false), 5);
                                                VirtualMouse.LeftClick();
                                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Teleport for repair..."));
                                                await Task.Delay(4000, token);
                                                // Wait shedule

                                                template = ImageDestroyedArmor;
                                                detector = new TestDetectors(template, null, 0.85f,
                                                Recalc(1102),
                                                Recalc(870, false),
                                                Recalc(53),
                                                Recalc(59, false));
                                                bool Splashscreen = true;
                                                while (Splashscreen)
                                                {
                                                    processName = Process.GetProcessesByName("Diablo IV");
                                                    if (processName.Length != 1 && !_gameCrashed)
                                                        await GameCrashed();
                                                    if (processName.Length != 1 && _gameCrashed)
                                                        return;

                                                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Splashscreen is shown..."));
                                                    using (var SpashscreenCapture = _globalScreenPrinter.CaptureScreenImage())
                                                    {
                                                        item = detector.ClickIfFound(SpashscreenCapture, false, true);
                                                        if (item.X > 0 && item.Y > 0)
                                                            Splashscreen = false;

                                                    }
                                                    await Task.Delay(1000, token);
                                                }

                                                for (int i = 3; i > 0; i--)
                                                {
                                                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = i + " seconds to repair..."));
                                                    await Task.Delay(1000, token);
                                                }

                                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Start repair..."));
                                                // CLick on Repair NPC  
                                                VirtualMouse.MoveTo(Recalc(739), Recalc(136, false), 5);
                                                VirtualMouse.LeftClick();

                                                // CLick on Repair Section
                                                await Task.Delay(2000, token);
                                                VirtualMouse.MoveTo(Recalc(350), Recalc(100, false), 5);
                                                VirtualMouse.LeftClick();

                                                // CLick on Repair
                                                await Task.Delay(1000, token);
                                                VirtualMouse.MoveTo(Recalc(466), Recalc(473, false), 5);
                                                VirtualMouse.LeftClick();
                                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Armor repaired..."));
                                                _RepairProcess = false;

                                            }
                                            else if (countLookedForTeleporter >= 3)
                                            {
                                                KeyboardWrapper.PressKey(KeyboardWrapper.VK_TAB);
                                                countLookedForTeleporter = 0;
                                                await Task.Delay(1000, token);
                                            }
                                            else
                                            {
                                                VirtualMouse.Scroll(-50);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //   ExceptionHandler.SendException(ex);
                                        int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                                        Debug.WriteLine("[" + line + "]" + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                _RepairProcess = false;
                            }
                        }
                    }

                    if (playerDead)
                        await Task.Delay(2000);
                    else
                        await Task.Delay(250);

                }

            }
            catch (Exception ex)
            {
                //   ExceptionHandler.SendException(ex);
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}