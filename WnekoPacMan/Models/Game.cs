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
        Red blinky;
        Pink pinky;
        Blue inky;
        Orange clyde;
        Dictionary<int, Ellipse> dotsList = new Dictionary<int, Ellipse>();

        int skipCounter = 0;
        int skipTreshold = 1;
        int interval = 10;
        int[,] gameMatrixWithDots =
        {
            {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 2 , -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, 2 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 0 , -1, -1, 0 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 0 , -1, -1, 0 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {0 , 0 , 0 , 0 , 0 , 0 , 1 , 0 , 0 , 0 , -1, -1, 0 , 0 , 0 , 0 , -1, -1, 0 , 0 , 0 , 1 , 0 , 0 , 0 , 0 , 0 , 0  },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, -1, -1, -1, -1, -1, 1 , -1, -1, 0 , -1, -1, -1, -1, -1, -1, -1, -1, 0 , -1, -1, 1 , -1, -1, -1, -1, -1, -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, 1 , -1, -1, -1, -1, 1 , -1 },
            {-1, 2 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 0 , 0 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 2 , -1 },
            {-1, -1, -1, 1 , -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, 1 , -1, -1, -1 },
            {-1, -1, -1, 1 , -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, 1 , -1, -1, -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , -1, -1, 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1, -1, 1 , -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 , -1 },
            {-1, 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , -1 },
            {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
        };


        int[,] gameMatrixEmpty =
            {
                {-1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1},
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
                {-1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1}
            };

        int[,] gameMatrix;
        int[] gameMatrixSize = new int[2];
        int cellSize;
        int dotsCount;

        public Game(int cellSize)
        {
            gameMatrix = gameMatrixWithDots;
            GameMatrixSize[0] = GameMatrix.GetLength(0);
            GameMatrixSize[1] = GameMatrix.GetLength(1);
            Human = new Human(this.GameMatrixSize, cellSize, this, new int[] { 23, 14 }, Directions.Down);
            blinky = new Red(this.GameMatrixSize, cellSize, this, new int[] { 5, 6 }, Directions.Down);
            pinky = new Pink(this.GameMatrixSize, cellSize, this, new int[] { 29, 1 }, Directions.Right);
            inky = new Blue(this.GameMatrixSize, cellSize, this, new int[] { 29, 26 }, Directions.Up);
            clyde = new Orange(this.GameMatrixSize, cellSize, this, new int[] { 5, 26 }, Directions.Left);
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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private int CountDots()
        {
            int count = 0;
            for(int i = 0; i < GameMatrixSize[0]; i++)
            {
                for(int j = 0; j < GameMatrixSize[1]; j++)
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
            gameMatrix[row, col] = 0;
            dotsList[row*100+col].Visibility = Visibility.Hidden;
            dotsCount--;
            if (dotsCount == 0)
            {
                EndGame();
            }
            return 0;
        }

        private void EndGame()
        {
            MessageBox.Show("You won!");
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
            if (GameMatrix[row,column] == 1)
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
                dotsList.Add(row*100+column, dot);
                return dot;
            }
            if(GameMatrix[row,column] == 2)
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
                dotsList.Add(row*100+column, energizer);
                return energizer;
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
                        return CellType.dot;
                    case 2:
                        return CellType.energizer;
                    case 3:
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
