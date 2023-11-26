using Emgu.CV;
using Emgu.CV.Structure;
using PixelAimbot.Classes.Misc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PixelAimbot
{
    partial class DiabloBot
    {
        public static bool IsWindowed;
        private static int _windowX;
        private static int _windowY;
        private static int _windowWidth;
        private static int _windowHeight;

        public static int ChaosAllRounds;
        public static int ChaosAllStucks;
        public static int ChaosRedStages;
        public static int ChaosGameCrashed;

        public static int HealthPercent = 75;

        private static readonly Random Random = new Random();

        public static int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

        public static CancellationTokenSource Cts = new CancellationTokenSource();
        public static CancellationTokenSource CtsSkills = new CancellationTokenSource();
        public static CancellationTokenSource CtsBoss = new CancellationTokenSource();

        private readonly Priorized_Skills _skills = new Priorized_Skills();

        private readonly CancellationTokenSource _discordToken = new CancellationTokenSource();

        private readonly PrintScreen _globalScreenPrinter = new PrintScreen();
        private readonly Random _humanizer = new Random();


        private string steampath = @"C:\Program Files(x86)\Steam\steam.exe";

        private bool _botIsRun = true;
        private bool _discordBotIsRun = true;
        private bool _firstSetupTransparency = true;
        private bool _formExists = false;
       
        private bool _logout;
        private DateTime _Logout;

        //SKILL AND COOLDOWN//
        private bool _1;
        private bool _2;
        private bool _3;
        private bool _4;

        private bool _1IsEmpty = true;
        private bool _2IsEmpty = true;
        private bool _3IsEmpty = true;
        private bool _4IsEmpty = true;

        private bool _repair;
        private bool _repairReset = true;
        private DateTime _repairTimer;
        private bool _restart;
        private int _restartInt = 0;


        private bool _gameCrashed;


        // DIABLO

        private bool _start;
        private bool _stop;
        private bool _stopped;
        private bool _EnemyFound;
        private bool _fightAgainstEnemys;
        private bool _LookForEnemy;
        private bool _LookForPotion;
        private bool _FightEnds;
        private int countNoTargetFound = 0;
        private int counterLookForPotion = 0;
        private int counterSearchForEnemy = 0;
        private bool hadEnemy;
        private bool playerDead;
        private bool playerFight;
        private int countLookedForTeleporter = 0;
        private bool _RepairProcess;
        private bool EnergyLow;
        private bool _CheckIfStuckOnPath;
        private bool _Stucked;
        private bool _PathSelected;

      
        // DIABLO

        private Timer _timer;

        private string _comboattack = "";
        public Config Conf = new Config();
        private byte _currentHealKey;
        private byte _currentMouseButton;
        public Task DiscordTask;

        public frmMinimized FormMinimized = new frmMinimized();

        public Image<Bgr, byte> ImageHealthPod = ByteArrayToImage(Images.healthPod);
        public Image<Bgr, byte> ImageEnemyHealth1 = ByteArrayToImage(Images.enemyHealth1);
        public Image<Bgr, byte> ImageEnemyHealth2 = ByteArrayToImage(Images.enemyHealth2);
        public Image<Bgr, byte> ImageDestroyedArmor = ByteArrayToImage(Images.destroyedArmor);
        public Image<Bgr, byte> ImageTeleporter = ByteArrayToImage(Images.teleporter);
        public Image<Bgr, byte> ImageDead1 = ByteArrayToImage(Images.dead1);
        public Image<Bgr, byte> ImageDead2 = ByteArrayToImage(Images.dead2);
        public Image<Bgr, byte> ImageDead3 = ByteArrayToImage(Images.dead3);
        public Image<Bgr, byte> ImageDead4 = ByteArrayToImage(Images.dead4);
        public Image<Bgr, byte> ImageEmptySkillBar = ByteArrayToImage(Images.emptySkillBar);
        public Image<Bgra, byte> ImagePlayerArrowUp = ByteArrayToImage(Images.playerArrowUp).Convert<Bgra, byte>();
        public Image<Gray, byte> ImagePlayerArrowUpMask = ByteArrayToImage(Images.playerArrowUpMask).Convert<Gray, byte>();

        public Image<Bgr, byte>[] Paths = new Image<Bgr, byte>[14];

        public Rotations rotation = new Rotations();


        public static string ConfigPath { get; set; } = Directory.GetCurrentDirectory() + @"\" + HWID.Get();
    }
}