using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;
using PixelAimbot.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    public partial class DiabloBot : Form
    {
        CancellationToken _token;
        CancellationToken _tokenSkills;

        public DiabloBot()
        {
            InitializeComponent();
            _token = Cts.Token;



            Config.Init();
            Conf = Config.Load();

            // Combine the base folder with your specific folder....
            if (Conf.username == "Mentalill" || Conf.username == "ShiiikK" && Debugger.IsAttached)
            {
                if (Application.OpenForms["Debugging"] == null)
                {
                    // new Debugging().Show();
                }
            }

            //Process[] processName = Process.GetProcessesByName("Diablo IV");
            //if (processName.Length == 1)
            //{
            //    handle = processName[0].MainWindowHandle;
            //    GetWindowRect(handle, out var rect);
            //    Screen screen = Screen.PrimaryScreen;

            //    if (screen.Bounds.Width > 2000 && screen.Bounds.Height > 1200 &&
            //        screen.Bounds.Width > (rect.Right - rect.Left) && screen.Bounds.Height > (rect.Bottom - rect.Top))
            //    {
            //        SetWindowPos(handle, HWND_BOTTOM, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
            //        GetWindowRect(handle, out rect);

            //        Task.Delay(5000);
            //        IsWindowed = true;
            //        _windowX = rect.X + 2;
            //        _windowY = rect.Y + 26;
            //        _windowWidth = 1920;
            //        _windowHeight = 1080;
            //        ScreenWidth = 1920;
            //        ScreenHeight = 1080;
            //    }
            //}

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, ScreenHeight-237);
            this.TopMost = true;

            this.FormBorderStyle = FormBorderStyle.None;
            RefreshRotationCombox();

            this.Text = RandomString(15);
            lbVersion.Text = Config.version;

            // Register HotKeys

            int firstHotkeyId = 1;
            int firstHotKeyKey = (int)Keys.F9;
            UnregisterHotKey(this.Handle, firstHotkeyId);
            Boolean f9Registered = RegisterHotKey(this.Handle, firstHotkeyId, 0x0000, firstHotKeyKey);

            int secondHotkeyId = 2;
            int secondHotKeyKey = (int)Keys.F10;
            UnregisterHotKey(this.Handle, secondHotkeyId);
            Boolean f10Registered = RegisterHotKey(this.Handle, secondHotkeyId, 0x0000, secondHotKeyKey);

            _discordToken = new CancellationTokenSource();
            try
            {
                DiscordTask = DiscordBotAsync(Conf.discorduser, _discordToken.Token);
            }
            catch
            {
                // ignored
            }

            SetupControls();
            // 4. Verify if both hotkeys were succesfully registered, if not, show message in the console
            if (!f9Registered)
            {
                btnStart_Click(null, null);
            }

            if (!f10Registered)
            {
                btnStop_Click(null, null);
            }
        }



        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private async Task GameCrashed()
        {
            try
            {
                if (_stop)
                {
                    _gameCrashed = true;

                    if (Cts != null)
                        Cts?.Cancel();
                    if (CtsSkills != null)
                        CtsSkills?.Cancel();

                    await resetProperties();
                    //  DiscordSendMessage("Game Crashed - Bot Stopped!");
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "GAME CRASHED - BOT STOPPED!"));
                    this.Show();
                    FormMinimized.Hide();
                    FormMinimized.sw.Reset();
                    await Task.Delay(_humanizer.Next(10, 240) + 1000);
                    _gameCrashed = false;
                    _start = false;
                    //await Task.Run(() => Game_Restart());
                }
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
        private async void GameNotStarted()
        {
            try
            {
                if (_stop)
                {
                    if (Cts != null)
                        Cts?.Cancel();
                    if (CtsSkills != null)
                        CtsSkills?.Cancel();
                    //if (_SelectNextPathToken != null)
                    //    _SelectNextPathToken?.Cancel();

                    await resetProperties();
                    this.Show();
                    FormMinimized.Hide();
                    FormMinimized.sw.Reset();
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Diablo IV is not running!"));
                    await Task.Delay(1500);
                    _start = false;
                }
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }
        private async Task resetProperties()
        {
            _1IsEmpty = _2IsEmpty = _3IsEmpty = _4IsEmpty = true;
            skill1 = skill2 = skill3 = skill4 = null;
            _EnemyFound = false;
            _LookForEnemy = false;
            _LookForPotion = false;
            _FightEnds = false;
            countNoTargetFound = 0;
            counterLookForPotion = 0;
            counterSearchForEnemy = 0;
            hadEnemy = false;
            playerDead = false;
            countLookedForTeleporter = 0;
            _RepairProcess = false;
            EnergyLow = false;
            _CheckIfStuckOnPath = false;
            _Stucked = false;
            _formExists = false;

            _repairReset = true;
            _stop = false;
            _stopped = true;
            _repairReset = true;
        
            _restart = false;
            _logout = false;
            starten = false;

        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stop)
                {
                    if (Cts != null)
                        Cts?.Cancel();
                    if (CtsSkills != null)
                        CtsSkills?.Cancel();
                    //if (_SelectNextPathToken != null)
                    //    _SelectNextPathToken?.Cancel();

                    await resetProperties();
                    this.Show();
                    FormMinimized.Hide();
                    FormMinimized.sw.Reset();
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "STOPPED!"));
                    await Task.Delay(1500);
                    lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "READY!"));
                    _start = false;
                }
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }


        public static TimeSpan ChaosTime;
        public static DateTime ChaosStart;
        public static DateTime ChaosStop;

        public static Bitmap StartInventar;
        public static Bitmap EndInventar;

        public static Bitmap StartInvColor;
        public static string startinv;

        public static Bitmap EndInvColor;
        public static string endinv;

        public static Bitmap StartInvGray;
        public static Bitmap EndInvGray;

        public static Bitmap SaveForUser;

        public static string SaveFor;


        private async void btnStart_Click(object sender, EventArgs e)
        {

            if (_start == false)
            {
                try
                {
                    _stopped = false;
                    _start = true;
                    _stop = true;
                    Cts = new CancellationTokenSource();
                    var token = Cts.Token;

                    token.ThrowIfCancellationRequested();
                    Process[] processName = Process.GetProcessesByName("Diablo IV");
                    if (processName.Length == 1)
                    {
                        handle = processName[0].MainWindowHandle;
                        SetForegroundWindow(handle);
                        lbStatus.Invoke((MethodInvoker)(() => lbStatus.Text = "Press F10 to stop!"));

                        if (!_formExists)
                        {
                            _formExists = true;
                            FormMinimized.StartPosition = FormStartPosition.Manual;
                            FormMinimized.Location = new Point(Recalc(0), Recalc(0, false));
                            FormMinimized.labelTitle.Location = new Point(Recalc(12), Recalc(-1, false));
                            FormMinimized.labelMinimizedState.Location = new Point(Recalc(160), Recalc(-1, false));
                            FormMinimized.labelRuntimer.Location = new Point(Recalc(464), Recalc(0, false));
                            FormMinimized.timerRuntimer.Enabled = true;
                            FormMinimized.sw.Reset();
                            FormMinimized.sw.Start();
                            FormMinimized.Show();
                            FormMinimized.Size = new Size(594, 28);
                            this.Hide();
                        }

                        if (chBoxAutoFight.Checked)
                        {
                            _1IsEmpty = _2IsEmpty = _3IsEmpty = _4IsEmpty = true;
                            await Task.Run(() => CheckForEmptySkill(token), token);
                        
                             GetSkill1();
                             GetSkill2();
                             GetSkill3();
                             GetSkill4();
                            await Task.Delay(1000);
                            Task.Run(() => FightAgainstEnemys(token), token);
                            Task.Run(() => SearchNearEnemys(token), token);
                            Task.Run(() => PlayerEnergy(token), token);
                            //Task.Run(() => SearchForMoreEnemys(token), token);
                        }

                        if (chBoxAutoPathfinding.Checked)
                            Task.Run(() => PathFinding(token), token);

                        if (chBoxAutoSearchItems.Checked)
                            Task.Run(() => SearchHealthPod(token), token);

                        if (chBoxAutoHeal.Checked)
                            Task.Run(() => PlayerHeal(token), token);

                        if (chBoxAutoRepair.Checked)
                            Task.Run(() => AutoRepair(token), token);

                        if (chBoxAutoRevive.Checked)
                            Task.Run(() => AutoRevive(token), token);
                    }
                    else
                        GameNotStarted();

                }
                catch (OperationCanceledException)
                {
                    // Handle canceled
                }
                catch (Exception)
                {
                    // Handle other exceptions
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLayout.SelectedItem is Layout_Keyboard currentLayout)
            {
                lbP1.Text = translateKey(currentLayout.One);
                lbP2.Text = translateKey(currentLayout.Two);
                lbP3.Text = translateKey(currentLayout.Three);
                lbP4.Text = translateKey(currentLayout.Four);

                //txBoxUltimateKey2.Text = txBoxUltimateKey.Text = translateKey(currentLayout.Y);
            }
        }

        private void comboBoxMouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentMouseButton = comboBoxMouse.SelectedIndex == 0
                ? KeyboardWrapper.VK_LBUTTON
                : KeyboardWrapper.VK_RBUTTON;
        }

        private void cmbHealKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbHealKey.SelectedIndex == 0)
            {
                _currentHealKey = KeyboardWrapper.VK_Q;
            }
            else if (cmbHealKey.SelectedIndex == 1)
            {
                _currentHealKey = KeyboardWrapper.VK_1;
            }
            else if (cmbHealKey.SelectedIndex == 2)
            {
                _currentHealKey = KeyboardWrapper.VK_2;
            }
            else if (cmbHealKey.SelectedIndex == 3)
            {
                _currentHealKey = KeyboardWrapper.VK_3;
            }
            else if (cmbHealKey.SelectedIndex == 4)
            {
                _currentHealKey = KeyboardWrapper.VK_4;
            }
            else if (cmbHealKey.SelectedIndex == 5)
            {
                _currentHealKey = KeyboardWrapper.VK_5;
            }
            else if (cmbHealKey.SelectedIndex == 6)
            {
                _currentHealKey = KeyboardWrapper.VK_6;
            }
            else if (cmbHealKey.SelectedIndex == 7)
            {
                _currentHealKey = KeyboardWrapper.VK_7;
            }
            else if (cmbHealKey.SelectedIndex == 8)
            {
                _currentHealKey = KeyboardWrapper.VK_8;
            }
            else if (cmbHealKey.SelectedIndex == 9)
            {
                _currentHealKey = KeyboardWrapper.VK_9;
            }
        }

        private void ChaosBot_Load(object sender, EventArgs e)
        {
            tabControl2.ItemSize = new Size(75, 15);
            List<Layout_Keyboard> layout = new List<Layout_Keyboard>();
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            Layout_Keyboard qwertz = new Layout_Keyboard
            {
                LAYOUTS = "QWERTZ",
                One = KeyboardWrapper.VK_1,
                Two = KeyboardWrapper.VK_2,
                Three = KeyboardWrapper.VK_3,
                Four = KeyboardWrapper.VK_4

            };
            layout.Add(qwertz);

            Layout_Keyboard qwerty = new Layout_Keyboard
            {
                LAYOUTS = "QWERTY",
                One = KeyboardWrapper.VK_1,
                Two = KeyboardWrapper.VK_2,
                Three = KeyboardWrapper.VK_3,
                Four = KeyboardWrapper.VK_4
            };
            layout.Add(qwerty);

            Layout_Keyboard azerty = new Layout_Keyboard
            {
                LAYOUTS = "AZERTY",
                One = KeyboardWrapper.VK_1,
                Two = KeyboardWrapper.VK_2,
                Three = KeyboardWrapper.VK_3,
                Four = KeyboardWrapper.VK_4
            };
            layout.Add(azerty);
            cmbLayout.DataSource = layout;
            cmbLayout.DisplayMember = "LAYOUTS";
            comboBoxMouse.SelectedIndex = 0;

            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

            SetupControls();

        }

        private void lbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void ChaosBot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }


        private void chBoxAutoRepair_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAutoRepair.Checked)
            {
                //txtRepair.ReadOnly = false;
            }
            else if (!chBoxAutoRepair.Checked)
            {
                //txtRepair.ReadOnly = true;
            }
        }



        private void chBoxLOGOUT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chBoxAutoLogout.Checked)
            {
                _logout = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetupControls();
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        private void SetupControls()
        {
            try
            {

                sldHealth.Value = 1019;
                HealthPercent = (1045 - sldHealth.Value) + 940;
                double distanceFromMin = (sldHealth.Value - sldHealth.Minimum);
                double sliderRange = (sldHealth.Maximum - sldHealth.Minimum);
                sliderPercent = 100 * (distanceFromMin / sliderRange);
                labelheal.Text = "Heal at: " + Convert.ToInt32(sliderPercent) + "% Life";

                path1.BackgroundImage = Resources.Path;
                path2.BackgroundImage = Resources.PathFalse;
                path3.BackgroundImage = Resources.PathFalse;
                path4.BackgroundImage = Resources.PathFalse;
                path5.BackgroundImage = Resources.PathFalse;
                path6.BackgroundImage = Resources.PathFalse;
                path7.BackgroundImage = Resources.PathFalse;
                path8.BackgroundImage = Resources.PathFalse;
                path9.BackgroundImage = Resources.PathFalse;
                path10.BackgroundImage = Resources.PathFalse;
                path11.BackgroundImage = Resources.PathFalse;
                path12.BackgroundImage = Resources.PathFalse;
                path13.BackgroundImage = Resources.PathFalse;
                path14.BackgroundImage = Resources.PathFalse;
                path1.Enabled = true;
                path2.Enabled = false;
                path3.Enabled = false;
                path4.Enabled = false;
                path5.Enabled = false;
                path6.Enabled = false;
                path7.Enabled = false;
                path8.Enabled = false;
                path9.Enabled = false;
                path10.Enabled = false;
                path11.Enabled = false;
                path12.Enabled = false;
                path13.Enabled = false;
                path14.Enabled = false;
                steampath = @"C:\Program Files (x86)\Steam\steam.exe";
                chBoxCrashDetection.Checked = true;
                checkBoxDiscordNotifications.Checked = true;

                chBoxAutoRevive.Checked = false;
                chBoxAutoHeal.Checked = true;
                chBoxAutoPathfinding.Checked = false;
                chBoxPathGenerator.Checked = false;
                chBoxAutoSearchItems.Checked = false;
                chBoxAutoLogout.Checked = false;
                chBoxAutoUnstuck.Checked = false;
               chBoxAutoFight.Checked = false;
                chBoxAutoPathfinding.Checked = false;

                cmbHOUR.Text = "";
                txHold1.Text = "500";
                txHold2.Text = "500";
                txHold3.Text = "500";
                txHold4.Text = "500";

                txA.Text = "500";
                txS.Text = "500";
                txD.Text = "500";
                txF.Text = "500";

                txPrio1.Text = "1";
                txPrio2.Text = "2";
                txPrio3.Text = "3";
                txPrio4.Text = "4";

                chBoxDouble1.Checked = false;
                chBoxDouble2.Checked = false;
                chBoxDouble3.Checked = false;
                chBoxDouble4.Checked = false;

                cmbHOUR.SelectedIndex = 0;
                cmbMINUTE.SelectedIndex = 0;


                cmbHealKey.SelectedIndex = 0;
              
                chBoxAutoRepair.Checked = false;
                txCharSelect.Text = "1";
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine("[" + line + "]" + ex.Message);
            }
        }

        private void btnInstructions_Click(object sender, EventArgs e)
        {
            //frmGuide form = new frmGuide();
            //form.Show();
        }

        // Function to convert an Image to Bitmap
        private Bitmap ConvertImageToBitmap(Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(image, 0, 0);
            }
            return bitmap;
        }

        public string ProcessAndEncryptImageFromBitmap(Bitmap image)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "tempImage.png");
            image.Save(tempFilePath, ImageFormat.Png);

            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                imageBytes = ms.ToArray();
            }

            string encodedString = Convert.ToBase64String(imageBytes);
            string encryptedString = FrmLogin.Blow1.Encrypt_CTR(encodedString);

            File.Delete(tempFilePath);

            return encryptedString;
        }

        public Image DecryptAndGetImage(string encryptedString)
        {
            string decryptedString = FrmLogin.Blow1.Decrypt_CTR(encryptedString);
            byte[] imageBytes = Convert.FromBase64String(decryptedString);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }

        private void buttonSaveRotation_Click(object sender, EventArgs e)
        {
 
            if (comboBoxRotations.Text != "")
            {
                if (comboBoxRotations.Text != "main")
                {
                    using (Bitmap bitmap = new Bitmap(path1.BackgroundImage))
                    {
                        rotation.Path1 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path2.BackgroundImage))
                    {
                        rotation.Path2 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path3.BackgroundImage))
                    {
                        rotation.Path3 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path4.BackgroundImage))
                    {
                        rotation.Path4 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path5.BackgroundImage))
                    {
                        rotation.Path5 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path6.BackgroundImage))
                    {
                        rotation.Path6 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path7.BackgroundImage))
                    {
                        rotation.Path7 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path8.BackgroundImage))
                    {
                        rotation.Path8 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path9.BackgroundImage))
                    {
                        rotation.Path9 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path10.BackgroundImage))
                    {
                        rotation.Path10 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path11.BackgroundImage))
                    {
                        rotation.Path11 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path12.BackgroundImage))
                    {
                        rotation.Path12 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path13.BackgroundImage))
                    {
                        rotation.Path13 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }
                    using (Bitmap bitmap = new Bitmap(path14.BackgroundImage))
                    {
                        rotation.Path14 = ProcessAndEncryptImageFromBitmap(bitmap);
                    }


                    rotation.Path1State = path1.Enabled;
                    rotation.Path2State = path2.Enabled;
                    rotation.Path3State = path3.Enabled;
                    rotation.Path4State = path4.Enabled;
                    rotation.Path5State = path5.Enabled;
                    rotation.Path6State = path6.Enabled;
                    rotation.Path7State = path7.Enabled;
                    rotation.Path8State = path8.Enabled;
                    rotation.Path9State = path9.Enabled;
                    rotation.Path10State = path10.Enabled;
                    rotation.Path11State = path11.Enabled;
                    rotation.Path12State = path12.Enabled;
                    rotation.Path13State = path13.Enabled;
                    rotation.Path14State = path14.Enabled;

                    rotation.DesignChanger = swDesignChanger.Checked;
                    rotation.CrashDetection = chBoxCrashDetection.Checked;
                    rotation.DiscordNotifications = checkBoxDiscordNotifications.Checked;
                    rotation.HealthSlider = sldHealth.Value;
                    rotation.Fight = chBoxAutoFight.Checked;
                    rotation.Revive = chBoxAutoRevive.Checked;
                    rotation.Repair = chBoxAutoRepair.Checked;
                    rotation.Heal = chBoxAutoHeal.Checked;
                    rotation.Logout = chBoxAutoLogout.Checked;
                    rotation.Pathfinding = chBoxAutoPathfinding.Checked;
                    rotation.Unstuck = chBoxAutoUnstuck.Checked;
                    rotation.PathGen = chBoxPathGenerator.Checked;
                    rotation.SearchItems = chBoxAutoSearchItems.Checked;

                    rotation.Holddown1 = txHold1.Text;
                    rotation.Holddown2 = txHold2.Text;
                    rotation.Holddown3 = txHold3.Text;
                    rotation.Holddown4 = txHold4.Text;

                    rotation.Prioritize1 = txPrio1.Text;
                    rotation.Prioritize2 = txPrio2.Text;
                    rotation.Prioritize3 = txPrio3.Text;
                    rotation.Prioritize4 = txPrio4.Text;

                    rotation.Doubleclick1 = chBoxDouble1.Checked;
                    rotation.Doubleclick2 = chBoxDouble2.Checked;
                    rotation.Doubleclick3 = chBoxDouble3.Checked;
                    rotation.Doubleclick4 = chBoxDouble4.Checked;

                    rotation.MouseSwitch = comboBoxMouse.SelectedIndex;
                    rotation.KeyboardLayout = cmbLayout.SelectedIndex;

                    rotation.LogoutMinute = cmbMINUTE.SelectedIndex;
                    rotation.LogoutHour = cmbHOUR.SelectedIndex;
                    rotation.HealKey = cmbHealKey.SelectedIndex;
                    rotation.Charselect = txCharSelect.Text;

                    rotation.SAVE(comboBoxRotations.Text);
                    Alert.Show("Rotation \"" + comboBoxRotations.Text + "\" saved");
                }
                else
                {
                    Alert.Show("Rotation can not be named \"main\"", FrmAlert.EnmType.Error);
                }
            }
            else
            {
                Alert.Show("Rotation name cannot be clear!", FrmAlert.EnmType.Error);
            }

        }

        private async void buttonLoadRotation_Click(object sender, EventArgs e)
        {
            if (comboBoxRotations.SelectedIndex >= 0)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;
                rotation = await Rotations.Load(comboBoxRotations.Text + ".ini", token);
                if (rotation != null)
                {

                    path1.BackgroundImage = DecryptAndGetImage(rotation.Path1);
                    path2.BackgroundImage = DecryptAndGetImage(rotation.Path2);
                    path3.BackgroundImage = DecryptAndGetImage(rotation.Path3);
                    path4.BackgroundImage = DecryptAndGetImage(rotation.Path4);
                    path5.BackgroundImage = DecryptAndGetImage(rotation.Path5);
                    path6.BackgroundImage = DecryptAndGetImage(rotation.Path6);
                    path7.BackgroundImage = DecryptAndGetImage(rotation.Path7);
                    path8.BackgroundImage = DecryptAndGetImage(rotation.Path8);
                    path9.BackgroundImage = DecryptAndGetImage(rotation.Path9);
                    path10.BackgroundImage = DecryptAndGetImage(rotation.Path10);
                    path11.BackgroundImage = DecryptAndGetImage(rotation.Path11);
                    path12.BackgroundImage = DecryptAndGetImage(rotation.Path12);
                    path13.BackgroundImage = DecryptAndGetImage(rotation.Path13);
                    path14.BackgroundImage = DecryptAndGetImage(rotation.Path14);

                    path1.Enabled = rotation.Path1State;
                    path2.Enabled = rotation.Path2State;
                    path3.Enabled = rotation.Path3State;
                    path4.Enabled = rotation.Path4State;
                    path5.Enabled = rotation.Path5State;
                    path6.Enabled = rotation.Path6State;
                    path7.Enabled = rotation.Path7State;
                    path8.Enabled = rotation.Path8State;
                    path9.Enabled = rotation.Path9State;
                    path10.Enabled = rotation.Path10State;
                    path11.Enabled = rotation.Path11State;
                    path12.Enabled = rotation.Path12State;
                    path13.Enabled = rotation.Path13State;
                    path14.Enabled = rotation.Path14State;

                    swDesignChanger.Checked = rotation.DesignChanger;
                    chBoxCrashDetection.Checked = rotation.CrashDetection;
                    checkBoxDiscordNotifications.Checked = rotation.DiscordNotifications;
                    sldHealth.Value = rotation.HealthSlider;
                    chBoxAutoRevive.Checked = rotation.Revive;
                    chBoxAutoHeal.Checked = rotation.Heal;
                    chBoxAutoSearchItems.Checked = rotation.SearchItems;
                    chBoxPathGenerator.Checked = rotation.PathGen;
                    chBoxAutoRepair.Checked = rotation.Repair;
                    chBoxAutoLogout.Checked = rotation.Logout;
                    chBoxAutoPathfinding.Checked = rotation.Pathfinding;
                    chBoxAutoUnstuck.Checked = rotation.Unstuck;
                    chBoxAutoFight.Checked = rotation.Fight;

                    txHold1.Text = rotation.Holddown1;
                    txHold2.Text = rotation.Holddown2;
                    txHold3.Text = rotation.Holddown3;
                    txHold4.Text = rotation.Holddown4;

                    txPrio1.Text = rotation.Prioritize1;
                    txPrio2.Text = rotation.Prioritize2;
                    txPrio3.Text = rotation.Prioritize3;
                    txPrio4.Text = rotation.Prioritize4;

                    chBoxDouble1.Checked = rotation.Doubleclick1;
                    chBoxDouble2.Checked = rotation.Doubleclick2;
                    chBoxDouble3.Checked = rotation.Doubleclick3;
                    chBoxDouble4.Checked = rotation.Doubleclick4;

                    comboBoxMouse.SelectedIndex = rotation.MouseSwitch;
                    cmbLayout.SelectedIndex = rotation.KeyboardLayout;
                    cmbHOUR.SelectedIndex = rotation.LogoutHour;
                    cmbMINUTE.SelectedIndex = rotation.LogoutMinute;


                    cmbHealKey.SelectedIndex = rotation.HealKey;

                    txCharSelect.Text = rotation.Charselect;


                    _currentMouseButton = comboBoxMouse.SelectedIndex == 0
                        ? KeyboardWrapper.VK_LBUTTON
                        : KeyboardWrapper.VK_RBUTTON;
                    Alert.Show("Rotation \"" + comboBoxRotations.Text + "\" loaded");
                }
            }
        }




        private void chBoxRevive_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAutoRevive.Checked)
            {

            }
            else if (!chBoxAutoRevive.Checked)
            {

            }
        }

        private void lbStatus_TextChanged(object sender, EventArgs e)
        {
            FormMinimized.labelMinimizedState.Text = lbStatus.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label18.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void comboBoxRotations_MouseEnter(object sender, EventArgs e)
        {
            RefreshRotationCombox();
        }

        private void btnSpecialSkillsInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Different classes have special abilities such as Wardancer esotericism.\n" +
                            "These abilities are grayscaled at the start of the dungeon and only\n" +
                            "become usable after a certain moment.\n\n" +
                            "If you use such abilities then select the appropriate\n" +
                            "key on which the ability sits.The bot does the rest on its own.\n\n" +
                            "You can deposit up to 4 such abilities.\n\n" +
                            "If you don't want to use any, then set all boxes to 'OFF'",
                "Special Abilitys like Esoterik etc.");
        }

        private void lbMoreRotations_Clicked(object sender, EventArgs e)
        {
            // Process.Start("https://user.symbiotic.link");
            Alert.Show("Under Developement", FrmAlert.EnmType.Info);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var t9 = Task.Run(() => Game_Restart());
        }

        private void txCharSelect_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                int x = Convert.ToInt16(txCharSelect.Text);
                if (x == 0 || x > 7)
                {
                    MessageBox.Show("You can only set Characters from 1 to 7!");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txInfo_MouseClick(object sender, MouseEventArgs e)
        {
            Label tb = (Label)sender;
            int visibleTime = 10000;  //in milliseconds
            ToolTip tt = new ToolTip();
            tt.Show("You can change ULTIMATE KEY\n" +
                    "under Classes -> Cast-Time Section.\n\n" +

                    "Change your Keyboard Layout on\n" +
                    "this DropDown Menu.", tb, 0, 0, visibleTime);

        }

        private void btnSteamPath_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.IsFolderPicker = false;
            MessageBox.Show("Select Steam.exe\n\n" + @"Example: C:\Program Files (x86)\Steam\steam.exe");
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                steampath = dialog.FileName;
                MessageBox.Show("Your Selection: " + dialog.FileName);
            }
        }

        private void DesignChanger_CheckedChanged(object sender, EventArgs e)
        {

            switch (swDesignChanger.Checked)
            {
                case true:
                    chBoxCrashDetection.CheckboxColor =
                    btnShowPath.ForeColor = lbPathInstruct.ForeColor =
                    groupPathGenerator.ForeColor = groupPathGenerator.BorderColor = groupPathGenerator.TextColor =
                   groupItemFilter.ForeColor = groupItemFilter.BorderColor = groupItemFilter.TextColor =
                   chBoxAutoUnstuck.CheckboxColor =
                   chBoxAutoPathfinding.CheckboxColor = chBoxAutoFight.CheckboxColor =
                        chBoxAutoRepair.CheckboxColor = chBoxAutoLogout.CheckboxColor =
                        chBoxAutoHeal.CheckboxColor = chBoxAutoRevive.CheckboxColor =
                        chBoxAutoSearchItems.CheckboxColor = chBoxPathGenerator.CheckboxColor = Color.Orange;
                    btnReset.ForeColor =
                     btnAutoSetup.ForeColor = btnInstructions.ForeColor = btnSteamPath.ForeColor =
                    buttonSaveRotation.ForeColor = buttonLoadRotation.ForeColor = Color.WhiteSmoke;
                    lbHeal.ForeColor =

                    lbClock.ForeColor = lbVersion.ForeColor = lbHeader.ForeColor =

                    lbto7.ForeColor = lbto14.ForeColor =
                   groupCasttime.ForeColor =
                    groupHeal.ForeColor = groupHeal.BorderColor = groupHeal.TextColor =
                    groupAttack.ForeColor = groupAttack.BorderColor = groupAttack.TextColor =
                    groupDetect.ForeColor = groupDetect.BorderColor = groupDetect.TextColor =
                    groupTelegram.ForeColor = groupTelegram.BorderColor = groupTelegram.TextColor =
                    groupMouse.ForeColor = groupMouse.BorderColor = groupMouse.TextColor =
                    groupValtan.ForeColor = groupValtan.BorderColor = groupValtan.TextColor =
                    groupAction.ForeColor = groupAction.BorderColor = groupAction.TextColor =
                    groupMain.ForeColor = groupMain.BorderColor = groupMain.TextColor = Color.Orange;


                    btnAutoSetup.BackColor = btnInstructions.BackColor = btnSteamPath.BackColor = lbHoverOver.ForeColor = Color.Peru;


                    swDesignChanger.BaseColor = Color.Black;
                    tabStart.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Orange;

                    this.BackgroundImage = Resources.backgroundOrange;

                    break;
                case false:
                    lbto7.ForeColor = lbto14.ForeColor =
                  chBoxCrashDetection.CheckboxColor =
                btnShowPath.ForeColor = lbPathInstruct.ForeColor =
                     groupPathGenerator.ForeColor = groupPathGenerator.BorderColor = groupPathGenerator.TextColor =
                     groupItemFilter.ForeColor = groupItemFilter.BorderColor = groupItemFilter.TextColor =
                     chBoxAutoPathfinding.CheckboxColor = chBoxAutoFight.CheckboxColor = chBoxAutoUnstuck.CheckboxColor =
                     chBoxAutoRepair.CheckboxColor = chBoxAutoLogout.CheckboxColor =
                     chBoxAutoHeal.CheckboxColor = chBoxAutoRevive.CheckboxColor =
                     chBoxAutoSearchItems.CheckboxColor = chBoxPathGenerator.CheckboxColor =
                  btnReset.ForeColor =
                  lbClock.ForeColor = lbVersion.ForeColor = lbHeader.ForeColor =
                  buttonSaveRotation.ForeColor = buttonLoadRotation.ForeColor =
                lbHeal.ForeColor =

                 groupCasttime.ForeColor =
                  groupHeal.ForeColor = groupHeal.BorderColor = groupHeal.TextColor =
                  groupAttack.ForeColor = groupAttack.BorderColor = groupAttack.TextColor =
                  groupDetect.ForeColor = groupDetect.BorderColor = groupDetect.TextColor =
                  groupTelegram.ForeColor = groupTelegram.BorderColor = groupTelegram.TextColor =
                  groupMouse.ForeColor = groupMouse.BorderColor = groupMouse.TextColor =
                  groupValtan.ForeColor = groupValtan.BorderColor = groupValtan.TextColor =
                  groupAction.ForeColor = groupAction.BorderColor = groupAction.TextColor =
                  groupMain.ForeColor = groupMain.BorderColor = groupMain.TextColor = Color.FromArgb(150, 82, 197);

                   btnAutoSetup.ForeColor = btnSteamPath.ForeColor = btnInstructions.ForeColor = Color.Black;

                    btnAutoSetup.BackColor = btnSteamPath.BackColor = btnInstructions.BackColor = Color.FromArgb(150, 82, 197);

                    lbHoverOver.ForeColor = Color.FromArgb(170, 85, 197);


                    swDesignChanger.BackColor = Color.Transparent;
                    swDesignChanger.BaseColor = Color.GhostWhite;
                    tabStart.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Purple;
                    this.BackgroundImage = Resources.backgroundDiablo;

                    break;
            }
        }


        private Point selectedPoint;
        bool alldone;



        private void SelectArea_MouseClickHandled(object sender, Point clickedPoint)
        {
            // Handle the clickedPoint in the calling form
            // You can close the SearchColorOverlay form if needed
            SearchColorOverlay selectArea = (SearchColorOverlay)sender;
            selectArea.Close();
            selectedPoint = clickedPoint;
            alldone = true;
        }

        private void Path_Hover(object sender, EventArgs e)
        {
            ImageBox imageBox = sender as ImageBox;
            imageBox.Size = new Size(45, 45);
        }

        private void Path_EndHover(object sender, EventArgs e)
        {
            ImageBox imageBox = sender as ImageBox;
            imageBox.Size = new Size(35, 35);
        }

        private async void path_CreateOrDelete(object sender, MouseEventArgs e)
        {
            ImageBox imageBox = sender as ImageBox;
            if (e.Button == MouseButtons.Right)
            {
                if (int.TryParse(imageBox.Name.Replace("path", ""), out int pathNumber) && pathNumber < 14)
                {
                    int pathNumber2 = pathNumber;
                    while (pathNumber < 14)
                    {
                        Control nextPath = Controls.Find("path" + (pathNumber + 1), true).FirstOrDefault();
                        if (nextPath != null)
                        {
                            Paths[pathNumber - 1] = null;
                            nextPath.Enabled = false;
                            nextPath.BackgroundImage = Resources.PathFalse;
                        }

                        if (pathNumber < 13)
                        {
                            Control nextArrow2 = groupPathGenerator.Controls.Find("arrow" + (pathNumber), true).FirstOrDefault();

                            Debug.WriteLine(nextArrow2.Name);
                            if (nextArrow2 != null)
                            {
                                nextArrow2.ForeColor = Color.FromArgb(131, 131, 131);
                            }
                        }
                        pathNumber++;
                    }
                    Control nextPath2 = Controls.Find("path" + (pathNumber2), true).FirstOrDefault();
                    if (nextPath2 != null)
                    {
                        nextPath2.Enabled = true;
                        nextPath2.BackgroundImage = Resources.Path;
                    }
                    if (pathNumber2 < 13 && pathNumber2 > 1)
                    {
                        Control nextArrow2 = groupPathGenerator.Controls.Find("arrow" + (pathNumber2 - 1), true).FirstOrDefault();

                        Debug.WriteLine(nextArrow2.Name);
                        if (nextArrow2 != null)
                            nextArrow2.ForeColor = Color.FromArgb(150, 82, 197);
                    }
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                alldone = false;
                SearchColorOverlay selectArea = new SearchColorOverlay();
                selectArea.MouseClickHandled += SelectArea_MouseClickHandled;
                selectArea.ShowDialog();
                while (!alldone)
                    await Task.Delay(100);
                Debug.WriteLine("X: " + selectedPoint.X + " Y: " + selectedPoint.Y);
                Bitmap bitmap = GetPath(selectedPoint).ToBitmap();
                imageBox.BackgroundImage = bitmap;

                int pathNumber;
                if (int.TryParse(imageBox.Name.Replace("path", ""), out pathNumber) && pathNumber < 14)
                {
                    Paths[pathNumber - 1] = GetPath(selectedPoint);
                    Control nextPath = Controls.Find("path" + (pathNumber + 1), true).FirstOrDefault();
                    if (nextPath != null)
                    {
                        nextPath.Enabled = true;
                        nextPath.BackgroundImage = Resources.Path;
                    }

                    if (pathNumber < 13)
                    {
                        Control nextArrow2 = groupPathGenerator.Controls.Find("arrow" + (pathNumber), true).FirstOrDefault();

                        Debug.WriteLine(nextArrow2.Name);
                        if (nextArrow2 != null)
                            nextArrow2.ForeColor = Color.FromArgb(150, 82, 197);
                    }
                }
                //FindPath(Paths[pathNumber - 1]);

            }
        }
        async Task ShowPaths(Image<Bgr, byte>[] images)
        {
            try
            {
                int i = 0;
                while (i <= images.Length && images[i] != null)
                {
                    var detector = new TestDetectors(
                        images[i],
                        null,
                        0.8f,
                        DiabloBot.Recalc(0),
                        DiabloBot.Recalc(0, false),
                        DiabloBot.Recalc(1920),
                        DiabloBot.Recalc(1080, false)
                    );

                    using (var screenCapture = _globalScreenPrinter.CaptureScreenImage())
                    {
                        Point item = detector.ClickIfFound(screenCapture, false, true);
                        if (item.X > 0 && item.Y > 0)
                        {
                            TaskCompletionSource<bool> invokeTaskCompletionSource = new TaskCompletionSource<bool>();

                            string pathText = "Found Path " + (i + 1);

                            lbStatus.Invoke((MethodInvoker)(() =>
                            {
                                lbStatus.Text = pathText;
                                lbStatus.Refresh(); // Force the label to update immediately
                                invokeTaskCompletionSource.SetResult(true);
                            }));

                            await invokeTaskCompletionSource.Task;
                            VirtualMouse.MoveTo(item.X, item.Y, 5);
                            await Task.Delay(250);
                            //VirtualMouse.RightClick();
                            //KeyboardWrapper.PressKey(KeyboardWrapper.VK_TAB);
                        }
                    }
                    i++;
                }

            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                Debug.WriteLine($"[{line}] {ex.Message}");
            }
        }

        private async void btnShowPath_Click(object sender, EventArgs e)
        {
            await ShowPaths(Paths);
        }

        private void btnAutoSetup_Click(object sender, EventArgs e)
        {
            Task.Run(() => AutoSetup());
        }

        private void chBoxAutoHeal_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAutoHeal.Checked)
            {
                cmbHealKey.Enabled = true;
                sldHealth.Enabled = true;
            }
            else
            {
                cmbHealKey.Enabled = false;
                sldHealth.Enabled = false;
            }
        }
    }
}