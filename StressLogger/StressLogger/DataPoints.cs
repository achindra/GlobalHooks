using System;
using System.Collections.Generic;

namespace StressLogger
{
    [Serializable]
    public class DataPoints
    {
        private String thisWord;
        private int charCount;
        private long timeDelta;
        private bool hasSpecial;
        private DateTime utcTime;

        public static List<DataPoints> dataSample = new List<DataPoints>();
        public static List<DataPoints> chartData = new List<DataPoints>();

        public DataPoints()
        {
            this.thisWord = "";
            this.charCount = 0;
            this.timeDelta = 0;
            this.hasSpecial = false;
            this.utcTime = DateTime.UtcNow;
        }
        public DataPoints(String thisWord, int charCount, long timeDelta, bool hasSpecial, DateTime now)
        {
            this.thisWord = thisWord;
            this.charCount = charCount;
            this.timeDelta = timeDelta;
            this.hasSpecial = hasSpecial;
            this.utcTime = now;
        }

        public String ThisWord
        {
            get { return thisWord; }
            set { thisWord = value; }
        }

        public int CharCount
        {
            get { return charCount; }
            set { charCount = value; }
        }

        public long TimeDelta
        {
            get { return timeDelta; }
            set { timeDelta = value; }
        }

        public bool HasSpecial
        {
            get { return hasSpecial; }
            set { hasSpecial = value; }
        }

        public DateTime UtcTime
        {
            get { return utcTime; }
            set { utcTime = value; }
        }

    }

}
