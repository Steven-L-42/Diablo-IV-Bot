using PixelAimbot.Classes.Misc;
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
        
        private async Task PlayerEnergy(CancellationToken token)
        {

            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);

                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    if (_EnemyFound && !playerDead)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1, token);
                        Color color = ColorTranslator.FromHtml("#121116");
                        Point cursor = ColorSearch.SearchAndMove(color, 5, Recalc(1000), Recalc(1030), Recalc(1300, false), Recalc(1330, false), true, false, false, false);

                        if (cursor.X != 0 && cursor.Y != 0)
                        {
                            EnergyLow = true;
                            VirtualMouse.LeftClick();
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Energy Low..."));
                        }
                        else
                        {
                            EnergyLow = false;
                        }
                    }



                    Random random = new Random();
                    var sleepTime = random.Next(250, 500);
                    await Task.Delay(sleepTime);
                }
            }
            catch (AggregateException)
            {
                Console.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Bug");
            }
            catch (Exception ex)
            {

                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}