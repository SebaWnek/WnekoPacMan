using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WnekoPacMan.Models;

namespace WnekoPacMan.ViewModels
{
    partial class MainWindowViewModel : INotifyPropertyChanged
    {
        int cellSize = 27;

        Game game;
        MainWindow main = (MainWindow)App.Current.MainWindow;
        int[] gridSize = new int[2];

        public int[] GridSize { get => gridSize; set => gridSize = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            game = new Game(cellSize);
            GenerateGameGrid();
            InitializeCommands();
            AddPlayers();
            GridSize[0] = main.gameGrid.RowDefinitions.Count;
            GridSize[1] = main.gameGrid.ColumnDefinitions.Count;
        }

        private void AddPlayers()
        {
            foreach (Player player in game.Players)
            {
                main.mainCanvas.Children.Add(player.GetPlayerGraphics());
                if (player is AI) main.gameGrid.Children.Add(((AI)player).GetTargetGraphics());
            }
        }

        private void GenerateGameGrid()
        {
            for (int i = 0; i < game.GameMatrixSize[0]; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(cellSize);
                main.gameGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < game.GameMatrixSize[1]; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(cellSize);
                main.gameGrid.ColumnDefinitions.Add(column);
            }
            GenerateGridUI();
        }

        private void GenerateGridUI()
        {
            UIElement element;
            for (int i = 0; i < game.GameMatrixSize[0]; i++)
            {
                for (int j = 0; j < game.GameMatrixSize[1]; j++)
                {
                    element = game.GetElement(i, j);
                    if (element != null) main.gameGrid.Children.Add(element);
                }
            }
        }



        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }
}
