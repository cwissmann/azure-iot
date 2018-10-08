using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Sensors;

namespace TemperatureApp
{
    public class DHT11
    {
        private UInt32[] _data;
        private GpioPin _dataPin;
        private bool _firstReading;
        private bool _lastResult;
        private double _secondsSinceLastRead;

        public DHT11()
        {
        }

        public DHT11(GpioPin dataPin)
        {
            if (dataPin != null)
            {
                _dataPin = dataPin;
                _secondsSinceLastRead = 0D;
                _firstReading = true;
                _data = new UInt32[6];

                _dataPin.SetDriveMode(GpioPinDriveMode.Input);
                _dataPin.Write(GpioPinValue.High);
                _firstReading = true;
                _secondsSinceLastRead = 0;
            }
            else
            {
                throw new ArgumentException("Parameter cannot be null.", "dataPin");
            }
        }

        public async Task<float> ReadTemperatureAsync(bool s)
        {
            return await Task.Run<float>(async () =>
            {
                float f = 0;

                if (await Read())
                {

                    // DHT11
                    f = _data[2];
                    if (s)
                    {
                        f = ConvertCtoF(f);
                    }

                    //switch (_sensorType)
                    //{
                    //    case SensorTypes.DHT11:
                    //        f = _data[2];
                    //        if (S)
                    //        {
                    //            f = ConvertCtoF(f);
                    //        }
                    //        break;
                    //    case SensorTypes.DHT22:
                    //    case SensorTypes.DHT21:
                    //        f = _data[2] & 0x7F;
                    //        f *= 256;
                    //        f += _data[3];
                    //        f /= 10;
                    //        if ((_data[2] & 0x80) != 0)
                    //        {
                    //            f *= -1;
                    //        }
                    //        if (S)
                    //        {
                    //            f = ConvertCtoF(f);
                    //        }
                    //        break;
                    //}
                }

                return f;
            });
        }

        public async Task<float> ReadHumidity()
        {
            return await Task.Run(async () =>
            {
                float f = 0;
                if (await Read())
                {
                    // DHT11
                    f = _data[0];

                    //switch (_sensorType)
                    //{
                    //    case SensorTypes.DHT11:
                    //        f = _data[0];
                    //        break;
                    //    case SensorTypes.DHT22:
                    //    case SensorTypes.DHT21:
                    //        f = _data[0];
                    //        f *= 256;
                    //        f += _data[1];
                    //        f /= 10;
                    //        break;
                    //}
                }
                return f;
            });
        }

        float ConvertCtoF(float c)
        {
            return c * 9 / 5 + 32;
        }

        float ConvertFtoC(float f)
        {
            return (f - 32) * 5 / 9;
        }

        private async Task<double> ComputeHeatIndex(float temperature, float percentHumidity, bool isFahrenheit)
        {
            return await Task.Run<double>(() =>
            {
                // Adapted from equation at: https://github.com/adafruit/DHT-sensor-library/issues/9 and
                // Wikipedia: http://en.wikipedia.org/wiki/Heat_index
                if (!isFahrenheit)
                {
                    // Celsius heat index calculation.
                    return -8.784695 +
                             1.61139411 * temperature +
                             2.338549 * percentHumidity +
                            -0.14611605 * temperature * percentHumidity +
                            -0.01230809 * Math.Pow(temperature, 2) +
                            -0.01642482 * Math.Pow(percentHumidity, 2) +
                             0.00221173 * Math.Pow(temperature, 2) * percentHumidity +
                             0.00072546 * temperature * Math.Pow(percentHumidity, 2) +
                            -0.00000358 * Math.Pow(temperature, 2) * Math.Pow(percentHumidity, 2);
                }
                else
                {
                    // Fahrenheit heat index calculation.
                    return -42.379 +
                             2.04901523 * temperature +
                            10.14333127 * percentHumidity +
                            -0.22475541 * temperature * percentHumidity +
                            -0.00683783 * Math.Pow(temperature, 2) +
                            -0.05481717 * Math.Pow(percentHumidity, 2) +
                             0.00122874 * Math.Pow(temperature, 2) * percentHumidity +
                             0.00085282 * temperature * Math.Pow(percentHumidity, 2) +
                            -0.00000199 * Math.Pow(temperature, 2) * Math.Pow(percentHumidity, 2);
                }
            });
        }

        private async Task<bool> Read()
        {

            var currentTimeSeconds = GetMSSinceEpoch();

            if (currentTimeSeconds < _secondsSinceLastRead)
            {
                _secondsSinceLastRead = 0;
            }

            if (!_firstReading && ((currentTimeSeconds - _secondsSinceLastRead) < 2000))
            {
                return _lastResult;
            }

            _firstReading = false;
            _secondsSinceLastRead = GetMSSinceEpoch();

            _data[0] = _data[1] = _data[2] = _data[3] = _data[4] = 0;

            _dataPin.Write(GpioPinValue.High);

            await Task.Delay(250);


            _dataPin.SetDriveMode(GpioPinDriveMode.Output);
            _dataPin.Write(GpioPinValue.Low);

            await Task.Delay(20);


            //TIME CRITICAL ###############
            _dataPin.Write(GpioPinValue.High);
            //=> DELAY OF 40 microseconds needed here


            _dataPin.SetDriveMode(GpioPinDriveMode.Input);
            //Delay of 10 microseconds needed here

            if (await ExpectPulse(GpioPinValue.Low) == 0)
            {
                _lastResult = false;
                return _lastResult;
            }

            if (await ExpectPulse(GpioPinValue.High) == 0)
            {
                _lastResult = false;
                return _lastResult;
            }

            // Now read the 40 bits sent by the sensor.  Each bit is sent as a 50
            // microsecond low pulse followed by a variable length high pulse.  If the
            // high pulse is ~28 microseconds then it's a 0 and if it's ~70 microseconds
            // then it's a 1.  We measure the cycle count of the initial 50us low pulse
            // and use that to compare to the cycle count of the high pulse to determine
            // if the bit is a 0 (high state cycle count < low state cycle count), or a
            // 1 (high state cycle count > low state cycle count).
            for (int i = 0; i < 40; ++i)
            {
                UInt32 lowCycles = await ExpectPulse(GpioPinValue.Low);
                if (lowCycles == 0)
                {

                    _lastResult = false;
                    return _lastResult;
                }
                UInt32 highCycles = await ExpectPulse(GpioPinValue.High);
                if (highCycles == 0)
                {

                    _lastResult = false;
                    return _lastResult;
                }
                _data[i / 8] <<= 1;
                // Now compare the low and high cycle times to see if the bit is a 0 or 1.
                if (highCycles > lowCycles)
                {
                    // High cycles are greater than 50us low cycle count, must be a 1.
                    _data[i / 8] |= 1;
                }
                // Else high cycles are less than (or equal to, a weird case) the 50us low
                // cycle count so this must be a zero.  Nothing needs to be changed in the
                // stored data.
            }
            //TIME CRITICAL_END #############
            // Check we read 40 bits and that the checksum matches.
            if (_data[4] == ((_data[0] + _data[1] + _data[2] + _data[3]) & 0xFF))
            {
                _lastResult = true;
                return _lastResult;
            }
            else
            {
                //Checksum failure!
                _lastResult = false;
                return _lastResult;
            }

        }

        private async Task<UInt32> ExpectPulse(GpioPinValue level)
        {
            return await Task.Run<UInt32>(() =>
            {
                UInt32 count = 0;

                long end = DateTime.Now.AddMilliseconds(1).Ticks;

                while (_dataPin.Read() == level)
                {
                    count++;

                    if (DateTime.Now.Ticks >= end)
                    {
                        return 0;
                    }
                }

                return count;
            });
        }

        private double GetMSSinceEpoch()
        {
            return (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
        }
    }
}
