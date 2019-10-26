using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WnekoPacMan.Models.Ghosts;

namespace WnekoPacMan.Models
{
    public enum CellType
    {
        wall,
        empty,
        dot,
        energizer,
        fruit,
        border,
        tunnel
    }
    public enum Intersction
    {
        intersection,
        turn,
        straigth
    }


    public class Game
    {
        DispatcherTimer frameTimer;
        Timer gameTimer;
        int gameTimerCounter;
        int gameTimerTime;
        Stopwatch stopwatch = new Stopwatch();
        int[] gameTimerIntervals =
        {
            7000,
            20000,
            7000,
            20000,
            5000,
            20000,
            5000,
            int.MaxValue
        };

        private Dictionary<int, CellType> cellTypes = new Dictionary<int, CellType>
        {
            {-2, CellType.tunnel },
            {-1, CellType.wall },
            {0, CellType.empty },
            {1, CellType.dot },
            {2, CellType.energizer },
            {3, CellType.fruit }
        };

        DispatcherTimer scaredTimer;
        int[] scaredTime = new int[] { 6000, 500, 500, 500, 500, 500 };
        int scaredCounter = 0;

        Player[] players;
        Human human;
        Red blinky;
        Pink pinky;
        Blue inky;
        Orange clyde;
        Dictionary<int, Ellipse> dotsList = new Dictionary<int, Ellipse>();
        Dictionary<int, int> scores = new Dictionary<int, int>
        {
            {1, 10 },
            {2, 50 }
        };

        int level = 1;
        int skipCounter = 0;
        int skipTreshold = 1;
        int interval = 8;
        int elroy1Treshold = 200;
        int elroy2Treshold = 100;

        int[,] gameMatrix;
        int[] gameMatrixSize = new int[2];
        int cellSize;
        int dotsCount;

        public Game(int cellSize)
        {
            gameMatrix = GameMatrices.gameMatrixWithDots;
            GameMatrixSize[0] = GameMatrix.GetLength(0);
            GameMatrixSize[1] = GameMatrix.GetLength(1);
            Human = new Human(this.GameMatrixSize, cellSize, this, new int[] { 23, 14 }, Directions.Left, 1);
            blinky = new Red(this.GameMatrixSize, cellSize, this, new int[] { 5, 6 }, Directions.Down, 1);
            pinky = new Pink(this.GameMatrixSize, cellSize, this, new int[] { 29, 1 }, Directions.Right, 1);
            inky = new Blue(this.GameMatrixSize, cellSize, this, new int[] { 29, 26 }, Directions.Up, 1);
            clyde = new Orange(this.GameMatrixSize, cellSize, this, new int[] { 5, 26 }, Directions.Left, 1);
            Players = new Player[] { Human, blinky, pinky, inky, clyde };
            foreach (Player ght in Players)
            {
                if (ght is AI)
                {
                    Human.HumanPositionChanged += ((AI)ght).OnHumanPositionChanged;
                }
            }
            blinky.RedPositionChanged += inky.OnRedPositionChanged;
            this.cellSize = cellSize;
            dotsCount = CountDots();
            frameTimer = new DispatcherTimer();
            frameTimer.Interval = TimeSpan.FromMilliseconds(interval);
            frameTimer.Tick += Timer_Tick;
            frameTimer.Start();
            gameTimer = new Timer();
            gameTimer.Interval = gameTimerIntervals[gameTimerCounter];
            gameTimer.Elapsed += GameTimer_Elapsed;
            gameTimer.Start();
            stopwatch.Start();
            scaredTimer = new DispatcherTimer();
            scaredTimer.Tick += ScaredTimer_Tick;
        }


        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            gameTimerCounter++;
            stopwatch.Restart();
            foreach (Player player in Players)
            {
                AI ghost = player as AI;
                if (gameTimerCounter % 2 == 1 && ghost != null) ghost.ChangeMode(AIModes.Normal, SpeedModes.Normal);
                else if (ghost != null) ghost.ChangeMode(AIModes.Scatter, SpeedModes.Normal);
            }
            gameTimer.Stop();
            gameTimer.Interval = gameTimerIntervals[gameTimerCounter];
            gameTimer.Start();
        }

        private void ChangeMode(AIModes mode, SpeedModes speed)
        {
            foreach (Player player in Players)
            {
                AI ghost = player as AI;
                if (ghost != null) ghost.ChangeMode(mode, speed);
            }
        }

        private int CountDots()
        {
            int count = 0;
            for (int i = 0; i < GameMatrixSize[0]; i++)
            {
                for (int j = 0; j < GameMatrixSize[1]; j++)
                {
                    if (GameMatrix[i, j] == 1 || GameMatrix[i, j] == 2)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (Player player in Players) player.Move();
            SkipCounter = SkipCounter == skipTreshold ? 0 : SkipCounter + 1;
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        internal int Eat(int row, int col)
        {
            int score = scores[gameMatrix[row, col]];
            if (CheckGridCellType(new int[] { row, col }) == CellType.energizer)
            {
                Energize();
            }
            gameMatrix[row, col] = 0;
            dotsList[row * 100 + col].Visibility = Visibility.Hidden;
            dotsCount--;
            if (dotsCount == elroy1Treshold)
            {
                blinky.BecomeElroy(1);
            }
            if (dotsCount == elroy2Treshold)
            {
                blinky.BecomeElroy(2);
            }
            if (dotsCount == 0)
            {
                EndGame();
            }
            return score;
        }

        private void Energize()
        {
            scaredCounter = 0;
            scaredTimer.Interval = TimeSpan.FromMilliseconds(scaredTime[0]);
            gameTimerTime = gameTimerIntervals[gameTimerCounter] - (int)stopwatch.ElapsedMilliseconds;
            gameTimer.Stop();
            stopwatch.Stop();
            foreach (Player player in Players)
            {
                AI ghost = player as AI;
                if (ghost != null)
                {
                    ghost.ChangeMode(AIModes.Random, SpeedModes.Fright);
                    ghost.ChangeColor(1);
                }
            }
            human.ChangeSpeed(SpeedModes.FrightHuman);
            scaredTimer.Start();
        }
        private void ScaredTimer_Tick(object sender, EventArgs e)
        {
            scaredCounter++;
            foreach (Player player in Players)
            {
                AI ghost = player as AI;
                if (ghost != null) ghost.ChangeColor(scaredCounter % 2);
            }
            if (scaredCounter == scaredTime.Length)
            {
                scaredTimer.Stop();
                foreach (Player player in Players)
                {
                    AI ghost = player as AI;
                    if (ghost != null)
                    {
                        ghost.ChangeMode(AIModes.Normal, SpeedModes.Normal);
                        ghost.ChangeColor(2);
                    }
                }
                gameTimer.Interval = gameTimerTime;
                gameTimer.Start();
                stopwatch.Start();
                return;
            }
            scaredTimer.Stop();
            scaredTimer.Interval = TimeSpan.FromMilliseconds(scaredTime[scaredCounter]);
            scaredTimer.Start();
        }

        private void EndGame()
        {
            MessageBox.Show("You won!");
            Environment.Exit(0);
        }

        public UIElement GetElement(int row, int column)
        {
            if (GameMatrix[row, column] == -1)
            {
                Rectangle wall = new Rectangle();
                wall.Fill = Brushes.Blue;
                wall.Width = cellSize;
                wall.Height = cellSize;
                wall.SetValue(Grid.RowProperty, row);
                wall.SetValue(Grid.ColumnProperty, column);
                return wall;
            }
            if (GameMatrix[row, column] == 1)
            {
                Ellipse dot = new Ellipse();
                dot.Fill = Brushes.White;
                dot.Height = 5;
                dot.Width = 5;
                Thickness thickness = new Thickness((cellSize - 5) / 2);
                dot.Margin = thickness;
                dot.SetValue(Grid.RowProperty, row);
                dot.SetValue(Grid.ColumnProperty, column);
                dot.Name = "dot" + row + "_" + column;
                dotsList.Add(row * 100 + column, dot);
                return dot;
            }
            if (GameMatrix[row, column] == 2)
            {
                Ellipse energizer = new Ellipse();
                energizer.Fill = Brushes.White;
                energizer.Height = 9;
                energizer.Width = 9;
                Thickness thickness = new Thickness((cellSize - 9) / 2);
                energizer.Margin = thickness;
                energizer.SetValue(Grid.RowProperty, row);
                energizer.SetValue(Grid.ColumnProperty, column);
                energizer.Name = "energizer" + row + "_" + column;
                dotsList.Add(row * 100 + column, energizer);
                return energizer;
            }
            else return null;
        }

        public CellType CheckGridCellType(int[] rowcol)
        {
            if (rowcol[0] >= 0 && rowcol[0] < gameMatrixSize[0] && rowcol[1] >= 0 && rowcol[1] < gameMatrixSize[1])
            {
                return cellTypes[gameMatrix[rowcol[0], rowcol[1]]];
            }
            return CellType.border;
        }

        internal bool[] CheckIntersection(int[] gridCell)
        {
            bool[] neighbours = new bool[4]; //up, down, left, right
            int[] up = { gridCell[0] - 1, gridCell[1] };
            int[] down = { gridCell[0] + 1, gridCell[1] };
            int[] left = { gridCell[0], gridCell[1] - 1 };
            int[] right = { gridCell[0], gridCell[1] + 1 };
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
