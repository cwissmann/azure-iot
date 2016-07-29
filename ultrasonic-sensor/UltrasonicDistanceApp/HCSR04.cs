using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace UltrasonicDistanceApp
{
    public class HCSR04
    {
        GpioController gpio = GpioController.GetDefault();

        GpioPin TriggerPin;
        GpioPin EchoPin;

        public HCSR04(int triggerPin, int echoPin)
        {
            Debug.WriteLine("Initialize HCSR04");
            Debug.WriteLine("TriggerPin {0}", triggerPin);
            Debug.WriteLine("EchoPin {0}", echoPin);

            this.TriggerPin = gpio.OpenPin(triggerPin);
            this.EchoPin = gpio.OpenPin(echoPin);

            this.TriggerPin.SetDriveMode(GpioPinDriveMode.Output);
            this.EchoPin.SetDriveMode(GpioPinDriveMode.Input);

            this.TriggerPin.Write(GpioPinValue.Low);

            Debug.WriteLine("Sensor initialized");
        }

        public double GetDistance()
        {
            try
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                mre.WaitOne(500);
                Stopwatch pulseLength = new Stopwatch();

                this.TriggerPin.Write(GpioPinValue.High);
                mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
                this.TriggerPin.Write(GpioPinValue.Low);

                while (this.EchoPin.Read() == GpioPinValue.Low)
                {
                }
                pulseLength.Start();

                while (this.EchoPin.Read() == GpioPinValue.High)
                {
                }
                pulseLength.Stop();

                TimeSpan timeBetween = pulseLength.Elapsed;
                Debug.WriteLine(timeBetween.ToString());
                double distance = timeBetween.TotalSeconds * 17000;

                return distance;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION GetDistance Message: {0}", e.Message);

                return 0.0;
            }
                       
        }
    }
}
