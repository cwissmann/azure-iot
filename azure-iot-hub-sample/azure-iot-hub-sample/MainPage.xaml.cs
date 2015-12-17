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

namespace azure_iot_hub_sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FEZHAT hat;
        private DispatcherTimer timer;

        static DeviceClient deviceClient;
        static string iotHubUri = "[iothuburi]";
        static string deviceId = "[deviceid]";
        static string deviceKey = "[devicekey]";
        static string connectionString = "[connectionstring]";

        public MainPage()
        {
            this.InitializeComponent();

            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Http1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            this.Setup();
        }

        private async void Setup()
        {
            this.hat = await FEZHAT.CreateAsync();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(30);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private async void OnTick(object sender, object e)
        {
            var temperature = this.hat.GetTemperature().ToString("N2");
            var lightLevel = this.hat.GetLightLevel().ToString("P2");

            var weatherDataPoint = new
            {
                deviceId = deviceId,
                temperature = temperature,
                lightLevel = lightLevel,
                time = DateTime.Now
            };

            var messageString = JsonConvert.SerializeObject(weatherDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient?.SendEventAsync(message);

            Debug.WriteLine("Message sent: " + message);
        }
    }
}
