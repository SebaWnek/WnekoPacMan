using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WnekoPacMan.Models
{
    public enum CellType
    {
        wall,
        empty,
        fruit,
        border
    }
    public enum Intersction
    {
        intersection,
        turn,
        straigth
    }


    public class Game
    {
        DispatcherTimer timer;

        Player[] players;
        Human human;
        AI ghost;

        int skipCounter = 0;
        int skipTreshold = 1;
        int interval = 10;

        int[,] gameMatrix =
            {
                {-1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 },
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  -1},
                {-1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1},
                {-1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  -1,  -1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1,  -1,  0 ,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0 ,  -1},
                {-1,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  -1},
                {-1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  0,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1}
            };
        int[] gameMatrixSize = new int[2];
        int cellSize;

        public Game(int cellSize)
        {
            Human = new Human(this.GameMatrixSize, cellSize, this, new int[] { 23, 14 }, Directions.Down);
            ghost = new AI(this.GameMatrixSize, cellSize, this, new int[] { 5, 6 }, Directions.Down);
            Human.HumanPositionChanged += ghost.OnHumanPositionChanged;
            Players = new Player[] { Human, ghost };
            this.cellSize = cellSize;
            GameMatrixSize[0] = GameMatrix.GetLength(0);
            GameMatrixSize[1] = GameMatrix.GetLength(1);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += Timer_Tick;
            timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (Player player in Players) player.Move();
            SkipCounter = SkipCounter == skipTreshold ? 0 : SkipCounter + 1;
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        public UIElement GetElement(int row, int collumn)
        {
            if (GameMatrix[row, collumn] == -1)
            {
                Rectangle wall = new Rectangle();
                wall.Fill = Brushes.Blue;
                wall.Width = cellSize;
                wall.Height = cellSize;
                wall.SetValue(Grid.RowProperty, row);
                wall.SetValue(Grid.ColumnProperty, collumn);
                return wall;
            }
            else return null;
        }

        public CellType CheckGridCellType(int[] rowcol)
        {
            object[] result = new object[2];
            if (rowcol[0] >= 0 && rowcol[0] < gameMatrixSize[0] && rowcol[1] >= 0 && rowcol[1] < gameMatrixSize[1])
            {
                switch (gameMatrix[rowcol[0], rowcol[1]])
                {
                    case -1:
                        return CellType.wall;
                    case 0:
                        return CellType.empty;
                    case 1:
                        return CellType.fruit;
                }
            }
            return CellType.border;
        }

        internal bool[] CheckIntersection(int[] gridCell)
        {
            bool[] neighbours = new bool[4]; //up, down, left, right
            int[] up = { gridCell[0] - 1, gridCell[1] };
            int[] down = { gridCell[0] + 1, gridCell[1] };
            int[] left = { gridCell[0], gridCell[1] - 1};
            int[] right = { gridCell[0], gridCell[1] + 1};
            if (up[0] < 0 || gameMatrix[up[0], up[1]] != -1) neighbours[0] = true;
            if (down[0] >= gameMatrixSize[0] || gameMatrix[down[0], down[1]] != -1) neighbours[1] = true;
            if (left[1] < 0 || gameMatrix[left[0], left[1]] != -1) neighbours[2] = true;
            if (right[1] >= gameMatrixSize[1] || gameMatrix[right[0], right[1]] != -1) neighbours[3] = true;
            return neighbours;
        }

        public int[] GameMatrixSize { get => gameMatrixSize; set => gameMatrixSize = value; }
        public int[,] GameMatrix { get => gameMatrix; set => gameMatrix = value; }
        public Player[] Players { get => players; set => players = value; }
        internal Human Human { get => human; set => human = value; }
        public int SkipCounter { get => skipCounter; set => skipCounter = value; }
    }
}
