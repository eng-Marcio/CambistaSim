using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class AgentMM5 : Agent
    {
        [NonSerialized()] private double[] history5 = new double[10];
        public AgentMM5(Game game, string name, bool isHuman) : base(game, name, isHuman)
        {
        }

        public override (double, bool) determineMove(int marketTime, double preOpen, double open, double max, double min, double close)
        {
            if (marketTime == Controler.CLOSED)
            {
                if(history5.Contains(0.0))
                {
                    for(int i = 0; i < history5.Length; i++)
                    {
                        history5[i] = close;
                    }
                    return (0, false);
                }
                double cashBr = Game.Controller.GetPatrimonyByAgent((Agent)this).cashBR;
                double cashOut = Game.Controller.GetPatrimonyByAgent((Agent)this).cashOut;
                double meanPrice = history5.Average();
                if ((cashBr > cashOut) && (close > meanPrice)) //sell all money
                {
                    return (cashBr, true);
                }
                else if ((cashBr < cashOut) && (close < meanPrice)) //buy back all money
                {
                    return (cashOut, false);
                }

                //rotate values on array
                for(int i = 1; i < history5.Length; i++)
                {
                    history5[i - 1] = history5[i];
                }
                history5[history5.Length - 1] = close;
            }
            
            return (0, false);
            
        }
    }
}
