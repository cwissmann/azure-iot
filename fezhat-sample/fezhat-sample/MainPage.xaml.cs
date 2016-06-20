using GHIElectronics.UWP.Shields;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace fezhat_sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FEZHAT hat;
        private DispatcherTimer timer;

        private static DeviceClient deviceClient;
        private string iotHubUri = "[iotHubUri]";
        private string deviceId = "[deviceId]";
        private string deviceKey = "[deviceKey]";
        private string connectionString = "[connectionString]";

        public MainPage()
        {
            this.InitializeComponent();

            this.Setup();
        }

        private async void Setup()
        {
            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Http1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            this.hat = await FEZHAT.CreateAsync();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(5);
            this.timer.Tick += OnTick;
            this.timer.Start();
        }

        private async void OnTick(object sender, object e)
        {
            LedSignal();

            var temperature = this.hat.GetTemperature().ToString("N2");
            var lightlevel = this.hat.GetLightLevel().ToString("P2");

            Debug.WriteLine("Temperature: " + temperature);
            Debug.WriteLine("LightLevel:  " + lightlevel);

            var weatherDataPoint = new
            {
                deviceId = deviceId,
                temperature = temperature,
                lightlevel = lightlevel,
                time = DateTime.Now
            };

            var messageString = JsonConvert.SerializeObject(weatherDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient?.SendEventAsync(message);
        }

        private async void LedSignal()
        {
            this.hat.D3.Color = FEZHAT.Color.Red;
            Debug.WriteLine("LedSignal:  RED");

            await Task.Delay(2000);

            this.hat.D3.Color = FEZHAT.Color.Black;
            Debug.WriteLine("LedSignal:  BLACK");
        }
    }
}
