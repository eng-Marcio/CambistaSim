using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class Game 
    {
        #region attributes
        [NonSerialized()] private Controler controler;
        private List<Agent> agents;
        private ExchangeRules exchangeRules;
        
        private String name;
        private int coin;
        private DateTime startingDate;
        private double startingMoney;
        private DateTime currentDate;
        private int currentTime = Controler.PRE_OPEN;

        private double multiplier;



        public Controler Controller { get => controler; set => controler = value; }
        public List<Agent> Agents { get => agents; set => agents = value; }
        public ExchangeRules ExchangeRules { get => exchangeRules;  }
        
        public string Name { get => name; }
        public int Coin { get => coin; }
        public DateTime StartingDate { get => startingDate; }
        public double StartingMoney { get => startingMoney; }
        public DateTime CurrentDate { get => currentDate; set => currentDate = value; }
        public int CurrentTime { get => currentTime; set => currentTime = value; }
        public double Multiplier { get => multiplier; set => multiplier = value; }



        #endregion

        #region Constructor
        public Game(Controler controler, String name, int coin, DateTime startingDate, double startingMoney, 
                    double taxOut, double operationOut, int delayOut, double taxIn, double operationIn, int delayIn, double monthlyCost, double deadline,
                    bool[] players)
        {
            this.controler = controler;
            this.name = name;
            this.coin = coin;
            this.startingDate = startingDate;
            this.currentDate = startingDate;
            this.startingMoney = startingMoney;
            this.exchangeRules = new ExchangeRules(taxOut, operationOut, delayOut, taxIn, operationIn,  delayIn, monthlyCost, deadline);
            setPlayers(players);
            multiplier = ((new Random()).NextDouble() * 3.8) + 0.2; //generates a random double between 0 and 4
        }

        #endregion

        #region methods
        private void setPlayers(bool[] players)
        {
            this.agents = new List<Agent>();
            if (players[0])
            {
                agents.Add(new AgentP1(this, "Player", true));
            }
            if (players[1])
            {
                agents.Add(new AgentMM5(this, "Moving Mean 10D", false));
            }
            if (players[2])
            {
                agents.Add(new AgentMM55(this, "Moving Mean 10D + 1%", false));
            }
            if (players[3])
            {
                agents.Add(new AgentDayTrader(this,"Day Trader", false));
            }
        }
        #endregion
    }
}
