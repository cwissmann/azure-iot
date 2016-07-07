using GalaSoft.MvvmLight.Messaging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConnectionMonitor
{
    public class StatusMonitor
    {
        private readonly string testUrl = "http://www.google.de";

        public async Task StartConnectionCheck()
        {
            while (true)
            {
                var result = this.CheckConnection();

                Messenger.Default.Send<ConnectionResult>(result.Result, ViewModelLocator.StatusUpdateToken);

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        public async Task<ConnectionResult> CheckConnection()
        {
            ConnectionResult connection = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = await client.GetAsync(this.testUrl);
                    string body = await result.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(body))
                    {
                        connection = new ConnectionResult(false);
                    }
                    else
                    {
                        connection = new ConnectionResult(true);
                    }
                }
            }
            catch (Exception ex)
            {
                connection = new ConnectionResult(false);
            }

            return connection;
        }
    }
}
