using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TetrisPro.Core.Config;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;

namespace TetrisPro.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IGameEngine _engine;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> boardCells = new();

    [ObservableProperty]
    private IList<PieceType> nextPieces = new List<PieceType>();

    [ObservableProperty]
    private Tetromino? holdPiece;

    [ObservableProperty] private int score;
    [ObservableProperty] private int level;
    [ObservableProperty] private int lines;

    public MainViewModel(IGameEngine engine)
    {
        _engine = engine;
    }

    [RelayCommand]
    private void NewGame()
    {
        _engine.StartNewGame(new GameConfig());
        UpdateState();
    }

    [RelayCommand] private void Pause() => _engine.Pause();
    [RelayCommand] private void Resume() => _engine.Resume();

    public void UpdateState()
    {
        var state = _engine.State;
        Score = state.Score;
        Level = state.Level;
        Lines = state.Lines;
        HoldPiece = state.HoldPiece;
        NextPieces = state.NextQueue.ToList();
        BoardCells = new ObservableCollection<CellViewModel>(
            state.Board.GetCells().Select(c => new CellViewModel(c.type switch
            {
                PieceType.I => "#00FFFF",
                PieceType.J => "#0000FF",
                PieceType.L => "#FFA500",
                PieceType.O => "#FFFF00",
                PieceType.S => "#00FF00",
                PieceType.T => "#800080",
                PieceType.Z => "#FF0000",
                _ => "#000000"
            })));
    }

    public class CellViewModel
    {
        public string Color { get; }
        public CellViewModel(string color) => Color = color;
    }
}
