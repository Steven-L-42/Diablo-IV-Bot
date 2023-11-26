using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PixelAimbot.Classes.Misc;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        //public async void Awakeningskill(CancellationToken token)
        //{
        //    try
        //    {
        //        token.ThrowIfCancellationRequested();
        //        await Task.Delay(1, token);

        //        await Task.Delay(humanizer.Next(10, 240) + (int.Parse(txLeaveTimerFloor2.Text) * 1000) - 7000, token);

        //        KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_V, 3000);
        //    }
        //    catch (AggregateException)
        //    {
        //        Debug.WriteLine("Expected");
        //    }
        //    catch (ObjectDisposedException)
        //    {
        //        Debug.WriteLine("Bug");
        //    }
        //    catch (Exception ex)
        //    {
        //        int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
        //        Debug.WriteLine("[" + line + "]" + ex.Message);
        //    }
        //}
    }
}