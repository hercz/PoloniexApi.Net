using System.Reflection;
using System.Text;
using System.Windows.Controls;

namespace Jojatekok.PoloniexAPI.Demo
{
    public sealed partial class MainWindow
    {
        private PoloniexClient PoloniexClient { get; set; }

        public MainWindow()
        {
            // Set icon from the assembly
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location).ToImageSource();

            InitializeComponent();

            PoloniexClient = new PoloniexClient(ApiKeys.PublicKey, ApiKeys.PrivateKey);
            LoadMarketSummaryAsync();
            LoadOtherInfoAsync();
            LoadOtherInfoAsync2();
        }

        private async void LoadMarketSummaryAsync()
        {
            var markets = await PoloniexClient.Markets.GetSummaryAsync();
            DataGrid1.Items.Clear();

            foreach (var market in markets)
            {
                DataGrid1.Items.Add(market);
            }

            DataGrid1.Items.Refresh();
        }

        private async void LoadOtherInfoAsync()
        {
            var trades = await PoloniexClient.Markets.GetTradesAsync(CurrencyPair.Parse("BTC_SYS"));
            OtherInfoList.Items.Clear();

            foreach (var market in trades)
            {
                var text = new StringBuilder();
                text.Append(market.Time);
                text.Append("//");
                text.Append(market.AmountBase);
                text.Append("//");
                text.Append(market.AmountQuote);
                text.Append("//");
                text.Append(market.PricePerCoin);
                text.Append("//");
                text.Append(market.Type);
                OtherInfoList.Items.Add(text);
            }

            OtherInfoList.Items.Refresh();
        }

        private async void LoadOtherInfoAsync2()
        {
            var balances = await PoloniexClient.Wallet.GetBalancesAsync();

            foreach (var balance in balances)
            {
                var text = new StringBuilder();
                text.Append("//Key=");
                text.Append(balance.Key);
                text.Append("//QuoteAvailable=");
                text.Append(balance.Value.QuoteAvailable);
                text.Append("//QuoteOnOrders=");
                text.Append(balance.Value.QuoteOnOrders);
                text.Append("//BitcoinValue=");
                text.Append(balance.Value.BitcoinValue);
                BalanceListbox.Items.Add(text);
            }

            BalanceListbox.Items.Refresh();
        }
    }
}
