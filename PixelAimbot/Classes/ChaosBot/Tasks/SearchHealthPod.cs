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
        private async Task SearchHealthPod(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                var template = ImageHealthPod;
                var detector = new TestDetectors(template, null, 0.75f,
                    Recalc(259),
                    Recalc(104, false),
                    Recalc(1325), Recalc(765, false));

                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    if (_FightEnds && !playerDead)
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            await Task.Delay(1, token);
                            using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                            {
                                Point item = detector.ClickIfFound(screenCapture, false, true);
                                if (item.X > 0 && item.Y > 0)
                                {
                                    _EnemyFound = true;
                                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Life-Pod found."));
                                    VirtualMouse.MoveTo(Recalc(item.X), Recalc(item.Y, false), 5);
                                    VirtualMouse.LeftClick();
                                }
                                else
                                {
                                    if (counterLookForPotion >= 5)
                                    {
                                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Life-Pod search..."));
                                        counterLookForPotion = 0;
                                        _LookForPotion = true;
                                        _FightEnds = false;
                                    }
                                    else
                                    {
                                        counterLookForPotion++;
                                        _LookForPotion = false;
                                    }
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                          //   ExceptionHandler.SendException(ex);
                            int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                            Debug.WriteLine("[" + line + "]" + ex.Message);
                        }
                    else _LookForPotion = false;
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "READY!"));

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