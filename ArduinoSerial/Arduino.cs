using System;
using System.Timers;
using System.IO.Ports;

namespace ArduinoSerial;

public class Arduino
{
    public Arduino()
    {
        PortName = string.Empty;
    }

    public bool AutodetectArduinoPort(int boundRate, TimeSpan timeout)
    {
        var ports = SerialPort.GetPortNames();
        foreach (var portName in ports)
        {
            try
            {
                SerialPort port = new SerialPort(portName, boundRate);
                if (!port.IsOpen)
                {
                    port.Open();
                    port.ReadTimeout = (int)timeout.TotalMilliseconds;
                    port.Write("detect");
                    var answer = port.ReadLine();
                    if (answer.Contains("detected"))
                    {
                        PortName = portName;
                        port.Close();
                        return true;
                    }
                    port.Close();
                }
            }
            catch
            {
                // ignored
            }
        }

        return false;
    }

    public bool PortDetected() => !PortName.Equals(String.Empty);

    public string PortName
    {
        get;
        private set;
    }
}