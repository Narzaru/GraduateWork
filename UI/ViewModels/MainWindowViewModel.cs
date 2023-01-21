using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Data;
using Data.Models;
using DynamicData;
using ReactiveUI;
using UI.Models;
using UI.ViewModels.FixedPoints;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Constructors

    public MainWindowViewModel()
    {
        m_content = List = new FixedPointsViewModel(this);
        SelectedComPort = String.Empty;
        IsConnectingInProgress = false;
        BoundRate = "115200";
        TimeOut = "4";
        PacketSize = "10";
        UpdateComPortsCommand = ReactiveCommand.Create(UpdatePorts);
        ConnectArduinoCommand = ReactiveCommand.Create(ConnectArduino);
        LoadFixedPointsCommand = ReactiveCommand.Create(LoadFixedPoints);
        NewFixedPointsCommand = ReactiveCommand.Create(NewFixedPoints);
    }

    #endregion

    #region BoundRateTextBox

    public string BoundRate { get; set; }

    #endregion

    #region TimeOutTextBox

    public string TimeOut { get; set; }

    #endregion

    #region PacketSizeTextBox

    public string PacketSize { get; set; }

    #endregion

    #region ComPortsCombatBox

    public List<string> ComPorts
    {
        get => m_listOfComPorts;
        set => this.RaiseAndSetIfChanged(ref m_listOfComPorts, value);
    }

    private List<string> m_listOfComPorts = new();

    public string SelectedComPort { get; set; }

    #endregion

    #region RefreshButton

    public ReactiveCommand<Unit, Unit> UpdateComPortsCommand { get; }

    public void UpdatePorts()
    {
        ComPorts = SerialPort.GetPortNames().ToList();
    }

    #endregion

    #region ConnectButton

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

    #endregion

    // TODO(Narzaru) move connection state to ArduinoModel class

    #region ConnectionState

    public bool IsConnectingInProgress
    {
        get => m_isConnectingInProgress;
        private set => this.RaiseAndSetIfChanged(ref m_isConnectingInProgress, value);
    }

    private bool m_isConnectingInProgress;

    #endregion

    #region FixedPointsView

    public ViewModelBase Content
    {
        get => m_content;
        set => this.RaiseAndSetIfChanged(ref m_content, value);
    }

    public FixedPointsViewModel List { get; }

    private ViewModelBase m_content;

    public ReactiveCommand<Unit, Unit> LoadFixedPointsCommand { get; set; }

    public void LoadFixedPoints()
    {
        var vm = new LoadFixedPointsViewModel();

        Observable.Merge(
                vm.OkCommand,
                vm.CancelCommand.Select(_ => (PointsSet?)null))
            .Subscribe(model =>
            {
                if (model != null)
                {
                    List.Points.Clear();
                    List.Points.AddRange(model.Points);
                }

                Content = List;
            });

        Content = vm;
    }

    public ReactiveCommand<Unit, Unit> NewFixedPointsCommand { get; set; }

    public void NewFixedPoints()
    {
        var vm = new NewFixedPointsViewModel();

        Observable.Merge(
                vm.CreateCommand,
                vm.CancelCommand.Select(_ => (PointsSet?)null))
            .Subscribe(model =>
            {
                if (model != null)
                {
                    using var dbContext = new FixedPointsContext();
                    dbContext.PointsSet!.Add(model);
                    dbContext.Points!.AddRange(model.Points);
                    dbContext.SaveChangesAsync();
                    List.Points.Clear();
                    List.Points.AddRange(model.Points);
                }

                Content = List;
            });

        Content = vm;
    }

    #endregion // FixedPointMenu

    #region Models

    private readonly ArduinoModel m_arduinoModel = new ArduinoModel();

    #endregion
}