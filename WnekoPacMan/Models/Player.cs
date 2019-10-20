﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WnekoPacMan.Models
{
    public enum Directions { Up, Down, Left, Right, Stop };
    public class Player : INotifyPropertyChanged
    {
        public static Random rnd = new Random();
        protected static Dictionary<Directions, int[]> movementDirections = new Dictionary<Directions, int[]>
        {
            {Directions.Up, new int[] { 0, -1 } },
            {Directions.Down, new int[] { 0, 1 }  },
            {Directions.Left, new int[] { -1, 0 } },
            {Directions.Right, new int[] { 1, 0 } },
            {Directions.Stop, new int[] {0,0} }
        };
        protected static Dictionary<Directions, int[]> gridDirections = new Dictionary<Directions, int[]>
        {
            {Directions.Up, new int[] { -1, 0 } },
            {Directions.Down, new int[] { 1, 0 }  },
            {Directions.Left, new int[] { 0, -1 } },
            {Directions.Right, new int[] { 0, 1 } }
        };
        protected int[] playerPosition; // position of middle
        protected int[] movementDirection = new int[] { 0, 0 };
        protected int[] firstGridCell = new int[2] ;
        protected Ellipse playerEllipse;
        protected float speed = 1;
        protected int[] gridCell = new int[2];
        protected Directions currentDirection;
        protected bool directionChanged = false;
        protected int[] gridSize;
        protected int cellSize;
        protected Game game;

        public Player(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir)
        {
            firstGridCell = cell;
            playerEllipse = new Ellipse();
            playerEllipse.Fill = Brushes.Red;
            playerEllipse.Height = cellSize;
            playerEllipse.Width = cellSize;
            playerPosition = new int[] { firstGridCell[1] * cellSize + cellSize / 2, firstGridCell[0] * cellSize + cellSize / 2 };
            Binding topBinding = new Binding("PlayerTop");
            Binding leftBinding = new Binding("PlayerLeft");
            topBinding.Source = this;
            leftBinding.Source = this;
            playerEllipse.SetBinding(Canvas.LeftProperty, leftBinding);
            playerEllipse.SetBinding(Canvas.TopProperty, topBinding);
            this.gridSize = gridSize;
            this.cellSize = cellSize;
            this.game = game;
        }

        protected bool CheckIfInTheMiddle()
        {
            return playerPosition[0] - (gridCell[1] * cellSize) == cellSize / 2 && playerPosition[1] - gridCell[0] * cellSize == cellSize / 2;
        }

        public int PlayerLeft
        {
            get => playerPosition[0] - cellSize / 2;
            set
            {
                playerPosition[0] = value + cellSize / 2;
                if (game.SkipCounter == 0)
                {
                    NotifyPropertyChanged(); 
                }
            }
        }
        public int PlayerTop
        {
            get => playerPosition[1] - cellSize / 2;
            set
            {
                playerPosition[1] = value + cellSize / 2;
                if (game.SkipCounter == 0)
                {
                    NotifyPropertyChanged(); 
                }
            }
        }

        public float Speed { get => speed; set => speed = value; }
        public int[] GridCell { get => gridCell; set => gridCell = value; }
        public bool DirectionChanged { get => directionChanged; set => directionChanged = value; }

        public virtual void Move()
        {
            PlayerLeft += (int)(Speed * movementDirection[0]);
            PlayerTop += (int)(Speed * movementDirection[1]);
            if (playerPosition[0] == 0 || playerPosition[1] == 0 || playerPosition[0] == gridSize[1] * cellSize || playerPosition[1] == gridSize[0] * cellSize)
            {
                JumpOverBorder();
            }
        }

        protected int[] GetNextCell(Directions dirrection)
        {
            int[] result = new int[2];
            result[0] = gridCell[0] + gridDirections[dirrection][0];
            result[1] = gridCell[1] + gridDirections[dirrection][1];
            return result;
        }

        protected int[,] GetNextCells()
        {
            throw new NotImplementedException();
        }

        protected void JumpOverBorder()
        {
            if (playerPosition[0] == 0) playerPosition[0] = gridSize[1] * cellSize;
            else if (playerPosition[1] == 0) playerPosition[1] = gridSize[0] * cellSize;
            else if (playerPosition[0] == gridSize[1] * cellSize) playerPosition[0] = 0;
            else if (playerPosition[1] == gridSize[0] * cellSize) playerPosition[1] = 0;
        }

        public UIElement GetPlayerGraphics()
        {
            return playerEllipse;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
