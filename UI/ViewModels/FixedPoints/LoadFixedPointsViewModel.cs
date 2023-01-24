using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        this.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != nameof(Filter)) return;
            
            CurrentPointsSet.Clear();
            if (string.IsNullOrWhiteSpace(Filter))
            {
                CurrentPointsSet.AddRange(FullPointsSet);
            }
            else
            {
                CurrentPointsSet.AddRange(FullPointsSet.Where(ps => ps.Label.Contains(Filter)));
            }
        };

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

            CurrentPointsSet.Clear();
            FullPointsSet.Clear();
            CurrentPointsSet.AddRange(result);
            FullPointsSet.AddRange(result);
        }

        // Create buttons handlers
        var okEnable = this.WhenAnyValue(
            property1: x => x.SelectedItem,
            selector: item => item is not null);

        OkCommand = ReactiveCommand.Create(() => SelectedItem, okEnable);
        CancelCommand = ReactiveCommand.Create(() => SelectedItem);
    }

    #endregion

    public ReactiveCommand<Unit, PointsSet?> OkCommand { get; private set; }
    public ReactiveCommand<Unit, PointsSet?> CancelCommand { get; private set; }

    #region PointsSetListBox

    public ObservableCollection<PointsSet> CurrentPointsSet { get; set; } = new();
    public List<PointsSet> FullPointsSet { get; set; } = new();

    public PointsSet? SelectedItem
    {
        get => m_selectedItem;
        set => this.RaiseAndSetIfChanged(ref m_selectedItem, value);
    }

    private PointsSet? m_selectedItem = null;

    #endregion

    public string Filter
    {
        get => m_filter;
        set => this.RaiseAndSetIfChanged(ref m_filter, value);
    }

    private string m_filter = string.Empty;
}