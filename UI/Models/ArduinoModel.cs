using System;
using ArduinoSerial;

namespace UI.Models;

public class ArduinoModel
{
    public ArduinoModel()
    {
        Terminal = String.Empty;
        m_arduino = new Arduino();
    }

    public void Connect(string comPortName, int boundRate, TimeSpan timeout, int packetSize)
    {
        Terminal = $"Trying to connect {comPortName}\n" + Terminal;
        m_arduino.Connect(comPortName, boundRate, timeout, packetSize);
        if (m_arduino.Driver.IsConnected)
            Terminal = $"Connected to {comPortName}\n" + Terminal;
        else
            Terminal = $"Cant connect to {comPortName}\n" + Terminal;
    }

    public string Terminal { get; private set; }
    public bool IsConnected => m_arduino.Driver.IsConnected;
    private Arduino m_arduino;
}