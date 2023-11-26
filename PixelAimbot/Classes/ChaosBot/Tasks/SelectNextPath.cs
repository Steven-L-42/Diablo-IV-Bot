using Emgu.CV;
using Emgu.CV.Structure;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        

        async Task SelectNextPath(Image<Bgr, byte> image, int i, CancellationToken NextPathToken)
        {
            try
            {
                if (image != null)
                {
                    
                    KeyboardWrapper.PressKey(KeyboardWrapper.VK_TAB);
                    await Task.Delay(1000, NextPathToken);

                    var detector = new TestDetectors(
                        image,
                        null,
                        0.9f,
                        Recalc(0),
                        Recalc(0, false),
                        Recalc(1920), 
                        Recalc(1080, false)
                    );

                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        Point item = detector.ClickIfFound(screenCapture, false, true);
                        if (item.X > 0 && item.Y > 0)
                        {

                            TaskCompletionSource<bool> invokeTaskCompletionSource = new TaskCompletionSource<bool>();

                            string pathText = (i) + ". path selected.";

                            lbStatus.Invoke((MethodInvoker)(() =>
                            {
                                lbStatus.Text = pathText;
                                lbStatus.Refresh();
                                invokeTaskCompletionSource.SetResult(true);
                            }));

                            await invokeTaskCompletionSource.Task;
                            NextPathToken.ThrowIfCancellationRequested();
                            VirtualMouse.MoveTo(item.X, item.Y, 5);
                            NextPathToken.ThrowIfCancellationRequested();
                            VirtualMouse.RightClick();
                            await Task.Delay(500, NextPathToken);
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_TAB);
                            _PathSelected = true;
                            playerFight = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine($"[{line}] {ex.Message}");
            }
            finally
            {
                _SelectNextPathToken?.Cancel();
                _SelectNextPathToken?.Dispose();
             
            }
        }

    }
}