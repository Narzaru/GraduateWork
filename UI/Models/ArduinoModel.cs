using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ArduinoSerial;
using ArduinoSerial.Command;

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

    public bool GoToZero()
    {
        Task.Run(() => m_arduino.Driver.SendInBlocking(
            m_builder
                .SetHeader(ProtocolCommands.SetPoints)
                .SetFirstPoint(new Vector2(0.0f, 0.0f))
                .SetSecondPoint(new Vector2(0.0f, 0.0f))
                .Build()));
        m_arduino.Driver.ReadInBlocking(17, TimeSpan.FromSeconds(5));

        return m_arduino.Driver.BytesRead[0] == ProtocolCommands.SetPointsSuccess.ToChar();
    }

    public bool GoToFixed(Vector2 first, Vector2 second)
    {
        m_arduino.Driver.SendInBlocking(
            m_builder
                .SetHeader(ProtocolCommands.SetPoints)
                .SetFirstPoint(first)
                .SetSecondPoint(second)
                .Build());
        m_arduino.Driver.ReadInBlocking(17, TimeSpan.FromSeconds(5));
        return m_arduino.Driver.BytesRead[0] == ProtocolCommands.SetPointsSuccess.ToChar();
    }

    public Arduino Arduino => m_arduino;
    public bool IsConnected => m_arduino.Driver.IsConnected;
    private Arduino m_arduino;
    private ArduinoCommandBuilder m_builder;
}