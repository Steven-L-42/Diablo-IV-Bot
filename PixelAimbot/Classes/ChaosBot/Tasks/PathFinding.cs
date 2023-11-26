using Emgu.CV;
using Emgu.CV.Structure;
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

        private CancellationTokenSource _SelectNextPathToken;
        int countBeforeUnstuck = 0;
        bool _GetMap = true;
        private async Task GetStuckedMap(CancellationToken token)
        {
            try
            {
                while (true)
                {

                    token.ThrowIfCancellationRequested();
                    if (!_EnemyFound)
                    {
                        //if (_Stucked)
                        //{
                        //    await Task.Delay(4000, token);
                        //    await GetMap();
                        //    Debug.WriteLine("SCREENSHOT IN STUCKED");
                        //    await Task.Delay(4000, token);
                        //}
                        //else
                        //{
                            await GetMap();
                            Debug.WriteLine("SCREENSHOT");
                            await Task.Delay(6000, token);
                        if (!_Stucked)
                        {
                            token.ThrowIfCancellationRequested();
                            _GetMap = false;
                            Task.Run(() => CheckIfStuckOnPath(token), token);
                            while (!_GetMap)
                                await Task.Delay(100, token);
                            while (_Stucked)
                                await Task.Delay(100, token);
                            Debug.WriteLine("DONE");
                        }
                    }
                  
                    await Task.Delay(100, token);
                }
            }
            catch (AggregateException)
            {
                Debug.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Bug");
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        int countTargets = 0;

        public async Task MoveToRelativePosition(CancellationToken token, int degree)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int xPos, yPos;

            int radius = 300;
            // TO DOWN
            if (degree >= 337 || degree < 23)
            {
                xPos = screenWidth / 2;
                yPos = screenHeight / 2 + radius;
            }
            // TO UP
            else if (degree >= 158 && degree < 203)
            {
                xPos = screenWidth / 2;
                yPos = screenHeight / 2 - radius;
            }
            // TO RIGHT
            else if (degree >= 158 && degree < 203)
            {
                xPos = screenWidth / 2 - radius;
                yPos = screenHeight / 2;
            }
            // TO LEFT
            else if (degree >= 248 && degree < 293)
            {
                xPos = screenWidth / 2 + radius;
                yPos = screenHeight / 2;
            }
            // TO Upper Right
            else if (degree >= 203 && degree < 248)
            {
                xPos = screenWidth / 2 + radius;
                yPos = screenHeight / 2 - radius;
            } 
            // TO Upper Left
            else if (degree >= 113 && degree < 158)
            {
                xPos = screenWidth / 2 - radius;
                yPos = screenHeight / 2 - radius;
            }
            // TO Bottom Right
            else if (degree >= 293 && degree < 337)
            {
                xPos = screenWidth / 2 + radius;
                yPos = screenHeight / 2 + radius - 70;
            }
            // TO Bottom Left
            else if (degree >= 23 && degree < 68)
            {
                xPos = screenWidth / 2 - radius;
                yPos = screenHeight / 2 + radius - 70;
            }
            else
            {
                double angle = degree * Math.PI / 180;
                double oppositeAngle = ((degree + 180) % 360) * Math.PI / 180;

                xPos = (int)(screenWidth / 2 + radius * Math.Cos(oppositeAngle));
                yPos = (int)(screenHeight / 2 - radius * Math.Sin(oppositeAngle)); 
            }
            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Unstuck to " + GetDirectionFromDegree(degree) + "!"));
            await Task.Run(() => GoToDirection(token, degree, xPos, yPos, radius), token);
        }
        public async Task GoToDirection(CancellationToken token, int degree, int xPos, int yPos, int radius)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                VirtualMouse.MoveTo(xPos, yPos + Recalc(70, false), 5);
                await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 1500);
                token.ThrowIfCancellationRequested();
                switch (degree)
                {
                    case var d when (d >= 337 || d < 23):
                        //  direction = "Bottom";
                        VirtualMouse.MoveTo(xPos + radius, yPos - Recalc(200, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 23 && d < 68):
                        //  direction = "Bottom Left";
                        VirtualMouse.MoveTo(xPos + radius, yPos + Recalc(130, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 78 && d < 113):
                        //  direction = "Left";
                        VirtualMouse.MoveTo(xPos + radius, yPos - Recalc(200, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 113 && d < 158):
                        //  direction = "Upper Left";
                        VirtualMouse.MoveTo(xPos + radius, yPos - Recalc(200, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 158 && d < 203):
                        //  direction = "Up";
                        VirtualMouse.MoveTo(xPos - radius, yPos + Recalc(130, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 203 && d < 248):
                        //  direction = "Upper Right";
                        VirtualMouse.MoveTo(xPos - radius, yPos - Recalc(200, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 248 && d < 293):
                        //  direction = "Right";
                        VirtualMouse.MoveTo(xPos - radius, yPos + Recalc(130, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    case var d when (d >= 293 && d < 337):
                        //   direction = "Bottom Right";
                        VirtualMouse.MoveTo(xPos - radius, yPos + Recalc(130, false), 5);
                        await KeyboardWrapper.AlternateHoldKey(KeyboardWrapper.VK_LBUTTON, 500);
                        break;
                    default:
                        //   direction = "unknown";
                        break;
                }
            }
            catch (AggregateException)
            {
                Debug.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Bug");
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        public string GetDirectionFromDegree(int degree)
        {
            string direction;

            switch (degree)
            {
                case var d when (d >= 337 || d < 23):
                    direction = "Bottom";
                    break;
                case var d when (d >= 23 && d < 68):
                    direction = "Bottom Left";
                    break;
                case var d when (d >= 78 && d < 113):
                    direction = "Left";
                    break;
                case var d when (d >= 113 && d < 158):
                    direction = "Upper Left";
                    break;
                case var d when (d >= 158 && d < 203):
                    direction = "Up";
                    break;
                case var d when (d >= 203 && d < 248):
                    direction = "Top Right";
                    break;
                case var d when (d >= 248 && d < 293):
                    direction = "Right";
                    break;
                case var d when (d >= 293 && d < 337):
                    direction = "Bottom Right";
                    break;
                default:
                    direction = "unknown";
                    break;
            }

            return direction;
        }

        private async Task PathFinding(CancellationToken token)
        {
            try
            {
                countTargets = 0;
                KeyboardWrapper.holdKeyDown = false;
                var correction = Recalc(70, false);
                if (chBoxPathGenerator.Checked && Paths[0] != null)
                {
                    _PathSelected = false;
                    _SelectNextPathToken = new CancellationTokenSource();
                    var NextPathToken = _SelectNextPathToken.Token;
                    await Task.Run(() => SelectNextPath(Paths[countTargets], 1, NextPathToken), token);
                    while (!_PathSelected)
                        await Task.Delay(10);
                }
                if(chBoxAutoUnstuck.Checked)
                    Task.Run(() => GetStuckedMap(token), token);
                countBeforeUnstuck = 0;
                while (_stopped == false)
                {
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length != 1 && !_gameCrashed)
                        await GameCrashed();
                    else if (processName.Length != 1 && _gameCrashed)
                        return;

                        if (!chBoxAutoRepair.Checked)
                        _RepairProcess = false;
                    if (!playerDead && !_RepairProcess)
                    {
                        countBeforeUnstuck++;
                        token.ThrowIfCancellationRequested();

                        //Color color = ColorTranslator.FromHtml("#5B1207");
                        //Color color = ColorTranslator.FromHtml("#622A1F");
                        //Color color = ColorTranslator.FromHtml("#5E2218");
                        Color color = ColorTranslator.FromHtml("#643A2F");


                        Point cursor = ColorSearch.SearchAndMove(color, 7, Recalc(100, false), Recalc(210, false), Recalc(1650), Recalc(1838), true, false, false, false);
                        Point myPositionOnWorld = new Point(Recalc(960), Recalc(520, false));


                        if (!chBoxAutoFight.Checked)
                            counterSearchForEnemy = 4;

                        if (cursor.X != 0 && cursor.Y != 0 && !_EnemyFound && !_LookForEnemy && !hadEnemy && !_LookForPotion && counterSearchForEnemy > 3)
                        {

                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Follow path..."));
                            Point screenResolution = new Point(ScreenWidth, ScreenHeight);

                            int minX = Recalc(333);
                            int maxX = Recalc(1587);
                            int minY = Recalc(136, false);
                            int maxY = Recalc(803, false);
                            double distance_x = (ScreenWidth - Recalc(myPositionOnWorld.X)) / 2;
                            double distance_y = (ScreenHeight - Recalc(myPositionOnWorld.Y, false)) / 2;

                            var enemy_position_on_minimap = ((cursor.X), (cursor.Y));
                            var my_position_on_minimap = (Recalc(1745), Recalc(160, false));
                            double multiplier = 5.5;

                            double deltaX = enemy_position_on_minimap.Item1 - my_position_on_minimap.Item1;
                            double deltaY = enemy_position_on_minimap.Item2 - my_position_on_minimap.Item2;
                            double angleInRadians = Math.Atan2(deltaY, deltaX);
                            double angleInDegrees = angleInRadians * 180 / Math.PI;
                            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                            double distanceInWorld = distance * multiplier;

                            double posx = myPositionOnWorld.X + Math.Cos(angleInRadians) * distanceInWorld;
                            double posy = myPositionOnWorld.Y + Math.Sin(angleInRadians) * distanceInWorld;

                            // Begrenze posx auf den Bereich von minX bis maxX
                            posx = Math.Max(Math.Min(posx, maxX), minX);

                            // Begrenze posy auf den Bereich von minY bis maxY
                            posy = Math.Max(Math.Min(posy, maxY), minY);

                            var absolutePositions = PixelToAbsolute(posx, posy, screenResolution);

                            token.ThrowIfCancellationRequested();
                            if (!_LookForEnemy && !hadEnemy && !_LookForPotion && _Stucked)
                            {
                                KeyboardWrapper.holdKeyDown = false;
                               
                                int degree = await Task.Run(() => FindPlayerOnMap(token), token);
                                if (degree > -1)
                                    Debug.WriteLine(degree);
                                    await Task.Run(() => MoveToRelativePosition(token, degree), token);
                                _Stucked = false;
                                _CheckIfStuckOnPath = false;
                            }
                            else
                            {
                                if (absolutePositions.Item2 >= ScreenHeight / 2)
                                VirtualMouse.MoveTo(absolutePositions.Item1, absolutePositions.Item2 - correction, 5);
                                else
                                    VirtualMouse.MoveTo(absolutePositions.Item1, absolutePositions.Item2 + correction, 5);

                                if (!KeyboardWrapper.holdKeyDown)
                                {
                                    KeyboardWrapper.holdKeyDown = true;
                                    KeyboardWrapper.HoldKeyBool(KeyboardWrapper.VK_LBUTTON);
                                }
                            }
                            countNoTargetFound = 0;
                        }
                        else
                        {
                            KeyboardWrapper.holdKeyDown = false;
                            if (countNoTargetFound < 25 && !_EnemyFound && !hadEnemy)
                                countNoTargetFound++;
                            //Debug.WriteLine(countNoTargetFound);
                            if (!_EnemyFound && !_LookForEnemy && !hadEnemy && !_LookForPotion && countNoTargetFound >= 25)
                            {
                                lbStatus.Invoke(
                                                      (MethodInvoker)(() => lbStatus.Text = "Target reached!"));
                                //VirtualMouse.MoveTo(myPositionOnWorld.X, myPositionOnWorld.Y, 5);

                                if (countTargets < 13 && !_EnemyFound && chBoxPathGenerator.Checked)
                                {
                                    _Stucked = false;
                                    playerFight = false;
                                    countTargets++;
                                    _PathSelected = false;
                                  
                                    Debug.WriteLine(countTargets);
                                    if (Paths[countTargets] == null)
                                        countTargets = 0;
                                    _SelectNextPathToken = new CancellationTokenSource();
                                    var NextPathToken = _SelectNextPathToken.Token;
                                    await Task.Run(() => SelectNextPath(Paths[countTargets], countTargets + 1, NextPathToken), token);
                                    while (!_PathSelected)
                                        await Task.Delay(100);
                                    await Task.Delay(2000);
                                }
                                else
                                    playerFight = true;
                            }
                        }
                    }
                    if (playerDead)
                    {
                        await Task.Delay(1000, token);
                    }
                    else
                        await Task.Delay(0, token);
                }
            }
            catch (AggregateException)
            {
                Debug.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Bug");
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        private async Task CheckIfStuckOnPath(CancellationToken token)
        {
            try
            {
                if (!playerDead && !_RepairProcess && !_Stucked && !_EnemyFound)
                {
                    token.ThrowIfCancellationRequested();
                    var template = ImageMap;
                    var detector = new TestDetectors(template, null, 0.985f, // CHANGE
                 
                    Recalc(1840),
                    Recalc(85, false),
                    Recalc(35), 
                    Recalc(160, false));
                    token.ThrowIfCancellationRequested();
                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        Point item = detector.ClickIfFound(screenCapture, false, true);
                        if (item.X > 0 && item.Y > 0)
                        {
                            lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Stuck detected!"));

                            _Stucked = true;
                            countNoTargetFound = 0;
                            Debug.WriteLine("STUCK DETECTED!");
                        }
                    }
                    Debug.WriteLine("MAP CHECKED");
                }
                else
                    Debug.WriteLine("MAP NOT CHECKED");
                _GetMap = true;
                await Task.Delay(1000, token);
            }
            catch (AggregateException)
            {
                Debug.WriteLine("Expected");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Bug");
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        private async Task<int> FindPlayerOnMap(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                {
                    int rotation = 0;
                    for (rotation = 0; rotation < 360; rotation ++)
                    {
                        if (_EnemyFound)
                            return -1;
                        token.ThrowIfCancellationRequested();
                        Image<Bgra, byte> rotatedImage = RotateImageBgra(ImagePlayerArrowUp, rotation);
                        Image<Gray, byte> rotatedImageGray = RotateImageGray(ImagePlayerArrowUpMask, rotation);
                        TestDetectorsWithMask detector = new TestDetectorsWithMask(rotatedImage, rotatedImageGray, 0.955f,
                            Recalc(1700),
                            Recalc(127, false),
                            Recalc(85),
                            Recalc(70, false));
                        Image<Bgra, byte> bgraScreenCapture = screenCapture.Convert<Bgra, byte>();
                        Point item = detector.ClickIfFound(bgraScreenCapture, false, true);
                        if (item.X > 0 && item.Y > 0)
                        {
                            FormMinimized.updateImage(rotatedImage.ToBitmap());

                            _Stucked = true;
                            countNoTargetFound = 0;
                            return rotation;
                        }
                    }
                    return -1;  
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Expected");
                return -1;
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
                return -1;
            }
        }

        private Image<Bgra, byte> RotateImageBgra(Image<Bgra, byte> image, float angle)
        {
            Image<Bgra, byte> rotatedImage = new Image<Bgra, byte>(image.Size);
            PointF center = new PointF(image.Width / 2f, image.Height / 2f);
            Mat rotationMatrix = new Mat();
            CvInvoke.GetRotationMatrix2D(center, -angle, 1.0, rotationMatrix);
            CvInvoke.WarpAffine(image, rotatedImage, rotationMatrix, image.Size);
 
            return rotatedImage;
        }

        private Image<Gray, byte> RotateImageGray(Image<Gray, byte> image, float angle)
        {
            Image<Gray, byte> rotatedImage = new Image<Gray, byte>(image.Size);
            PointF center = new PointF(image.Width / 2f, image.Height / 2f);
            Mat rotationMatrix = new Mat();
            CvInvoke.GetRotationMatrix2D(center, -angle, 1.0, rotationMatrix);
            CvInvoke.WarpAffine(image, rotatedImage, rotationMatrix, image.Size);

            return rotatedImage;
        }
    }
}