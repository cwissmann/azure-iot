using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace UltrasonicDistanceApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HCSR04 ultrasonicSensor;
        private LEDController ledController;

        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            this.Setup();
        }

        private void Setup()
        {
            this.ultrasonicSensor = new HCSR04(27, 22);
            this.ledController = new LEDController(19, 26);

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(500);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            var distance = this.ultrasonicSensor.GetDistance();

            Debug.WriteLine("Distance: {0} cm", distance);

            if (distance > 10.0)
            {
                Debug.WriteLine("Green");
                this.ledController.TurnOnGreenPin();
            }
            else
            {
                Debug.WriteLine("Red");
                this.ledController.TurnOnRedPin();
            }
        }
    }
}
