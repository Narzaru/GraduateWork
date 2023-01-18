using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Threading;
using ReactiveUI;
using UI.Models;

namespace UI.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        SelectedComPort = String.Empty;
        IsConnectingInProgress = false;
        BoundRate = "115200";
        TimeOut = "4";
        PacketSize = "10";
        UpdateComPortsCommand = ReactiveCommand.Create(UpdatePorts);
        ConnectArduinoCommand = ReactiveCommand.Create(ConnectArduino);
    }

    // Bound rate text box
    public string BoundRate { get; set; }

    // Time out text box
    public string TimeOut { get; set; }

    // Packet size text box
    public string PacketSize { get; set; }

    // Com ports combat box
    public List<string> ComPorts
    {
        get => m_listOfComPorts;
        set => this.RaiseAndSetIfChanged(ref m_listOfComPorts, value);
    }

    public string SelectedComPort { get; set; }

    // Refresh button
    public ReactiveCommand<Unit, Unit> UpdateComPortsCommand { get; }

    public void UpdatePorts()
    {
        ComPorts = SerialPort.GetPortNames().ToList();
    }

    // Connect button
    // TODO(Narzaru) move connection state to model class
    public ReactiveCommand<Unit, Unit> ConnectArduinoCommand { get; }
    private void ConnectArduino()
    {
        var thread = new Thread(() =>
        {
            IsConnectingInProgress = true;
            m_arduinoModel.Connect(
                SelectedComPort,
                int.Parse(BoundRate),
                TimeSpan.FromSeconds(int.Parse(TimeOut)),
                int.Parse(PacketSize)
            );
            IsConnectingInProgress = false;
        });
        thread.Start();
    }
    public bool IsConnectingInProgress
    {
        get => m_isConnectingInProgress;
        private set => this.RaiseAndSetIfChanged(ref m_isConnectingInProgress, value);
    }
    private bool m_isConnectingInProgress;

    // current list of com ports
    private List<string> m_listOfComPorts = new();

    // Models
    private readonly ArduinoModel m_arduinoModel = new ArduinoModel();
}