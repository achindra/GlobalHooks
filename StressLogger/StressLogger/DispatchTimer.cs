using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Collections.Generic;

namespace StressLogger
{

    public static class DispatchTimer
    {
        public static DispatcherTimer timer;

        public static void Init()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += new EventHandler(dispatchTimer_Tick);
            timer.Start();

            try
            {
                String t = File.ReadAllText(@"dataSample.json");
                var _result = JsonConvert.DeserializeObject<List<DataPoints>>(t);
                DataPoints.chartData = _result;
            }
            catch
            {
                DataPoints.chartData = null;
            }
            
        }

        public static void Shutdown()
        {
            timer?.Stop();
            dispatchTimer_Tick(null, null);
        }

        public static void dispatchTimer_Tick(object sender, EventArgs e)
        {
            string json;
            if (DataPoints.dataSample.Count > 0)
            {
                lock (DataPoints.dataSample)
                {
                    json = JsonConvert.SerializeObject(DataPoints.dataSample);
                    DataPoints.dataSample.Clear();
                }
                FileStream fs = new FileStream(@"dataSample.json", FileMode.OpenOrCreate);
                if (fs.Length > 0)
                {
                    fs.SetLength(fs.Length - 1);
                }
                fs.Close();
                File.AppendAllText(@"dataSample.json", json.Replace('[',','));
            }
        }
    }
}
