using System;
using System.Collections.Generic;
using System.Reactive;
using Data.Models;
using ReactiveUI;
using static System.String;

namespace UI.ViewModels.FixedPoints;

public class NewFixedPointsViewModel : ViewModelBase
{
    public NewFixedPointsViewModel()
    {
        var createCommandEnable = this.WhenAnyValue(
            property1: x => x.FirstPointPositionX,
            property2: x => x.FirstPointPositionY,
            property3: x => x.SecondPointPositionX,
            property4: x => x.SecondPointPositionY,
            property5: x => x.m_IsCorrectInput,
            selector: (p1, p2, p3, p4, p5)
                => !IsNullOrWhiteSpace(p1) &&
                   !IsNullOrWhiteSpace(p2) &&
                   !IsNullOrWhiteSpace(p3) &&
                   !IsNullOrWhiteSpace(p4) && p5);

        CreateCommand = ReactiveCommand.Create(() => new PointsSet()
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
        });
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit, PointsSet> CreateCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

    public string Label { get; set; } = Empty;
    public string FirstPointPositionX { get; set; } = Empty;
    public string FirstPointPositionY { get; set; } = Empty;
    public string SecondPointPositionX { get; set; } = Empty;
    public string SecondPointPositionY { get; set; } = Empty;

    private bool m_IsCorrectInput = false;
}