using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PixelAimbot.Classes.Misc;
using Telegram.Bot;

namespace PixelAimbot.Classes.AsyncTasks
{
    public static class Messenger
    {
        public static async Task RunBotAsync(string telegramToken, CancellationToken token)
        {
            ChaosBot.telegramBotRunning = true;
            var bot = new TelegramBotClient(telegramToken);
            int offset = -1;
            ChaosBot.botIsRun = true;
            ChaosBot.buttonConnectTelegram.Text = "disconnect";
            while (ChaosBot.botIsRun)
            {
                Telegram.Bot.Types.Update[] updates;

                try
                {
                    updates = await bot.GetUpdatesAsync(offset);
                    ChaosBot.telegramBotRunning = true;
                }
                catch (Exception ex)
                {
                    ChaosBot.botIsRun = false;
                    continue;
                }


                foreach (var update in updates)
                {
                    offset = update.Id + 1;

                    if (update.Message == null)
                    {
                        continue;
                    }

                    if (update.Message.Date < DateTime.Now.AddHours(-2))
                    {
                        continue;
                    }

                    string text = update.Message.Text.ToLower();
                    var chatId = update.Message.Chat.Id;
                    if (text.Contains("/help"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Currently supported Commands")
                            .AppendLine("/start - Starts the Bot")
                            .AppendLine("/stop - Stops the Bot doing anything")
                            .AppendLine("/info - Returns currently runtime and state of Bot")
                            .AppendLine("/inv - Send a screenshot of your inventory")
                            .AppendLine("/screen - Send a Screenshot of Game");

                        await bot.SendTextMessageAsync(chatId, sb.ToString());
                    }

                    if (text.Contains("/start"))
                    {
                        if (ChaosBot._start == false)
                        {
                            ChaosBot.btnStart_Click(null, null);
                            await bot.SendTextMessageAsync(chatId, "Bot started");
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chatId, "Bot already running!");
                        }
                    }

                    if (text.Contains("/stop"))
                    {
                        if (ChaosBot._stop)
                        {
                            ChaosBot.btnPause_Click(null, null);
                            
                            await bot.SendTextMessageAsync(chatId, "Bot stopped!");
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chatId, "Bot isnt running!");
                        }
                    }

                    if (text.Contains("/info"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("State: " + ChaosBot.lbStatus.Text)
                            .AppendLine("Runtime: " + ChaosBot.formMinimized.sw.Elapsed.Hours.ToString("D2") + ":" +
                                        ChaosBot.formMinimized.sw.Elapsed.Minutes.ToString("D2") + ":" +
                                        ChaosBot.formMinimized.sw.Elapsed.Seconds.ToString("D2"));

                        await bot.SendTextMessageAsync(chatId, sb.ToString());
                    }

                    if (text.Contains("/inv"))
                    {
                        KeyboardWrapper.PressKey(KeyboardWrapper.VK_I);
                        await Task.Delay(100);
                        var picture = new PrintScreen();
                        var screen = picture.CaptureScreen();

                        Stream stream =
                            ChaosBot.ToStream(
                                ChaosBot.cropImage(screen,
                                    new Rectangle(FishBot.recalc(1322), PixelAimbot.FishBot.recalc(189, false),
                                        FishBot.recalc(544), FishBot.recalc(640, false))), ImageFormat.Png);
                        await bot.SendPhotoAsync(chatId, stream);
                        KeyboardWrapper.PressKey(KeyboardWrapper.VK_I);
                    }

                    if (text.Contains("/screen"))
                    {
                        var picture = new PrintScreen();
                        Stream stream = ChaosBot.ToStream(picture.CaptureScreen(), ImageFormat.Png);
                        await bot.SendPhotoAsync(chatId, stream);
                    }
                }
            }
        }
    }
}