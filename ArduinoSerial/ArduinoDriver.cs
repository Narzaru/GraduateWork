using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;

// ReSharper disable All

namespace ArduinoSerial;

public class ArduinoDriver : IDisposable
{
    public byte[] ReadBytes { get; set; } = new byte[] { };
    public TimeSpan TimeOut { get; set; }

    public int PacketSize { get; set; }

    public ArduinoDriver(string comPort, int baudRate, TimeSpan timeOut, int packetSize)
    {
        try
        {
            TimeOut = timeOut;
            PacketSize = packetSize;
            m_serial = new SerialPort(comPort, baudRate);

            if (!m_serial.IsOpen)
            {
                m_serial.DataReceived += Arduino_DataReceived;

                try
                {
                    m_serial.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Serial couldn't open", ex);
                }
            }
            else throw new Exception("Error Serial is already open!");
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    public byte[] SendCommand(object obj)
    {
        IsReadingComplete = false;

        if (m_serial.IsOpen)
        {
            if (obj is ArduinoCommands arduinoCommand)
            {
                m_serial.Write(arduinoCommand.ToString());
            }
            else if (obj is string command)
            {
                m_serial.Write(command);
            }
            else if (obj is byte[] bytes)
            {
                m_serial.Write(bytes, 0, bytes.Length);
            }
        }

        var timer = Stopwatch.StartNew();
        while (!IsReadingComplete)
        {
            if (timer.ElapsedMilliseconds > TimeOut.TotalMilliseconds)
            {
                timer.Stop();
                IsReadingComplete = true;
            }
        }

        ReadBytes = Bytes.ToArray();
        return ReadBytes;
    }

    public void Close()
    {
        m_serial.Close();
    }

    private void Arduino_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (sender is SerialPort serialPort)
        {
            int bytesToRead = serialPort.BytesToRead;
            byte[] bytes = new byte[bytesToRead];
            serialPort.Read(bytes, 0, bytesToRead);
            foreach (var b in bytes)
            {
                Bytes.Add(b);
            }

            if (Bytes.Count == PacketSize)
            {
                IsReadingComplete = true;
            }
        }
    }

    private SerialPort m_serial;
    private bool IsReadingComplete;
    private List<byte> Bytes { get; set; } = new();

    #region dispose_reggion

    private bool disposedValue = false; // To detect redundant calls

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (m_serial.IsOpen)
                    Close();
            }

            m_serial = null;
            disposedValue = true;
        }
    }

    #endregion
}