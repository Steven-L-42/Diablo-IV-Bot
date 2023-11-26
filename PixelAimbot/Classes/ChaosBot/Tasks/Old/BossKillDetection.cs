using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        public bool starten;
        public bool awakening;
        public bool gefunden;
        public bool _bossKillDetection;
        public bool SearchRedBoss;
        public bool FindRedBoss;
        public bool Floor3BossGesichtet;
        //private async Task BossKillDetection(CancellationToken BossKill)
        //{

        //    _token.ThrowIfCancellationRequested();
        //    BossKill.ThrowIfCancellationRequested();

        //    try
        //    {
        //        int FoundBossAndLeave = 0;
        //        gefunden = false;
        //        //if (_floor3)
        //        //{
        //        //    FoundBossAndLeave = 10;
        //        //    SearchRedBoss = true;
        //        //    Floor3BossGesichtet = false;


        //        //}

        //        _token.ThrowIfCancellationRequested();
        //        BossKill.ThrowIfCancellationRequested();
        //        while (starten == true)
        //        {

        //            try
        //            {
        //                _token.ThrowIfCancellationRequested();

        //                BossKill.ThrowIfCancellationRequested();
        //                float threshold = 0.8f;
        //                if (ScreenWidth > 1920)
        //                {
        //                    threshold = 0.77f;    
        //                }
                        
        //                //var BossTemplate = ImageBossHp;
                        

        //                //var BossDetector = new BossDetector(BossTemplate, null, threshold);

        //                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
        //                {
        //                    var Boss = BossDetector.GetClosestEnemy(screenCapture, false);

        //                    //if (!Boss.HasValue && _floor3 == true && SearchRedBoss == true)
        //                    //{

        //                    //    await Task.Delay(3000);
        //                    //    SearchRedBoss = false;

        //                    //}
        //                    if (Boss.HasValue && gefunden == false)
        //                    {
        //                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "BOSS FIGHT!"));
        //                        Floor3BossGesichtet = true;

        //                        while (!gefunden && chBoxAwakening.Checked)
        //                        {
        //                            try
        //                            {
        //                                _token.ThrowIfCancellationRequested();

        //                                BossKill.ThrowIfCancellationRequested();
        //                                object Awakening = Pixel.PixelSearch(DiabloBot.Recalc(1161),
        //                                    DiabloBot.Recalc(66, false), DiabloBot.Recalc(1187),
        //                                    DiabloBot.Recalc(83, false), 0x9C1B16, 50);
        //                                if (Awakening.ToString() == "0")
        //                                {
        //                                    _doUltimateAttack = true;
        //                                    _searchboss = false;
        //                                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "AWAKENING..."));

        //                                    #region Awakening

        //                                    for (var i = 0; i < 50; i++)
        //                                    {
        //                                        KeyboardWrapper.PressKey(KeyboardWrapper.VK_V);
        //                                        await Task.Delay(10);
        //                                    }

        //                                    #endregion

        //                                    await Task.Delay(500);
        //                                    _doUltimateAttack = false;
        //                                    _searchboss = true;
        //                                    if (FoundBossAndLeave == 0)
        //                                    {
        //                                        FoundBossAndLeave++;
        //                                        gefunden = true;
        //                                    }
        //                                    if (_floor3)
        //                                    {
        //                                        FoundBossAndLeave = 15;
        //                                        gefunden = true;
        //                                        SearchRedBoss = false;
        //                                    }

        //                                }

        //                            }
        //                            catch (AggregateException)
        //                            {
        //                                Console.WriteLine("Expected");
        //                            }
        //                            catch (ObjectDisposedException)
        //                            {
        //                                Console.WriteLine("Bug");
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                              //   ExceptionHandler.SendException(ex);
        //                                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
        //                                Debug.WriteLine("[" + line + "]" + ex.Message);
        //                            }

        //                            Random random2 = new Random();
        //                            var sleepTime2 = random2.Next(100, 150);
        //                            await Task.Delay(sleepTime2);


        //                        }

        //                        if (FoundBossAndLeave == 0)
        //                        {
        //                            FoundBossAndLeave++;
        //                            gefunden = true;
        //                        }
        //                        if (_floor3)
        //                        {
        //                            FoundBossAndLeave = 15;
        //                            gefunden = true;
        //                            SearchRedBoss = false;
        //                        }


        //                    }
        //                    else if (!Boss.HasValue && gefunden == true)
        //                    {
        //                        _token.ThrowIfCancellationRequested();
        //                        BossKill.ThrowIfCancellationRequested();

        //                        if (_redStage == 0)
        //                        {

        //                            _token.ThrowIfCancellationRequested();
        //                            BossKill.ThrowIfCancellationRequested();
        //                            FoundBossAndLeave = 2;
        //                            _redStage++;

        //                            await Task.Delay(1, BossKill);


        //                            _searchboss = false;
        //                            _potions = false;
        //                            //  _revive = false;
        //                            _ultimate = false;
        //                            _floor1 = false;
        //                            _floor2 = false;
        //                            _floorFight = false;

        //                            _stopped = false;

        //                            _portalIsDetected = true;
        //                            _portalIsNotDetected = false;
                                  
        //                            //var t7 = Task.Run(() => SEARCHPORTAL(_token), BossKill);
        //                            //await Task.WhenAny( t7);
        //                        }
        //                        else
        //                        if (FoundBossAndLeave == 1 || FoundBossAndLeave == 10 || FoundBossAndLeave == 15)
        //                        {
        //                            _token.ThrowIfCancellationRequested();

        //                            BossKill.ThrowIfCancellationRequested();
        //                            gefunden = false;
        //                            await Task.Delay(_humanizer.Next(10, 240) + 3000, BossKill);
        //                            _token.ThrowIfCancellationRequested();

        //                            BossKill.ThrowIfCancellationRequested();
        //                            if (FoundBossAndLeave == 15)
        //                            {
        //                                ChaosRedStages++;
        //                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "RedStage Complete..."));
        //                            }
        //                            else if (FoundBossAndLeave == 10)
        //                            {
        //                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "RedStage not found!"));
        //                            }
        //                            else
        //                            {
        //                                lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Floor2 Complete..."));
        //                            }

        //                            starten = false;

        //                            _stopped = true;
        //                            _portalIsDetected = false;

        //                            _portalIsNotDetected = false;
        //                            _floorFight = false;
        //                            _searchboss = false;
        //                            _revive = false;
        //                            _ultimate = false;
        //                            _portaldetect = false;
        //                            _potions = false;
        //                            _floor1 = false;
        //                            _floor2 = false;
        //                            _floor3 = false;

        //                            _bard = false;
        //                            _gunlancer = false;
        //                            _shadowhunter = false;
        //                            _paladin = false;
        //                            _glavier = false;
        //                            _deathblade = false;
        //                            _destroyer = false;
        //                            _sharpshooter = false;
        //                            _sorcerer = false;
        //                            _soulfist = false;
        //                            _sharpshooter = false;
        //                            _berserker = false;

        //                            _doUltimateAttack = true;
        //                            _q = true;
        //                            _w = true;
        //                            _e = true;
        //                            _r = true;
        //                            _a = true;
        //                            _s = true;
        //                            _d = true;
        //                            _f = true;
        //                            _token.ThrowIfCancellationRequested();

        //                            BossKill.ThrowIfCancellationRequested();
        //                            //var leave = Task.Run(() => Leavedungeon(_token), BossKill);
        //                            //await Task.WhenAny(new[] { leave });
        //                        }
        //                    }
        //                }




        //            }
        //            catch (AggregateException)
        //            {
        //                Console.WriteLine("Expected");
        //            }
        //            catch (ObjectDisposedException)
        //            {
        //                Console.WriteLine("Bug");
        //            }
        //            catch (Exception ex)
        //            {
        //              //   ExceptionHandler.SendException(ex);
        //                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
        //                Debug.WriteLine("[" + line + "]" + ex.Message);
        //            }
        //            Random random = new Random();
        //            var sleepTime = random.Next(100, 150);
        //            await Task.Delay(sleepTime);
        //        }
        //    }
        //    catch (AggregateException)
        //    {
        //        Console.WriteLine("Expected");
        //    }
        //    catch (ObjectDisposedException)
        //    {
        //        Console.WriteLine("Bug");
        //    }
        //    catch (Exception ex)
        //    {
        //      //   ExceptionHandler.SendException(ex);
        //        int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
        //        Debug.WriteLine("[" + line + "]" + ex.Message);
        //    }

        //}

    }
}