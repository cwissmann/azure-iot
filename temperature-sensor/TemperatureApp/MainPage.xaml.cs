using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
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

namespace TemperatureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController gpioController;

        private DHT11 dhtSensor;

        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            //this.Setup();
            this.SetupTest();
        }

        private void Setup()
        {
            this.gpioController = GpioController.GetDefault();

            var dataPin = gpioController.OpenPin(4);

            dhtSensor = new DHT11(dataPin);

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(2);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void SetupTest()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(2);
            this.timer.Tick += TimerTest_Tick;
            this.timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            var humidity = await dhtSensor.ReadHumidity();
            var temperature = await dhtSensor.ReadTemperatureAsync(false);

            Debug.WriteLine("Hum:  {0}", humidity);
            Debug.WriteLine("Temp: {0}", temperature);
        }

        private void TimerTest_Tick(object sender, object e)
        {
            Debug.WriteLine("App is running: {0}", DateTime.Now.Second);
        }
    }
}
