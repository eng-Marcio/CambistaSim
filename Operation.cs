using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class Operation
    {
        #region attributes
        private Agent agent;

        public static int Cash_Deposit = 0;
        public static int Transfer_In = 1;
        public static int Transfer_Out = 2;

        public int Type;
        public double Quantity;
        public double Result;
        public int RetainedTime;

        public DateTime TransactionDate;
        #endregion
    }
}
