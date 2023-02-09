using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ArduinoSerial;

public enum ProtocolCommands
{
    Repeat,
    Message,
    Echo,
    SetPoints,
    SetPointsSuccess,
    SetPointsFailed,
    StartMove,
    MoveCompleted,
    MoveFailed,
    Error
}

public static class ProtocolCommandsExtension
{
    public static char ToChar(this ProtocolCommands command)
        => m_dictionary[command];


    private static Dictionary<ProtocolCommands, char> m_dictionary = new()
    {
        { ProtocolCommands.Repeat, 'R' },
        { ProtocolCommands.Message, '!' },
        { ProtocolCommands.Echo, 'W' },
        { ProtocolCommands.SetPoints, 'S' },
        { ProtocolCommands.SetPointsSuccess, 's' },
        { ProtocolCommands.SetPointsFailed, 'f' },
        { ProtocolCommands.StartMove, 'M' },
        { ProtocolCommands.MoveCompleted, 'm' },
        { ProtocolCommands.MoveFailed, 'x' },
        { ProtocolCommands.Error, 'E' }
    };
}

public class ArduinoCommandBuilder
{
    public ArduinoCommandBuilder()
    {
        m_bytes = new List<byte>();
        // TODO(narzaru) hardcode here !
        m_command = new BaseArduinoCommand(17);
        m_message = string.Empty;
        m_header = char.MinValue;
        m_firstPoint = new Vector2();
        m_secondPoint = new Vector2();
    }

    public ArduinoCommandBuilder SetHeader(ProtocolCommands command)
    {
        m_header = command.ToChar();
        return this;
    }

    public ArduinoCommandBuilder SetFirstPoint(Vector2 point)
    {
        m_firstPoint = point;
        return this;
    }

    public ArduinoCommandBuilder SetSecondPoint(Vector2 point)
    {
        m_secondPoint = point;
        return this;
    }

    public ArduinoCommandBuilder SetMessage(string message)
    {
        m_bytes.AddRange(Encoding.ASCII.GetBytes(message));
        return this;
    }

    public BaseArduinoCommand Build()
    {
        // Put packet header to output list of bytes
        m_bytes.Add(Convert.ToByte(m_header));

        // if message is not set, load data from a points
        // else load message to output list of bytes
        if (string.IsNullOrWhiteSpace(m_message))
        {
            m_bytes.AddRange(BitConverter.GetBytes(m_firstPoint.X));
            m_bytes.AddRange(BitConverter.GetBytes(m_firstPoint.Y));
            m_bytes.AddRange(BitConverter.GetBytes(m_secondPoint.X));
            m_bytes.AddRange(BitConverter.GetBytes(m_secondPoint.Y));
        }
        else
        {
            if (m_message.Length < m_command.BytesCount)
            {
                m_message = m_message.PadRight(m_command.BytesCount - m_message.Length);
            }
            m_bytes.AddRange(Encoding.UTF8.GetBytes(m_message));
        }

        // copy bytes list to bytes array
        for (int i = 0; i < m_bytes.Count; ++i)
        {
            m_command.Bytes[i] = m_bytes[i];
        }

        // After work clear the data to add the ability to set a other parameters.
        m_message = string.Empty;
        m_bytes.Clear();
        
        // return command class
        return m_command;
    }

    private BaseArduinoCommand m_command;
    private List<byte> m_bytes;

    private string m_message;
    private char m_header;
    private Vector2 m_firstPoint;
    private Vector2 m_secondPoint;
}