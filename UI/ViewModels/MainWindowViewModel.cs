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
        IsConnectionInProgress = false;
        m_arduino = new ArduinoModel();
        ConnectCommand = ReactiveCommand.Create<string>(ConnectToArduino);
        UpdatePortsCommand = ReactiveCommand.Create(UpdateListOfPorts);
        Ports = SerialPort.GetPortNames().ToList();
    }

    public List<string> Ports { get; private set; }
    public ReactiveCommand<Unit, Unit> UpdatePortsCommand { get; }
    public void UpdateListOfPorts()
    {
        Ports = SerialPort.GetPortNames().ToList();
    }

    public ReactiveCommand<string, Unit> ConnectCommand { get; }
    public void ConnectToArduino(string parameter)
    {
        IsConnectionInProgress = true;
        var thread = new Thread(() =>
        {
            m_arduino.Connect(parameter, 115200, TimeSpan.FromSeconds(8), 10);
            TerminalPresenter = m_arduino.Terminal;
            IsConnectionInProgress = false;
        });
        thread.Start();
    }

    public string TerminalPresenter
    {
        get => m_virtualTerminal;
        private set => this.RaiseAndSetIfChanged(ref m_virtualTerminal, value);
    }

    public bool IsConnectionInProgress
    {
        get => m_isConnectionInProgress;
        private set => this.RaiseAndSetIfChanged(ref m_isConnectionInProgress, value);
    }

    private string m_virtualTerminal = String.Empty;
    private bool m_isConnectionInProgress;
    private ArduinoModel m_arduino;
}