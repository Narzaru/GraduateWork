using System;
using System.Threading.Tasks;
using ArduinoSerial;

namespace UI.Models;

public class ArduinoModel
{
    public ArduinoModel()
    {
        m_arduino = new Arduino();
    }

    public void Connect(string comPortName, int boundRate, TimeSpan timeout, int packetSize)
    {
        m_arduino.Connect(comPortName, boundRate, timeout, packetSize);
    }
    
    public bool IsConnected => m_arduino.Driver.IsConnected;
    private Arduino m_arduino;
}