using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    [Serializable()]
    class AgentDayTrader : Agent
    {
        public AgentDayTrader(Game game, string name, bool isHuman) : base(game, name, isHuman)
        {
        }

        public override (double, bool) determineMove(int marketTime, double preOpen, double open, double max, double min, double close)
        {
            double cashBr = Game.Controller.GetPatrimonyByAgent((Agent)this).cashBR;
            double cashOut = Game.Controller.GetPatrimonyByAgent((Agent)this).cashOut;
            if(marketTime == Controler.POS_OPEN)
            {
                if((open > preOpen) && (cashBr > 0))
                {
                    return (cashBr, true);
                }
            }
            if(marketTime == Controler.CLOSED || marketTime == Controler.PRE_OPEN)
            {
                if(cashOut > 0)
                {
                    return (cashOut, false);
                }
            }
            return (0, false);
        }
    }
}
