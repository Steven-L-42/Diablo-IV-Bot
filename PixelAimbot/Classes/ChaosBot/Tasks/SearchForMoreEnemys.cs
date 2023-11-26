using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Microsoft.IO;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        public async Task SearchForMoreEnemys(CancellationToken token)
        {
            while (_stopped == false)
            {
                try
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    if (processName.Length != 1 && _gameCrashed)
                        return;

                    if (!_EnemyFound && hadEnemy)
                    {
                        _LookForEnemy = true;

                        Point center = new Point(960, 420);

                        int radius = 75;
                        int stepSize = 6;

                        VirtualMouse.MoveTo(center.X + radius, center.Y, 5);


                        for (int i = 0; i < 360; i += stepSize)
                        {
                            int x = (int)(center.X + radius * Math.Cos(i * Math.PI / 180));
                            int y = (int)(center.Y + radius * Math.Sin(i * Math.PI / 180));

                            await Task.Delay(0, token);
                            if (_EnemyFound)
                            {
                               
                                lbStatus.Invoke(
                                                (MethodInvoker)(() => lbStatus.Text = "Enemy found..."));
                                hadEnemy = false;
                                _LookForEnemy = false;
                                break;
                            }
                            else
                                VirtualMouse.MoveTo(x, y, 0);
                        }

                        if (!_EnemyFound && hadEnemy)
                        {
                            radius = 175;
                            VirtualMouse.MoveTo(center.X + radius, center.Y, 5);
                            for (int i = 0; i < 360; i += stepSize)
                            {
                                int x = (int)(center.X + radius * Math.Cos(i * Math.PI / 180));
                                int y = (int)(center.Y + radius * Math.Sin(i * Math.PI / 180));

                                await Task.Delay(0, token);
                                if (_EnemyFound)
                                {
                                    lbStatus.Invoke(
                                                (MethodInvoker)(() => lbStatus.Text = "Enemy found..."));
                                    hadEnemy = false;
                                    _LookForEnemy = false;
                                    break;
                                }
                                else
                                    VirtualMouse.MoveTo(x, y, 0);
                            }
                            _FightEnds = true;
                        }
                        countNoTargetFound = 0;
                        hadEnemy = false;
                        _LookForEnemy = false;
                    }
                }
                catch (Exception ex)
                {

                    int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                    Debug.WriteLine("[" + line + "]" + ex.Message);
                }
                await Task.Delay(1, token);
            }
        }
    }
}