using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CambistaSim
{
    public partial class MainWindow : Form
    {
        #region attributes
        private Controler controler;

        private bool settingRules = true;

        private double DefaultEuroTax = 0;
        private double DefaultEuroFee = 0;
        private int DefaultEuroDelay = 0;
        private double DefaultReaisTax = 0;
        private double DefaultReaisFee = 0;
        private int DefaultReaisDelay = 0;
        private double DefaultMonthlyCost = 0;
        private double DefaultDeadline = -1000;

        //operation control variables
        private int selectedPlayer = 0;
        

        #endregion

        #region constructor
        public MainWindow()
        {
            controler = new Controler(this);
            InitializeComponent();

            playersChecks.CheckOnClick = true;
            checksForVisualization.CheckOnClick = true;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM";
        }
        #endregion

        #region button handlers
        private void newGameButton_Click(object sender, EventArgs e)
        {
            //reset all information
            taxSetCheckbox.Checked = false;
            coinCombobox.SelectedIndex = 0;
            randomStartDateBox.Checked = true;
            dayField.Value = 1;
            monthField.Value = 6;
            yearField.Value = 2001;
            startingAmmountField.Value = 10000;

            DefaultEuroTax = 0;
            DefaultEuroFee = 0;
            DefaultEuroDelay = 0;
            DefaultReaisTax = 0;
            DefaultReaisFee = 0;
            DefaultReaisDelay = 0;
            DefaultMonthlyCost = 0;
            DefaultDeadline = -1000;
            toEuroTaxField.Value = 0;
            
            
            SimTab.SelectedIndex = 1;
        }

        private void loadGameButton_Click(object sender, EventArgs e)
        {

        }

        private void seeDataButton_Click(object sender, EventArgs e)
        {

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void callExchangeSetButton_Click(object sender, EventArgs e)
        {
            toEuroTaxField.Value = new Decimal(DefaultEuroTax);
            toEuroFee.Value = new Decimal(DefaultEuroFee);
            toEuroDelay.Value = DefaultEuroDelay;
            toReaisTax.Value = new Decimal(DefaultReaisTax);
            toReaisFee.Value = new Decimal(DefaultReaisFee);
            toReaisDdlay.Value = DefaultReaisDelay;
            monthlyCostField.Value = new Decimal(DefaultMonthlyCost);
            deadlineField.Value = new Decimal(DefaultDeadline);
            settingRules = true;
            SimTab.SelectedIndex = 2;
        }

        private void okNewButton_Click(object sender, EventArgs e)
        {
            if(!taxSetCheckbox.Checked)
            {
                MessageBox.Show("Taxes and Rules not Set", "Error on Starting Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(coinCombobox.SelectedIndex == 0)
            {
                MessageBox.Show("Coin not selected", "Error on Starting Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!randomStartDateBox.Checked && (controler.isValidDate((int)dayField.Value, (int)monthField.Value, (int)yearField.Value).Equals(DateTime.Today)))
            {
                MessageBox.Show("Invalid Starting Date", "Error on Starting Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(playersChecks.CheckedItems.Count == 0)
            {
                MessageBox.Show("No players Selected", "Error on Starting Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool[] players = new bool[playersChecks.Items.Count];
            for(int i = 0; i < playersChecks.CheckedIndices.Count; i++)
            {
                players[playersChecks.CheckedIndices[i]] = true;
            }
            int coin = coinCombobox.SelectedIndex;
            if (coin == Controler.RANDOM)
                coin = controler.generateRandomCoin();
            DateTime date;
            if(randomStartDateBox.Checked)
            {
                date = controler.generateRandomStartingDate();
            }
            else
            {
                date = new DateTime((int)yearField.Value, (int)monthField.Value, (int)dayField.Value);
            }

            controler.startSimulation("DefaultGame", coin, date, (double)startingAmmountField.Value, (double)toEuroTaxField.Value / 100, (double)toEuroFee.Value, (int)toEuroDelay.Value, (double)toReaisTax.Value / 100, (double)toReaisFee.Value, (int)toReaisDdlay.Value, (double)monthlyCostField.Value, (double)deadlineField.Value, players);
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            refreshOperation();
            //set default game information
            SimTab.SelectedIndex = 3;
        }

        private void cancelNewButton_Click(object sender, EventArgs e)
        {
            SimTab.SelectedIndex =  0;
        }

        private void noBorderRulesButton_Click(object sender, EventArgs e)
        {
            toEuroTaxField.Value = 0;
            toEuroFee.Value = 0;
            toEuroDelay.Value = 0;
            toReaisTax.Value = 0;
            toReaisFee.Value = 0;
            toReaisDdlay.Value = 0;
            monthlyCostField.Value = 0;
            deadlineField.Value = -1000;
        }

        private void TransferwiseButton_Click(object sender, EventArgs e)
        {
            toEuroTaxField.Value = new Decimal(0.011);
            toEuroFee.Value = new Decimal(27.03);
            toEuroDelay.Value = 3;
            toReaisTax.Value = 0;
            toReaisFee.Value = new Decimal(13.79);
            toReaisDdlay.Value = 2;
            monthlyCostField.Value = 0;
            deadlineField.Value = -1000;
        }

        private void BBExchangeButton_Click(object sender, EventArgs e)
        {
            toEuroTaxField.Value = new Decimal(0.0307);
            toEuroFee.Value = 0;
            toEuroDelay.Value = 1;
            toReaisTax.Value = 0;
            toReaisFee.Value = new Decimal(0.0307);
            toReaisDdlay.Value = 1;
            monthlyCostField.Value = 0;
            deadlineField.Value = -1000;
        }

        private void okRulesButton_Click(object sender, EventArgs e)
        {
            if (settingRules)
            {
                taxSetCheckbox.Checked = true;
                DefaultEuroTax = (double) toEuroTaxField.Value;
                DefaultEuroFee = (double)toEuroFee.Value;
                DefaultEuroDelay = (int)toEuroDelay.Value;
                DefaultReaisTax = (double)toReaisTax.Value;
                DefaultReaisFee = (double)toReaisFee.Value;
                DefaultReaisDelay = (int)toReaisDdlay.Value;
                DefaultMonthlyCost = (double)monthlyCostField.Value;
                DefaultDeadline = (double)deadlineField.Value;
                SimTab.SelectedIndex = 1;
                settingRules = false;
                return;
            }
            taxesControlPanel.Enabled = true;
            SimTab.SelectedIndex = 3;
        }

        private void cancelRulesButton_Click(object sender, EventArgs e)
        {
            if (settingRules)
            {
                SimTab.SelectedIndex = 1;
                return;
            }
            taxesControlPanel.Enabled = true;
            SimTab.SelectedIndex = 3;
        }

        private void previousPlayerButton_Click(object sender, EventArgs e)
        {
            selectedPlayer--;
            if (selectedPlayer == -1)
                selectedPlayer = controler.Patrimonies.Count - 1;
            refreshPatrimonyInfo();
        }

        private void nextPlayerButton_Click(object sender, EventArgs e)
        {
            selectedPlayer = (selectedPlayer + 1) % controler.Patrimonies.Count;
            refreshPatrimonyInfo();
        }

        private void saveGameButton_Click(object sender, EventArgs e)
        {

        }

        private void exitOperationButton_Click(object sender, EventArgs e)
        {

        }

        private void go1StepButton_Click(object sender, EventArgs e)
        {
            controler.processStepForward();
            refreshOperation();
        }

        private void goNextDayButton_Click(object sender, EventArgs e)
        {
            do
            {
                controler.processStepForward();
            } while (controler.MarketTime != Controler.PRE_OPEN);
            refreshOperation();
        }

        private void go1WeekButton_Click(object sender, EventArgs e)
        {
            DateTime limit = controler.getCurrentDate() + new TimeSpan(7, 0, 0, 0);
            int days = 0;
            do
            {
                controler.processStepForward();
                if (controler.MarketTime != Controler.PRE_OPEN)
                    days++;
            } while ((controler.MarketTime != Controler.PRE_OPEN) || (DateTime.Compare(controler.getCurrentDate(), limit) < 0));
            refreshOperation();

        }

        private void go1YearButton_Click(object sender, EventArgs e)
        {
            DateTime limit = controler.getCurrentDate() + new TimeSpan(365, 0, 0, 0);
            int days = 0;
            do
            {
                controler.processStepForward();
                if (controler.MarketTime != Controler.PRE_OPEN)
                    days++;
            } while ((controler.MarketTime != Controler.PRE_OPEN) || (DateTime.Compare(controler.getCurrentDate(), limit) < 0));
            refreshOperation();
        }

        private void sellMoneyButton_Click(object sender, EventArgs e)
        {
            double value = (double)ammountToSellField.Value;
            double price = controler.LastClosingPrice;
            if(controler.MarketTime == Controler.POS_OPEN)
            {
                price = controler.OpenPrice;
            }
            else if(controler.MarketTime == Controler.CLOSED)
            {
                price = controler.ClosingPrice;
            }
            if(realCoinSellRadio.Checked)
            {
                if(controler.Patrimonies.ElementAt(0).cashBR >= value)
                {
                    controler.executeOrder(controler.Patrimonies.ElementAt(0).Agent, price, value, true);
                }
            }
            else if(controler.Patrimonies.ElementAt(0).cashOut >= value)
            {
                controler.executeOrder(controler.Patrimonies.ElementAt(0).Agent, price, value, false);
            }
            else 
            { 
                MessageBox.Show("Invalid operation");
            }
            refreshPatrimonyInfo();
        }

        private void setOrderButton_Click(object sender, EventArgs e)
        {
            if (controler.Patrimonies.ElementAt(0).Agent.OrderSet)
            {
                controler.Patrimonies.ElementAt(0).Agent.resetOrder();
                setOrderButton.Text = "Set Order";
            }
            else
            {
                controler.Patrimonies.ElementAt(0).Agent.setOrder(realCoinSellRadio.Checked, (double)ammountConditionField.Value, (double)priceBoundaryField.Value);
                setOrderButton.Text = "Reset Order";
            }
        }
        private void seeExchangeRulesButton_Click(object sender, EventArgs e)
        {
            ExchangeRules rules = controler.Game.ExchangeRules;
            toEuroTaxField.Value = new Decimal(rules.TaxOut*100);
            toEuroFee.Value = new Decimal(rules.OperationOut);
            toEuroDelay.Value = rules.DelayOut;
            toReaisTax.Value = new Decimal(rules.TaxIn*100);
            toReaisFee.Value = new Decimal(rules.OperationIn);
            toReaisDdlay.Value = rules.DelayIn;
            monthlyCostField.Value = new Decimal(rules.MonthlyCost);
            deadlineField.Value = new Decimal(rules.Deadline);

            taxesControlPanel.Enabled = false;
            SimTab.SelectedIndex = 2;
        }
        #endregion

        #region eventhandler
        private void randomStartDateBox_CheckedChanged(object sender, EventArgs e)
        {
            dayField.Enabled = !randomStartDateBox.Checked;
            monthField.Enabled = !randomStartDateBox.Checked;
            yearField.Enabled = !randomStartDateBox.Checked;
        }

        private void radio1w_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void radio2w_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void radio1m_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void radio2m_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void radio1y_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void radioAll_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void checksForVisualization_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void smoothChartCheck_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }

        private void showAllPlayersCheck_CheckedChanged(object sender, EventArgs e)
        {
            refreshChart();
        }
        #endregion

        #region operation Interface control


        public void refreshOperation()
        {
            //update pricing values
            preOpenIndicator.BackColor = Color.White;
            posOpenIndicator.BackColor = Color.White;
            CloseIndicator.BackColor = Color.White;
            previousCloseValueField.Text = "" + controler.LastClosingPrice;
            openField.Text = "";
            maxField.Text = "";
            minField.Text = "";
            closeField.Text = "";
            dateField.Text = controler.getCurrentDate().ToString("dddd, dd MMMM ") + (controler.getCurrentDate().Year - controler.Game.StartingDate.Year + 1);
            if (controler.MarketTime == Controler.PRE_OPEN)
            {
                preOpenIndicator.BackColor = Color.Lime;
                priceLabel.Text = "" + controler.LastClosingPrice;
                if(controler.Prices.Count > 1)
                {
                    variationLabel.Text = "" + Math.Round(Controler.CalculatePatrimonyVariation(controler.Prices.ElementAt(controler.Prices.Count - 2), controler.LastClosingPrice), 5) + '%';
                    if (controler.Prices.ElementAt(controler.Prices.Count - 2) <= controler.LastClosingPrice)
                    {
                        priceLabel.ForeColor = Color.Green;
                        variationLabel.ForeColor = Color.Green;
                    }
                    else
                    {
                        priceLabel.ForeColor = Color.Red;
                        variationLabel.ForeColor = Color.Red;
                    }
                }
            }
            else if(controler.MarketTime == Controler.POS_OPEN)
            {
                posOpenIndicator.BackColor = Color.Lime;
                openField.Text = "" + controler.OpenPrice;
                priceLabel.Text = "" + controler.OpenPrice;
                variationLabel.Text = "" + Math.Round(Controler.CalculatePatrimonyVariation(controler.LastClosingPrice, controler.OpenPrice), 5) + '%';
                if (controler.LastClosingPrice <= controler.OpenPrice)
                {
                    priceLabel.ForeColor = Color.Green;
                    variationLabel.ForeColor = Color.Green;
                }
                else
                {
                    priceLabel.ForeColor = Color.Red;
                    variationLabel.ForeColor = Color.Red;
                }
            }
            else //pos close
            {
                CloseIndicator.BackColor = Color.Lime;
                openField.Text = "" + controler.OpenPrice;
                maxField.Text = "" + controler.MaxPrice;
                minField.Text = "" + controler.MinPrice;
                closeField.Text = "" + controler.ClosingPrice;

                priceLabel.Text = "" + controler.ClosingPrice;
                variationLabel.Text = "" + Math.Round(Controler.CalculatePatrimonyVariation(controler.LastClosingPrice, controler.ClosingPrice),5) + '%';
                if (controler.LastClosingPrice <= controler.ClosingPrice)
                {
                    priceLabel.ForeColor = Color.Green;
                    variationLabel.ForeColor = Color.Green;
                }
                else
                {
                    priceLabel.ForeColor = Color.Red;
                    variationLabel.ForeColor = Color.Red;
                }
            }

            //set patrimony information
            refreshPatrimonyInfo();

            //update charts
            refreshChart();

            if (!controler.Patrimonies.ElementAt(0).Agent.OrderSet)
            {
                setOrderButton.Text = "Set Order";
            }
            else
            {
                setOrderButton.Text = "Reset Order";
            }
        }
        public void refreshPatrimonyInfo()
        {
            double price = controler.LastClosingPrice;
            if (controler.MarketTime == Controler.POS_OPEN)
                price = controler.OpenPrice;
            else if (controler.MarketTime == Controler.CLOSED)
                price = controler.ClosingPrice;
            PatrimonyControl target = controler.Patrimonies.ElementAt(this.selectedPlayer);
            playerField.Text = target.Agent.name;
            valueReaisField.Text = "" + Math.Round(target.cashBR, 2);
            valueOutField.Text = "" + Math.Round(target.cashOut, 2);
            double retBr = 0;
            double retOut = 0;
            for(int i = 0; i < target.retainer.Count; i++)
            {
                retBr += target.retainer.ElementAt(i).Item1;
                retOut += target.retainer.ElementAt(i).Item2;
            }
            retainedReaisField.Text = "" + Math.Round(retBr, 2);
            retainedOutField.Text = "" + Math.Round(retOut, 2);

            EquivalentOutField.Text = "" + Math.Round((target.cashBR + retBr) / price, 2);
            EquivalentReaisField.Text = "" + Math.Round((target.cashOut + retOut) * price, 2);
            double curPatr = target.Patrimony.Last();
            patrimonyField.Text = "" + curPatr;

            //calculate and set gains
            double pastPatr = target.Patrimony.ElementAt(Controler.getIndexOf(controler.getCurrentDate() - (new TimeSpan(7, 0, 0, 0)), target.times));
            gain1WField.Text = "" + Controler.CalculatePatrimonyVariation(pastPatr, curPatr);
            if (curPatr >= pastPatr)
                gain1WField.ForeColor = Color.Green;
            else
                gain1WField.ForeColor = Color.Red;
            
            pastPatr = target.Patrimony.ElementAt(Controler.getIndexOf(controler.getCurrentDate() - (new TimeSpan(30, 0, 0, 0)), target.times));
            gain1MField.Text = "" + Controler.CalculatePatrimonyVariation(pastPatr, curPatr);
            if (curPatr >= pastPatr)
                gain1MField.ForeColor = Color.Green;
            else
                gain1MField.ForeColor = Color.Red;
            
            double past2Patr = target.Patrimony.ElementAt(Controler.getIndexOf(controler.getCurrentDate() - (new TimeSpan(60, 0, 0, 0)), target.times));
            gainLastMonthField.Text = "" + Controler.CalculatePatrimonyVariation(past2Patr, pastPatr);
            if (pastPatr >= past2Patr)
                gainLastMonthField.ForeColor = Color.Green;
            else
                gainLastMonthField.ForeColor = Color.Red;

            pastPatr = target.Patrimony.ElementAt(Controler.getIndexOf(controler.getCurrentDate() - (new TimeSpan(365, 0, 0, 0)), target.times));
            gain12MField.Text = "" + Controler.CalculatePatrimonyVariation(pastPatr, curPatr);
            if (curPatr >= pastPatr)
                gain12MField.ForeColor = Color.Green;
            else
                gain12MField.ForeColor = Color.Red;

            pastPatr = target.Patrimony.ElementAt(0);
            totalGainField.Text = "" + Controler.CalculatePatrimonyVariation(pastPatr, curPatr);
            if (curPatr >= pastPatr)
                totalGainField.ForeColor = Color.Green;
            else
                totalGainField.ForeColor = Color.Red;
        }

        public void refreshChart()
        {
            TimeSpan intervalSelected = new TimeSpan(0);
            if (radio1M.Checked)
            {
                intervalSelected = TimeSpan.FromDays(30);
            }
            else if (radio3M.Checked)
            {
                intervalSelected = TimeSpan.FromDays(90);
            }
            else if (radio6M.Checked)
            {
                intervalSelected = TimeSpan.FromDays(180);
            }
            else if (radio1Y.Checked)
            {
                intervalSelected = TimeSpan.FromDays(360);
            }
            else if (radio2y.Checked)
            {
                intervalSelected = TimeSpan.FromDays(720);
            }

            chart.Series.Clear();
            if (checksForVisualization.CheckedIndices.Contains(0))
            {
                Series series = controler.generateSerie(controler.Prices, controler.times, intervalSelected, smoothChartCheck.Checked);
                series.Name = "Price Quotation";
                series.Color = Color.Black;
                chart.Series.Add(series);
            }
            if (checksForVisualization.CheckedIndices.Contains(1))
            {
                Series series = controler.generateSerie(controler.PriceVariation, controler.times, intervalSelected, smoothChartCheck.Checked);
                series.Name = "Price Variation";
                series.Color = Color.Gray;
                chart.Series.Add(series);
            }
            if (checksForVisualization.CheckedIndices.Contains(2))
            {
                for (int i = 0; i < controler.Patrimonies.Count; i++)
                {
                    Series series = controler.generateSerie(controler.Patrimonies.ElementAt(i).Patrimony, controler.Patrimonies.ElementAt(i).times, intervalSelected, smoothChartCheck.Checked);
                    series.Name = "Patrimony " + controler.Patrimonies.ElementAt(i).Agent.name;
                    chart.Series.Add(series);
                    if (!showAllPlayersCheck.Checked)
                        break;
                }
            }
            if (checksForVisualization.CheckedIndices.Contains(3))
            {
                for (int i = 0; i < controler.Patrimonies.Count; i++)
                {
                    Series series = controler.generateSerie(controler.Patrimonies.ElementAt(i).PatrimonyVariation, controler.Patrimonies.ElementAt(i).times, intervalSelected, smoothChartCheck.Checked);
                    series.Name = "Patrimony Variation" + controler.Patrimonies.ElementAt(i).Agent.name;
                    chart.Series.Add(series);
                    if (!showAllPlayersCheck.Checked)
                        break;
                }
            }
            chart.ChartAreas[0].RecalculateAxesScale();
        }
        #endregion

    }
}
