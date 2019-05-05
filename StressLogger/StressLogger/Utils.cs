using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.IO;
using System;

namespace StressLogger
{
    public static class Utils
    {
        public static bool WriteRegistry()
        {
            try
            {
                String appName = Process.GetCurrentProcess().MainModule.ModuleName;
                String systemPath = Environment.SystemDirectory;

                RegistryKey regHandle = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                regHandle.SetValue("StressLogger", Path.Combine(systemPath, appName));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "StressLogger",
                    MessageBoxButton.OK);
                return false;
            }
        }

        public static bool CheckInstall()
        {
            try
            {
                String appName = Process.GetCurrentProcess().MainModule.ModuleName;
                String appPath = Assembly.GetExecutingAssembly().Location;
                String systemPath = Environment.SystemDirectory;
                if (!appPath.Equals(systemPath))
                {
                    File.Copy(appPath, 
                        Path.Combine(systemPath, appName),
                        true);
                }
                    
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "StressLogger",
                    MessageBoxButton.OK);
                return false;
            }
        }
    }
}
