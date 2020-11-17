using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    abstract class Agent
    {
        #region attributes
        private Game game;
        private bool humanPlayer = false;
        public string name;
        private List<Operation> operations;

        //defines order
        private bool orderSet = false;
        private bool moneyOut = false;
        private double value;
        private double priceTarget;

        public bool HumanPlayer { get => humanPlayer; }
        public bool OrderSet { get => orderSet; }
        public bool MoneyOut { get => moneyOut; }
        public double Value { get => value; }
        public double PriceTarget { get => priceTarget; }
        public Game Game { get => game; }
        public List<Operation> Operations { get => operations; }
        #endregion

        #region constructor
        public Agent(Game game, string name, bool isHuman)
        {
            this.game = game;
            this.name = name;
            this.humanPlayer = isHuman;
            this.operations = new List<Operation>();
        }
        #endregion

        #region methods
        public abstract (double, bool) determineMove(int marketTime, double preOpen, double open, double max, double min, double close);

        public void setOrder(bool moneyOut, double value, double priceTarget)
        {
            orderSet = true;
            this.moneyOut = moneyOut;
            this.value = value;
            this.priceTarget = priceTarget;
        }
        public void resetOrder()
        {
            orderSet = false;
        }
        #endregion
    }
}
