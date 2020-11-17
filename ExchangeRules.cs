using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class ExchangeRules
    {
        #region attributes
        private double taxOut;
        private double operationOut;
        private int delayOut;
        private double taxIn;
        private double operationIn;
        private int delayIn;
        private double monthlyCost;
        private double deadline;

        public double TaxOut { get => taxOut; }
        public double OperationOut { get => operationOut; }
        public int DelayOut { get => delayOut;}
        public double TaxIn { get => taxIn; }
        public double OperationIn { get => operationIn; }
        public int DelayIn { get => delayIn; }
        public double MonthlyCost { get => monthlyCost; }
        public double Deadline { get => deadline; }

        #endregion

        #region constructor
        public ExchangeRules(double taxOut, double operationOut, int delayOut, double taxIn, double operationIn, int delayIn, double monthlyCost, double deadline)
        {
            this.taxOut = taxOut;
            this.operationOut = operationOut;
            this.delayOut = delayOut;
            this.taxIn = taxIn;
            this.operationIn = operationIn;
            this.delayIn = delayIn;
            this.monthlyCost = monthlyCost;
            this.deadline = deadline;
        }

        #endregion

        #region methods
        public (double,int) transferIn(double money, double rate)  
        {
            double toTransfer = money * (1 - taxIn) - operationIn;
            return (Math.Round(toTransfer*rate, 2), this.delayIn);
        }
        public (double, int) transferOut(double money, double rate)
        {
            double toTransfer = money * (1 - taxOut) - operationOut ;
            return (Math.Round(toTransfer / rate, 2), this.delayOut);
        }
        #endregion
    }
}
