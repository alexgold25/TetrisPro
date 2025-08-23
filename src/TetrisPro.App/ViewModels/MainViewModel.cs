using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using TetrisPro.Core.Config;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Models;
using TetrisPro.Core.AI;

namespace TetrisPro.App.ViewModels;

/// <summary>
/// View model bridging the core game engine with the WPF views.
/// It exposes the playfield, next queue and hold piece as collections of
/// coloured cells so that the XAML controls can render them directly.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IGameEngine _engine;
    private readonly AutoPlayer _ai;

    /// <summary>Cells representing the playfield (10×20).</summary>
    public ObservableCollection<CellVm> BoardCells { get; } = new();

    /// <summary>Cells representing the upcoming pieces.</summary>
    public ObservableCollection<IEnumerable<CellVm>> NextPieces { get; } = new();

    [ObservableProperty] private IEnumerable<CellVm>? holdPiece;
    [ObservableProperty] private int score;
    [ObservableProperty] private int level;
    [ObservableProperty] private int lines;
    [ObservableProperty] private TimeSpan elapsedTime;
    [ObservableProperty] private string statusText = "Ready";
    [ObservableProperty] private bool aiEnabled;
    [ObservableProperty] private string aiText = "AI Off";
    [ObservableProperty] private string speedText = "1x";

    public MainViewModel(IGameEngine engine, AutoPlayer ai)
    {
        _engine = engine;
        _ai = ai;

        // Initialise board with empty cells so bindings are ready before the
        // first update occurs.
        for (int i = 0; i < Board.Width * Board.VisibleHeight; i++)
        {
            BoardCells.Add(new CellVm
            {
                Fill = new SolidColorBrush(Color.FromRgb(55, 55, 55)),
                Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30))
            });
        }
    }

    [RelayCommand]
    private void NewGame()
    {
        _engine.StartNewGame(new GameConfig());
        UpdateState();
    }

    [RelayCommand] private void Pause() => _engine.Pause();
    [RelayCommand] private void Resume() => _engine.Resume();
    [RelayCommand]
    private void ToggleAi()
    {
        AiEnabled = !AiEnabled;
        _ai.Enabled = AiEnabled;
        _ai.ResetSpeed();
        AiText = AiEnabled ? "AI On" : "AI Off";
        SpeedText = "1x";
    }

    [RelayCommand]
    private void CycleSpeed()
    {
        _ai.CycleSpeed();
        SpeedText = $"{_ai.SpeedMultiplier}x";
    }

    /// <summary>
    /// Refreshes all values exposed by the view model from the engine's state.
    /// Should be called each frame by <see cref="Views.MainWindow"/>.
    /// </summary>
    public void UpdateState()
    {
        var state = _engine.State;

        Score = state.Score;
        Level = state.Level;
        Lines = state.Lines;
        ElapsedTime = state.Elapsed;
        StatusText = state.Status.ToString();

        // Update board / active / ghost pieces.
        UpdateBoardCells(state);

        // Update next queue.
        NextPieces.Clear();
        foreach (var type in state.NextQueue)
        {
            var piece = new Tetromino(type, ColorFor(type));
            NextPieces.Add(CreateMiniBoard(piece));
        }

        // Update hold piece.
        HoldPiece = state.HoldPiece is null
            ? null
            : CreateMiniBoard(state.HoldPiece);
    }

    private void UpdateBoardCells(GameState state)
    {
        // Prepare a working array of colours for the board.
        var fills = new SolidColorBrush[Board.Width, Board.VisibleHeight];
        var stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30));

        for (int y = 0; y < Board.VisibleHeight; y++)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                fills[x, y] = new SolidColorBrush(Color.FromRgb(55, 55, 55));
            }
        }

        // Locked cells on the board.
        foreach (var (x, y, type) in state.Board.GetCells())
        {
            if (type.HasValue)
                fills[x, y] = HexColor(ColorFor(type.Value));
        }

        // Ghost piece.
        if (state.GhostPiece is not null)
        {
            foreach (var cell in state.GhostPiece.Cells)
            {
                var pos = cell + state.GhostPiece.Position;
                int y = pos.Y - Board.HiddenRows;
                if (pos.X >= 0 && pos.X < Board.Width && y >= 0 && y < Board.VisibleHeight)
                    fills[pos.X, y] = new SolidColorBrush(Color.FromRgb(80, 80, 80));
            }
        }

        // Active piece.
        if (state.ActivePiece is not null)
        {
            var brush = HexColor(state.ActivePiece.Color);
            foreach (var cell in state.ActivePiece.Cells)
            {
                var pos = cell + state.ActivePiece.Position;
                int y = pos.Y - Board.HiddenRows;
                if (pos.X >= 0 && pos.X < Board.Width && y >= 0 && y < Board.VisibleHeight)
                    fills[pos.X, y] = brush;
            }
        }

        // Push colours into the observable collection.
        for (int y = 0; y < Board.VisibleHeight; y++)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                int idx = y * Board.Width + x;
                BoardCells[idx].Fill = fills[x, y];
                BoardCells[idx].Stroke = stroke;
            }
        }
    }

    private static IEnumerable<CellVm> CreateMiniBoard(Tetromino piece)
    {
        var cells = new CellVm[16];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new CellVm
            {
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30))
            };
        }

        var minX = piece.Cells.Min(c => c.X);
        var minY = piece.Cells.Min(c => c.Y);
        var brush = HexColor(piece.Color);

        foreach (var c in piece.Cells)
        {
            int x = c.X - minX;
            int y = c.Y - minY;
            if (x >= 0 && x < 4 && y >= 0 && y < 4)
            {
                cells[y * 4 + x].Fill = brush;
            }
        }

        return cells;
    }

    private static string ColorFor(PieceType type) => type switch
    {
        PieceType.I => "#00FFFF",
        PieceType.J => "#0000FF",
        PieceType.L => "#FFA500",
        PieceType.O => "#FFFF00",
        PieceType.S => "#00FF00",
        PieceType.T => "#800080",
        PieceType.Z => "#FF0000",
        _ => "#FFFFFF"
    };

    private static SolidColorBrush HexColor(string hex)
        => new((Color)ColorConverter.ConvertFromString(hex)!);

    /// <summary>Simple view model representing a single cell.</summary>
    public class CellVm : ObservableObject
    {
        private Brush _fill = Brushes.Transparent;
        public Brush Fill
        {
            get => _fill;
            set => SetProperty(ref _fill, value);
        }

        private Brush _stroke = Brushes.Transparent;
        public Brush Stroke
        {
            get => _stroke;
            set => SetProperty(ref _stroke, value);
        }
    }
}

