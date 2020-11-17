using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambistaSim
{
    class AgentP1 : Agent
    {
        public AgentP1(Game game, string name, bool isHuman) : base(game, name, isHuman)
        {
        }

        public override (double, bool) determineMove(int marketTime, double preOpen, double open, double max, double min, double close)
        {
            return (0, false);
        }
    }
}
