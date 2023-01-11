using System;
using System.Diagnostics;
using System.Timers;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

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
            if (portName == "COM1") continue; // !TODO(narzaru) delete this <---
            var arduinoDriver = new ArduinoDriver(portName, boundRate, timeout, 9);
            var answer = arduinoDriver.SendCommand("W????????");
            if (Encoding.ASCII.GetString(answer).Contains("!ARDUINO"))
            {
                PortName = portName;
                return true;
            }
        }

        return false;
    }

    public bool PortDetected() => !PortName.Equals(String.Empty);

    public string PortName { get; private set; }
}