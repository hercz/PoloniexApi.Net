using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using Jojatekok.PoloniexAPI.WalletTools;
using MvvmLight1.Model;
using Timer = System.Timers.Timer;

namespace MvvmLight1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDataService dataService)
        {
            myNotEmptyBalances = new Dictionary<string, Balance>();
            PoloniexClient = new PoloniexClient(ApiKeys.PublicKey, ApiKeys.PrivateKey);
            LoadNotEmptyBalancesAsync();
            GetLastSysPrice();
            GetSysTrades();
            StartTimer();
            StartTime = DateTime.Now;

           
        }

        public PoloniexClient PoloniexClient { get; set; }

        public IList<ITrade> SysTrades
        {
            get
            {
                return mySysTrades;
            }
            set
            {
                if (mySysTrades != null && mySysTrades.Equals(value))
                {
                    return;
                }
                mySysTrades = value;
                RaisePropertyChanged();
            }
        }

        public IDictionary<string, Balance> NotEmptyBalances
        {
            get
            {
                return myNotEmptyBalances;
            }
            set
            {
                if (myNotEmptyBalances == value)
                {
                    return;
                }
                myNotEmptyBalances = value;
                RaisePropertyChanged();
            }
        }

        public DateTime StartTime
        {
            get
            {
                return myStartTime;
            }
            set
            {
                myStartTime = value;
                RaisePropertyChanged();
            }
        }

        public double SysCoinLastPrice
        {
            get
            {
                return mySysCoinLastPrice;
            }
            set
            {
                if (mySysCoinLastPrice.Equals(value))
                {
                    return;
                }
                mySysCoinLastPrice = value;
                RaisePropertyChanged();
            }
        }

        public DateTime LogTime
        {
            get
            {
                return myLogTime;
            }
            set
            {
                myLogTime = value;
                RaisePropertyChanged();
            }
        }

        public DateTime SysPriceLastRefreshTime
        {
            get
            {
                return mySysPriceLastRefreshTime;
            }
            set
            {
                mySysPriceLastRefreshTime = value;
                RaisePropertyChanged();
            }
        }

        private void StartTimer()
        {
            Timer timer = new Timer(20000);

            timer.Elapsed += TimerOnElapsed;

            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            LoadNotEmptyBalancesAsync();
            Thread.Sleep(500);
            GetLastSysPrice();
        }

        private async void GetLastSysPrice()
        {
            var summary = await PoloniexClient.Markets.GetSummaryAsync();

            var btcSys = "BTC_SYS";
            var sysCoin = summary[CurrencyPair.Parse(btcSys)];

            SysCoinLastPrice = sysCoin.PriceLast;
            SysPriceLastRefreshTime = DateTime.Now;
        }


        private async void GetSysTrades()
        {
            var sysTrades = await PoloniexClient.Markets.GetTradesAsync(CurrencyPair.Parse("BTC_SYS"));
            SysTrades = new List<ITrade>();
            foreach (var sysTrade in sysTrades)
            {
                SysTrades.Add(sysTrade);
            }
        }

        private async void LoadNotEmptyBalancesAsync()
        {
            var balances = await PoloniexClient.Wallet.GetBalancesAsync();

            NotEmptyBalances.Clear();
            foreach (var balance in balances)
            {

                if (balance.Value.BitcoinValue > 0)
                {
                    NotEmptyBalances.Add(balance);
                }
            }
            LogTime = DateTime.Now;
        }



        private IDictionary<string, Balance> myNotEmptyBalances;
        private DateTime myLogTime;
        private DateTime myStartTime;
        private IList<ITrade> mySysTrades;
        private double mySysCoinLastPrice;
        private DateTime mySysPriceLastRefreshTime;
    }
}