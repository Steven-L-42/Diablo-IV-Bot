using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;

namespace PixelAimbot
{
    partial class DiabloBot
    {

        private async Task AutoRevive(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                TestDetectors detector;
                Image<Bgr, byte> template;
                float treshold = 0.7f;
  
                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1, token);

                    Point item = Point.Empty;

                    // Schleife zur Überprüfung der verschiedenen Templates
                    for (int i = 0; i < 4; i++)
                    {
                        

                        switch (i)
                        {
                            case 0:
                                template = ImageDead1;
                                detector = new TestDetectors(template, null, treshold,
                                                             Recalc(853),
                                                             Recalc(610, false),
                                                             Recalc(218),
                                                             Recalc(194, false));
                                break;
                            case 1:
                                template = ImageDead2;
                                detector = new TestDetectors(template, null, treshold,
                                                             Recalc(853),
                                                             Recalc(610, false),
                                                             Recalc(218),
                                                             Recalc(194, false));
                                break;
                            case 2:
                                template = ImageDead3;
                                detector = new TestDetectors(template, null, treshold,
                                                             Recalc(853),
                                                             Recalc(610, false),
                                                             Recalc(218),
                                                             Recalc(194, false));
                                break;
                            case 3:
                                template = ImageDead4;
                                detector = new TestDetectors(template, null, treshold,
                                                             Recalc(853),
                                                             Recalc(610, false),
                                                             Recalc(218),
                                                             Recalc(194, false));
                                break;
                            default:
                                continue;
                        }

                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1, token);

                        using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                        {
                            item = detector.ClickIfFound(screenCapture, false, true);
                        }

                        if (item.X > 0 && item.Y > 0)
                        {
                            playerDead = true;
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Player dead..."));
                            await Task.Delay(2000, token);
                            token.ThrowIfCancellationRequested();
                            VirtualMouse.MoveTo(Recalc(955), Recalc(925, false), 5);
                            VirtualMouse.LeftClick();
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Player revived..."));
                            playerDead = false;
                            break; // Die Schleife beenden, wenn der Spieler wiederbelebt wurde
                        }
                    }

                    await Task.Delay(5000);
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