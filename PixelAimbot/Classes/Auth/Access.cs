using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Windows.Forms;
using PixelAimbot.Classes.Misc;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;

namespace PixelAimbot.Classes.Auth
{
    public static class Access
    {
        public class UserData
        {
            public string error { get; set; }
            public int id_username { get; set; }
            public int id_role { get; set; }
            public string discord { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string hwid { get; set; }
            public int trial { get; set; }
            public string expiredate { get; set; }
          
        }
        public static void CheckAccessAsyncCall(string hwid)
        {
            try
            {
                if (FrmLogin.Username == "")
                {
                    Alert.Show("Username field is empty!", FrmAlert.EnmType.Info);
                }else
                if (FrmLogin.Password == "")
                {
                    Alert.Show("Password field is empty!", FrmAlert.EnmType.Info);
                }
                else
                {
                    var url = "";
                   if (hwid != "") 
                        url = $"https://about-steven.de/symbioticGetCheckUser/{FrmLogin.Username}/{FrmLogin.Password}/{hwid}";
                   else
                        url = $"https://about-steven.de/symbioticGetCheckUser/{FrmLogin.Username}/{FrmLogin.Password}/{HWID.Get()}";
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    var webClient = new WebClient();
                    webClient.CachePolicy = noCachePolicy;

                    webClient.DownloadStringAsync(new Uri(url));
                    webClient.DownloadStringCompleted += (s, e) =>
                    {
                        try
                        {
                            CheckAccess(e.Result);
                        }
                        catch (Exception ex)
                        {
                            //   ExceptionHandler.SendException(ex);
                            Alert.Show("Webserver currently not Available!\n" +
                                        "Try Again later.", FrmAlert.EnmType.Error);
                        }
                    };
                }
               
            }
            catch (Exception ex)
            {
              //   ExceptionHandler.SendException(ex);
                Alert.Show("Webserver currently not Available!\n" +
                            "Try Again later.", FrmAlert.EnmType.Error);
            }
        }

        public static bool CheckAccess(string response)
        {
            try
            {
              
                UserData user = JsonConvert.DeserializeObject<UserData>(response);
                switch (user.error)
                {
                    case "expired":
                        Alert.Show("Licence is not active or expired.\n" +
                               "Please contact an Administrator.", FrmAlert.EnmType.Error);
                        return false;
                    case "username":
                        Alert.Show("Username not known.\n" +
                               "Please contact an Administrator.",
                       FrmAlert.EnmType.Error);
                        return false;
                    case "password":
                        Alert.Show("Password is wrong.\n" +
                              "Please contact an Administrator.",
                      FrmAlert.EnmType.Error);
                        return false;
                    case "hwid":
                        Alert.Show("Your HWID seems changed,\n" +
                                "please reset it or contact an Administrator.",
                        FrmAlert.EnmType.Error);
                        return false;
                }
                if (Application.OpenForms.OfType<DiabloBot>().Count() == 1)
                    Application.OpenForms.OfType<DiabloBot>().First().Close();

                DiabloBot form = new DiabloBot();
            
                //if (FrmLogin.LicenceInformations["discord"]?.ToString() != "")
                //{
                //    form.Conf.discorduser = FrmLogin.LicenceInformations["discord"]?.ToString();
                //    form.Conf.Save();
                //}

                form.Show();
                Application.OpenForms.OfType<FrmLogin>().First().Hide();

                return true;


            }
            catch (WebException)
            {
                Alert.Show("Server is not reachable,\n" +
                            "please try again later.", FrmAlert.EnmType.Error);
                return false;
            }
            catch (Exception ex)
            {
              //   ExceptionHandler.SendException(ex);
                Alert.Show(ex.Message, FrmAlert.EnmType.Error);
                return false;
            }
        }
    }
}