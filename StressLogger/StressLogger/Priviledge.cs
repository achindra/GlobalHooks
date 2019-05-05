using System;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows;

namespace StressLogger
{
    public static class Priviledge
    {
        public static bool Escalate()
        {
            bool isAdmin = false;
            try
            {
                WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    isAdmin = true;
                }
                else
                {
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.UseShellExecute = true;
                    proc.WorkingDirectory = Environment.CurrentDirectory;
                    proc.FileName = System.Windows.Forms.Application.ExecutablePath;
                    proc.Verb = "runas";

                    try
                    {
                        Process.Start(proc);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Application cannot run without higher priviledges",
                            "StressLogger!",
                            MessageBoxButton.OK);
                    }
                    App.Current.Shutdown();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
                MessageBox.Show(
                    ex.Message, 
                    "StressLogger", 
                    MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                isAdmin = false;
                MessageBox.Show(
                    ex.Message, 
                    "StressLogger", 
                    MessageBoxButton.OK);
            }

            return isAdmin;
        }
    }
}
