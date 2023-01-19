using System;
using System.IO.Ports;

namespace ArduinoSerial;

public class Arduino
{
    public Arduino()
    {
        PortName = string.Empty;
        m_driver = new ArduinoDriver();
    }

    public bool PortDetected() => !string.IsNullOrWhiteSpace(PortName);

    public string PortName { get; private set; }

    public ArduinoDriver Driver => m_driver;

    public bool Connect(string comPortName, int boundRate, TimeSpan timeout, int packetSize)
    {
        m_driver.OpenConnection(comPortName, boundRate);
        if (m_driver.IsConnected)
        {
            m_driver.SendCommand("W!ARDUINO!");
            m_driver.ReadAnswer(packetSize, timeout);
            if (m_driver.AsString.Contains("!ARDUINO!"))
            {
                PortName = comPortName;
                return true;
            }
            m_driver.CloseConnection();
        }

        return false;
    }

    public bool AutodetectArduinoPort(int boundRate, TimeSpan timeout, int packetSize)
    {
        var ports = SerialPort.GetPortNames();

        foreach (var port in ports)
        {
            Connect(port, boundRate, timeout, packetSize);
            if (PortDetected())
            {
                return true;
            }
        }

        return false;
    }

    private ArduinoDriver m_driver;
}