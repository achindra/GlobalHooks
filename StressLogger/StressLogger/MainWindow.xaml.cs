using System.Threading;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace StressLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHook keyboard;
        private ContextMenu ctxMenu;
        
        int noOfChars = 0;
        bool hasSpecialKey = false;
        string typedText;
        bool skipWord = false;

        public static Mutex _mutex = null;
        public MainWindow()
        {            
            InitializeComponent();

            bool isOwned = false;            
            _mutex = new Mutex(true, "StressLogger", out isOwned);
            if(!isOwned)
            {
                App.Current.Shutdown();
                return;
            }

            if (Priviledge.Escalate())
            {
                Utils.CheckInstall();
                Utils.WriteRegistry();

                ctxMenu = new ContextMenu();
                ctxMenu.MenuItems.Add("E&xit", new EventHandler(Menu_Exit));
                ctxMenu.MenuItems.Add("&Open", new EventHandler(Menu_Open));

                TrayIcon.notifyIcon.ContextMenu = ctxMenu;
                TrayIcon.SetupTrayIcon();

                Watch.SetupWatch();

                keyboard = new KeyboardHook();
                keyboard.KeyPressEvent += Keyboard_KeyPressEvent;

                DispatchTimer.Init();
                
                this.ShowInTaskbar = false;
                this.WindowState = WindowState.Minimized;

                List<String> data = new List<string>();
                data.Add("All"); data.Add("Bubble"); data.Add("Line"); data.Add("Column");
                comboBox.ItemsSource = data; comboBox.SelectedIndex = 0;
                
                comboBox.SelectionChanged += ComboBox_SelectionChanged;
                myChart.DataContext = DataPoints.chartData;
            }
            Hide();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //var comboBox = sender as ComboBox;
            myChart.Series.Clear();
            if(comboBox.SelectedItem.ToString().Equals("Bubble"))
            {
                myChart.Series.Add(BubbleSeries);
            } else if (comboBox.SelectedItem.ToString().Equals("Line"))
            {
                myChart.Series.Add(LineSeries);
            } else if (comboBox.SelectedItem.ToString().Equals("Column"))
            {
                myChart.Series.Add(ColumnSeries);
            } else
            {
                myChart.Series.Add(BubbleSeries);
                myChart.Series.Add(LineSeries);
                myChart.Series.Add(ColumnSeries);
            }
        }

        private void Menu_Open(object sender, EventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Normal;
            TrayIcon.notifyIcon_WindowResizeHandler();
        }

        private void Menu_Exit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
        
        private void Keyboard_KeyPressEvent(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.LeftShift ||
                e.KeyPressed == Key.RightShift ||
                e.KeyPressed == Key.LeftCtrl ||
                e.KeyPressed == Key.RightCtrl)
            {
                hasSpecialKey = true;
            }

            if (e.KeyPressed >= Key.A && e.KeyPressed <= Key.Z)
            {
                if (!Watch.watch.IsRunning)
                    Watch.watch.Start();
                typedText += e.KeyPressed.ToString();
                noOfChars++;
            }
            else if((e.KeyPressed >= Key.NumPad0 && e.KeyPressed <= Key.NumPad9) ||
                (e.KeyPressed >= Key.D0 && e.KeyPressed <= Key.D9))
            {
                //reset; alphanumeric data
                skipWord = true;                
            }

            switch (e.KeyPressed)
            {
                case Key.Return:
                case Key.Escape:
                case Key.Space:
                case Key.Tab:
                case Key.OemComma:
                case Key.Decimal:
                case Key.OemMinus:
                case Key.OemPeriod:
                case Key.OemQuestion:
                case Key.OemSemicolon:
                    if (0 != noOfChars && false == skipWord)
                    {
                        DataPoints sample = new DataPoints(
                                                            typedText,
                                                            noOfChars,
                                                            Watch.watch.ElapsedMilliseconds,
                                                            hasSpecialKey,
                                                            DateTime.UtcNow);
                        
                        lock (DataPoints.dataSample)
                        {
                            DataPoints.dataSample.Add(sample);
                        }
                        TrayIcon.notifyIcon.BalloonTipText = DataPoints.dataSample.Count +
                                                             " Samples collected";
                    }
                    Watch.watch.Reset();
                    typedText = "";
                    noOfChars = 0;
                    hasSpecialKey = false;
                    break;

                default:
                    break;
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Handle cleanup
            DispatchTimer.Shutdown();
            TrayIcon.notifyIcon.Dispose();
        }
        
        private void Window_StateChanged(object sender, EventArgs e)
        {
            TrayIcon.notifyIcon_WindowResizeHandler();            
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            DispatchTimer.dispatchTimer_Tick(this, null);
            myChart.DataContext = DataPoints.chartData;
        }
    }
}
