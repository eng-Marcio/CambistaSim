using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace CambistaSim
{
    class PatrimonyControl
    {
        #region attributes
        private Controler controler;
        private Agent agent;

        public List<Double> Patrimony;
        public List<Double> PatrimonyVariation;
        public List<DateTime> times;

        public double cashBR;
        public double cashOut;
        public List<(Double, Double, DateTime)> retainer;  //first double is for national money, second is for foreign money, the date is the date of liquidation

        internal Controler Controler { get => controler; }
        internal Agent Agent { get => agent; }


        #endregion

        #region constructor
        public PatrimonyControl(Controler controler, Agent agent, double cash)
        {
            this.controler = controler;
            this.agent = agent;
            this.cashBR = cash;
            this.cashOut = 0;
            retainer = new List<(Double, Double, DateTime)>();
            this.Patrimony = new List<Double>();
            this.PatrimonyVariation = new List<Double>();
            this.times = new List<DateTime>();
            Patrimony.Add(cash);
            PatrimonyVariation.Add(0);
            times.Add(controler.Game.CurrentDate - TimeSpan.FromDays(1));
        }
        #endregion
    }
}
