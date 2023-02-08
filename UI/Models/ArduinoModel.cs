using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ArduinoSerial;

namespace UI.Models;

public class ArduinoModel
{
    public ArduinoModel()
    {
        m_arduino = new Arduino();
        m_builder = new ArduinoCommandBuilder();
    }

    public void Connect(string comPortName, int boundRate, TimeSpan timeout, int packetSize)
    {
        m_arduino.Connect(comPortName, boundRate, timeout, packetSize);
    }

    public void GoToZero()
    {
        m_arduino.Driver.Send(
            m_builder
                .SetHeader(ProtocolCommands.SetPoints)
                .SetFirstPoint(new Vector2(0.0f, 0.0f))
                .SetSecondPoint(new Vector2(0.0f, 0.0f))
                .Build());
        m_arduino.Driver.Read(17, TimeSpan.FromSeconds(5));

        if (m_arduino.Driver.AsString[0] == ProtocolCommands.SetPointsSuccess.ToChar())
        {
            Thread.Sleep(TimeSpan.FromSeconds(4));
        }
    }

    public void GoToFixed(Vector2 first, Vector2 second)
    {
        m_arduino.Driver.Send(
            m_builder
            .SetHeader(ProtocolCommands.SetPoints)
            .SetFirstPoint(first)
            .SetSecondPoint(second)
            .Build());
        m_arduino.Driver.Read(17, TimeSpan.FromSeconds(5));
        if (m_arduino.Driver.AsString[0] == ProtocolCommands.SetPointsSuccess.ToChar())
        {
            // !TODO(narzaru) do stuff
        }
    }

    public bool IsConnected => m_arduino.Driver.IsConnected;
    private Arduino m_arduino;
    private ArduinoCommandBuilder m_builder;
}