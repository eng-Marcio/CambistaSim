using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;

namespace CambistaSim
{
    class IOControl
    {
        private Controler controler;

        private DateTime startDate;
        private DateTime currentDate;

        string[] currentLine;

        private StreamReader priceReader;
        public Controler Controler { get => controler; }
        public DateTime CurrentDate { get => currentDate; }

        #region constructor
        public IOControl(Controler controler)
        {
            this.controler = controler;
            //initialize data reading objects

        }
        #endregion

        #region methods
        public void setCoin(int coin, DateTime startDate, DateTime currentDate)
        {
            this.startDate = startDate;
            this.currentDate = currentDate;

            string source;
            switch (coin)
            {
                case Controler.EUR:
                    source = "EUR";
                    break;
                case Controler.USD:
                    source = "USD";
                    break;
                default:
                    throw new Exception("No coin set");
            }

            priceReader = new StreamReader("./HistoryFiles/" + source + "_BRL.csv");
            
            //go until it finds starting date on csv
            DateTime readedDate = new DateTime(2001, 5, 25);
            priceReader.ReadLine(); // jump header
            while (DateTime.Compare(readedDate, (startDate - TimeSpan.FromDays(1))) < 0)
            {
                currentLine = priceReader.ReadLine().Split(';');
                readedDate = getDate(currentLine[0]);
            }
        }

        public static DateTime getDate(string date)
        {
            char[] date_ = date.ToCharArray();
            int day = Int32.Parse("" + date_[0] + date_[1]);
            int month = Int32.Parse("" + date_[2] + date_[3]);
            int year = Int32.Parse("" + date_[4] + date_[5] + date_[6] + date_[7]);
            return new DateTime(year, month, day);
        }

        public string[] getNextDayInfo()
        {
            currentLine = priceReader.ReadLine().Split(';');
            currentDate = getDate(currentLine[0]);
            return currentLine;
        }
        #endregion
    }
}
