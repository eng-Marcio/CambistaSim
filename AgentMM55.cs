using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class AgentMM55 : Agent
    {
        [NonSerialized()] private double[] history5 = new double[10];
        public AgentMM55(Game game, string name, bool isHuman) : base(game, name, isHuman)
        {
        }

        public override (double, bool) determineMove(int marketTime, double preOpen, double open, double max, double min, double close)
        {
            if (marketTime == Controler.CLOSED)
            {
                if (history5.Contains(0.0))
                {
                    for (int i = 0; i < history5.Length; i++)
                    {
                        history5[i] = close;
                    }
                    return (0, false);
                }

                //rotate values on array
                for (int i = 1; i < history5.Length; i++)
                {
                    history5[i - 1] = history5[i];
                }
                history5[history5.Length - 1] = close;
            }
            if(marketTime == Controler.PRE_OPEN)
            {
                double cashBr = Game.Controller.GetPatrimonyByAgent((Agent)this).cashBR;
                double cashOut = Game.Controller.GetPatrimonyByAgent((Agent)this).cashOut;
                double meanPrice = history5.Average();
                if (cashBr > cashOut) //set order to sell
                {
                    double target = meanPrice * 1.01; //1% above moving mean curve
                    this.setOrder(true, cashBr, target);
                }
                else if (cashBr < cashOut) //set order to buy
                {
                    double target = meanPrice;
                    this.setOrder(false, cashOut, target);
                }
            }

            return (0, false);
        }
    }
}
