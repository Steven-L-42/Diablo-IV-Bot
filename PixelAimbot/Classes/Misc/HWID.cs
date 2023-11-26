using DeviceId;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace PixelAimbot.Classes.Misc
{
    internal class HWID
    {

        public static string Get()
        {
            string cpuInfo = string.Empty;
            string driveInfo = string.Empty;
            string macAddress = string.Empty;

            ManagementObjectCollection moc = new ManagementClass("win32_processor").GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            moc = new ManagementClass("win32_logicaldisk").GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (driveInfo == "" && mo.Properties["description"].Value.ToString() == "Local Fixed Disk")
                {
                    driveInfo = mo.Properties["volumeSerialNumber"].Value.ToString();
                    break;
                }
            }

            moc = new ManagementClass("win32_networkadapterconfiguration").GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (macAddress == "")
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        macAddress = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }

            string combinedInfo = cpuInfo + driveInfo + macAddress;
            return GetHash(combinedInfo);
        }

        private static string GetHash(string value)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            return GetHexString(sec.ComputeHash(bytes));
        }

        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                s += b.ToString("X2");
            }
            return s.ToLower();
        }
    }
}
