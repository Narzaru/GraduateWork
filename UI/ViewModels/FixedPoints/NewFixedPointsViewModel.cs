using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Data.Models;
using ReactiveUI;
using static System.String;
using Point = Data.Models.Point;

namespace UI.ViewModels.FixedPoints;

public class NewFixedPointsViewModel : ViewModelBase
{
    public NewFixedPointsViewModel()
    {
        var createCommandEnabled = this.WhenAnyValue(
            property1: x => x.FirstPointPositionX,
            property2: x => x.FirstPointPositionY,
            property3: x => x.SecondPointPositionX,
            property4: x => x.SecondPointPositionY,
            selector: (p1, p2, p3, p4) =>
                !IsNullOrWhiteSpace(p1) &&
                !IsNullOrWhiteSpace(p2) &&
                !IsNullOrWhiteSpace(p3) &&
                !IsNullOrWhiteSpace(p4));

        CreateCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                return new PointsSet
                {
                    Label = Label,
                    CreationTime = DateTime.Now,
                    Points = new List<Point>
                    {
                        new Point
                        {
                            PositionX = float.Parse(FirstPointPositionX),
                            PositionY = float.Parse(FirstPointPositionY)
                        },
                        new Point
                        {
                            PositionX = float.Parse(SecondPointPositionX),
                            PositionY = float.Parse(SecondPointPositionY)
                        }
                    }
                };
            }
            catch
            {
                return null;
            }
        }, createCommandEnabled);
        CancelCommand = ReactiveCommand.Create(() => (PointsSet?)null);
    }

    public ReactiveCommand<Unit, PointsSet?> CreateCommand { get; set; }
    public ReactiveCommand<Unit, PointsSet?> CancelCommand { get; set; }

    public string Label { get; set; } = Empty;

    #region PoinstCoordinatedTextBox

    public string FirstPointPositionX
    {
        get => m_firstPointPositionX;
        set => this.RaiseAndSetIfChanged(ref m_firstPointPositionX, value);
    }

    public string FirstPointPositionY
    {
        get => m_firstPointPositionY;
        set => this.RaiseAndSetIfChanged(ref m_firstPointPositionY, value);
    }

    public string SecondPointPositionX
    {
        get => m_secondPointPositionX;
        set => this.RaiseAndSetIfChanged(ref m_secondPointPositionX, value);
    }

    public string SecondPointPositionY
    {
        get => m_secondPointPositionY;
        set => this.RaiseAndSetIfChanged(ref m_secondPointPositionY, value);
    }

    private string m_firstPointPositionX = Empty;
    private string m_firstPointPositionY = Empty;
    private string m_secondPointPositionX = Empty;
    private string m_secondPointPositionY = Empty;

    #endregion
}