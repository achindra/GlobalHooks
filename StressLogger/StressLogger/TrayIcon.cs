using System;
using System.Windows;
using System.Windows.Forms;

namespace StressLogger
{
    public static class TrayIcon
    {
        public static NotifyIcon notifyIcon = new NotifyIcon();
        public static void SetupTrayIcon()
        {
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.Icon = Resource.text_log;
            notifyIcon.BalloonTipTitle = "StressLogger";
            notifyIcon.BalloonTipText = "Started...";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Text = "StressLogger";
            notifyIcon.Visible = true;
        }

        private static void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            notifyIcon_WindowResizeHandler();
        }

        private static void notifyIcon_Click(object sender, EventArgs e)
        {
            notifyIcon.ShowBalloonTip(2000);
        }

        public static void notifyIcon_WindowResizeHandler()
        {
            if(App.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
            {
                App.Current.MainWindow.ShowInTaskbar = true;
                App.Current.MainWindow.Visibility = Visibility.Visible;
                App.Current.MainWindow.Show();
                notifyIcon.Visible = false;
            }
            else
            {
                //myChart.Series.Clear();
                DataPoints.chartData.Clear();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                App.Current.MainWindow.Hide();
                App.Current.MainWindow.ShowInTaskbar = false;
                App.Current.MainWindow.Visibility = Visibility.Hidden;
                GC.Collect();
            }
        }
    }
}
