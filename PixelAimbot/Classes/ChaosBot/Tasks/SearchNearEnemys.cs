using Emgu.CV.Structure;
using Emgu.CV;
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
        private async Task SearchNearEnemys(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1, token);
                
                float treshold = 0.75f;
                TestDetectors detector;
                Image<Bgr, byte> template;
                int correction = Recalc(70, false);
                playerFight = true;
                while (_stopped == false)
                {

                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                    Point item = Point.Empty;
                    for (int i = 0; i < 2; i++)
                    {


                        switch (i)
                        {
                            case 0:
                                template = ImageEnemyHealth1;
                                detector = new TestDetectors(template, null, treshold,
                                    DiabloBot.Recalc(460),
                                    DiabloBot.Recalc(120, false),
                                    DiabloBot.Recalc(1000),
                                    DiabloBot.Recalc(780, false));
                                break;
                            case 1:
                                template = ImageEnemyHealth2;
                                detector = new TestDetectors(template, null, treshold,
                                     DiabloBot.Recalc(460),
                                     DiabloBot.Recalc(120, false),
                                     DiabloBot.Recalc(1000),
                                     DiabloBot.Recalc(780, false));
                                break;
                            default:
                                continue;
                        }

                        token.ThrowIfCancellationRequested();
                        if (!playerDead && playerFight)
                        {
                            using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                            {
                                item = detector.ClickIfFound(screenCapture, false, true);
                            }

                            if (item.X > 0 && item.Y > 0)
                            {
                                _EnemyFound = true;
                                counterSearchForEnemy = 0;
                                VirtualMouse.MoveTo(item.X + 35, item.Y + correction, 5);
                                VirtualMouse.LeftClick();
                                _fightAgainstEnemys = true;
                                break;
                            }
                            else
                            {
                                if (counterSearchForEnemy <= 5)
                                    counterSearchForEnemy++;
                                _fightAgainstEnemys = false;
                                _EnemyFound = false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
              
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
    }
}