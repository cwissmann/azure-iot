using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace simulated_device
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "[iothuburi]";
        static string deviceId = "[deviceid]";
        static string deviceKey = "[devicekey]";
        static string connectionString = "[connectionstring]";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated Device ({0})\n", deviceId);
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

            SendMessageToCloudAsync();
            Console.ReadLine();
        }

        public static async void SendMessageToCloudAsync()
        {
            var currentTime = DateTime.Now;

            double avgTemperature = 20.00;
            double avgLightLevel = 70.00;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = avgTemperature + rand.NextDouble() * 4 - 2;
                double currentLightLevel = (avgLightLevel + rand.NextDouble() * 4 - 2) / 100;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    temperature = currentTemperature.ToString("N2", CultureInfo.InvariantCulture),
                    lightLevel = currentLightLevel.ToString("P2", CultureInfo.InvariantCulture),
                    time = currentTime
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", currentTime, messageString);

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
    }
}
