using GHIElectronics.UWP.Gadgeteer.Mainboards;
using GTMO = GHIElectronics.UWP.Gadgeteer.Modules;
using GHIElectronics.UWP.GadgeteerCore;
using System;
using System.Collections.Generic;
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
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PowerSwitch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FEZCream mainboard;
        private GTMO.RelayX1 relay1;
        private GTMO.Button button;

        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            this.Setup();
        }

        private async void Setup()
        {
            this.mainboard = await Module.CreateAsync<FEZCream>();
            this.relay1 = await Module.CreateAsync<GTMO.RelayX1>(this.mainboard.GetProvidedSocket(4));
            this.button = await Module.CreateAsync<GTMO.Button>(this.mainboard.GetProvidedSocket(3));

            this.ProgramStarted();
        }

        private void ProgramStarted()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            var pressed = this.button.IsPressed();

            if (pressed)
            {
                Debug.WriteLine("Button pressed");

                if (this.relay1.Enabled)
                {
                    Debug.WriteLine("TurnOff");
                    this.relay1.TurnOff();
                }
                else
                {
                    Debug.WriteLine("TurnOn");
                    this.relay1.TurnOn();
                }
            }
        }
    }
}
