using System;
using TradeApi.History;
using TradeApi.Indicators;
using Runtime.Script;
using System.Drawing;
using TradeApi.ToolBelt;

namespace SimpleMAStrategy
{
    public class SimpleMAStrategy : StrategyBuilder
    {
        [InputParameter(InputType.Numeric, "Fast MA Period", 0)]
        [SimpleNumeric(1D, 999D)]
        public int FastMAPeriod = 10;

        [InputParameter(InputType.Numeric, "Slow MA Period", 1)]
        [SimpleNumeric(1D, 999D)]
        public int SlowMAPeriod = 20;

        [InputParameter(InputType.Combobox, "MA Type", 2)]
        [ComboboxItem("Simple", MAMode.Simple)]
        [ComboboxItem("Exponential", MAMode.Exponential)]
        [ComboboxItem("Weighted", MAMode.Weighted)]
        public MAMode MAType = MAMode.Simple;

        private BuiltInIndicator fastMA;
        private BuiltInIndicator slowMA;

        public SimpleMAStrategy()
        {
            Name = "Simple MA Crossover Strategy";
            Description = "A simple trading strategy based on moving average crossovers";

            AddLine("Fast MA", Color.Blue, 1, LineStyle.Solid);
            AddLine("Slow MA", Color.Red, 1, LineStyle.Solid);
        }

        protected override void OnInit()
        {
            fastMA = MovingAverage(Close, FastMAPeriod, MAType);
            slowMA = MovingAverage(Close, SlowMAPeriod, MAType);
        }

        protected override void OnUpdate(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
            {
                SetValue(fastMA.Value, 0);
                SetValue(slowMA.Value, 1);

                if (fastMA.Value > slowMA.Value && fastMA.PreviousValue <= slowMA.PreviousValue)
                {
                    DrawText("Buy", "BUY", Color.Green, true);
                }
                else if (fastMA.Value < slowMA.Value && fastMA.PreviousValue >= slowMA.PreviousValue)
                {
                    DrawText("Sell", "SELL", Color.Red, true);
                }
            }
        }

        private void DrawText(string name, string text, Color color, bool isNew)
        {
            var textObj = AddText(name, text, color);
            textObj.SetPoint(0, GetTime(), High[0]);
            textObj.IsNew = isNew;
        }
    }
} 