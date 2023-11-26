using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    partial class DiabloBot
    {

        private async Task FightAgainstEnemys(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    if (_fightAgainstEnemys)
                        foreach (KeyValuePair<byte, int> skill in _skills.skillset.OrderBy(x => x.Value))
                        {
                            token.ThrowIfCancellationRequested();
                            if (_stopped)
                                return;
                            if (_EnemyFound)
                                if (!isKeyOnCooldownGray(skill.Key) && !isKeySet(skill.Key))
                                {
                                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "SKILL Pressed: " + skill.Value));
                                    //   await KeyboardWrapper.AlternateHoldKey(skill.Key, CasttimeByKey(skill.Key));
                                    KeyboardWrapper.PressKey(skill.Key);
                                    if (IsDoubleKey(skill.Key))
                                        KeyboardWrapper.PressKey(skill.Key);

                                    SetKeyCooldownGray(skill.Key); // Set Cooldown

                                    await Task.Delay(50, token);
                                }
                                else
                                {

                                    lbStatus.Invoke(
                                        (MethodInvoker)(() => lbStatus.Text = "Auto Attack"));
                                    if (EnergyLow)
                                        VirtualMouse.LeftClick();
                                    else
                                        VirtualMouse.RightClick();
                                    await Task.Delay(250, token);

                                }
                            countNoTargetFound = 0;


                        }
                    await Task.Delay(50, token);

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