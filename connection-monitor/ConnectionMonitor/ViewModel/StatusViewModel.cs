using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace ConnectionMonitor
{
    public class StatusViewModel : ViewModelBase
    {
        private readonly string onlineMessage = "Online";

        private readonly string offlineMessage = "Offline";

        private readonly Windows.UI.Color green = Windows.UI.Colors.Green;

        private readonly Windows.UI.Color red = Windows.UI.Colors.Red;

        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);

        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

        private string statusMessage = null;

        public StatusViewModel()
        {
            if (this.IsInDesignMode)
            {
                this.RedBrush = this.grayBrush;
                this.StatusMessage = this.onlineMessage;
            }
            else
            {
                this.GreenBrush = this.grayBrush;
                this.RedBrush = this.grayBrush;
            }

            Messenger.Default.Register<ConnectionResult>(this, ViewModelLocator.StatusUpdateToken, this.UpdateStatus);
            Task.Run(() =>
            {
                StatusMonitor monitor = new StatusMonitor();
                monitor.StartConnectionCheck();
            });
        }

        public SolidColorBrush GreenBrush
        {
            get
            {
                return this.greenBrush;
            }
            set
            {
                if (value == this.greenBrush)
                {
                    return;
                }

                this.greenBrush = value;
                this.RaisePropertyChanged();
            }
        }

        public SolidColorBrush RedBrush
        {
            get
            {
                return this.redBrush;
            }
            set
            {
                if (value == this.redBrush)
                {
                    return;
                }

                this.redBrush = value;
                this.RaisePropertyChanged();
            }
        }

        public string StatusMessage
        {
            get
            {
                return this.statusMessage;
            }

            set
            {
                if (value == this.statusMessage)
                {
                    return;
                }

                this.statusMessage = value;
                this.RaisePropertyChanged();
            }
        }


        private void UpdateStatus(ConnectionResult result)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (result.Online)
                {
                    this.StatusMessage = this.onlineMessage;

                    this.GreenBrush = new SolidColorBrush(this.green);
                    this.RedBrush = this.grayBrush;
                }
                else
                {
                    this.StatusMessage = this.offlineMessage;

                    this.RedBrush = new SolidColorBrush(this.red);
                    this.GreenBrush = this.grayBrush;
                }
            });
        }
    }
}
