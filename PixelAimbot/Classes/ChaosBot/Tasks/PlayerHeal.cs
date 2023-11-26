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
        double sliderPercent;
        public void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (sldHealth.Value < 966)
                    sldHealth.Value = 966;
                Debug.WriteLine(sldHealth.Value);
                HealthPercent = (1045 - sldHealth.Value) + 940;
                double distanceFromMin = (sldHealth.Value - sldHealth.Minimum);
                double sliderRange = (sldHealth.Maximum - sldHealth.Minimum);
                sliderPercent = 100 * (distanceFromMin / sliderRange);
                labelheal.Text = "Heal at: " + Convert.ToInt32(sliderPercent) + "% Life";
                Debug.WriteLine("HealthPercent:" + HealthPercent);
                Debug.WriteLine("FOUND with %:" + sliderPercent);
            }catch (Exception ex) { }
        }
        private async Task PlayerHeal(CancellationToken token)
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
                    
                    if (!playerDead && !_RepairProcess)
                    {
                        
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1, token);
                        Color color;

                        if (sliderPercent >= 75)
                            color = ColorTranslator.FromHtml("#0A0E12");
                        else if (sliderPercent >= 50)
                            color = ColorTranslator.FromHtml("#161516");
                        else
                            color = ColorTranslator.FromHtml("#242B2C");
                        Point cursor = ColorSearch.SearchAndMove(color, 5, Recalc(HealthPercent - 10), Recalc(HealthPercent), Recalc(600, false), Recalc(620, false), true, false, false, false);
                       
                        if (cursor.X != 0 && cursor.Y != 0)
                        {
                            Debug.WriteLine("HealthPercent:" + HealthPercent);
                            Debug.WriteLine("FOUND with %:" + sliderPercent);
                            KeyboardWrapper.PressKey(_currentHealKey);
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Drink healing potion..."));
                        }
                    }


                    Random random = new Random();
                    var sleepTime = random.Next(500, 750);
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
                //   ExceptionHandler.SendException(ex);
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}