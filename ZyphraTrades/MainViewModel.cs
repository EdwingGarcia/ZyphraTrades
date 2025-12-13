using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Presentation;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ITradeRepository _repo;

    public ObservableCollection<Trade> Trades { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand AddDummyTradeCommand { get; }

    public MainViewModel(ITradeRepository repo)
    {
        _repo = repo;
        RefreshCommand = new AsyncCommand(LoadAsync);
        AddDummyTradeCommand = new AsyncCommand(AddDummyAsync);
    }

    public async Task LoadAsync()
    {
        Trades.Clear();
        var items = await _repo.GetAllAsync();
        foreach (var t in items) Trades.Add(t);
    }

    public async Task AddDummyAsync()
    {
        var trade = new Trade
        {
            Symbol = "AUDUSD",
            Entry = 0.6550m,
            Exit = 0.6580m,
            PnL = 25.50m,
            Notes = "Primera prueba de journal"
        };

        await _repo.AddAsync(trade);
        await LoadAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
