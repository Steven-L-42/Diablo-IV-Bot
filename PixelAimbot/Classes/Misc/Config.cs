using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAimbot.Classes.Misc
{
    public class Config
    {
        //public static string version { get; set; } = "2.8.6r";    // Aktuelle Old und Basis Version
        //public static string version { get; set; } = "3.5.9r";    // Aktuelle Stable Version
        public static string version { get; set; } = "0.0.2b";     // Aktuelle Developer Version

        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string discorduser { get; set; } = "";
        public bool alphaVersion { get; set; } = false;
        public bool betaVersion { get; set; } = false;

        private static string ConfigPath { get; set; } = Directory.GetCurrentDirectory() + @"\" + HWID.Get();
        private static string ConfigFileName { get; set; } = "main.ini";

        public static void Init()
        {
            string newIni = Directory.GetCurrentDirectory() + @"\" + HWID.Get() + @"\main.ini";
            string newConfigurationFolder = Directory.GetCurrentDirectory() + @"\" + HWID.Get();
            if (!File.Exists(newIni))
            {
                if (!Directory.Exists(newConfigurationFolder))
                {
                    Directory.CreateDirectory(newConfigurationFolder);
                }
                var createdFile = File.Create(newIni);
                createdFile.Close();
            }
        }

        public void Save()
        {
            string output = JsonConvert.SerializeObject(this);
            using (StreamWriter writer = new StreamWriter(Path.Combine(ConfigPath, ConfigFileName)))
            {
                writer.Write(FrmLogin.Blow1.Encrypt_CTR(output));
            }
        }

        public static Config Load()
        {
            using (StreamReader reader = new StreamReader(Path.Combine(ConfigPath, "main.ini")))
            {
                string output = reader.ReadToEnd(); //
                if (output != "")
                {
                    return JsonConvert.DeserializeObject<Config>(FrmLogin.Blow1.Decrypt_CTR(output));
                }
            }
            return new Config();
        }
    }

    public class Rotations
    {
        public string Charselect { get; set; } = "1";
        public bool DesignChanger { get; set; } = false;
        public bool CrashDetection { get; set; } = true;
        public bool DiscordNotifications { get; set; } = true;
        public int HealthSlider { get; set; } = 801;
        public bool Pathfinding { get; set; }
        public bool Fight { get; set; }
        public bool Repair { get; set; }
        public bool Heal { get; set; }
        public bool PathGen { get; set; }
        public bool SearchItems { get; set; }
        public int HealKey { get; set; }
        public bool Revive { get; set; }
        public bool Logout { get; set; }
        public int LogoutHour { get; set; } = DateTime.Now.Hour;
        public int LogoutMinute { get; set; } = DateTime.Now.Minute;
        public bool CooldownDetection { get; set; } = true;
        public bool Unstuck { get; set; } = false;

        public string Cooldown1 { get; set; } = "500";
        public string Cooldown2 { get; set; } = "500";
        public string Cooldown3 { get; set; } = "500";
        public string Cooldown4 { get; set; } = "500";

        public string Holddown1 { get; set; } = "500";
        public string Holddown2 { get; set; } = "500";
        public string Holddown3 { get; set; } = "500";
        public string Holddown4 { get; set; } = "500";

        public string Prioritize1 { get; set; } = "1";
        public string Prioritize2 { get; set; } = "2";
        public string Prioritize3 { get; set; } = "3";
        public string Prioritize4 { get; set; } = "4";

        public bool Doubleclick1 { get; set; }
        public bool Doubleclick2 { get; set; }
        public bool Doubleclick3 { get; set; }
        public bool Doubleclick4 { get; set; }

        public string Path1 { get; set; } = "";
        public string Path2 { get; set; } = "";
        public string Path3 { get; set; } = "";
        public string Path4 { get; set; } = "";
        public string Path5 { get; set; } = "";
        public string Path6 { get; set; } = "";
        public string Path7 { get; set; } = "";
        public string Path8 { get; set; } = "";
        public string Path9 { get; set; } = "";
        public string Path10 { get; set; } = "";
        public string Path11 { get; set; } = "";
        public string Path12 { get; set; } = "";
        public string Path13 { get; set; } = "";
        public string Path14 { get; set; } = "";

        public bool Path1State { get; set; } = true;
        public bool Path2State { get; set; } = false;
        public bool Path3State { get; set; } = false;
        public bool Path4State { get; set; } = false;
        public bool Path5State { get; set; } = false;
        public bool Path6State { get; set; } = false;
        public bool Path7State { get; set; } = false;
        public bool Path8State { get; set; } = false;
        public bool Path9State { get; set; } = false;
        public bool Path10State { get; set; } = false;
        public bool Path11State { get; set; } = false;
        public bool Path12State { get; set; } = false;
        public bool Path13State { get; set; } = false;
        public bool Path14State { get; set; } = false;

        public int MouseSwitch { get; set; }
        public int KeyboardLayout { get; set; }
       
        public async Task<string> GetBearer(string username)
        {
            HttpClient client = new HttpClient();
            string url_main = $"https://about-steven.de/generateBearerToken/{username}";
            string token = await client.GetStringAsync(url_main);
            return token;
        }

        public byte[] Zip(string uncompressedData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
                using (StreamWriter writer = new StreamWriter(gzipStream))
                {
                    writer.Write(uncompressedData);
                }

                return ms.ToArray();
            }
        }

        public async void SAVE(string filename)
        {
            try
            {
                var url = "https://about-steven.de/symbioticD4PostSaves";
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                var webClient = new WebClient();
                webClient.CachePolicy = noCachePolicy;
                string username = FrmLogin.Username;

                var setting = new Rotations
                {
                    Unstuck = this.Unstuck,
                    Charselect = this.Charselect,
                    DesignChanger = this.DesignChanger,
                    CrashDetection = this.CrashDetection,
                    DiscordNotifications = this.DiscordNotifications,
                    HealthSlider = this.HealthSlider,
                    Pathfinding = this.Pathfinding,
                    Fight = this.Fight,
                    Repair = this.Repair,
                    Heal = this.Heal,
                    PathGen = this.PathGen,
                    SearchItems = this.SearchItems,
                    HealKey = this.HealKey,
                    Revive = this.Revive,
                    Logout = this.Logout,
                    LogoutHour = this.LogoutHour,
                    LogoutMinute = this.LogoutMinute,
                    CooldownDetection = this.CooldownDetection,
                    Cooldown1 = this.Cooldown1,
                    Cooldown2 = this.Cooldown2,
                    Cooldown3 = this.Cooldown3,
                    Cooldown4 = this.Cooldown4,
                    Holddown1 = this.Holddown1,
                    Holddown2 = this.Holddown2,
                    Holddown3 = this.Holddown3,
                    Holddown4 = this.Holddown4,
                    Prioritize1 = this.Prioritize1,
                    Prioritize2 = this.Prioritize2,
                    Prioritize3 = this.Prioritize3,
                    Prioritize4 = this.Prioritize4,
                    Doubleclick1 = this.Doubleclick1,
                    Doubleclick2 = this.Doubleclick2,
                    Doubleclick3 = this.Doubleclick3,
                    Doubleclick4 = this.Doubleclick4,
                    MouseSwitch = this.MouseSwitch,
                    KeyboardLayout = this.KeyboardLayout,
                    Path1State = this.Path1State,
                    Path2State = this.Path2State,
                    Path3State = this.Path3State,
                    Path4State = this.Path4State,
                    Path5State = this.Path5State,
                    Path6State = this.Path6State,
                    Path7State = this.Path7State,
                    Path8State = this.Path8State,
                    Path9State = this.Path9State,
                    Path10State = this.Path10State,
                    Path11State = this.Path11State,
                    Path12State = this.Path12State,
                    Path13State = this.Path13State,
                    Path14State = this.Path14State
                };
                var serialized = JsonConvert.SerializeObject(setting);
                var settings = FrmLogin.Blow1.Encrypt_CTR(serialized);
                var paths = new Rotations
                {
                    Path1 = FrmLogin.Blow1.Encrypt_CTR(Path1),
                    Path2 = FrmLogin.Blow1.Encrypt_CTR(Path2),
                    Path3 = FrmLogin.Blow1.Encrypt_CTR(Path3),
                    Path4 = FrmLogin.Blow1.Encrypt_CTR(Path4),
                    Path5 = FrmLogin.Blow1.Encrypt_CTR(Path5),
                    Path6 = FrmLogin.Blow1.Encrypt_CTR(Path6),
                    Path7 = FrmLogin.Blow1.Encrypt_CTR(Path7),
                    Path8 = FrmLogin.Blow1.Encrypt_CTR(Path8),
                    Path9 = FrmLogin.Blow1.Encrypt_CTR(Path9),
                    Path10 = FrmLogin.Blow1.Encrypt_CTR(Path10),
                    Path11 = FrmLogin.Blow1.Encrypt_CTR(Path11),
                    Path12 = FrmLogin.Blow1.Encrypt_CTR(Path12),
                    Path13 = FrmLogin.Blow1.Encrypt_CTR(Path13),
                    Path14 = FrmLogin.Blow1.Encrypt_CTR(Path14),
                };

                byte[] path1 = Zip(Path1);
                byte[] path2 = Zip(Path2);
                byte[] path3 = Zip(Path3);
                byte[] path4 = Zip(Path4);
                byte[] path5 = Zip(Path5);
                byte[] path6 = Zip(Path6);
                byte[] path7 = Zip(Path7);
                byte[] path8 = Zip(Path8);
                byte[] path9 = Zip(Path9);
                byte[] path10 = Zip(Path10);
                byte[] path11 = Zip(Path11);
                byte[] path12 = Zip(Path12);
                byte[] path13 = Zip(Path13);
                byte[] path14 = Zip(Path14);


                var requestData = new
                {
                    username,
                    filename,
                    settings,
                    path1,
                    path2,
                    path3,
                    path4,
                    path5,
                    path6,
                    path7,
                    path8,
                    path9,
                    path10,
                    path11,
                    path12,
                    path13,
                    path14
                };

                var jsonData = JsonConvert.SerializeObject(requestData);

                try
                {
                    string bearer = await GetBearer(username);
                    webClient.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearer}");
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    var response = webClient.UploadData(url, "POST", Encoding.UTF8.GetBytes(jsonData));
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                    }
                    else
                    {
                        Alert.Show("Try Again later.\n\n" +
                          "Webserver currently not Available!");
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static async Task<Rotations> Load(string filename, CancellationToken token)
        {

            try
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                var url = $"https://about-steven.de/symbioticD4GetSave/{FrmLogin.Username}/{filenameWithoutExtension}/";
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                var webClient = new WebClient();
                webClient.CachePolicy = noCachePolicy;
                var result = "";
                webClient.DownloadStringAsync(new Uri(url));
                webClient.DownloadStringCompleted += (s, e) =>
                {
                    result = e.Result;
                };

                while (result == "")
                {
                    await Task.Delay(100);
                }
                var entries = JArray.Parse(result)[0];

                string settingsData = (string)entries["settings"];
                string path1 = (string)entries["path1"];
                string path2 = (string)entries["path2"];
                string path3 = (string)entries["path3"];
                string path4 = (string)entries["path4"];
                string path5 = (string)entries["path5"];
                string path6 = (string)entries["path6"];
                string path7 = (string)entries["path7"];
                string path8 = (string)entries["path8"];
                string path9 = (string)entries["path9"];
                string path10 = (string)entries["path10"];
                string path11 = (string)entries["path11"];
                string path12 = (string)entries["path12"];
                string path13 = (string)entries["path13"];
                string path14 = (string)entries["path14"];

                Rotations rotations = new Rotations();
                string decryptedData = FrmLogin.Blow1.Decrypt_CTR((string)entries["settings"]);
                rotations = JsonConvert.DeserializeObject<Rotations>(decryptedData);
                for (int i = 1; i <= 14; i++)
                {
                    string path = (string)entries[$"path{i}"];

                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(path)))
                    {
                        using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Decompress))
                        using (StreamReader reader = new StreamReader(gzipStream))
                        {
                            string uncompressedData = reader.ReadToEnd();
                            //decryptedData = FrmLogin.Blow1.Decrypt_CTR(uncompressedData);

                            switch (i)
                            {
                                case 1:
                                    rotations.Path1 = uncompressedData;
                                    break;
                                case 2:
                                    rotations.Path2 = uncompressedData;
                                    break;
                                case 3:
                                    rotations.Path3 = uncompressedData;
                                    break;
                                case 4:
                                    rotations.Path4 = uncompressedData;
                                    break;
                                case 5:
                                    rotations.Path5 = uncompressedData;
                                    break;
                                case 6:
                                    rotations.Path6 = uncompressedData;
                                    break;
                                case 7:
                                    rotations.Path7 = uncompressedData;
                                    break;
                                case 8:
                                    rotations.Path8 = uncompressedData;
                                    break;
                                case 9:
                                    rotations.Path9 = uncompressedData;
                                    break;
                                case 10:
                                    rotations.Path10 = uncompressedData;
                                    break;
                                case 11:
                                    rotations.Path11 = uncompressedData;
                                    break;
                                case 12:
                                    rotations.Path12 = uncompressedData;
                                    break;
                                case 13:
                                    rotations.Path13 = uncompressedData;
                                    break;
                                case 14:
                                    rotations.Path14 = uncompressedData;
                                    break;
                                default:
                                    // Handle invalid path index
                                    break;
                            }
                        }
                    }
                }

                //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(path1)))
                //{
                //    using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Decompress))
                //    using (StreamReader reader = new StreamReader(gzipStream))
                //    {
                //        string uncompressedData = reader.ReadToEnd();
                //   

                //       
                //    }
                //}
                return rotations;
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<Rotations>(FrmLogin.Blow1.Decrypt_CTR(null));
            }
        }

        //public async void SAVE2(string filename)
        //{
        //    try
        //    {
        //        var url = "https://about-steven.de/symbioticD4PostSaves";
        //        var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
        //        var webClient = new WebClient();
        //        webClient.CachePolicy = noCachePolicy;
        //        string username = FrmLogin.Username;
        //        Rotations setting = this;
        //        var jsonData = JsonConvert.SerializeObject(setting);
        //        var uncompressedData = FrmLogin.Blow1.Encrypt_CTR(jsonData);
        //        byte[] settings;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
        //            using (StreamWriter writer = new StreamWriter(gzipStream))
        //            {
        //                writer.Write(uncompressedData);
        //            }

        //            settings = ms.ToArray();
        //        }

        //        var requestData = new
        //        {
        //            username,
        //            filename,
        //            settings
        //        };

        //        jsonData = JsonConvert.SerializeObject(requestData);

        //        try
        //        {
        //            string bearer = await GetBearer(username);
        //            webClient.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearer}");
        //            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        //            var response = webClient.UploadData(url, "POST", Encoding.UTF8.GetBytes(jsonData));
        //        }
        //        catch (WebException ex)
        //        {
        //            if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Unauthorized)
        //            {
        //            }
        //            else
        //            {
        //                Alert.Show("Try Again later.\n\n" +
        //                  "Webserver currently not Available!");
        //            }
        //        }
        //    }
        //    catch (Exception ex) { }
        //}

        //public void Save(string filename)
        //{
        //    try
        //    {
        //        string output = JsonConvert.SerializeObject(this);
        //        /* Save all Configurations additional within Database for future changes */
        //        //var config = Config.Load();

        //        //File.WriteAllText(Directory.GetCurrentDirectory() + @"\" + HWID.Get() + @"\" + filename + ".ini", output);
        //        var url = $"https://about-steven.de/symbioticD4PostSaves/{LoginData.username}/{filename}/{FrmLogin.Blow1.Encrypt_CTR(output)}";
        //        var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
        //        var webClient = new WebClient();
        //        webClient.CachePolicy = noCachePolicy;

        //        var response = webClient.UploadData(url, "POST", Encoding.UTF8.GetBytes(""));
        //        var result = Encoding.UTF8.GetString(response);
        //        var responseString = result;
        //    }
        //    catch (Exception ex)
        //    {

        //    }


        //}


    }
}
