using System;
using System.IO.Ports;
using System.Linq;

namespace ArduinoSerial;

public class Arduino
{
    #region Properties

    public bool PortDetected() => !string.IsNullOrWhiteSpace(PortName);
    public string PortName { get; private set; }
    public Interfaces.IArduinoDriver Driver => m_driver;

    #endregion

    public Arduino()
    {
        PortName = string.Empty;
        m_driver = new Driver.Driver();
        m_commandBuilder = new Command.ArduinoCommandBuilder();
    }

    ~Arduino()
    {
        m_driver.Dispose();
    }

    public bool Connect(string comPortName, int boundRate, TimeSpan timeout, int packetSize)
    {
        m_driver.OpenConnection(comPortName, boundRate);
        if (!m_driver.IsConnected) return false;

        var command = m_commandBuilder
            .SetHeader(Command.ProtocolCommands.Echo)
            .SetMessage("HelloArduino")
            .SetPacketSize(17)
            .Build();

        m_driver.SendBlocking(command);
        m_driver.ReadBlocking(packetSize, timeout);
        if (m_driver.BytesRead.SequenceEqual(command.Bytes))
        {
            PortName = comPortName;
            return true;
        }

        m_driver.CloseConnection();

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

    #region data

    private Interfaces.IArduinoDriver m_driver;
    private Command.ArduinoCommandBuilder m_commandBuilder;

    #endregion
}