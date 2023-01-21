using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Data;
using Data.Models;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace UI.ViewModels.FixedPoints;

public class LoadFixedPointsViewModel : ViewModelBase
{
    #region Constructors

    public LoadFixedPointsViewModel()
    {
        // Load fixed points from PointsSet database
        using (var dbContext = new FixedPointsContext())
        {
            if (dbContext.PointsSet is null)
            {
                throw new NullReferenceException("Can't load data base");
            }

            var result = dbContext.PointsSet
                .Include(p => p.Points)
                .ToList();

            PointsSet.Clear();
            PointsSet.AddRange(result);
        }

        var okEnable = this.WhenAnyValue(
            property1: x => x.SelectedItem,
            selector: item => item is not null);
        
        OkCommand = ReactiveCommand.Create<PointsSet>(() => SelectedItem!, okEnable);
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    #endregion

    public ReactiveCommand<Unit, PointsSet> OkCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }

    #region PointsSetListBox

    public ObservableCollection<PointsSet> PointsSet { get; set; } = new();

    public PointsSet? SelectedItem
    {
        get => m_selectedItem;
        set => this.RaiseAndSetIfChanged(ref m_selectedItem, value);
    }

    private PointsSet? m_selectedItem = null;

    #endregion
}