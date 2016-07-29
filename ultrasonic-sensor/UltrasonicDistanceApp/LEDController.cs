using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace UltrasonicDistanceApp
{
    public class LEDController
    {
        GpioController gpio = GpioController.GetDefault();

        GpioPin GreenPin;
        GpioPin RedPin;

        public LEDController(int greenPin, int redPin)
        {
            this.GreenPin = gpio.OpenPin(greenPin);
            this.RedPin = gpio.OpenPin(redPin);

            this.GreenPin.SetDriveMode(GpioPinDriveMode.Output);
            this.RedPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void TurnOnGreenPin()
        {
            try
            {
                this.GreenPin.Write(GpioPinValue.High);
                this.RedPin.Write(GpioPinValue.Low);
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION TurnOnGreen Message: {0}", e.Message);
            }
        }

        public void TurnOnRedPin()
        {
            this.GreenPin.Write(GpioPinValue.Low);
            this.RedPin.Write(GpioPinValue.High);
        }

        public void TurnOnAll()
        {
            this.GreenPin.Write(GpioPinValue.High);
            this.RedPin.Write(GpioPinValue.High);
        }

        public void TurnOffAll()
        {
            this.GreenPin.Write(GpioPinValue.Low);
            this.RedPin.Write(GpioPinValue.Low);
        }

    }
}
