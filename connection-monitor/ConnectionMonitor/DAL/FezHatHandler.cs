using GalaSoft.MvvmLight.Messaging;
using GHIElectronics.UWP.Shields;

namespace ConnectionMonitor
{
    public class FezHatHandler
    {
        private FEZHAT hat;

        public FezHatHandler()
        {
            this.Setup();

            Messenger.Default.Register<ConnectionResult>(this, ViewModelLocator.StatusUpdateToken, this.UpdateStatus);
        }

        private async void Setup()
        {
            this.hat = await FEZHAT.CreateAsync();

            this.hat.D2.Color = FEZHAT.Color.Black;
            this.hat.D3.Color = FEZHAT.Color.Black;
        }

        private void UpdateStatus(ConnectionResult result)
        {
            if (this.hat == null)
            {
                return;
            }

            if (result.Online)
            {
                this.hat.D2.Color = FEZHAT.Color.Green;
                this.hat.D3.Color = FEZHAT.Color.Black;
            }
            else
            {
                this.hat.D2.Color = FEZHAT.Color.Black;
                this.hat.D3.Color = FEZHAT.Color.Red;
            }
        }
    }
}
