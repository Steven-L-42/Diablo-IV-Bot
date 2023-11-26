using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        private async Task Screenshot(CancellationToken token)
        {
            while (_start)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1, token);
                    var newImage = new Bitmap(_globalScreenPrinter.CaptureScreen()).ToImage<Bgr, byte>();
                    newImage.Dispose();
                    

                }
                catch (Exception ex)
                {
                    int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                    Debug.WriteLine("[" + line + "]" + ex.Message);
                }
            }
        }
    }
}