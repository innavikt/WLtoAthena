using System;
using System.Drawing;
using WealthLab;
using WealthLab.Indicators;


namespace v1
{
    class TestStrategyScript : WealthScript
    {
        private StrategyParameter slowPeriod;
        private StrategyParameter fastPeriod; 
        
        public TestStrategyScript()
        {
            fastPeriod = CreateParameter("Fast Period", 20, 1, 100, 1);
            slowPeriod = CreateParameter("Slow Period", 50, 20, 300, 5);
        }

        protected override void Execute()
        {
            //obtain periods from parameters
            int fastPer = fastPeriod.ValueInt;
            int slowPer = slowPeriod.ValueInt;

            SMA smaFast = SMA.Series(Close, fastPer);
            SMA smaSlow = SMA.Series(Close, slowPer);

            PlotSeries(PricePane, smaFast, Color.Green, LineStyle.Solid, 2);
            PlotSeries(PricePane, smaSlow, Color.Red, LineStyle.Solid, 2);

            for (int bar = Math.Max(fastPer, slowPer); bar < Bars.Count; bar++)
            {
                if (IsLastPositionActive)
                {
                    if (CrossUnder(bar, smaFast, smaSlow))
                        SellAtMarket(bar + 1, LastPosition);
                }
                else
                {
                    if (CrossOver(bar, smaFast, smaSlow))
                        BuyAtMarket(bar + 1);
                }
            }
        }
    }
}
