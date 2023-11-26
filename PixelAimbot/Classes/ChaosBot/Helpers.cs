using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json.Linq;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    partial class DiabloBot
    {

        private PrintScreen printScreenPicture = new PrintScreen();
        private void ChangeSkillSet(object sender, EventArgs e)
        {
            if (txPrio1.Text != "" && txPrio2.Text != "" && txPrio3.Text != "" && txPrio4.Text != "")
                _skills.skillset = new Dictionary<byte, int>
                {
                    {KeyboardWrapper.VK_1, int.Parse(txPrio1.Text)},
                    {KeyboardWrapper.VK_2, int.Parse(txPrio2.Text)},
                    {KeyboardWrapper.VK_3, int.Parse(txPrio3.Text)},
                    {KeyboardWrapper.VK_4, int.Parse(txPrio4.Text)}
                }.ToList();
        }

        private void CheckIsDigit(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        public static string ReadArea(Bitmap screenCapture, int x, int y, int width, int height, string whitelist = "")
        {
            /*tess.Configuration.EngineMode = TesseractEngineMode.TesseractAndLstm;
            tess.Language = OcrLanguage.EnglishBest;
            tess.MultiThreaded = true;
            tess.Configuration.ReadBarCodes = false;
            tess.Configuration.RenderSearchablePdfsAndHocr = false;
            tess.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.Auto;
            if (whitelist != "")
            {
                tess.Configuration.WhiteListCharacters = whitelist;
            }


            string result = "";
            try
            {
                using (var input = new OcrInput())
                {
                    var contentArea = new Rectangle() {X = x, Y = y, Height = height, Width = width};
                    
                    input.AddImage(screenCapture, contentArea);
                    result = tess.Read(input).Text;
                    Debug.WriteLine(result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.SendException(ex);
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
*/
            return null;
        }

        public static Bitmap SetGrayscale(Bitmap img)
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var c = img.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

                    img.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return img;

        }

        public static Bitmap RemoveNoise(Bitmap bmap)
        {

            for (var x = 0; x < bmap.Width; x++)
            {
                for (var y = 0; y < bmap.Height; y++)
                {
                    var pixel = bmap.GetPixel(x, y);
                    if (pixel.R < 162 && pixel.G < 162 && pixel.B < 162)
                        bmap.SetPixel(x, y, Color.Black);
                    else if (pixel.R > 162 && pixel.G > 162 && pixel.B > 162)
                        bmap.SetPixel(x, y, Color.White);
                }
            }

            return bmap;
        }
        private static readonly RNGCryptoServiceProvider Generator = new RNGCryptoServiceProvider();

        public static int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            Generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }

        private string translateKey(int key)
        {
            string translate;
            switch (key)
            {
                case 81:
                    translate = "Q";
                    break;

                case 87:
                    translate = "W";
                    break;

                case 69:
                    translate = "E";
                    break;

                case 82:
                    translate = "R";
                    break;

                case 65:
                    translate = "A";
                    break;

                case 83:
                    translate = "S";
                    break;

                case 68:
                    translate = "D";
                    break;

                case 70:
                    translate = "F";
                    break;

                case 89:
                    translate = "Y";
                    break;

                case 90:
                    translate = "Z";
                    break;

                case 0:
                    translate = "LEFT WALK";
                    break;

                case 1:
                    translate = "RIGHT WALK";
                    break;

                case 49:
                    translate = "1";
                    break;
                case 50:
                    translate = "2";
                    break;
                case 51:
                    translate = "3";
                    break;
                case 52:
                    translate = "4";
                    break;

                default:
                    translate = key.ToString();
                    break;
            }

            return translate;
        }

      

        public static (int, int) PixelToAbsolute(double x, double y, Point screenResolution)
        {
            int newX = (int)(x); // / screenResolution.X * 65535);
            int newY = (int)(y); // / screenResolution.Y * 65535);
            return (newX, newY);
        }

        public static (double, double) MinimapToDesktop(double x, double y, int additionalPercent = 20)
        {
            double calculatedPercentX = (x) / 394 * 100;
            double calculatedPercentY = (y) / 340 * 100;
            double multiplierX = 1;
            double multiplierY = 1;
            if (calculatedPercentX > 50)
            {
                calculatedPercentX += additionalPercent;
            }
            else if (calculatedPercentX < 50)
            {
                calculatedPercentX -= additionalPercent;
            }

            if (calculatedPercentY > 50)
            {
                calculatedPercentY += additionalPercent;
            }
            else if (calculatedPercentY < 50)
            {
                calculatedPercentY -= additionalPercent;
            }

            double resultX;
            double resultY;
            if (IsWindowed)
            {
                resultX = 1920 / 100 * (calculatedPercentX * multiplierX);
                resultY = 1080 / 100 * (calculatedPercentY * multiplierY);
            }
            else
            {
                resultX = Screen.PrimaryScreen.Bounds.Width / 100 * (calculatedPercentX * multiplierX);
                resultY = Screen.PrimaryScreen.Bounds.Height / 100 * (calculatedPercentY * multiplierY);
            }


            //  resultX = ((resultX - (Screen.PrimaryScreen.Bounds.Width / 2)) * 0.5) +  
            return (resultX, resultY);
        }

        public static Image<Bgr, Byte> ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Bitmap returnImage = (Bitmap)Image.FromStream(ms);

                return returnImage.ToImage<Bgr, byte>();
            }
            else
                return null;
        }

        public static int Recalc(int value, bool horizontal = true)
        {
            int screenWidth = 1920; // Standardbreite des Codes
            int screenHeight = 1080; // Standardhöhe des Codes

            // Überprüfen der aktuellen Monitorauflösung
            int currentScreenWidth = ScreenWidth;
            int currentScreenHeight = ScreenHeight;

            // Anpassung der Koordinate basierend auf der Monitorauflösung
            if (currentScreenWidth != screenWidth && horizontal)
            {
                value = (value * currentScreenWidth) / screenWidth;
            }
            else if (currentScreenHeight != screenHeight && !horizontal)
            {
                value = (value * currentScreenHeight) / screenHeight;
            }
            else if (currentScreenWidth != screenWidth && !horizontal)
            {
                value = (value * currentScreenWidth) / screenWidth;
            }
            else if (currentScreenHeight != screenHeight && horizontal)
            {
                value = (value * currentScreenHeight) / screenHeight;
            }

            return value;
        }
        //public static int Recalc(int value, bool horizontal = true)
        //{
        //    int returnValue = value;

        //        decimal oldResolution;
        //        decimal newResolution;
        //        if (horizontal)
        //        {
        //            oldResolution = 1920;
        //            newResolution = ScreenWidth;
        //        }
        //        else
        //        {
        //            oldResolution = 1080;
        //            newResolution = ScreenHeight;
        //        }

        //        if (oldResolution != newResolution)
        //        {
        //            decimal normalized = value * newResolution;
        //            decimal rescaledPosition = normalized / oldResolution;

        //            returnValue = Decimal.ToInt32(rescaledPosition);
        //        }


        //    return returnValue;
        //}


        /// ///////////////////     /// ///////////////////

        // MAKE SCREENSHOT FROM ABILITYS
        // 
        public Image<Bgr, byte> skill1;
        public Image<Bgr, byte> skill2;
        public Image<Bgr, byte> skill3;
        public Image<Bgr, byte> skill4;
   
        public Image<Bgr, byte> ImageMap;
        private async Task GetMap()
        {
            try
            {
                ImageMap = MapScreenCapture(ImageFormat.Jpeg, DiabloBot.Recalc(1835), DiabloBot.Recalc(80, false)).ToImage<Bgr, Byte>();
                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Map Screenshot {0} {1}", ex.GetType().Name, ex.Message);
            }
        }
        private Image<Bgr, byte> GetPath(Point point)
        {
            try
            {
                ImageMap = PathScreenCapture(ImageFormat.Jpeg, point.X, point.Y).ToImage<Bgr, Byte>();
                return ImageMap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Map Screenshot {0} {1}", ex.GetType().Name, ex.Message);
            }
            return null;
        }
        public Bitmap MapScreenCapture(ImageFormat imageFormat, int sourceX, int sourceY)
        {
            var screen = printScreenPicture.CaptureScreen();
            return CropImage(screen,
                new Rectangle(sourceX, sourceY,
                    50, 170));
        }
        public Bitmap PathScreenCapture(ImageFormat imageFormat, int sourceX, int sourceY)
        {
            var screen = printScreenPicture.CaptureScreen();
            return CropImage(screen,
                new Rectangle(sourceX - 22, sourceY - 22,
                    45, 45));
        }

        private void GetSkill1()
        {
            try
            {
                if (!_1IsEmpty)
                    skill1 = AbilityScreen(ImageFormat.Jpeg, DiabloBot.Recalc(778), DiabloBot.Recalc(981, false)).ToImage<Bgr, Byte>(); 
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill Q Screenshot {0} {1}", ex.GetType().Name, ex.Message);
            
            }
        }
        private void GetSkill2()
        {
            try
            {
                if (!_2IsEmpty)
                    skill2 = AbilityScreen(ImageFormat.Jpeg, DiabloBot.Recalc(841), DiabloBot.Recalc(981, false)).ToImage<Bgr, Byte>(); 
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill W Screenshot {0} {1}", ex.GetType().Name, ex.Message);
                //return null;
            }
        }
        private void GetSkill3()
        {
            try
            {
                if (!_3IsEmpty)
                    skill3 = AbilityScreen(ImageFormat.Jpeg, DiabloBot.Recalc(904), DiabloBot.Recalc(981, false)).ToImage<Bgr, Byte>(); 
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill E Screenshot {0} {1}", ex.GetType().Name, ex.Message);
            }
        }
        private void GetSkill4()
        {
            try
            {
                if (!_4IsEmpty)
                    skill4 = AbilityScreen(ImageFormat.Jpeg, DiabloBot.Recalc(967), DiabloBot.Recalc(981, false)).ToImage<Bgr, Byte>();
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill R Screenshot {0} {1}", ex.GetType().Name, ex.Message);
            }
        }


        // COMPARE SCREENSHOTS WITH OPENCV
        //

        public async Task CheckForEmptySkill(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var template = ImageEmptySkillBar;

                var detector1 = new ScreenDetector(template, null, 0.6f, DiabloBot.Recalc(774),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                var detector2 = new ScreenDetector(template, null, 0.6f, DiabloBot.Recalc(838),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                var detector3 = new ScreenDetector(template, null, 0.6f, DiabloBot.Recalc(901),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                var detector4 = new ScreenDetector(template, null, 0.6f, DiabloBot.Recalc(964),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                {
                    var item1 = detector1.CompareImage(screenCapture);
                    if (item1 != null)
                    {
                        VirtualMouse.MoveTo(item1.Value.X + 27, item1.Value.Y + 27, 5);
                        _1IsEmpty = true;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 1 unused."));
                    }
                    else
                    {
                        _1IsEmpty = false;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 1 is setted."));
                    }
                }
                await Task.Delay(100);
                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                {
                    var item2 = detector2.CompareImage(screenCapture);
                    if (item2 != null)
                    {
                        VirtualMouse.MoveTo(item2.Value.X + 27, item2.Value.Y + 27, 5);
                        _2IsEmpty = true;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 2 unused."));
                    }
                    else
                    {
                        _2IsEmpty = false;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 2 is setted."));
                    }
                }
                await Task.Delay(100);
                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                {
                    var item3 = detector3.CompareImage(screenCapture);
                    if (item3 != null)
                    {
                        VirtualMouse.MoveTo(item3.Value.X + 27, item3.Value.Y + 27, 5);
                        _3IsEmpty = true;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 3 unused."));
                    }
                    else
                    {
                        _3IsEmpty = false;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 3 is setted."));
                    }
                }
                await Task.Delay(100);
                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                {
                    var item4 = detector4.CompareImage(screenCapture);
                    if (item4 != null)
                    {
                        VirtualMouse.MoveTo(item4.Value.X + 27, item4.Value.Y + 27, 5);
                        _4IsEmpty = true;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 4 unused."));
                    }
                    else
                    {
                        _4IsEmpty = false;
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Ability 4 is setted."));
                    }
                }
                await Task.Delay(100);

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

        public async Task Skill1(CancellationToken tokenSkills)
        {
            try
            {
                tokenSkills.ThrowIfCancellationRequested();
                var template = skill1;
                var detector = new ScreenDetector(template, null, 0.9f, DiabloBot.Recalc(774),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                while (_1)
                {
                    tokenSkills.ThrowIfCancellationRequested();
                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        var item = detector.CompareImage(screenCapture);
                        if (item != null)
                        {
                            _1 = false;
                            Debug.WriteLine("1 ist wieder Aktiv");
                        }
                    }
                    await Task.Delay(100);
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
        public async Task Skill2(CancellationToken tokenSkills)
        {
            try
            {
                tokenSkills.ThrowIfCancellationRequested();
                var template = skill2;
                var detector = new ScreenDetector(template, null, 0.9f, DiabloBot.Recalc(838),
                        DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                while (_2)
                {
                    tokenSkills.ThrowIfCancellationRequested();
                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        var item = detector.CompareImage(screenCapture);
                        if (item != null)
                        {
                            _2 = false;
                            Debug.WriteLine("2 ist wieder Aktiv");
                        }
                    }
                    await Task.Delay(100);
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
        public async Task Skill3(CancellationToken tokenSkills)
        {
            try
            {
                tokenSkills.ThrowIfCancellationRequested();
                var template = skill3;
                var detector = new ScreenDetector(template, null, 0.9f, DiabloBot.Recalc(901),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                while (_3)
                {
                    tokenSkills.ThrowIfCancellationRequested();
                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        var item = detector.CompareImage(screenCapture);
                        if (item != null)
                        {
                            _3 = false;
                            Debug.WriteLine("3 ist wieder Aktiv");
                        }
                    }
                    await Task.Delay(100);
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
        public async Task Skill4(CancellationToken tokenSkills)
        {
            try
            {
                tokenSkills.ThrowIfCancellationRequested();
                var template = skill4;
                var detector = new ScreenDetector(template, null, 0.9f, DiabloBot.Recalc(964),
                    DiabloBot.Recalc(978, false), DiabloBot.Recalc(57), DiabloBot.Recalc(57, false));
                while (_4)
                {
                    tokenSkills.ThrowIfCancellationRequested();
                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        var item = detector.CompareImage(screenCapture);
                        if (item != null)
                        {
                            _4 = false;
                            Debug.WriteLine("4 ist wieder Aktiv");
                        }
                    }
                    await Task.Delay(100);
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

        private void SetKeyCooldownGray(byte key)
        {
            switch (key)
            {
                case KeyboardWrapper.VK_1:
                    _1 = true;
                    Debug.WriteLine("1: PRESSED");
                    Task.Run(() => Skill1(_tokenSkills), _tokenSkills);
                    break;
                case KeyboardWrapper.VK_2:
                    _2 = true;
                    Debug.WriteLine("2: PRESSED");
                    Task.Run(() => Skill2(_tokenSkills), _tokenSkills);
                    break;
                case KeyboardWrapper.VK_3:
                    _3 = true;
                    Debug.WriteLine("3: PRESSED");
                    Task.Run(() => Skill3(_tokenSkills), _tokenSkills);
                    break;
                case KeyboardWrapper.VK_4:
                    _4 = true;
                    Debug.WriteLine("4: PRESSED");
                    Task.Run(() => Skill4(_tokenSkills), _tokenSkills);
                    break;
            }
        }
        
        private bool isKeySet(byte key)
        {
            var returnBoolean = false;
            switch (key)
            {
                case KeyboardWrapper.VK_1:
                    returnBoolean = _1IsEmpty;
                    break;

                case KeyboardWrapper.VK_2:
                    returnBoolean = _2IsEmpty;
                    break;

                case KeyboardWrapper.VK_3:
                    returnBoolean = _3IsEmpty;
                    break;

                case KeyboardWrapper.VK_4:
                    returnBoolean = _4IsEmpty;
                    break;
            }

            return returnBoolean;
        }
        private bool isKeyOnCooldownGray(byte key)
        {
            var returnBoolean = false;
            switch (key)
            {
                case KeyboardWrapper.VK_1:
                    returnBoolean = _1;
                    break;

                case KeyboardWrapper.VK_2:
                    returnBoolean = _2;
                    break;

                case KeyboardWrapper.VK_3:
                    returnBoolean = _3;
                    break;

                case KeyboardWrapper.VK_4:
                    returnBoolean = _4;
                    break;
            }

            return returnBoolean;
        }

        /// ///////////////////     /// ///////////////////

        private bool isKeyOnCooldown(byte key)
        {
            var returnBoolean = false;
            switch (key)
            {

                case KeyboardWrapper.VK_1:
                    returnBoolean = _1;
                    break;

                case KeyboardWrapper.VK_2:
                    returnBoolean = _2;
                    break;

                case KeyboardWrapper.VK_3:
                    returnBoolean = _3;
                    break;

                case KeyboardWrapper.VK_4:
                    returnBoolean = _4;
                    break;
            }

            return returnBoolean;
        }

        private Point calculateFromCenter(int x, int y, int RecX = 500, int RecY = 390)
        {
            var centerX = ScreenWidth / 2;
            var centerY = ScreenHeight / 2;

            var resultX = centerX - Recalc(RecX) + x;
            var resultY = centerY - Recalc(RecY, false) + y;

            return new Point(resultX, resultY);
        }

        private bool IsDoubleKey(byte key)
        {
            var checkboxState = false;
            switch (key)
            {

                case KeyboardWrapper.VK_1:
                    checkboxState = chBoxDouble1.Checked;
                    break;

                case KeyboardWrapper.VK_2:
                    checkboxState = chBoxDouble2.Checked;
                    break;

                case KeyboardWrapper.VK_3:
                    checkboxState = chBoxDouble3.Checked;
                    break;

                case KeyboardWrapper.VK_4:
                    checkboxState = chBoxDouble4.Checked;
                    break;
            }

            return checkboxState;
        }

        private static Bitmap CropImage(Image img, Rectangle cropArea)
        {
            using (var bmpImage = new Bitmap(img))
            {
                return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }
        }


        private void RefreshRotationCombox()
        {
            comboBoxRotations.Items.Clear();

            //var config = Config.Load();

            var url = $"https://about-steven.de/symbioticD4GetSaves/{LoginData.username}";
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            var webClient = new WebClient();
            webClient.CachePolicy = noCachePolicy;

            webClient.DownloadStringAsync(new Uri(url));
            webClient.DownloadStringCompleted += (s, e) =>
            {
                try
                {

                    foreach (var entries in JArray.Parse(e.Result))
                    {
                        comboBoxRotations.Items.Add(entries["filename"] ?? "Unknown Name");
                        //   File.WriteAllText(Directory.GetCurrentDirectory() + @"\" + HWID.Get() + @"\" + entries["savename"] + ".ini", entries.ToString());

                    }
                    //var files = Directory.GetFiles(ConfigPath);
                    //foreach (var file in files)
                    //    if (Path.GetFileNameWithoutExtension(file) != "main")
                    //        comboBoxRotations.Items.Add(Path.GetFileNameWithoutExtension(file));


                }
                catch (Exception ex)
                {
                    //   ExceptionHandler.SendException(ex);
                    Alert.Show("Webserver currently not Available!\n" +
                                "Try Again later.", FrmAlert.EnmType.Error);
                }
            };
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }


        public Bitmap AbilityScreen(ImageFormat imageFormat, int sourceX, int sourceY)
        {
            var screen = printScreenPicture.CaptureScreen();
            return CropImage(screen,
                new Rectangle(sourceX, sourceY,
                    47, 47));
        }


        private static void CheckIfLoadScreen()
        {
            bool _ChaosStartDetect = true;

            while (_ChaosStartDetect == true)
            {
                try
                {
                    object StartDetect = Pixel.PixelSearch(Recalc(1898), Recalc(10, false), Recalc(1911),
                        Recalc(22, false), 0x000000, 15);

                    if (StartDetect.ToString() == "0")
                    {
                        _ChaosStartDetect = false;
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
}