using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CambistaSim
{
    class Controler
    {
        #region attributes
        private MainWindow mainWindow;
        private IOControl ioControl;
        private List<PatrimonyControl> patrimonies;
        private Game game;

        //data for charts
        public List<Double> Prices;
        public List<Double> PriceVariation;
        public List<DateTime> times;

        //exchange daily information
        private string currentLine;
        private bool starting = true;
        private double lastClosingPrice;
        private double openPrice;
        private double maxPrice;
        private double minPrice;
        private double closingPrice;

        private int marketTime = CLOSED;



        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        public List<PatrimonyControl> Patrimonies { get => patrimonies; set => patrimonies = value; }
        public IOControl IoControl { get => ioControl; set => ioControl = value; }
        public Game Game { get => game; set => game = value; }
        public double LastClosingPrice { get => lastClosingPrice; }
        public double OpenPrice { get => openPrice; }
        public double MaxPrice { get => maxPrice; }
        public double MinPrice { get => minPrice; }
        public double ClosingPrice { get => closingPrice; }
        public int MarketTime { get => marketTime; }
        #endregion

        #region const dictionary
        public const int EUR = 1;
        public const int USD = 2;
        public const int RANDOM = 3;

        public static DateTime OldestDateOnData = new DateTime(2001, 6, 1); //oldest date gathered from datasets used in the program regarding the historical exchange prices
        public static DateTime NewstDateOnData = new DateTime(2020, 7, 1); //newest data on dataset

        public const int PRE_OPEN = 0;
        public const int POS_OPEN = 1;
        public const int CLOSED = 2;
        #endregion

        #region constructor
        public Controler(MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
            ioControl = new IOControl(this);
        }
        #endregion


        #region methods
        public DateTime isValidDate(int day, int month, int year)
        {
            try
            {
                DateTime date = new DateTime(year, month, day);
                if (DateTime.Compare(date, Controler.NewstDateOnData - TimeSpan.FromDays(365.0)) <= 0)
                {
                    if (DateTime.Compare(date, Controler.OldestDateOnData) >= 0)
                    {
                        return date;
                    }
                }
                return DateTime.Today;
            }
            catch (ArgumentOutOfRangeException)
            {
                return DateTime.Today;
            }
        }
        public DateTime generateRandomStartingDate()
        {
            Random random = new Random();
            DateTime date = DateTime.Today;
            while (date.Equals(DateTime.Today))
            {
                date = isValidDate(random.Next(1, 31), random.Next(1, 13), random.Next(2001, 2010));
            }
            return date;
        }
        public int generateRandomCoin()
        {
            return (new Random()).Next(1, RANDOM - 1);
        }

        public void startSimulation(String name, int coin, DateTime startingDate, double startingMoney,
                                    double taxOut, double operationOut, int delayOut, double taxIn, double operationIn, int delayIn, double monthlyCost, double deadline,
                                    bool[] players)
        {
            ioControl.setCoin(coin, startingDate, startingDate);
            this.Game = new Game(this, name, coin, startingDate, startingMoney,
                                    taxOut, operationOut, delayOut, taxIn, operationIn, delayIn, monthlyCost, deadline,
                                    players);

            this.Prices = new List<Double>();
            this.PriceVariation = new List<Double>();
            this.times = new List<DateTime>();

            //create runtime players' patrimony
            patrimonies = new List<PatrimonyControl>();
            for (int i = 0; i < game.Agents.Count; i++)
            {
                patrimonies.Add(new PatrimonyControl(this, game.Agents.ElementAt(i), startingMoney));
                Operation op = new Operation();
                op.Type = Operation.Cash_Deposit;
                op.Quantity = startingMoney;
                op.TransactionDate = startingDate;
                game.Agents.ElementAt(i).Operations.Add(op);
            }
            processStepForward();
        }

        //operations methods
        #region operation cash_flow control
        public void processStepForward()
        {
            if ((marketTime == CLOSED) && !starting) //patrimonies must be updated before "day changes"
            {

                double var;
                if (Prices.Count == 0)
                    var = 0;
                else
                    var = CalculatePatrimonyVariation(Prices.Last(), closingPrice); //percentage price variation
                Prices.Add(closingPrice);
                PriceVariation.Add(Math.Round(var, 4));
                times.Add(game.CurrentDate);
                for (int i = 0; i < patrimonies.Count; i++)
                {
                    updatePatrimony(patrimonies.ElementAt(i), closingPrice);
                }
            }
            marketTime = (marketTime + 1) % (CLOSED + 1);
            game.CurrentTime = marketTime;

            if(marketTime == PRE_OPEN)
            {
                lastClosingPrice = closingPrice;
                string[] daily = ioControl.getNextDayInfo();
                game.CurrentDate = ioControl.CurrentDate;
                openPrice = Math.Round(Double.Parse(daily[2]) * game.Multiplier, 4);
                maxPrice = Math.Round(Double.Parse(daily[3]) * game.Multiplier, 4); 
                minPrice = Math.Round(Double.Parse(daily[4]) * game.Multiplier, 4);
                closingPrice = Math.Round(Double.Parse(daily[1]) * game.Multiplier, 4);

                if (starting)
                    lastClosingPrice = openPrice;

                //check if some money got unblocked
                for(int i = 0; i < patrimonies.Count; i++)
                {
                    for(int j = 0; j < patrimonies.ElementAt(i).retainer.Count; j++)
                    {
                        if(DateTime.Compare(patrimonies.ElementAt(i).retainer.ElementAt(j).Item3, game.CurrentDate) <= 0)
                        {
                            patrimonies.ElementAt(i).cashBR += patrimonies.ElementAt(i).retainer.ElementAt(j).Item1;
                            patrimonies.ElementAt(i).cashOut += patrimonies.ElementAt(i).retainer.ElementAt(j).Item2;
                            patrimonies.ElementAt(i).retainer.RemoveAt(j);
                        }
                    }
                }
            }
            double tempPreClo = lastClosingPrice;
            double tempOpen = openPrice;
            double tempMax = maxPrice;
            double tempMin = minPrice;
            double tempClose = closingPrice;
            if (marketTime < CLOSED)
            {
                tempClose = 0;
                tempMax = 0;
                tempMin = 0;
            }
            else if(marketTime < POS_OPEN)
            {
                tempOpen = 0;
            }
            if(!starting)
            { 
                for(int i = 0; i < game.Agents.Count; i++)
                {
                    //check orders
                    if (game.Agents.ElementAt(i).OrderSet)
                    {
                        if (marketTime == POS_OPEN)
                        {
                            if (targetPriceCrossed(game.Agents.ElementAt(i).PriceTarget, tempPreClo, tempOpen)) //order must be executed
                            {
                                executeOrder(game.Agents.ElementAt(i), tempOpen, game.Agents.ElementAt(i).Value, game.Agents.ElementAt(i).MoneyOut);
                                game.Agents.ElementAt(i).resetOrder();
                            }
                        }
                        else if (marketTime == CLOSED)
                        {
                            if (targetPriceCrossed(game.Agents.ElementAt(i).PriceTarget, tempMin, tempMax)) //check if price target was crossed during the day at any moment
                            {
                                executeOrder(game.Agents.ElementAt(i), game.Agents.ElementAt(i).PriceTarget, game.Agents.ElementAt(i).Value, game.Agents.ElementAt(i).MoneyOut);
                                game.Agents.ElementAt(i).resetOrder();
                            }
                        }
                    }

                    //check players moves
                    if (game.Agents.ElementAt(i).HumanPlayer)
                        continue;
                    (double ammount, bool moneyOut) = game.Agents.ElementAt(i).determineMove(marketTime, tempPreClo, tempOpen, tempMax, tempMin, tempClose);
                
                    if(ammount > 0)
                    {
                        double price = tempPreClo;
                        if (marketTime == POS_OPEN)
                            price = tempOpen;
                        else if (marketTime == CLOSED)
                            price = tempClose;
                        executeOrder(game.Agents.ElementAt(i), price, ammount, moneyOut);
                    }
                }
            }
            starting = false;
        }

        public static bool targetPriceCrossed(double target, double v1, double v2)
        {
            return ((target > v1 && target < v2) || (target > v2 && target < v1));
        }
        public PatrimonyControl GetPatrimonyByAgent(Agent agent)
        {
            for(int i = 0; i < patrimonies.Count; i++)
            {
                if (agent.Equals(patrimonies.ElementAt(i).Agent))
                    return patrimonies.ElementAt(i);
            }
            return null;
        }

        public void updatePatrimony(PatrimonyControl patrimony, double price)
        {
            double sumPatrimony = patrimony.cashBR + patrimony.cashOut * price;
            for (int i = 0; i < patrimony.retainer.Count; i++)
            {
                sumPatrimony = sumPatrimony + patrimony.retainer.ElementAt(i).Item1 + patrimony.retainer.ElementAt(i).Item2 * price;
            }
            double var = CalculatePatrimonyVariation(patrimony.Patrimony.Last(), sumPatrimony);
            patrimony.Patrimony.Add(sumPatrimony);
            patrimony.PatrimonyVariation.Add(var);
            patrimony.times.Add(game.CurrentDate);
        }
        public void executeOrder(Agent agent, double price, double ammount, bool moneyOut)
        {
            Operation op = new Operation();
            op.TransactionDate = game.CurrentDate;
            op.Quantity = ammount;
            PatrimonyControl patrimony = GetPatrimonyByAgent(agent);
            if (moneyOut && patrimony.cashBR >= ammount)
            {
                (double transfered, int retained) = game.ExchangeRules.transferOut(ammount, price);
                patrimony.cashBR -= ammount;
                op.Type = Operation.Transfer_Out;
                op.Result = transfered;
                op.RetainedTime = retained;
                if(retained == 0)
                {
                    patrimony.cashOut += transfered;
                }
                else
                {
                    DateTime liquidationDate = game.CurrentDate + (new TimeSpan(retained, 0, 0, 0)); //create a date object which points to the date of money arriving
                    patrimony.retainer.Add((0, transfered, liquidationDate));
                }

            }
            else if (!moneyOut && patrimony.cashOut >= ammount)
            {
                (double transfered, int retained) = game.ExchangeRules.transferIn(ammount, price);
                patrimony.cashOut -= ammount;
                op.Type = Operation.Transfer_In;
                op.RetainedTime = retained;
                op.Result = transfered;
                if (retained == 0)
                {
                    patrimony.cashBR += transfered;
                }
                else
                {
                    DateTime liquidationDate = game.CurrentDate + (new TimeSpan(retained, 0, 0, 0)); //create a date object which points to the date of money arriving
                    patrimony.retainer.Add((transfered, 0, liquidationDate));
                }
            }
            else
            {
                MessageBox.Show("Error in Execute order");
                return;
            }
            agent.Operations.Add(op);
        }
        #endregion


        //interface delivery information
        #region methods for interfacing
        public Series generateSerie(List<Double> yValues, List<DateTime> xvalues, TimeSpan interval, bool smooth)
        {

            Series ser = new Series();
            ser.ChartType = SeriesChartType.Line;
            ser.XValueType = ChartValueType.DateTime;
            
            if (yValues.Count == 0)
                return ser;
            
            //find starting point
            DateTime limit = game.CurrentDate - interval;
            int i = 0;
            if (interval.Ticks!= 0) //if interval parameter is zero, returns all data
            {
                for(i = xvalues.Count - 1; i > 0; i--)
                {
                    if (DateTime.Compare(xvalues.ElementAt(i), limit) <= 0)
                        break;
                }
            }
            double y1 = yValues.ElementAt(i);
            //feed series of data
            for(int j = i; j < xvalues.Count; j++)
            {
                double val = yValues.ElementAt(j);
                if(smooth) // implement simple first order filter
                {
                    val = 0.9 * y1 + 0.1 * val;
                    y1 = val;
                }
                ser.Points.AddXY(xvalues.ElementAt(j), val);
            }
            return ser;
        }
        public static double CalculatePatrimonyVariation(double bef, double after)
        {
            return Math.Round(((after / bef - 1) * 100), 5);
        }

        public DateTime getCurrentDate()
        {
            return game.CurrentDate;
        }

        public static int getIndexOf(DateTime targetPos, List<DateTime> list)
        {
            for(int i = (list.Count - 1); i>=0; i--)
            {
                if (DateTime.Compare(targetPos, list.ElementAt(i)) >= 0)
                    return i;
            }
            return 0;
        }
        #endregion

        #endregion
    }
}
